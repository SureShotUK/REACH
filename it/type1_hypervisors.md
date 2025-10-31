# Type 1 Hypervisors: Overview and Homelab Setup Guide

## What is a Type 1 Hypervisor?

A **Type 1 hypervisor** (also called a "bare-metal hypervisor") is virtualization software that runs directly on the physical hardware without requiring a host operating system. Unlike Type 2 hypervisors that run as applications within Windows or Linux, Type 1 hypervisors replace the traditional OS entirely.

### Architecture

```
Traditional Computer:          Type 2 Hypervisor:           Type 1 Hypervisor:
┌─────────────────┐           ┌─────────────────┐          ┌─────────────────┐
│  Applications   │           │   Virtual VMs   │          │   Virtual VMs   │
├─────────────────┤           ├─────────────────┤          ├─────────────────┤
│  Operating OS   │           │ Hypervisor App  │          │   Hypervisor    │
├─────────────────┤           ├─────────────────┤          ├─────────────────┤
│    Hardware     │           │    Host OS      │          │    Hardware     │
└─────────────────┘           ├─────────────────┤          └─────────────────┘
                              │    Hardware     │
                              └─────────────────┘
```

### Key Characteristics

**Direct Hardware Access:**
- Hypervisor has complete control over CPU, memory, storage, and network
- No host OS overhead or interference
- Near-native performance for VMs

**Superior Isolation:**
- VMs are isolated at hardware level
- Smaller attack surface (no host OS to compromise)
- Better security boundaries between VMs

**Enterprise Features:**
- Live migration (move running VMs between hosts)
- High availability and clustering
- Advanced resource management
- Centralized management consoles

**Efficiency:**
- Lower resource overhead (2-5% vs 10-15% for Type 2)
- Better memory and CPU utilization
- Optimized I/O performance

---

## Popular Type 1 Hypervisors for Homelab

### 1. Proxmox VE (Recommended for Beginners)

**Overview:**
- Free and open-source
- Based on Debian Linux with KVM and LXC
- Excellent web-based management interface
- Large community and documentation

**Pros:**
- Completely free (no licensing costs)
- Easy to use web GUI
- Supports both VMs (KVM) and containers (LXC)
- Built-in backup, storage, and network management
- Active community support

**Cons:**
- Enterprise features require paid subscription (optional)
- Not as polished as commercial alternatives

**Best For:** Homelab beginners, learning virtualization, mixed VM/container workloads

---

### 2. VMware ESXi

**Overview:**
- Industry-standard enterprise hypervisor
- Free version available (ESXi Free)
- Professional management tools (vSphere)

**Pros:**
- Industry-leading features and stability
- Extensive hardware compatibility
- Professional experience transferable to enterprise environments
- Mature ecosystem

**Cons:**
- Free version has significant limitations (no vCenter API, backup APIs)
- Broadcom acquisition has changed licensing (uncertain future for free version)
- More restrictive than open-source alternatives

**Best For:** Learning enterprise VMware skills, production-quality homelab, hardware compatibility concerns

---

### 3. Microsoft Hyper-V Server (Free)

**Overview:**
- Free standalone Hyper-V without Windows Server
- Core-based (command-line only on host)
- Managed via Windows Admin Center or Hyper-V Manager

**Pros:**
- Completely free with no limitations
- Full Hyper-V features (live migration, replication, clustering)
- Excellent Windows VM performance
- Good for learning Microsoft technologies

**Cons:**
- Core edition requires PowerShell skills
- Less intuitive than Proxmox web UI
- Primarily focused on Windows VMs (Linux support improving)

**Best For:** Windows-focused environments, learning Microsoft technologies, Windows Server labs

---

### 4. XCP-ng (Xen Orchestra)

**Overview:**
- Free and open-source (based on Citrix XenServer)
- Xen-based hypervisor
- Managed via Xen Orchestra web interface

**Pros:**
- Fully open-source with no feature limitations
- Excellent web management (Xen Orchestra)
- Strong enterprise features
- Active development community

