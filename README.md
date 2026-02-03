<p align="center">
  <a href="README.md">中文</a> | <a href="README_EN.md">English</a>
</p>

<h1 align="center">井口裝置繪圖工具</h1>

<p align="center">
  一款用於繪製井口裝置示意圖的 Windows 桌面應用程序
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET%20Framework-4.8-blue" alt=".NET Framework">
  <img src="https://img.shields.io/badge/Platform-Windows-lightgrey" alt="Platform">
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License">
  <img src="https://img.shields.io/github/v/release/mycing/wellhead-device-drawing-tool" alt="Release">
</p>

---

## 目錄

- [功能特點](#功能特點)
- [快速開始](#快速開始)
- [操作指南](#操作指南)
- [項目結構](#項目結構)
- [架構設計](#架構設計)
- [編譯構建](#編譯構建)
- [數據存儲](#數據存儲)
- [技術棧](#技術棧)
- [更新日誌](#更新日誌)
- [貢獻指南](#貢獻指南)
- [開源協議](#開源協議)

---

## 功能特點

### 核心功能
- **豐富的內置裝置庫** - 包含 20+ 種常用井口裝置（萬能防噴器、閘板防噴器、套管頭、轉盤面等）
- **SVG 矢量圖形支持** - 裝置圖片縮放不失真
- **自定義裝置導入** - 支持 JPG、PNG、BMP、GIF、SVG 格式
- **樹狀標籤管理** - 多層級分類管理文字標籤
- **模板庫系統** - 保存/載入常用裝置組合，支持資料夾分類

### 畫布功能
- **拖放操作** - 直覺的拖動移動、滾輪縮放
- **智能對齊** - 兩種對齊模式快速整理畫布
- **多種截圖方式** - 手動框選、自動識別、長圖拼接

### 用戶體驗
- **實時預覽** - 懸停顯示裝置預覽圖
- **即時搜索** - 標籤和模板的快速過濾
- **自動保存** - 數據自動持久化，無需手動保存

---

## 快速開始

### 下載安裝
1. 從 [Releases](https://github.com/mycing/wellhead-device-drawing-tool/releases) 下載最新版本
2. 解壓到任意目錄
3. 運行 `4.18.exe`

### 系統要求
- Windows 7 / 8 / 10 / 11
- .NET Framework 4.8 或更高版本

### 基本操作流程
```
選擇裝置 → 點擊畫布放置 → 添加文字標籤 → 調整位置大小 → 自動對齊 → 截圖/保存模板
```

---

## 操作指南

### 界面佈局
```
┌──────────────────────────────────────────────────────────────┐
│  [使用未上色裝置]                                    [幫助]   │
├────────────┬────────────────────┬────────────────────────────┤
│            │    標籤管理         │                            │
│  井口裝置   │   ┌──────────┐     │                            │
│  選擇列表   │   │ 搜索框    │     │         繪圖畫布           │
│            │   ├──────────┤     │                            │
│  • 萬能防噴器│   │ 標籤樹    │     │                            │
│  • 閘板防噴器│   │          │     │                            │
│  • 套管頭   │   └──────────┘     │                            │
│  • ...     ├────────────────────┤                            │
│            │    模板庫          │                            │
│            │   ┌──────────┐     │                            │
│            │   │ 搜索框    │     │                            │
│            │   ├──────────┤     │                            │
│            │   │ 模板樹    │     │                            │
│            │   │          │     │                            │
│            │   └──────────┘     │                            │
└────────────┴────────────────────┴────────────────────────────┘
```

### 滑鼠操作

| 區域 | 左鍵 | 右鍵 | 滾輪 |
|------|------|------|------|
| 裝置列表 | 選擇裝置 | 添加/刪除自定義裝置 | - |
| 標籤樹 | 選擇標籤 | 添加/重命名/刪除節點 | - |
| 模板樹 | 載入模板 | 管理資料夾和模板 | - |
| 畫布空白 | 放置裝置/標籤 | 功能菜單 | 滾動畫布 |
| 畫布控件 | 拖動移動 | 刪除/對齊/截圖 | 縮放大小 |

### 右鍵菜單功能
- **刪除** - 移除選中控件
- **自動對齊** - 智能排列（兩種模式交替）
- **自動截圖** - 一鍵截取全部內容
- **截圖** - 手動框選截圖區域
- **清空畫布** - 清除所有內容
- **添加樣例到庫** - 保存為模板

---

## 項目結構

```
4.18/
├── 4.18.sln                    # 解決方案文件
├── README.md                   # 項目說明（中文）
├── README_EN.md                # 項目說明（英文）
└── 4.18/                       # 主項目目錄
    ├── 4.18.csproj             # 項目文件
    ├── App.config              # 應用配置
    ├── packages.config         # NuGet 包配置
    │
    ├── # 核心文件
    ├── Program.cs              # 程序入口
    ├── Form1.cs                # 主窗體邏輯
    ├── Form1.Designer.cs       # 主窗體設計器
    ├── Form1.resx              # 主窗體資源
    │
    ├── # 自定義控件
    ├── SvgDrawPicturePanel.cs  # SVG 裝置面板
    ├── ImagePicturePanel.cs    # 圖片裝置面板
    ├── DrawstringPanel.cs      # 文字標籤面板
    ├── TagTreeUserControl.cs   # 標籤樹控件
    ├── CustomScrollBar.cs      # 自定義滾動條
    ├── RecursiveTreeView.cs    # 遞歸樹視圖
    │
    ├── # 功能管理器
    ├── AutoAlignManager.cs     # 自動對齊管理
    ├── AutoCaptureManager.cs   # 自動截圖管理
    ├── ScreenCaptureManager.cs # 手動截圖管理
    ├── PanelManager.cs         # 面板管理
    ├── PanelSampleLibrarySaver.cs  # 模板保存
    │
    ├── # 數據模型
    ├── Device.cs               # 裝置類
    ├── TemplateTreeNodeData.cs # 模板樹節點數據
    │
    ├── # 輔助類
    ├── HelpDialog.cs           # 幫助對話框
    ├── MenuStyleHelper.cs      # 菜單樣式輔助
    │
    ├── Properties/             # 項目屬性
    │   ├── AssemblyInfo.cs
    │   ├── Resources.resx
    │   └── Settings.settings
    │
    └── Resources/              # 內置資源文件
        ├── *.svg               # SVG 矢量裝置圖
        ├── *.png               # PNG 裝置圖
        └── *.bmp               # BMP 裝置圖
```

---

## 架構設計

### 核心類關係

```
Form1 (主窗體)
├── Panel2 (畫布容器)
│   ├── SvgDrawPicturePanel    # SVG 裝置
│   ├── ImagePicturePanel      # 圖片裝置
│   └── DrawstringPanel        # 文字標籤
│
├── TagTreeUserControl         # 標籤管理
│   └── TreeNodeData           # 標籤數據模型
│
├── TreeView (treeViewTemplates)  # 模板庫
│   └── TemplateTreeNodeData   # 模板數據模型
│
├── AutoAlignManager           # 自動對齊
├── AutoCaptureManager         # 自動截圖
├── ScreenCaptureManager       # 手動截圖
└── PanelSampleLibrarySaver    # 模板保存
```

### 數據模型

#### TemplateTreeNodeData
```csharp
[Serializable]
public class TemplateTreeNodeData
{
    public string Text { get; set; }                    // 節點名稱
    public List<PanelInfo> TemplateData { get; set; }   // 模板數據（資料夾為 null）
    public List<TemplateTreeNodeData> Children { get; set; }  // 子節點
    public bool IsFolder => TemplateData == null;       // 是否為資料夾
    public bool IsTemplate => TemplateData != null;     // 是否為模板
}
```

#### TreeNodeData (標籤)
```csharp
[Serializable]
public class TreeNodeData
{
    public string Text { get; set; }                    // 標籤文字
    public List<TreeNodeData> Children { get; set; }    // 子節點
}
```

### 面板控件繼承

```
Panel (System.Windows.Forms)
├── SvgDrawPicturePanel   # 支持 SVG 渲染、等比縮放
├── ImagePicturePanel     # 支持圖片渲染、高質量插值
└── DrawstringPanel       # 支持動態字體縮放
```

---

## 編譯構建

### 環境要求
- Visual Studio 2019 或更高版本
- .NET Framework 4.8 SDK

### 編譯步驟

1. **克隆倉庫**
   ```bash
   git clone https://github.com/mycing/wellhead-device-drawing-tool.git
   cd wellhead-device-drawing-tool
   ```

2. **還原 NuGet 包**
   ```bash
   nuget restore 4.18.sln
   ```
   或在 Visual Studio 中右鍵解決方案 → 還原 NuGet 包

3. **編譯項目**
   ```bash
   msbuild 4.18.sln /p:Configuration=Release
   ```
   或在 Visual Studio 中按 F5 / Ctrl+Shift+B

4. **輸出位置**
   ```
   4.18/bin/Release/  或  4.18/bin/Debug/
   ```

### NuGet 依賴
| 包名 | 版本 | 用途 |
|------|------|------|
| Svg | 3.4.7 | SVG 矢量圖渲染 |
| Newtonsoft.Json | 13.0.3 | JSON 序列化 |
| System.Data.SQLite | 1.0.119 | SQLite 數據庫 |
| EntityFramework | 6.4.4 | ORM 框架 |

---

## 數據存儲

### 文件說明
| 文件名 | 格式 | 說明 |
|--------|------|------|
| `template_library.bin` | Binary | 模板庫（資料夾結構+模板內容） |
| `tagtree_items.bin` | Binary | 標籤樹結構 |
| `pictures/` | 目錄 | 用戶自定義裝置圖片 |

### 序列化方式
使用 `BinaryFormatter` 進行二進制序列化，確保數據完整性。

### 備份建議
- 定期備份程序目錄下的 `.bin` 文件
- 遷移時複製整個程序資料夾

---

## 技術棧

| 類別 | 技術 |
|------|------|
| 語言 | C# |
| 框架 | .NET Framework 4.8 |
| UI | Windows Forms |
| 圖形渲染 | GDI+、SVG.NET |
| 數據持久化 | BinaryFormatter |
| 截圖 | Win32 API (BitBlt) |

---

## 更新日誌

### v1.0.0 (2026.2)
- 標籤樹支持多層級結構
- 模板庫支持資料夾分類
- 新增標籤和模板搜索功能
- 新增重命名功能
- 優化幫助文檔

### v0.2.0 (2025.5)
- 更新為 SVG 矢量圖形格式
- 優化截圖功能，支持多方向截圖
- 改進圖片縮放算法

### v0.1.0 (2025.2)
- 初始版本發布
- 基本裝置繪製功能
- 截圖和模板保存

---

## 貢獻指南

歡迎提交 Issue 和 Pull Request！

### 提交 Issue
- Bug 報告：請附上復現步驟和錯誤信息
- 功能建議：請描述使用場景和期望效果

### 提交 PR
1. Fork 本倉庫
2. 創建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

### 代碼規範
- 遵循 C# 命名規範
- 添加必要的註釋
- 保持代碼簡潔

---

## 開源協議

本項目採用 [MIT License](LICENSE) 開源協議。

---

<p align="center">
  如果這個項目對你有幫助，歡迎給一個 ⭐ Star！
</p>
