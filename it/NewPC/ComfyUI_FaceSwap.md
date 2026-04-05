# ComfyUI Face Swap Workflow — Step by Step

**Goal**: Take two images and swap the face from one into the other.
**Nodes used**: Load Image × 2 → ReActor → Save Image
**ComfyUI**: `https://amelai.tail926601.ts.net:8188`

---

## The Plan

```
Load Image (target) ──► ReActor ──► Save Image
Load Image (source) ──►
```

- **Target image** — the image you want to put the new face into
- **Source image** — the reference photo containing the face to use

That's it. Four nodes, two connections from Load Image into ReActor, one out to Save Image.

---

## Steps

### Step 1 — Clear the canvas

In ComfyUI, go to the top menu and click **Clear**. If it asks to confirm, click yes.

You should now have a completely empty dark canvas.

---

### Step 2 — Add the first Load Image node (target)

Right-click on an empty area of the canvas.

Navigate to: **image** → **Load Image**

Click it. A node appears on the canvas with:
- A preview area at the top
- A `choose file to upload` button
- An `image` dropdown
- Two output circles on the right: **IMAGE** and **MASK**

This will be your **target image** — the image whose face will be replaced. Label it mentally as "target" (you cannot rename nodes but you can position them clearly).

Place it on the left side of your canvas.

---

### Step 3 — Add a second Load Image node (source)

Right-click on empty canvas space again.

Navigate to: **image** → **Load Image**

Place this one below or near the first Load Image node.

This will be your **source image** — the reference photo containing the face you want to use.

---

### Step 4 — Add the ReActor node

Right-click on empty canvas space.

Navigate to: **ReActor** → **Fast Face Swap** (it may also appear as **ReActor Node** depending on version)

Place it to the right of your two Load Image nodes. It is a larger node with several settings inside it.

You should see:
- Input circles on the left: **input_image** and **source_image**
- Several dropdown settings and sliders inside
- An output circle on the right: **IMAGE**

---

### Step 5 — Connect the target image to ReActor

Click and drag from the **IMAGE** output circle (right side) of your **first Load Image node** (target) to the **input_image** input circle (left side) of the ReActor node.

A coloured wire will appear connecting them.

> **input_image** = the image you are swapping the face **into** (the target)

---

### Step 6 — Connect the source image to ReActor

Click and drag from the **IMAGE** output circle (right side) of your **second Load Image node** (source) to the **source_image** input circle (left side) of the ReActor node.

> **source_image** = the reference photo containing the face you want **to use**

---

### Step 7 — Add a Save Image node

Right-click on empty canvas space.

Navigate to: **image** → **Save Image**

Place it to the right of the ReActor node.

---

### Step 8 — Connect ReActor to Save Image

Click and drag from the **IMAGE** output circle (right side) of the ReActor node to the **images** input circle (left side) of the Save Image node.

Your canvas should now look like this:

```
[ Load Image (target) ] ──► [ ReActor ] ──► [ Save Image ]
[ Load Image (source) ] ──►
```

All four nodes are connected. If any connection line is missing, re-check Steps 5, 6, and 8.

---

### Step 9 — Upload your images

**In the target Load Image node** (top one):
- Click **choose file to upload**
- Select the image you want to swap a face **into**
- A preview will appear inside the node

**In the source Load Image node** (bottom one):
- Click **choose file to upload**
- Select the **reference face photo** — a clear, front-facing photo works best
- A preview will appear inside the node

---

### Step 10 — Check the ReActor settings

Inside the ReActor node you will see several settings. Use these as a starting point:

| Setting | Recommended value | What it does |
|---------|-------------------|-------------|
| `enabled` | `true` (ticked) | Master on/off switch |
| `swap_model` | `inswapper_128.onnx` | The face swap model — should already be selected |
| `facedetection` | `retinaface_resnet50` | Face detection method — good default |
| `face_restore_model` | `GFPGANv1.4` | Sharpens and blends the swapped face |
| `face_restore_visibility` | `1` | Full face restoration (0 = none, 1 = full) |
| `codeformer_weight` | `0.5` | Only relevant if using CodeFormer restore model |
| `input_faces_index` | `0` | Which face in the target to replace (0 = first detected) |
| `source_faces_index` | `0` | Which face in the source to use (0 = first detected) |

> **Note**: `GFPGANv1.4` will be downloaded automatically the first time you run a face swap. This is normal — it takes a moment on first use only.

---

### Step 11 — Run it

Click **Queue Prompt** in the top menu.

Watch the progress bar. The first run may take longer as face analysis models download automatically.

When complete, the result appears in the Save Image node and is saved to `/opt/comfyui/output/` on the server.

---

### Step 12 — Save the workflow

Once it is working, save it for future use:

Menu → **Save** → name it `faceswap-basic.json`

Any generated image also has the workflow embedded — drag it back onto the canvas to restore this workflow in a future session.

---

## If Something Goes Wrong

| Problem | Likely cause | What to try |
|---------|-------------|-------------|
| ReActor node shows red/error outline | Missing model or dependency | Check `docker logs comfyui \| tail -30` |
| `swap_model` dropdown is empty | `inswapper_128.onnx` not found | Verify the file is at `/mnt/models/comfyui/reactor/inswapper_128.onnx` |
| Face not detected | Poor source photo | Use a clearer, more front-facing photo with good lighting |
| Result looks blurry or patchy | Face restore not running | Ensure `face_restore_model` is set and `face_restore_visibility` is 1 |
| Result unchanged from target | `enabled` is false | Tick the `enabled` checkbox in the ReActor node |
| Queue runs but output looks wrong | Wrong image connected to wrong input | Double-check Step 5 and 6 — input_image is target, source_image is reference |

---

*Back to main guide: `Learn_ComfyUI.md`*