**Cons:**
- Smaller community than Proxmox
- Xen Orchestra requires separate installation
- Steeper learning curve

**Best For:** Advanced users, those wanting Xen experience, enterprise-like features

---

### 5. KVM/libvirt (Advanced)

**Overview:**
- Built into Linux kernel
- Managed via command-line (virsh) or web UI (Cockpit, oVirt)
- Maximum flexibility and customization

**Pros:**
- Native Linux kernel integration
- Ultimate control and customization
- No additional software layer
- Used by major cloud providers

**Cons:**
- Requires significant Linux expertise
- Manual configuration and management
- No out-of-box management GUI

**Best For:** Linux experts, learning low-level virtualization, custom deployments

---

## Hardware Requirements

### Minimum Homelab Server Specs

**CPU:**
- **Required:** x86-64 processor with virtualization extensions
  - Intel: VT-x and VT-d (check CPU specs on Intel ARK)
  - AMD: AMD-V and AMD-Vi
- **Recommended:** 4+ cores (8+ threads with hyperthreading)
- **Ideal:** Intel Xeon or AMD EPYC (server-grade)

**RAM:**
- **Minimum:** 16GB
- **Recommended:** 32GB+
- **Ideal:** 64GB or more
- **Rule of Thumb:** Host needs 2-4GB + sum of all VM requirements

**Storage:**
- **Hypervisor Boot:** 32GB+ (USB drive or small SSD)
- **VM Storage:**
  - SSD: 500GB+ for VM disks (NVMe preferred)
  - HDD: 2TB+ for bulk storage/backups
- **Ideal:** Separate drives for OS and VM storage

**Network:**
- **Minimum:** 1Gbps Ethernet
- **Recommended:** 2+ NICs (management + VM traffic)
- **Ideal:** 10Gbps for storage networks (iSCSI, NFS)

**Other:**
- IPMI/iLO/iDRAC for remote management (enterprise servers)
- UPS for clean shutdowns and uptime

### Check CPU Virtualization Support

**Windows:**
```powershell
systeminfo | findstr /C:"Virtualization Enabled"
```

**Linux:**
```bash
egrep -c '(vmx|svm)' /proc/cpuinfo
# Non-zero result means virtualization supported
```

**BIOS/UEFI:**
- Enable Intel VT-x or AMD-V
- Enable Intel VT-d or AMD-Vi (IOMMU) for PCIe passthrough
- Disable Secure Boot (may interfere with some hypervisors)

---

## Proxmox VE Setup Guide (Recommended for Homelabs)

### Why Proxmox?
- Free and fully-featured
- Easy web GUI
- Great for learning
- Large community
- Supports VMs and containers

### Step 1: Download Proxmox VE

1. Visit: https://www.proxmox.com/en/downloads
2. Download latest Proxmox VE ISO installer
3. Create bootable USB:
   - **Windows:** Use Rufus or Etcher
   - **Linux:** `dd if=proxmox.iso of=/dev/sdX bs=1M status=progress`

### Step 2: Prepare Installation

**Network Planning:**
- Assign static IP for Proxmox host (e.g., 192.168.1.100)
- Note your gateway (router IP, e.g., 192.168.1.1)
- DNS servers (e.g., 8.8.8.8, 1.1.1.1)
- Subnet mask (usually 255.255.255.0 or /24)

**Storage Planning:**
- Identify which disk for Proxmox installation
- Plan separate disks/partitions for VM storage
- Consider RAID if multiple disks (ZFS recommended)

### Step 3: Installation

1. **Boot from USB**
   - Enter BIOS/UEFI boot menu (F11, F12, Del, or ESC)
   - Select USB drive

2. **Proxmox Installer**
   - Select "Install Proxmox VE (Graphical)"
   - Accept EULA

