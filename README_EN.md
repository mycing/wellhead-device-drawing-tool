<p align="center">
  <a href="README.md">中文</a> | <a href="README_EN.md">English</a>
</p>

# Wellhead Device Drawing Tool

A Windows desktop application for creating wellhead device schematic diagrams.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

## Features

- **Rich Built-in Device Library** - Includes 20+ common wellhead devices such as annular BOP, ram BOP, casing head, rotary table, etc.
- **Custom Devices** - Import your own device images (JPG, PNG, SVG formats supported)
- **Flexible Label System** - Tree-structured text label management with multi-level categorization
- **Template Library** - Save frequently used device combinations as templates for one-click reuse
- **Smart Alignment** - Auto-align feature to quickly organize devices and text on the canvas
- **Easy Screenshot** - Supports manual screenshot, auto screenshot, and long image stitching
- **Color/Line Toggle** - Some devices support both colored fill and line outline styles

## Interface Overview

The software interface is divided into four main areas:
- **Left** - Wellhead device selection list
- **Middle Top** - Label management tree
- **Middle Bottom** - Saved template library
- **Right** - Drawing canvas

## Quick Start

1. **Place Device** - Select a device from the left list, click on canvas to place
2. **Add Label** - Select text from label tree, click on canvas to place
3. **Adjust Layout** - Drag to move, scroll wheel to resize
4. **Auto Align** - Right-click menu "Auto Align" for one-click arrangement
5. **Save/Screenshot** - Save as template or screenshot to clipboard

## User Guide

### Canvas Operations
| Action | Description |
|--------|-------------|
| Left Click | Place selected device or label |
| Left Drag | Move device or label position |
| Scroll Wheel (on control) | Resize device or label |
| Scroll Wheel (on blank area) | Scroll canvas up/down |
| Right Click | Open context menu |

### Context Menu Functions
- **Delete** - Remove selected control
- **Auto Align** - Automatically arrange all controls
- **Auto Screenshot** - Smart capture content area
- **Screenshot** - Manually select capture area
- **Clear Canvas** - Remove all content
- **Add to Library** - Save current canvas as template

## System Requirements

- Windows 7 / 8 / 10 / 11
- .NET Framework 4.7.2 or higher

## Installation

1. Download the latest version from [Releases](https://github.com/mycing/wellhead-device-drawing-tool/releases)
2. Extract to any directory
3. Run `4.18.exe`

## Data Storage

Program data is stored in the software directory:
- `template_library.bin` - Template library
- `tagtree_items.bin` - Label tree
- `pictures/` - Custom device images

> Back up the entire program folder when needed

## Tech Stack

- C# / Windows Forms
- .NET Framework 4.7.2
- SVG.NET (SVG rendering)
- System.Data.SQLite

## License

This project is licensed under the MIT License.

## Contributing

Issues and Pull Requests are welcome!
