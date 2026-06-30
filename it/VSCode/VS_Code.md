# Visual Studio Code Setup for C# .NET 10 Development

## Overview

This guide provides step-by-step instructions for setting up Visual Studio Code on Windows 11 as your development environment for C# .NET 10 applications. By the end, you'll have a working development environment ready to build console applications that fetch JSON from web APIs and store data in PostgreSQL.

**Estimated setup time:** 30-45 minutes

---

## Step 1: Setting Up Visual Studio Code

### Download and Install VS Code

1. **Download VS Code:**
   - Go to: <a href="https://code.visualstudio.com/download" target="_blank">https://code.visualstudio.com/download</a>
   - The Windows installer will download automatically
   - Save the file (`VSCodeSetup-x64.exe` or `VSCodeSetup-x86.exe`)

2. **Install VS Code:**
   - Double-click the downloaded installer
   - Click "Yes" on the User Account Control prompt
   - Follow the installation wizard:
     - **Accept the license** and click "Next"
     - **Installation location:** Keep default, click "Next"
     - **Select Start Menu Folder:** Keep default, click "Next"
     - On the Additional Tasks screen, check ALL boxes:
       - ✓ Add "Open with Code" action to Windows Explorer file and folder context menus
       - ✓ Add "Open with Code" action to Windows Explorer directory context menus
       - ✓ Register Code as an editor for supported file types
       - ✓ **Add to PATH (requires shell restart)** ← This is important!
       - ✓ Register Code as a file for VS Code Project files
     - Click "Install"
   - Click "Finish" when installation completes
   - VS Code will launch automatically

