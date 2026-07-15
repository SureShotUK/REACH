# FileBrowser — 403 Forbidden on Delete

**Status (2026-07-15): DIAGNOSED, FIX NOT YET APPLIED.** Root cause identified with evidence; the `chown` has not been run or verified.

## Symptom

Logged into FileBrowser as `admin`. Browsing and downloading work normally. Deleting a file returns **403 Forbidden**. In Settings → User Management, the **Delete** permission shows a tick but is **greyed out** and cannot be changed.

## Why this is misleading

Two completely unrelated causes produce an identical 403, and the browser cannot tell them apart.

### 1. The permission bit (`Perm.Delete`)

`http/resource.go`:

```go
func resourceDeleteHandler(fileCache FileCache) handleFunc {
	return withUser(func(_ http.ResponseWriter, r *http.Request, d *data) (int, error) {
		if r.URL.Path == "/" || !d.user.Perm.Delete {
			return http.StatusForbidden, nil
		}
```

**`admin` does not imply `delete`.** There is no admin override in this path — `Perm.Admin` grants the Settings and User Management screens only. The `Delete` bit must be set independently. Note also that `r.URL.Path == "/"` returns 403 before the permission test, so the scope root itself can never be deleted.

### 2. A real Linux filesystem error

`http/utils.go:34`:

```go
func errToStatus(err error) int {
	switch {
	case err == nil:
		return http.StatusOK
	case os.IsPermission(err):
		return http.StatusForbidden
```

An `EACCES` from `RemoveAll` is translated into **the same 403**. So a filesystem permission problem is indistinguishable from a missing permission bit at the HTTP layer.

### The greyed-out checkbox is a UI quirk, not a stored value

`frontend/src/components/settings/Permissions.vue` puts `:disabled="admin"` on every sub-permission checkbox, so ticking Administrator locks all the others. The `admin` computed setter only does `this.perm.admin = value` — it never forces the others true.

**Therefore: a greyed-out tick on Delete means `perm.delete` genuinely IS `true` in the database.** If you see that and still get a 403, the permission bit is not your problem — it is the filesystem.

## The Unix rule that actually bites

**Deleting a file requires write permission on the directory containing it, not on the file itself.** The unlink modifies the directory entry.

This is exactly why browsing and downloading work while delete fails: read and traverse need only `r-x`, which a normal `755` directory grants to everyone. Removing an entry needs `w` on the directory, which `755` grants only to the owner.

## The Docker bind-mount trap

Docker auto-creates bind-mount targets as `root:root`. On this host, `/home/steve/rag-output` is mounted at `/srv`, and further mounts are layered *inside* it — so Docker created those mountpoint directories on the host, owned by root:

```
drwxrwxr-x 12 1000 1000  /home/steve/rag-output      <- fine, container writes here OK
drwxr-xr-x  2    0    0  comfyui-amelia-output       <- root-owned STUB (bind-mount target)
drwxr-xr-x  2    0    0  comfyui-output              <- root-owned STUB
drwxr-xr-x  2    0    0  comfyui-workflows           <- root-owned STUB
drwxr-x---  3 1000 1000  HSE                         <- steve-owned, deletes work (control case)
```

**Chowning the stub does nothing.** The container never reads it — it sees the mounted source filesystem at that path instead. The permissions that matter are on the source:

| Container path | Real source | Fix |
|---|---|---|
| `/srv/comfyui-output` | `/docs/Projects/Claude Code Shared/Output` | CIFS — `chown` won't stick, needs `uid=1000` in mount options |
| `/srv/comfyui-workflows` | `/opt/comfyui/workflows` | `chown` the source |
| `/srv/comfyui-amelia-output` | `/opt/comfyui-amelia/output` | `chown` the source |
| `comfyui-input`, `comfyui-amelia-input` | real dirs in `rag-output` | `chown` directly |

## Diagnosis

```bash
# What UID does FileBrowser run as?
docker exec filebrowser id
# -> uid=1000(user) gid=1000(user)

# The DIRECTORY (note -d: stat the dir itself; -n: numeric UIDs to compare)
ls -ldn /home/steve/rag-output

# Can the container actually write there?
docker exec filebrowser touch /srv/.deleteme && echo 'WRITE OK' || echo 'WRITE DENIED'

# The real errno, unlaundered by FileBrowser's error mapping
docker exec filebrowser rm '/srv/<path>/<file>'
```

Before chowning a source directory, **check what UID the writing container runs as**:

```bash
docker exec comfyui-amelia id
```

If it is root (`uid=0`), chowning the directory to steve is free — root ignores permission bits and keeps writing as before. If it runs as some *other* non-root UID, handing the directory to steve could break its writes, and a shared group is the correct fix instead.

## Fix

```bash
sudo chown steve:steve /opt/comfyui-amelia/output
sudo chown steve:steve /opt/comfyui/workflows
sudo chown steve:steve /home/steve/rag-output/comfyui-input /home/steve/rag-output/comfyui-amelia-input
```

No restart needed — permissions are read live at unlink time.

### Why no `-R`

You only need to own the **directory**, because that is what unlinking checks. Files inside can stay root-owned and will still delete correctly. This matters: ComfyUI runs as root and writes new root-owned files on every generation. Owning the directory keeps deletes working forever; a `chown -R` fixes today's files and silently rots the moment ComfyUI writes the next one.

### Why not `chmod 777`

It works, and it also means anything able to reach that path can rewrite the RAG output. Matching the UID achieves the same result without the exposure.

## Notes

- `comfyui-output` is the CIFS share from irwinnas. CIFS enforces the permissions of the account used to mount the share — being root inside the container buys nothing there. Needs `uid=1000` in the mount options rather than a `chown`.
- The `docker exec filebrowser touch /srv/.deleteme` diagnostic leaves a `.deleteme` file in `/home/steve/rag-output` — remove it.