3. **Target Disk Selection**
   - Choose installation disk
   - **Filesystem options:**
     - **ext4:** Simple, reliable (single disk)
     - **ZFS:** Advanced features, snapshots (recommended if 2+ disks)
     - **LVM:** Flexible storage management
   - Click "Options" for RAID configuration (ZFS RAID1, RAID10, etc.)

4. **Location and Time Zone**
   - Select your country, timezone, keyboard layout

5. **Administration Password**
   - Set root password (save in password manager)
   - Enter email for system notifications

6. **Network Configuration**
   - **Management Interface:** Select physical NIC (usually eno1, enp0s1, etc.)
   - **Hostname (FQDN):** pve.homelab.local (or your domain)
   - **IP Address:** 192.168.1.100/24 (static IP)
   - **Gateway:** 192.168.1.1 (your router)
   - **DNS Server:** 8.8.8.8 (or your preferred DNS)

7. **Confirm and Install**
   - Review settings
   - Click "Install"
   - Wait 5-10 minutes

8. **Reboot**
   - Remove USB drive
   - System will reboot into Proxmox

### Step 4: First Login

1. **Access Web Interface**
   - Open browser: https://192.168.1.100:8006
   - Accept self-signed certificate warning
   - **Username:** root
   - **Password:** (password you set during install)