3. **First launch:**
   - When VS Code opens, you'll see a welcome screen
   - You can skip the "Import Settings from VS Code" prompt
   - On the bottom right, you may see a notification to install recommended extensions — dismiss this for now (we'll install them in Step 4)

### Basic VS Code Orientation (First-Time User)

**The VS Code Interface:**

| Component | Location | Purpose |
|-----------|----------|---------|
| **Activity Bar** | Far left vertical bar | Contains icons for Explorer (files), Search, Source Control (Git), Extensions (add-ons), and Debug |
| **Explorer** | Left side panel (when active) | Shows your project files and folders in a tree view |
| **Editor Area** | Center of window | Where your code files are displayed and edited |
| **Status Bar** | Bottom of window | Shows current branch, file encoding, line ending, current language |
| **Terminal** | Bottom panel (can be toggled) | Command-line interface within VS Code |
| **Command Palette** | Press `Ctrl + Shift + P` | Search for any command in VS Code |
| **Output Panel** | Bottom panel (can be toggled) | Shows build output, logs, and tool messages |

**Essential Keyboard Shortcuts (Windows):**

| Shortcut | Action |
|----------|--------|
| `Ctrl + N` | New file |
| `Ctrl + O` | Open file |
| `Ctrl + Shift + F` | Open folder/workspace |
| `Ctrl + S` | Save file |
| `Ctrl + P` | Quick open (search for files) |
| `Ctrl + ` ` ` (backtick) | Toggle integrated terminal |
| `Ctrl + B` | Toggle side bar |
| `Ctrl + Shift + B` | Run build task |
| `Ctrl + Shift + P` | Open Command Palette |
| `Ctrl + ` (backtick) | Open integrated terminal |
| `Ctrl + K, Ctrl + S` | View keyboard shortcuts |

**Getting Started Navigation:**

1. **Open a folder:** Press `Ctrl + Shift + F` or click "Open Folder" on welcome screen
   - Navigate to your project folder
   - Click "Select Folder"
   - Explorer panel will show all files in your project

2. **Create a new file:** In Explorer, right-click on your project folder → "New File"
   - Name the file (e.g., `Program.cs`)
   - Click anywhere in the editor to start typing

3. **Use the Terminal:** Press `Ctrl + `` (backtick) to open the integrated terminal
   - Run `dotnet --version` to verify .NET SDK
   - All commands shown in this guide can be run in this terminal

4. **Change between views:** Click icons on the Activity Bar (left side)
   - Files icon = Explorer (file management)
   - Search icon = Search (find and replace across project)
   - Branch icon = Source Control (Git operations)

**VS Code Tips for C# Development:**

- **IntelliSense:** As you type C# code, VS Code will show suggestions (pressed `Ctrl + Space` to trigger)
- **Inline hints:** C# Dev Kit shows parameter names inline when you call methods
- **Code formatting:** Press `Shift + Alt + F` to auto-format your code
- **Go to definition:** Right-click on any method name and select "Go to Definition" (or press `F12`)
- **Find all references:** Right-click → "Find All References" to see where a method is used

**Customizing the Layout:**

- **Dark theme (recommended):** `Ctrl + K, Ctrl + T` → Select "Dark (Visual Studio)" or "Dark Modern"
- **Adjust font size:** `Ctrl + Mouse Wheel` to zoom in/out
- **Split editor:** Drag a tab to the side to create a side-by-side view
- **Dock the terminal:** Click the `x` on the terminal panel to undock, or use `Ctrl + Shift + `` to toggle

**Note:** VS Code is highly customizable. Don't worry about memorizing everything — you'll discover features as you need them. The most important thing is that you can navigate files, open the terminal, and start writing code.

---

## Step 1.5: Setting Up a Local LLM for AI-Assisted Coding

For AI-assisted coding within VS Code without sending code to external services, you can run a local language model. This provides privacy and offline capability while still giving you intelligent code completion and generation.

### Option A: Use Ollama (Recommended for Beginners)

Ollama is the easiest way to run local LLMs on Windows, with excellent VS Code integration.

#### 1. Install Ollama

**Download Ollama:**
- Navigate to: <a href="https://ollama.com/download/windows" target="_blank">https://ollama.com/download/windows</a>
- Download the Windows installer
- Run the installer and follow prompts
- Click "Finish" when complete

**Verify installation:**
```powershell
ollama --version
```

#### 2. Pull Your First Model

Ollama supports many models. For coding assistance, recommended models include:

| Model | Size | Best For | Command |
|-------|------|----------|---------|
| **phi4** | ~4GB | Fast code completion | `ollama pull phi4` |
| **codellama** | ~4-7GB | Pure coding tasks | `ollama pull codellama` |
| **qwen2.5-coder** | ~2-7GB | Best overall coding | `ollama pull qwen2.5-coder` |
| **llama3.2** | ~3GB | General coding help | `ollama pull llama3.2` |

**Recommended for C# development:**
```powershell
ollama pull qwen2.5-coder:7b
```

This gives you a model trained specifically on code across multiple languages including C#.

#### 3. Test Your Model

```powershell
# Start an interactive session with the model
ollama run qwen2.5-coder:7b

# Type a coding question:
# "How do I connect to PostgreSQL in C#?"
```

#### 4. Configure VS Code for Local LLM

**Install Continue Extension:**
- Open VS Code
- Press `Ctrl + Shift + X` to open Extensions
- Search for **"Continue"**
- Install the **Continue** extension by Continue.dev

**Setup Continue for Remote Ollama Server:**

Since your Ollama server is running on a remote machine (100.79.83.113), you'll configure Continue to connect to it.

1. **Install Continue Extension:**
   - In VS Code, press `Ctrl + Shift + X`
   - Search for **"Continue"**
   - Install the **Continue** extension by Continue.dev

2. **Configure Continue:**
   - Press `Ctrl + Shift + P` → "Continue: Open Settings"
   - This opens your configuration file

3. **Update the configuration file with your remote Ollama endpoint:**

**Option A: Workspace-specific configuration (`.vscode/.continue/config.json`):**

```json
{
  "models": [
    {
      "title": "Remote Ollama - Qwen3 Coder",
      "provider": "ollama",
      "model": "qwen3-coder:30b",
      "apiBase": "http://100.79.83.113:1134"
    }
  ],
  "tabAutocompletionModel": {
    "title": "Remote Ollama - Fast",
    "provider": "ollama",
    "model": "phi4",
    "apiBase": "http://100.79.83.113:1134"
  }
}
```

**Option B: User-wide configuration (`~/.continue/config.json`):**

If you want this configuration available in all your VS Code workspaces:

```json
{
  "models": [
    {
      "title": "Remote Ollama - Qwen3 Coder",
      "provider": "ollama",
      "model": "qwen3-coder:30b",
      "apiBase": "http://100.79.83.113:1134"
    }
  ],
  "tabAutocompletionModel": {
    "title": "Remote Ollama - Fast",
    "provider": "ollama",
    "model": "phi4",
    "apiBase": "http://100.79.83.113:1134"
  }
}
```

**Configuration Notes:**
- `apiBase`: The URL of your remote Ollama server
- `model`: The model name to use (must match what's pulled on the server)
- `phi4` for fast tab completions (if available on your server)
- `qwen3-coder:30b` for chat and code generation

**Important:** If you have multiple models on your server, ensure:
1. The model is pulled on the server: `ollama pull qwen3-coder:30b`
2. The model name exactly matches what's on the server (case-sensitive)

**Key Features After Setup:**
- **Ctrl+L** — Open chat panel (ask coding questions, get explanations)
- **Ctrl+Shift+L** — Edit selected code with AI
- **Tab autocompletion** — Press Tab to accept suggested completions
- **"Add Code to Chat"** — Right-click any code and select "Add to Chat" for context
- **Slash commands:**
  - `/explain` — Explain the selected code
  - `/fix` — Suggest fixes for errors
  - `/docs` — Find documentation references
  - `/commit` — Generate commit messages

### Option B: Use LM Studio + Open WebUI

An alternative approach using LM Studio for model management.

#### 1. Install LM Studio

- Navigate to: <a href="https://lmstudio.ai/" target="_blank">https://lmstudio.ai/</a>
- Download LM Studio for Windows
- Install and run

#### 2. Find and Download a Model

- Use the search bar in LM Studio to find coding models
- Look for models tagged with "coding" or "llama"
- Download a model (typically `.gguf` format)

#### 3. Start Local Server

- Go to "Server" tab in LM Studio
- Select your downloaded model
- Click "Start Server"
- Note the local URL (typically `http://localhost:1234`)

#### 4. Configure VS Code

Use the **"LM Studio"** VS Code extension:
- Install LM Studio extension
- Connect to your local server
- Configure in extension settings

### Option C: Use Ollama WebUI + Continue

For a better model discovery experience:

#### 1. Install Ollama WebUI (optional)

```powershell
# Pull WebUI container
docker run -d -p 3000:8000 -v ollama:/root/.ollama -e OLLAMA_HOST=0.0.0.0 --name ollama-webui ollama-webui:latest
```

Then access `http://localhost:3000` in your browser for a web interface to browse and pull models.

### Model Comparison for Development

| Use Case | Recommended Model | VRAM Required | Inference Speed |
|----------|-------------------|---------------|-----------------|
| **Fast autocompletion** | phi4 (4GB) | 4GB | Very Fast |
| **General coding help** | qwen2.5-coder:7b | 8GB | Fast |
| **Complex code generation** | codellama:70b | 48GB | Slow |
| **Balanced option** | llama3.2:1b/3b | 4-8GB | Fast |

**For most developers starting out:**
- Start with `phi4` for autocompletion
- Add `qwen2.5-coder:7b` for chat interactions
- You can switch between them as needed

### Alternative: Use VS Code's Built-in Copilot with Local Backend

If you want GitHub Copilot features but with local models:

1. **Use Copilot with Ollama provider** — Configure Copilot to use local models
2. **Use Codeium** — Free alternative with local model support
3. **Use Tabnine** — Has local deployment option for enterprise

### Privacy and Security Notes

**Local LLM Advantages:**
- ✓ Your code never leaves your machine
- ✓ Works offline
- ✓ No API costs
- ✓ Full control over which models you run

**Considerations:**
- ⚠ Local models are smaller than cloud models (less capable on complex tasks)
- ⚠ Requires decent hardware (8GB+ RAM recommended)
- ⚠ Inference can be slow on CPU-only systems (consider GPU for faster results)
- ⚠ First model download may take 2-5 minutes (3-8GB file)

### Troubleshooting Local LLM Setup

**Issue: Ollama not found after installation**
```powershell
# Restart your terminal and try again
ollama --version
```

**Issue: Model download failing**
```powershell
# Check your Ollama installation location
ollama list

# Ensure you have disk space (typically 5-10GB for coding models)
```

**Issue: Continue extension not connecting**
```powershell
# Verify Ollama is running
ollama list

# Check if Ollama server is accessible
curl http://localhost:11434
```

**Issue: Slow code completion**
- Try switching to a smaller model for autocompletion (`phi4`)
- Ensure you have enough RAM available
- Consider enabling GPU acceleration if you have NVIDIA GPU

---

## Remote Ollama Server Configuration

### Connecting to Your Server at 100.79.83.113:1134

**1. Verify the Ollama server is accessible:**

```powershell
# Test the connection
curl http://100.79.83.113:1134

# Should return: "Ollama is running"

# List available models on the server
curl http://100.79.83.113:1134/api/tags
```

**2. Verify the model is pulled on the server:**

```powershell
# Check if qwen3-coder:30b is available
curl http://100.79.83.113:1134/api/tags | ConvertFrom-Json | Select-Object -ExpandProperty models | Where-Object {$_.name -like "*qwen3-coder*"}
```

**3. Pull the model if not present:**

On the **Ollama server machine** (100.79.83.113):
```powershell
ollama pull qwen3-coder:30b
```

**4. Configure firewall:**

Ensure port **1134** is accessible from your local machine:
```powershell
# From your local machine, test connectivity:
Test-NetConnection -ComputerName 100.79.83.113 -Port 1134
```

**5. If using HTTPS or custom certificates:**

For servers with SSL certificates, update the configuration:

```json
{
  "models": [
    {
      "title": "Remote Ollama - Qwen3 Coder",
      "provider": "ollama",
      "model": "qwen3-coder:30b",
      "apiBase": "https://100.79.83.113:1134",
      "allowInsecure": true
    }
  ]
}
```

**6. Authentication (if your server requires it):**

```json
{
  "models": [
    {
      "title": "Remote Ollama - Qwen3 Coder",
      "provider": "ollama",
      "model": "qwen3-coder:30b",
      "apiBase": "http://100.79.83.113:1134",
      "headers": {
        "Authorization": "Bearer YOUR_API_KEY"
      }
    }
  ]
}
```

**7. Continue Extension Settings:**

After configuring, press `Ctrl + Shift + P` → "Continue: Reload Window" to apply changes.

**8. Verify connection in Continue:**

1. Open the Continue sidebar (Ctrl + L)
2. Click on the model dropdown (top right)
3. You should see "Remote Ollama - Qwen3 Coder"
4. Select it and try asking a simple question

### Quick Setup Summary

```powershell
# 1. Install Ollama from https://ollama.com/download/windows
# 2. Install "Continue" extension in VS Code
# 3. Pull a coding model:
ollama pull qwen2.5-coder:7b

# 4. Configure .vscode/.continue/config.json (as shown above)
# 5. Reload VS Code: Ctrl + Shift + P → "Reload Window"

# Test: Press Ctrl + L to open the Continue chat
```

**Estimated additional setup time:** 10-15 minutes

---

## Step 2: Working with NuGet Packages

NuGet is the package manager for .NET. It allows you to easily add, update, and remove external libraries and dependencies for your projects.

### Understanding NuGet

**What is NuGet?**
- Package manager for .NET that provides access to thousands of reusable libraries
- Packages are distributed as `.nupkg` files (essentially ZIP archives)
- All packages are hosted on [nuget.org](https://www.nuget.org) (public) or private feeds
- Package references are stored in your `.csproj` file

### Discovering Packages

#### Option 1: Visual Studio Marketplace in VS Code

1. **Open the Extensions view:** `Ctrl + Shift + X`
2. **Search for "NuGet"** and install "NuGet Package Viewer" extension
3. **Browse packages directly from VS Code**

#### Option 2: Online NuGet Gallery

Visit: <a href="https://www.nuget.org/packages" target="_blank">https://www.nuget.org/packages</a>

**To search for a package:**
1. Type your keyword in the search bar (e.g., "PostgreSQL", "HttpClient")
2. Review the top results
3. Click on a package to see:
   - Description and features
   - Version history
   - Download count and ratings
   - Dependencies
   - Installation instructions

#### Option 3: Command Line Search

```powershell
# Search for packages related to "PostgreSQL"
dotnet search postgresql

# Or use nuget.org directly via CLI
nuget search postgresql -Source https://api.nuget.org/v3/index.json
```

### Adding Packages to Your Project

#### Method A: Using Package Manager Console (Recommended for Beginners)

1. **Open Package Manager Console:**
   - In VS Code, press `Ctrl + Shift + P`
   - Type "Open Integrated Terminal" and press Enter
   - Run the command below

2. **Add a package:**
   ```powershell
   # Syntax
   dotnet add package <PackageName>

   # Example: Add Npgsql for PostgreSQL
   dotnet add package Npgsql

   # Example: Add System.Text.Json for JSON parsing
   dotnet add package System.Text.Json

   # Example: Add a specific version
   dotnet add package Newtonsoft.Json --version 13.0.3
   ```

#### Method B: Adding to .csproj Manually

For projects using SDK-style format (`.csproj`), you can manually add package references:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- Existing packages -->
    <PackageReference Include="Npgsql" Version="8.0.5" />
    
    <!-- Add new packages here -->
    <PackageReference Include="System.Text.Json" Version="8.0.5" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.1" />
  </ItemGroup>

</Project>
```

**Then restore packages:**
```powershell
dotnet restore
```

#### Method C: Using IntelliSense in VS Code

As you type in your `Program.cs` or other `.cs` files:

1. **Type `#r` for script files or let IntelliSense kick in for `.cs` files**
2. **Type `using` followed by a namespace** (e.g., `using Npgsql`)
3. **If the package isn't installed, VS Code will show a lightbulb icon**
4. **Click the lightbulb** → "Add Package Reference"
5. **Select the version** you want to install
6. **VS Code automatically adds the package** and restores it

### Popular Packages for Your JSON Parser Project

Here are recommended packages for fetching JSON and connecting to PostgreSQL:

| Package | NuGet ID | Description | Install Command |
|---------|----------|-----|-----------|
| **Npgsql** | `Npgsql` | ADO.NET provider for PostgreSQL | `dotnet add package Npgsql` |
| **System.Text.Json** | `System.Text.Json` | Modern JSON serialization (built into .NET) | `dotnet add package System.Text.Json` |
| **Microsoft.Extensions.Http** | `Microsoft.Extensions.Http` | HttpClient factory and middleware | `dotnet add package Microsoft.Extensions.Http` |
| **Microsoft.Extensions.Configuration** | `Microsoft.Extensions.Configuration` | Configuration system | `dotnet add package Microsoft.Extensions.Configuration` |
| **Microsoft.Extensions.Configuration.Json** | `Microsoft.Extensions.Configuration.Json` | JSON configuration file support | `dotnet add package Microsoft.Extensions.Configuration.Json` |
| **Microsoft.Extensions.Configuration.UserSecrets** | `Microsoft.Extensions.Configuration.UserSecrets` | Secure secret storage for dev | `dotnet add package Microsoft.Extensions.Configuration.UserSecrets` |
| **Polly** | `Polly` | Resilience and transient fault handling | `dotnet add package Polly` |
| **Serilog** | `Serilog` |Simple logging | `dotnet add package Serilog` |
| **Serilog.Sinks.Console** | `Serilog.Sinks.Console` | Console output for Serilog | `dotnet add package Serilog.Sinks.Console` |

### Managing Package Versions

**View current packages in your project:**
```powershell
# List all packages with versions
dotnet list package

# Or view the .csproj file directly for PackageReference entries
```

**Update a package to latest version:**
```powershell
dotnet add package Npgsql --prerelease
dotnet add package Npgsql --version 8.0.5
```

**Downgrade a package:**
```powershell
dotnet add package Npgsql --version 7.0.0
```

**Remove a package:**
```powershell
dotnet remove package Npgsql
```

### NuGet Configurations (nuget.config)

For advanced scenarios like private package sources, create a `nuget.config` file in your project root:

**Example: `nuget.config` for project with private feed:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!-- Clear existing sources first -->
    <clear />
    
    <!-- Your private feed -->
    <add key="MyCompanyPackages" value="https://nuget.yourcompany.com/v3/index.json" />
    
    <!-- Public NuGet gallery -->
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  
  <packageSourcesToBuild>
    <add key="MyCompanyPackages" />
  </packageSourcesToBuild>
</configuration>
```

**Add package source from command line:**
```powershell
# Add a new package source
dotnet nuget add source --name "MyPackages" --source "https://myfeeds.nuget.org/v3/index.json"

# List all configured sources
dotnet nuget list source

# Remove a source
dotnet nuget remove source --name "MyPackages"
```

### Private NuGet Feeds

If you have your own internal NuGet packages to share:

#### Azure Artifacts (Microsoft DevOps)

1. **Create a feed in Azure DevOps**
2. **Configure nuget.config:**
   ```xml
   <configuration>
     <packageSources>
       <add key="AzureArtifacts" value="https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json" />
       <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
     </packageSources>
     <credentials>
       <AzureArtifacts>
         <clear />
         <add keyUsername="yourusername" value="YOUR_PAT_TOKEN" />
       </AzureArtifacts>
     </credentials>
   </configuration>
   ```

#### Internal File Share

```powershell
# Add a file-based feed
dotnet nuget add source --name "LocalFeed" --source "\\fileserver\share\NuGetPackages"
```

### Working with Prerelease Packages

```powershell
# Add prerelease version
dotnet add package MyPackage --prerelease

# View available prerelease versions
dotnet list package --include-transitive
```

### Troubleshooting Common NuGet Issues

**Issue: "Could not find a part of the path" during restore**

**Solution:**
```powershell
# Clean and restore
dotnet clean
dotnet restore
```

**Issue: "Could not resolve this reference" after adding package**

**Solution:**
```powershell
# Reload workspace in VS Code
Ctrl + Shift + P → "C# Dev Kit: Reload Workspace"

# Or simply restart VS Code
```

**Issue: Package version conflict**

**Solution:**
```powershell
# View dependency tree
dotnet list package --include-transitive

# Check for conflicting versions
dotnet restore --verbosity diagnostic
```

**Issue: "Authentication is required to access this source"**

**Solution:**
```powershell
# Check your nuget.config for authentication requirements
cat nuget.config

# Set authentication credentials
dotnet nuget setApiKey YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### Best Practices

**1. Pin specific versions:**
- Avoid using latest versions in production code
- Always specify versions in `.csproj` for reproducibility

**2. Update packages regularly:**
```powershell
# Update all packages to latest compatible versions
dotnet list package --outdated
```

**3. Use `global.json` for SDK version pinning:**
```json
{
  "sdk": {
    "version": "10.0.x",
    "rollForward": "latestMinor"
  }
}
```

**4. Don't commit `obj/` or `bin/`:**
- These are generated during build
- Should be in `.gitignore`

**5. Use `user-secret` for development secrets:**
- Never commit database passwords to source control
- See **Step 7: Configure .NET User Secrets**

---

## Step 3: Install .NET 10 SDK

### Download and Install

1. **Download .NET 10 SDK:**
   - Navigate to the .NET download page: <a href="https://dotnet.microsoft.com/download" target="_blank">https://dotnet.microsoft.com/download</a>
   - Scroll to ".NET 10 (LTS)" section
   - Click the Windows x64 Installer for ".NET 10.0 SDK"
   - Save the installer (`dotnet-sdk-10.0.x-win-x64.exe`)

2. **Run the installer:**
   - Double-click the downloaded installer
   - Accept the license terms and click "Install"
   - Wait for installation to complete
   - Click "Finish"

3. **Restart your terminal:**
   - Close all open command prompts/PowerShell windows
   - Open a new PowerShell or Command Prompt

4. **Verify installation:**
   ```powershell
   dotnet --version
   dotnet --list-sdks
   ```
   - You should see version `10.0.x` listed

---

## Step 3: Install PostgreSQL

### Download and Install

1. **Download PostgreSQL:**
   - Navigate to PGSQL download page: <a href="https://www.postgresql.org/download/windows/" target="_blank">https://www.postgresql.org/download/windows/</a>
   - Click the link for version 16 or later (latest stable)
   - Download the Windows x64 installer

2. **Run the installer:**
   - Double-click the installer
   - Follow these settings:
     - **Directory:** Keep default (`C:\Program Files\PostgreSQL\16\`)
     - **Superuser password:** Create a strong password (remember this!)
     - **Port:** 5432 (default)
     - **Locale:** English (United States) - or your preference
     - **Components:** Check all options (PostgreSQL Server, pgAdmin 4, Stack Builder)
   - Click "Install"
   - When complete, check "Launch pgAdmin" and click "Finish"

3. **Create a database for development:**
   - Open pgAdmin 4 from Start menu
   - Expand Servers → PostgreSQL 16 → Databases
   - Right-click "Databases" → "Create" → "Database"
   - Name: `jsonapp` (or your preferred name)
   - Owner: postgres (default)
   - Click "Save"

4. **Create a development user:**
   - In pgAdmin, expand your PostgreSQL server
   - Right-click "Login/Group roles" → "Create" → "Role"
   - Name: `appuser`
   - Password: Set a secure password
   - Go to "Definition" tab → Check "Superuser" (optional, but useful for dev)
   - Go to "SQL" tab (below) → Click "SQL" button
   - In SQL window, run:
     ```sql
     CREATE DATABASE jsonapp_dev OWNER appuser;
     GRANT ALL PRIVILEGES ON DATABASE jsonapp_dev TO appuser;
     ```
   - Click "Save"

5. **Verify PostgreSQL installation:**
   ```powershell
   psql -U appuser -d jsonapp_dev -h localhost -p 5432
   ```
   - Enter password when prompted
   - You should see `jsonapp_dev=#` prompt

---

## Step 4: Install VS Code Extensions

### Essential Extensions for C# Development

1. **Open VS Code**

2. **Install C# Dev Kit (Required):**
   - Click the Extensions icon (four squares) on left sidebar
   - Search for "C#"
   - Install the **C# Dev Kit** by Microsoft
   - This automatically installs the C# extension by Microsoft

3. **Install Npgsql (PostgreSQL support):**
   - Search for "Npgsql"
   - Install **Npgsql for Visual Studio** by .NET Foundation

4. **Install Recommended Extensions:**
   - **GitLens** - Enhanced Git capabilities
   - **Powershell** - PowerShell support
   - **SQL Server (mssql)** - Query and manage SQL Server databases
   - **PostgreSQL IntelliSense** - Enhanced IntelliSense for PostgreSQL
   - **C# Playground** - Share and test C# code snippets

5. **Verify installed extensions:**
   - Extensions sidebar should show:
     - C# Dev Kit
     - C# (Microsoft)
     - Npgsql
     - GitLens
     - PowerShell

---

## Step 5: Configure Workspace Settings

### Recommended VS Code Settings

1. **Create workspace settings:**
   - In VS Code, press `Ctrl + Shift + P`
   - Type "Open Workspace Settings (JSON)"
   - Select it to open `.vscode/settings.json`

2. **Add the following configuration:**

```json
{
    "dotnet.defaultSolution": "jsonparser.sln",
    "editor.defaultFormatter": "ms-dotnettools.csharp",
    "editor.formatOnSave": true,
    "editor.codeActionsOnSave": {
        "source.fixAll.csharp": "explicit"
    },
    "files.associations": {
        "*.cs": "csharp"
    },
    "files.exclude": {
        "**/bin": true,
        "**/obj": true,
        "**/.git": true
    },
    "omnisharp.enableImportCompletion": true,
    "omnisharp.enableRoslynAnalyzers": true,
    "omnisharp.enableEditorConfigSupport": true,
    "Csharp.showUsagesInQuickFix": true,
    "editor.rulers": [120],
    "terminal.integrated.profiles.windows": {
        "PowerShell": {
            "source": "PowerShell",
            "icon": "terminal-powershell",
            "args": ["-NoProfile", "-Command", "$env:PSModulePath = $env:PSModulePath -split ';' | Where-Object { $_ }; $PSModulePath" ]
        }
    },
    "terminal.integrated.defaultProfile.windows": "PowerShell"
}
```

3. **Configure .NET User Secrets:**
   - This allows secure storage of database credentials

---

## Step 6: Create Your First .NET 10 Project Structure

### Project File Structure

Create a dedicated folder for your development work:

```
C:\Development\VSCodeProjects\
├── jsonparser/
│   ├── jsonparser.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── .config/
│       └── dotnet-tools.json
└── .gitignore
```

### Create the project folder

Open PowerShell and run:

```powershell
# Create development directory
New-Item -ItemType Directory -Force -Path "C:\Development\VSCodeProjects"
Set-Location "C:\Development\VSCodeProjects"

# Create project directory
New-Item -ItemType Directory -Force -Path "jsonparser"
Set-Location "jsonparser"
```

### Create project files manually

Since we're learning VS Code first, we'll create these files to understand the structure:

**1. Create .csproj file:**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>14.0</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Npgsql" Version="8.0.5" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.1" />
  </ItemGroup>

</Project>
```

**2. Create .gitignore file:**

```gitignore
# .NET Core/6.0+
bin/
obj/
*.user
*.userosscache
*.sln.docstates

# Visual Studio
.vs/

# User secrets
user-secrets*.json
!user-secrets*.json.example

# Build results
[Dd]ebug/
[Rr]elease/
x64/
x86/
bld/
[Bb]in/
[Oo]bj/

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# NuGet Packages
*.nupkg
*.snupkg
**/[Pp]ackages/*
!**/[Pp]ackages/build/**
*.nuget.props
*.nuget.targets

# Roslyn
*.roslyn

# Build results of dotnet native tools
**/[Dd]otnet/h/
**/Native/**
*.ni.dll
*.ni.exe

# JetBrains Rider
.idea/
*.sln.iml

# Code Coverage
coverage/
*.coverage
coveragelogs/

# Test results
TestResults/
*.trx
TestLog.*

# Analyzers
*.ruleset

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.tlog
*.vspscc
*_iak.cs
*_akt.cs
*.buildlog

# Other
*.xml
*.bak
*.cache
*.orig
*.tmp
*.log

# Operating System Files
.DS_Store
.DS_Store?
._*
.Spotlight-V100
.Trashes
ehthumbs.db
Thumbs.db
```

**3. Create appsettings.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=jsonapp_dev;Username=appuser;Password=YOUR_PASSWORD_HERE"
  },
  "ApiSettings": {
    "BaseUrl": "https://bnn.stats.bellmedia.ca/bnn/api",
    "StockEndpoint": "/stock/marketChart"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

**⚠ IMPORTANT:** Replace `YOUR_PASSWORD_HERE` with your actual PostgreSQL password or configure using User Secrets.

---

## Step 7: Configure .NET User Secrets

### Set up secure credential storage

**Open PowerShell and run:**

```powershell
Set-Location "C:\Development\VSCodeProjects\jsonparser"

# Enable user secrets for the project
dotnet user-secrets init

# Add your database password securely
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=jsonapp_dev;Username=appuser;Password=YOUR_PASSWORD_HERE"

# Mark secrets as user secrets
dotnet user-secrets set "IsUserSecretsEnabled": "true"
```

**Note:** User secrets are stored in `%APPDATA%\Microsoft\UserSecrets\<SECRET_ID>\secrets.json` - they are NOT committed to git and remain on your machine only.

---

## Step 8: Test Your Development Environment

### Verify installations

1. **Verify .NET SDK:**
   ```powershell
   dotnet --version
   # Should output: 10.0.x
   ```

2. **Verify PostgreSQL:**
   ```powershell
   psql -U appuser -d jsonapp_dev -h localhost -p 5432 -c "SELECT version();"
   ```

3. **Verify VS Code extensions:**
   - In VS Code, press `Ctrl + Shift + P`
   - Type "C# Dev Kit: Reload Workspace"
   - Ensure no errors appear

### Create test .NET application

```powershell
# In your jsonparser folder
dotnet new console -f net10.0
```

Run a test build:

```powershell
dotnet build
```

Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## Step 9: Database Table Setup

### Create the stock price table

In pgAdmin 4 or via SQL client:

```sql
-- Connect to jsonapp_dev database

-- Create table for stock prices
CREATE TABLE IF NOT EXISTS stock_prices (
    id SERIAL PRIMARY KEY,
    published_date DATE NOT NULL,
    price NUMERIC(12, 4) NOT NULL,
    symbol VARCHAR(20) NOT NULL DEFAULT 'USCRWCAS:IND',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Add index for faster date lookups
CREATE INDEX idx_stock_prices_date ON stock_prices(published_date DESC);
CREATE INDEX idx_stock_prices_symbol ON stock_prices(symbol);

-- Grant permissions
GRANT ALL PRIVILEGES ON TABLE stock_prices TO appuser;
GRANT USAGE, SELECT ON SEQUENCE stock_prices_id_seq TO appuser;

-- Test the connection
SELECT * FROM stock_prices LIMIT 5;
```

---

## Step 10: Configure Git for GitHub Repository

### Initialize Git repository

```powershell
# In your jsonparser folder
git init
git add .
git commit -m "Initial setup for C# .NET 10 JSON parser project"
```

### Create .github configuration

Create a `.github` folder with `FUNDING.yml` (optional):

```yaml
# Optional: Add your GitHub username for sponsorships
github: [your-username]
```

### Push to existing GitHub repository

```powershell
# Connect to your existing repository
git remote add origin https://github.com/SureShotUK/jsonparser
git branch -M main
git push -u origin main
```

**Replace `SureShotUK/jsonparser`** with your actual GitHub repository URL.

---

## Next Steps: Building Your JSON Parser Application

Now that your development environment is configured, you're ready to build your first practical application. Here's what the next phase will cover:

### Phase 2: Building the JSON Parser

**Features:**
- Fetch stock data from BNN API using HttpClient
- Parse JSON response using System.Text.Json
- Allow symbol selection from a predefined list or custom input
- Store data in PostgreSQL with automatic duplicate detection
- Support multiple time periods (1 week, 1 month, 3 months, 1 year)

**What you'll learn:**
- C# 14 features: primary constructors, collection expressions
- Async/await patterns with HttpClient
- Configuration with Microsoft.Extensions.Configuration
- Error handling and retry logic
- User experience design with console prompts

**When you're ready, let me know and we'll build the application together!**

---

## Troubleshooting Common Issues

### Issue: VS Code not recognizing C# project

**Solution:**
1. Press `Ctrl + Shift + P`
2. Type "C# Dev Kit: Reload Workspace"
3. Wait for extension initialization

### Issue: .NET version mismatch

**Solution:**
```powershell
dotnet --list-sdks
# If version 8 or older is shown, download .NET 10 SDK
```

### Issue: PostgreSQL connection failed

**Solution:**
1. Verify PostgreSQL service is running:
   ```powershell
   Get-Service -Name postgresql-x64-16
   ```
2. Check password and connection string
3. Verify database exists:
   ```powershell
   psql -U postgres -l
   ```

### Issue: User secrets not working

**Solution:**
1. Verify secrets are initialized:
   ```powershell
   dotnet user-secrets list
   ```
2. Check secret name matches `appsettings.json` exactly
3. Restart VS Code after setting secrets

### Issue: Git push fails

**Solution:**
```powershell
# Check existing remotes
git remote -v

# If origin already exists with different URL:
git remote set-url origin https://github.com/your-repo-url

# Push:
git push -u origin main
```

---

## Summary

By following this guide, you have:

✓ Installed Visual Studio Code with C# Dev Kit
✓ Configured .NET 10 SDK on Windows 11
✓ Set up PostgreSQL with dedicated development database
✓ Installed essential VS Code extensions
✓ Created a workspace structure ready for development
✓ Configured secure credential storage with User Secrets
✓ Set up Git for GitHub repository integration

**Your development environment is now ready for building C# .NET 10 applications!**

---

## Useful Resources

- **C# Documentation**: <a href="https://learn.microsoft.com/en-us/dotnet/csharp/" target="_blank">https://learn.microsoft.com/en-us/dotnet/csharp/</a>
- **.NET 10 Release Notes**: <a href="https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0" target="_blank">https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0</a>
- **PostgreSQL Documentation**: <a href="https://www.postgresql.org/docs/" target="_blank">https://www.postgresql.org/docs/</a>
- **VS Code C# Extensions**: <a href="https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp" target="_blank">https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp</a>

---

*Last updated: 2026-04-17*
