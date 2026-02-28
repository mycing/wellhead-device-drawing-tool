<p align="center">
  <a href="README.md">English</a> | <a href="README_CN.md">中文</a>
</p>

<h1 align="center">Wellhead Device Drawing Tool</h1>

<p align="center">
  A Windows desktop application for creating wellhead device schematic diagrams
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET%20Framework-4.8-blue" alt=".NET Framework">
  <img src="https://img.shields.io/badge/Platform-Windows-lightgrey" alt="Platform">
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License">
  <img src="https://img.shields.io/github/v/release/mycing/wellhead-device-drawing-tool" alt="Release">
</p>

---

## Table of Contents

- [Features](#features)
- [Quick Start](#quick-start)
- [User Guide](#user-guide)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Build](#build)
- [Data Storage](#data-storage)
- [Tech Stack](#tech-stack)
- [Changelog](#changelog)
- [Contributing](#contributing)
- [License](#license)

---

## Features

### Core Features
- **Rich Built-in Device Library** - 20+ common wellhead devices (Annular BOP, Ram BOP, Casing Head, Rotary Table, etc.)
- **SVG Vector Graphics Support** - Lossless scaling for device images
- **Custom Device Import** - Supports JPG, PNG, BMP, GIF, SVG formats
- **Tree-structured Label Management** - Multi-level categorization for text labels
- **Template Library System** - Save/load device combinations with folder organization
- **Batch JSON Diagram Import** - Supports single JSON, multiple concatenated JSON blocks, and JSON arrays
- **Multilingual UI** - English, Simplified Chinese, Traditional Chinese, Spanish, French, Portuguese, Russian, Persian, Norwegian, Japanese, Korean, Arabic

### Canvas Features
- **Drag & Drop** - Intuitive drag-to-move and scroll-to-zoom
- **Smart Alignment** - Two alignment modes for quick organization
- **Multiple Screenshot Methods** - Manual selection, auto-capture, long image stitching

### User Experience
- **Real-time Preview** - Hover to show device preview
- **Instant Search** - Quick filtering for labels and templates
- **Auto Save** - Automatic data persistence
- **Copyable Error Reports** - JSON import errors can be copied with full diagnostic details

---

## Quick Start

### Installation
1. Download the latest version from [Releases](https://github.com/mycing/wellhead-device-drawing-tool/releases)
2. Extract to any directory
3. Run `WellheadDiagram.exe`

### System Requirements
- Windows 7 / 8 / 10 / 11
- .NET Framework 4.8 or higher

### Basic Workflow
```
Select Device → Click Canvas to Place → Add Text Labels → Adjust Position/Size → Auto Align → Screenshot/Save Template
```

---

## User Guide

### Interface Layout
```
┌──────────────────────────────────────────────────────────────┐
│  [Use Uncolored Devices]                             [Help]   │
├────────────┬────────────────────┬────────────────────────────┤
│            │    Label Manager    │                            │
│  Wellhead  │   ┌──────────┐     │                            │
│  Device    │   │ Search    │     │       Drawing Canvas       │
│  List      │   ├──────────┤     │                            │
│            │   │ Label Tree│     │                            │
│  • Annular │   │          │     │                            │
│  • Ram BOP │   └──────────┘     │                            │
│  • Casing  ├────────────────────┤                            │
│  • ...     │    Template Library │                            │
│            │   ┌──────────┐     │                            │
│            │   │ Search    │     │                            │
│            │   ├──────────┤     │                            │
│            │   │ Templates │     │                            │
│            │   │          │     │                            │
│            │   └──────────┘     │                            │
└────────────┴────────────────────┴────────────────────────────┘
```

### Mouse Operations

| Area | Left Click | Right Click | Scroll Wheel |
|------|------------|-------------|--------------|
| Device List | Select device | Add/Delete custom device | - |
| Label Tree | Select label | Add/Rename/Delete node | - |
| Template Tree | Load template | Manage folders & templates | - |
| Canvas (blank) | Place device/label | Context menu | Scroll canvas |
| Canvas (control) | Drag to move | Delete/Align/Screenshot | Resize |

### Context Menu Functions
- **Delete** - Remove selected control
- **Auto Align** - Smart arrangement (two alternating modes)
- **Auto Screenshot** - One-click capture all content
- **Screenshot** - Manual selection capture
- **Clear Canvas** - Remove all content
- **Add to Library** - Save as template
- **Save to Current Template** - Overwrite the currently loaded template
- **Import JSON** - Build diagrams quickly from AI-generated JSON

---

## Project Structure

```
4.18/
├── 4.18.sln                    # Solution file
├── README.md                   # Project description (English)
├── README_CN.md                # Project description (Chinese)
└── 4.18/                       # Main project directory
    ├── 4.18.csproj             # Project file (output: WellheadDiagram.exe)
    ├── App.config              # Application config
    ├── packages.config         # NuGet packages config
    │
    ├── # Core Files
    ├── Program.cs              # Entry point
    ├── Form1.cs                # Main form logic
    ├── Form1.Designer.cs       # Main form designer
    ├── Form1.resx              # Main form resources
    │
    ├── # Custom Controls
    ├── SvgDrawPicturePanel.cs  # SVG device panel
    ├── ImagePicturePanel.cs    # Image device panel
    ├── DrawstringPanel.cs      # Text label panel
    ├── TagTreeUserControl.cs   # Label tree control
    ├── CustomScrollBar.cs      # Custom scroll bar
    ├── RecursiveTreeView.cs    # Recursive tree view
    │
    ├── # Feature Managers
    ├── AutoAlignManager.cs     # Auto alignment
    ├── AutoCaptureManager.cs   # Auto screenshot
    ├── ScreenCaptureManager.cs # Manual screenshot
    ├── PanelManager.cs         # Panel management
    ├── PanelSampleLibrarySaver.cs  # Template saving
    ├── CanvasContextMenuFactory.cs # Canvas context menu factory
    │
    ├── # Data Models
    ├── Device.cs               # Device class
    ├── TemplateTreeNodeData.cs # Template tree node data
    ├── WellheadJsonSchema.cs   # JSON schema model
    │
    ├── # Helper Classes
    ├── HelpDialog.cs           # Help dialog
    ├── MenuStyleHelper.cs      # Menu style helper
    ├── LocalizationManager.cs  # Localization manager
    ├── LanguageOptionMapper.cs # Language option mapping
    ├── BuiltInDeviceCatalog.cs # Built-in device mapping
    ├── WellheadJsonImporter.cs # JSON import parser
    ├── JsonImportDialog.cs     # JSON import dialog
    │
    ├── Properties/             # Project properties
    │   ├── AssemblyInfo.cs
    │   ├── Resources.resx
    │   └── Settings.settings
    │
    └── Resources/              # Built-in resources
        ├── *.svg               # SVG vector devices
        ├── *.png               # PNG device images
        └── *.bmp               # BMP device images
```

---

## Architecture

### Core Class Relationships

```
Form1 (Main Form)
├── Panel2 (Canvas Container)
│   ├── SvgDrawPicturePanel    # SVG devices
│   ├── ImagePicturePanel      # Image devices
│   └── DrawstringPanel        # Text labels
│
├── TagTreeUserControl         # Label management
│   └── TreeNodeData           # Label data model
│
├── TreeView (treeViewTemplates)  # Template library
│   └── TemplateTreeNodeData   # Template data model
│
├── AutoAlignManager           # Auto alignment
├── AutoCaptureManager         # Auto screenshot
├── ScreenCaptureManager       # Manual screenshot
└── PanelSampleLibrarySaver    # Template saving
```

### Data Models

#### TemplateTreeNodeData
```csharp
[Serializable]
public class TemplateTreeNodeData
{
    public string Text { get; set; }                    // Node name
    public List<PanelInfo> TemplateData { get; set; }   // Template data (null for folders)
    public List<TemplateTreeNodeData> Children { get; set; }  // Child nodes
    public bool IsFolder => TemplateData == null;       // Is folder
    public bool IsTemplate => TemplateData != null;     // Is template
}
```

#### TreeNodeData (Labels)
```csharp
[Serializable]
public class TreeNodeData
{
    public string Text { get; set; }                    // Label text
    public List<TreeNodeData> Children { get; set; }    // Child nodes
}
```

### Panel Control Hierarchy

```
Panel (System.Windows.Forms)
├── SvgDrawPicturePanel   # SVG rendering, aspect ratio scaling
├── ImagePicturePanel     # Image rendering, high-quality interpolation
└── DrawstringPanel       # Dynamic font scaling
```

---

## Build

### Requirements
- Visual Studio 2019 or later
- .NET Framework 4.8 SDK

### Build Steps

1. **Clone Repository**
   ```bash
   git clone https://github.com/mycing/wellhead-device-drawing-tool.git
   cd wellhead-device-drawing-tool
   ```

2. **Restore NuGet Packages**
   ```bash
   nuget restore 4.18.sln
   ```
   Or right-click solution in Visual Studio → Restore NuGet Packages

3. **Build Project**
   ```bash
   msbuild 4.18.sln /p:Configuration=Release
   ```
   Or press F5 / Ctrl+Shift+B in Visual Studio

4. **Output Location**
   ```
   4.18/bin/Release/  or  4.18/bin/Debug/
   ```

### NuGet Dependencies
| Package | Version | Purpose |
|---------|---------|---------|
| Svg | 3.4.7 | SVG vector rendering |
| Newtonsoft.Json | 13.0.3 | JSON serialization |
| ExCSS | 4.2.3 | SVG/CSS style parsing |
| System.Buffers | 4.5.1 | Buffer utilities |
| System.Memory | 4.5.5 | Memory/span support |
| System.Numerics.Vectors | 4.5.0 | Vector math support |
| System.Resources.Extensions | 9.0.0 | Resource extension support |
| System.Runtime.CompilerServices.Unsafe | 6.0.0 | Runtime low-level support |

---

## Data Storage

### File Description
| Filename | Format | Description |
|----------|--------|-------------|
| `template_library.bin` | Binary | Template library (folder structure + templates) |
| `tagtree_items.bin` | Binary | Label tree structure |
| `pictures/` | Directory | User custom device images |
| `language.conf` | Text | Language/UI setting persistence |
| `listbox3_items.bin` | Binary | Legacy template data (migration compatibility) |
| `listbox3_items.json` | JSON | Legacy template data (debug/compatibility) |

### Serialization
Uses `BinaryFormatter` for binary serialization to ensure data integrity.

### Backup Recommendations
- Regularly backup `.bin` files in the program directory
- Copy entire program folder when migrating

---

## Tech Stack

| Category | Technology |
|----------|------------|
| Language | C# |
| Framework | .NET Framework 4.8 |
| UI | Windows Forms |
| Graphics | GDI+, SVG.NET |
| Data Persistence | BinaryFormatter |
| Screenshot | Win32 API (BitBlt) |

---

## Changelog

### v1.2.0 (2026.2.27)
- Added full 12-language switching (EN/zh-CN/zh-TW/ES/FR/PT/RU/FA/NO/JA/KO/AR), including built-in device names, menus, and help docs
- JSON import upgraded: supports single JSON, concatenated multi-JSON input, and JSON arrays with multilingual key/type aliases
- JSON diagnostics upgraded: reports JSON index, line, position, and device index; error dialog supports one-click copy
- Canvas upgraded to bidirectional scrolling (vertical + horizontal) with dynamic bounds based on rendered content
- Added **Save to Current Template** with dynamic enable/disable based on local template mapping state

### v1.1.0 (2026.2.26)
- Template library upgraded from flat list to tree structure with folders, rename/delete, copy/paste, and search filtering
- Added tag tree search with live filtering; fixed add/delete/paste persistence sync in filter mode
- Moved colored/uncolored device toggle into Settings dialog (replacing main-form legacy switch)
- Refactored help system into hierarchical navigation with language-aware content
- Improved DPI layout strategy and search-box positioning for high scaling factors

### v1.0.0 (2026.2)
- Label tree supports multi-level structure
- Template library supports folder organization
- Added label and template search functionality
- Added rename feature
- Improved help documentation

### v0.2.0 (2025.5)
- Updated to SVG vector graphics format
- Optimized screenshot functionality, supports multi-directional capture
- Improved image scaling algorithm

### v0.1.0 (2025.2)
- Initial release
- Basic device drawing functionality
- Screenshot and template saving

---

## Contributing

Issues and Pull Requests are welcome!

### Submitting Issues
- Bug reports: Please include reproduction steps and error messages
- Feature requests: Please describe use case and expected behavior

### Submitting PRs
1. Fork this repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style
- Follow C# naming conventions
- Add necessary comments
- Keep code clean and simple

---

## License

This project is licensed under the [MIT License](LICENSE).

---

<p align="center">
  If this project helps you, please give it a ⭐ Star!
</p>