2. **Dismiss Subscription Notice**
   - Click "OK" on "No valid subscription" popup
   - This appears on every login (cosmetic only, doesn't affect functionality)

3. **Optional: Remove Subscription Popup**
   ```bash
   # SSH into Proxmox or use web Shell
   sed -Ezi.bak "s/(Ext.Msg.show\(\{\s+title: gettext\('No valid subscription')/void\(\{ \/\/\1/g" /usr/share/javascript/proxmox-widget-toolkit/proxmoxlib.js
   systemctl restart pveproxy.service
   ```

### Step 5: Update System

1. **Configure Repositories** (Remove Enterprise, Add No-Subscription)

   Via Web UI:
   - Click node name → Updates → Repositories
   - Disable "pve-enterprise" repository
   - Add "pve-no-subscription" repository

   Via SSH:
   ```bash
   # Disable enterprise repo
   echo "# deb https://enterprise.proxmox.com/debian/pve bookworm pve-enterprise" > /etc/apt/sources.list.d/pve-enterprise.list

   # Add no-subscription repo
   echo "deb http://download.proxmox.com/debian/pve bookworm pve-no-subscription" > /etc/apt/sources.list.d/pve-no-subscription.list
   ```

2. **Update Proxmox**
   ```bash
   apt update
   apt dist-upgrade -y
   ```

   Or via Web UI: Updates → Refresh → Upgrade

3. **Reboot if kernel updated**
   ```bash
   reboot
   ```

### Step 6: Storage Configuration

**Default Storage:**
- **local:** VM templates and ISO images
- **local-lvm:** VM disks (thin-provisioned LVM)

**Add Additional Storage (Optional):**

1. **Add Second Disk for VM Storage**
   - Datacenter → Storage → Add → Directory (or LVM, ZFS)
   - Point to mounted disk/partition

2. **Network Storage (NAS/SAN)**
   - Storage → Add → NFS/SMB/iSCSI
   - Enter NAS IP and share details

### Step 7: Upload ISO Images

1. **Download ISOs**
   - Ubuntu Server: https://ubuntu.com/download/server
   - Windows 10/11: https://www.microsoft.com/software-download

2. **Upload to Proxmox**
   - Select node → local storage → ISO Images
   - Click "Upload" or "Download from URL"
   - Wait for upload to complete

### Step 8: Create Your First VM

1. **Click "Create VM" (top right)**

2. **General:**
   - **VM ID:** 100 (auto-increments)
   - **Name:** ubuntu-server-01
   - Click "Next"

3. **OS:**
   - **ISO Image:** Select uploaded Ubuntu ISO
   - **Guest OS Type:** Linux
   - **Version:** 6.x - 2.6 Kernel
   - Click "Next"

4. **System:**
   - **Graphics card:** Default (or VirtIO-GPU for better performance)
   - **Machine:** q35 (modern chipset)
   - **BIOS:** OVMF (UEFI) or SeaBIOS (legacy)
   - **Add TPM:** Enable if Windows 11
   - **Add EFI Disk:** Enable if using UEFI
   - Click "Next"

5. **Disks:**
   - **Bus/Device:** SCSI (VirtIO SCSI best performance)
   - **Storage:** local-lvm
   - **Disk size:** 32GB (adjust as needed)
   - **Cache:** Write back (best performance)
   - **Discard:** Enable for thin provisioning
   - **SSD emulation:** Enable if on SSD
   - Click "Next"

6. **CPU:**
   - **Sockets:** 1
   - **Cores:** 2-4
   - **Type:** host (best performance)
   - Click "Next"

7. **Memory:**
   - **Memory:** 2048-4096 MB (2-4GB)
   - **Minimum memory:** Leave blank (ballooning disabled)
   - Click "Next"

8. **Network:**
   - **Bridge:** vmbr0 (default)
   - **Model:** VirtIO (paravirtualized, best performance)
   - Click "Next"

9. **Confirm:**
   - **Start after created:** Check this
   - Click "Finish"

10. **Access VM Console**
    - Click on VM → Console
    - Proceed with OS installation

### Step 9: Install Guest Agent (After OS Install)

**Linux (Ubuntu/Debian):**
```bash
sudo apt update
sudo apt install qemu-guest-agent
sudo systemctl enable qemu-guest-agent
sudo systemctl start qemu-guest-agent
```

**Windows:**
- Mount VirtIO driver ISO to VM
- Run virtio-win-guest-tools.exe installer
- Reboot

**Enable in Proxmox:**
- VM → Options → QEMU Guest Agent → Edit → Enable

---

## VMware ESXi Setup Guide (Alternative)

### Step 1: Download ESXi

1. Create free VMware account: https://customerconnect.vmware.com/
2. Download ESXi 8.0 ISO (free version)
3. Note your license key (provided after registration)

### Step 2: Create Bootable USB

**Windows:**
- Use Rufus: Select ISO, write to USB in DD mode

**Linux:**
```bash
dd if=ESXi-8.0.iso of=/dev/sdX bs=4M status=progress
```

### Step 3: Installation

1. **Boot from USB**
2. **ESXi Installer**
   - Press Enter to continue
   - Accept EULA (F11)
   - Select installation disk
   - Select keyboard layout
   - Enter root password
   - Confirm installation (F11)
   - Reboot when complete

### Step 4: Initial Configuration

1. **Configure Management Network**
   - Press F2 at boot screen
   - Login with root password
   - Configure Management Network → IPv4 Configuration
   - Set static IP address
   - Set subnet mask, gateway, DNS
   - Test connectivity (ping gateway)

2. **Access Web Interface**
   - Open browser: https://esxi-ip-address
   - Login with root credentials
   - Enter license key (or use evaluation mode)

### Step 5: Create Datastore

1. **Storage → Datastores → New datastore**
2. Select unused disk
3. Choose filesystem (VMFS6)
4. Name datastore
5. Confirm creation

### Step 6: Upload ISOs

1. **Storage → Datastores → datastore1 → Browse**
2. Create "ISO" folder
3. Upload ISO files

### Step 7: Create VM

1. **Virtual Machines → Create/Register VM**
2. Select "Create a new virtual machine"
3. Configure name, OS type
4. Select storage
5. Customize hardware (CPU, RAM, disk, network)
6. Mount ISO
7. Finish and power on

---

## Microsoft Hyper-V Server Setup (Windows-Focused)

### Step 1: Download

1. Download Hyper-V Server 2022 (Free): https://www.microsoft.com/en-us/evalcenter/evaluate-hyper-v-server-2022
2. Create bootable USB with Rufus

### Step 2: Installation

1. Boot from USB
2. Select language, time, keyboard
3. Click "Install now"
4. Select disk
5. Wait for installation
6. Set Administrator password

### Step 3: Initial Configuration (sconfig)

1. **Server Configuration Tool (sconfig) launches automatically**
2. Configure:
   - Option 2: Computer name
   - Option 8: Network settings (static IP)
   - Option 6: Download and install updates
   - Option 12: Log off (apply settings)

### Step 4: Enable Remote Management

```powershell
# Enable PowerShell remoting
Enable-PSRemoting -Force

# Enable Hyper-V management
Set-NetFirewallRule -DisplayGroup "Hyper-V Management Clients" -Enabled True
```

### Step 5: Manage from Windows PC

1. **Install Hyper-V Management Tools on Windows 10/11 PC**
   - Settings → Apps → Optional Features → Add Feature
   - Install "Hyper-V Management Tools"

   Or via PowerShell:
   ```powershell
   Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Management-PowerShell
   ```

2. **Connect to Hyper-V Server**
   - Open Hyper-V Manager
   - Connect to Server → Enter Hyper-V Server IP
   - Enter credentials

### Step 6: Create VMs via Hyper-V Manager

- Follow standard Hyper-V VM creation wizard
- Configure virtual switches for networking
- Create VMs as needed

---

## Networking in Homelab Type 1 Hypervisors

### Basic Concepts

**Virtual Bridge:**
- Connects VMs to physical network
- VMs get IPs from your router (DHCP)
- VMs accessible from LAN

**NAT:**
- VMs share host's IP
- Outbound access only
- Not directly accessible from LAN

**Internal Network:**
- VMs talk to each other only
- No external access
- Isolated testing environment

### Proxmox Network Configuration

**Default:** Linux Bridge (vmbr0) connected to physical NIC

**Add Additional Bridge:**
1. Node → Network → Create → Linux Bridge
2. Configure IP (or leave blank for VM-only bridge)
3. Link to physical NIC or leave for internal network
4. Apply configuration

**VLANs (Advanced):**
```bash
# Edit /etc/network/interfaces
auto vmbr0.10
iface vmbr0.10 inet manual
    vlan-raw-device vmbr0
```

### ESXi Network Configuration

**Virtual Switch (vSwitch):**
- Networking → Virtual switches → Add standard virtual switch
- Link to physical NIC (uplink)
- Assign VMs to port groups

**Port Groups:**
- Create port groups for different network segments
- Assign VLANs if needed
- Attach VMs to appropriate port group

---

## Homelab Best Practices

### 1. Documentation

**Keep records of:**
- IP address assignments
- VM purposes and configurations
- Network topology
- Passwords (in password manager)
- Changes and updates

### 2. Backups

**Proxmox:**
- Built-in backup: Datacenter → Backup → Add
- Schedule regular backups to external storage
- Test restore procedures

**ESXi:**
- Free version: Use ghettoVCB script
- Export VM templates for quick rebuilds
- Snapshot before major changes (don't keep long-term)

### 3. Updates and Patching

- Subscribe to security mailing lists
- Update hypervisor regularly
- Test updates on non-critical VMs first
- Schedule maintenance windows

### 4. Resource Management

- Don't over-allocate RAM (hypervisor needs headroom)
- Monitor CPU usage and adjust VM cores
- Use thin provisioning for disk space
- Set up monitoring (Prometheus, Grafana)

### 5. Security

- Change default passwords
- Enable firewall rules
- Separate management network (if possible)
- Use VLANs for network segmentation
- Regular security audits
- Keep SSH keys secure

### 6. Power Management

- Configure UPS shutdown scripts
- Graceful VM shutdowns before host shutdown
- Test power failure scenarios

---

## Common Homelab Use Cases

### Learning Lab
- Active Directory domain
- Windows Server practice
- Linux distribution testing
- Networking practice (pfSense/OPNsense)

### Home Infrastructure
- Home automation (Home Assistant VM)
- Network services (DNS, DHCP, VPN)
- Media server (Plex/Jellyfin)
- File server (TrueNAS, Samba)

### Development Environment
- Web server testing (LAMP/LEMP stacks)
- Container orchestration (Kubernetes)
- CI/CD pipelines (Jenkins, GitLab)
- Database testing

### Security and Testing
- Security tools (Kali Linux VM)
- Malware analysis (isolated network)
- Penetration testing practice
- Vulnerability scanning

---

## Troubleshooting Common Issues

### VM Won't Start

**Check:**
- Sufficient RAM available on host
- Storage space available
- CPU virtualization enabled in BIOS
- VM configuration valid (especially boot order)

**Proxmox:**
```bash
qm start <vmid> --verbose
# Check logs: /var/log/syslog
```

### No Network Connectivity

**Check:**
- Virtual NIC connected in VM settings
- Correct bridge/vSwitch assignment
- Physical NIC linked to bridge
- Cable connected to physical NIC
- DHCP/static IP configured in guest OS

### Performance Issues

**Optimize:**
- Install guest agent/tools
- Use VirtIO drivers (paravirtualization)
- Allocate appropriate CPU cores (don't over-allocate)
- Enable CPU host passthrough
- Use SSD storage with caching enabled
- Check host resource utilization

### Cannot Access Web Interface

**Check:**
- Correct IP address and port
- Firewall not blocking (8006 for Proxmox, 443 for ESXi)
- Service running: `systemctl status pveproxy` (Proxmox)
- Network cable connected
- Try from different device/network

---

## Cost Considerations

### Hardware Budget

**Entry Level ($300-500):**
- Used Dell R720/HP DL380 Gen8
- 32-64GB RAM
- 2x Xeon CPUs
- Refurbished enterprise servers

**Mid-Range ($500-1000):**
- Newer used server (Dell R730, HP DL380 Gen9)
- 64-128GB RAM
- More efficient CPUs (lower power)

**DIY Build ($600-1500):**
- Consumer/workstation parts
- AMD Ryzen or Intel Core
- Quieter and more power-efficient
- ECC RAM recommended but not required

**High-End ($1500+):**
- Modern server hardware
- AMD EPYC or Intel Xeon Scalable
- 128GB+ RAM
- NVMe storage
- 10Gbps networking

### Operating Costs

**Power Consumption:**
- Enterprise servers: 150-400W idle
- DIY builds: 50-150W idle
- Calculate: kWh × hours × electricity rate
- Consider power-efficient CPUs

**Cooling:**
- Enterprise servers are LOUD
- May require separate room or soundproofing
- Factor in additional cooling costs

---

## Resources and Community

### Documentation
- **Proxmox:** https://pve.proxmox.com/wiki/Main_Page
- **ESXi:** https://docs.vmware.com/en/VMware-vSphere/
- **Hyper-V:** https://docs.microsoft.com/en-us/virtualization/

### Communities
- **Reddit:** r/homelab, r/proxmox, r/vmware
- **Forums:** Proxmox Forums, VMware Communities
- **Discord:** Homelab Discord servers

### YouTube Channels
- Craft Computing
- NetworkChuck
- Learn Linux TV
- Techno Tim

---

## Summary

**Type 1 hypervisors provide bare-metal virtualization for homelabs with:**
- Superior performance and efficiency
- Enterprise-grade features
- Professional skill development
- Flexible infrastructure

**Recommended Starting Point:**
- **Hypervisor:** Proxmox VE (free, easy, powerful)
- **Hardware:** Used enterprise server or DIY build (32GB+ RAM)
- **First VMs:** Linux server, pfSense router, Windows Server
- **Next Steps:** Experiment, document, expand, learn

**Key Takeaways:**
1. Type 1 hypervisors run directly on hardware for best performance
2. Proxmox VE is the most beginner-friendly option
3. Plan your network and storage before installation
4. Start small and expand as you learn
5. Documentation and backups are critical

Your homelab journey starts with installing that first hypervisor. Pick one, install it, and start building!
