using System;
using System.Collections.Generic;
using System.IO;

namespace _4._18
{
    /// <summary>
    /// 語言枚舉
    /// </summary>
    public enum Language
    {
        TraditionalChinese,
        SimplifiedChinese,
        English
    }

    /// <summary>
    /// 本地化管理器 - 管理多語言字符串
    /// </summary>
    internal static class LocalizationManager
    {
        private static Language _currentLanguage = Language.TraditionalChinese;
        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.conf");

        private static readonly Dictionary<string, Dictionary<Language, string>> Strings = new Dictionary<string, Dictionary<Language, string>>
        {
            // ===== 主窗體 =====
            ["FormTitle"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "井口裝置繪圖工具" },
                { Language.SimplifiedChinese, "井口装置绘图工具" },
                { Language.English, "Wellhead Device Drawing Tool" }
            },
            ["DeviceSelection"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "井口裝置選擇" },
                { Language.SimplifiedChinese, "井口装置选择" },
                { Language.English, "Device Selection" }
            },
            ["TagManagement"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "標籤管理" },
                { Language.SimplifiedChinese, "标签管理" },
                { Language.English, "Label Manager" }
            },
            ["BOPConfig"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "防噴器配置" },
                { Language.SimplifiedChinese, "防喷器配置" },
                { Language.English, "BOP Configuration" }
            },
            ["SavedTemplates"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "已保存模板" },
                { Language.SimplifiedChinese, "已保存模板" },
                { Language.English, "Saved Templates" }
            },
            ["UseUncoloredDevice"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "使用未上色裝置" },
                { Language.SimplifiedChinese, "使用未上色装置" },
                { Language.English, "Use Uncolored Devices" }
            },
            ["Help"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "幫助" },
                { Language.SimplifiedChinese, "帮助" },
                { Language.English, "Help" }
            },
            ["Settings"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "設置" },
                { Language.SimplifiedChinese, "设置" },
                { Language.English, "Settings" }
            },

            // ===== Panel2 右鍵菜單 =====
            ["Delete"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "刪除" },
                { Language.SimplifiedChinese, "删除" },
                { Language.English, "Delete" }
            },
            ["AutoCapture"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "自動截圖" },
                { Language.SimplifiedChinese, "自动截图" },
                { Language.English, "Auto Screenshot" }
            },
            ["Capture"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "截圖" },
                { Language.SimplifiedChinese, "截图" },
                { Language.English, "Screenshot" }
            },
            ["ClearCanvas"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "清空畫布" },
                { Language.SimplifiedChinese, "清空画布" },
                { Language.English, "Clear Canvas" }
            },
            ["OpenSample"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "打開裝置樣例" },
                { Language.SimplifiedChinese, "打开装置样例" },
                { Language.English, "Open Device Sample" }
            },
            ["AutoAlign"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "自動對齊" },
                { Language.SimplifiedChinese, "自动对齐" },
                { Language.English, "Auto Align" }
            },
            ["AddSampleToLibrary"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "添加樣例到庫" },
                { Language.SimplifiedChinese, "添加样例到库" },
                { Language.English, "Add to Library" }
            },

            // ===== ListBox1 右鍵菜單 =====
            ["AddCustomDevice"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "添加自繪裝置" },
                { Language.SimplifiedChinese, "添加自绘装置" },
                { Language.English, "Add Custom Device" }
            },
            ["DeleteCurrentDevice"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "刪除當前裝置" },
                { Language.SimplifiedChinese, "删除当前装置" },
                { Language.English, "Delete Current Device" }
            },

            // ===== 模板樹右鍵菜單 =====
            ["Rename"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "重命名" },
                { Language.SimplifiedChinese, "重命名" },
                { Language.English, "Rename" }
            },
            ["AddFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "新增資料夾" },
                { Language.SimplifiedChinese, "新增文件夹" },
                { Language.English, "New Folder" }
            },
            ["AddSubFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "新增子資料夾" },
                { Language.SimplifiedChinese, "新增子文件夹" },
                { Language.English, "New Subfolder" }
            },
            ["AddDeviceToLibrary"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "添加裝置到庫" },
                { Language.SimplifiedChinese, "添加装置到库" },
                { Language.English, "Add Device to Library" }
            },
            ["AddDeviceToFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "添加裝置到此資料夾" },
                { Language.SimplifiedChinese, "添加装置到此文件夹" },
                { Language.English, "Add Device to This Folder" }
            },

            // ===== 對話框 =====
            ["SaveTemplateTitle"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "保存模板到庫" },
                { Language.SimplifiedChinese, "保存模板到库" },
                { Language.English, "Save Template to Library" }
            },
            ["SelectTargetFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "選擇目標資料夾（不選擇則保存為根節點）：" },
                { Language.SimplifiedChinese, "选择目标文件夹（不选择则保存为根节点）：" },
                { Language.English, "Select target folder (root if none selected):" }
            },
            ["TemplateName"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "模板名稱：" },
                { Language.SimplifiedChinese, "模板名称：" },
                { Language.English, "Template Name:" }
            },
            ["Save"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "保存" },
                { Language.SimplifiedChinese, "保存" },
                { Language.English, "Save" }
            },
            ["Cancel"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "取消" },
                { Language.SimplifiedChinese, "取消" },
                { Language.English, "Cancel" }
            },
            ["TemplateNameEmpty"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "模板名稱不能為空。" },
                { Language.SimplifiedChinese, "模板名称不能为空。" },
                { Language.English, "Template name cannot be empty." }
            },
            ["Error"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "錯誤" },
                { Language.SimplifiedChinese, "错误" },
                { Language.English, "Error" }
            },
            ["SaveToFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "保存到「{0}」" },
                { Language.SimplifiedChinese, "保存到「{0}」" },
                { Language.English, "Save to \"{0}\"" }
            },
            ["ConfirmDelete"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "確認刪除" },
                { Language.SimplifiedChinese, "确认删除" },
                { Language.English, "Confirm Delete" }
            },
            ["ConfirmDeleteFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "確定要刪除資料夾「{0}」及其所有內容嗎？" },
                { Language.SimplifiedChinese, "确定要删除文件夹「{0}」及其所有内容吗？" },
                { Language.English, "Delete folder \"{0}\" and all its contents?" }
            },
            ["ConfirmDeleteTemplate"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "確定要刪除模板「{0}」嗎？" },
                { Language.SimplifiedChinese, "确定要删除模板「{0}」吗？" },
                { Language.English, "Delete template \"{0}\"?" }
            },
            ["NewFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "新資料夾" },
                { Language.SimplifiedChinese, "新文件夹" },
                { Language.English, "New Folder" }
            },
            ["NewSubFolder"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "新子資料夾" },
                { Language.SimplifiedChinese, "新子文件夹" },
                { Language.English, "New Subfolder" }
            },
            ["ImageFileFilter"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "圖像文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.SimplifiedChinese, "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.English, "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" }
            },
            ["SelectCustomDevice"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "選擇自繪裝置" },
                { Language.SimplifiedChinese, "选择自绘装置" },
                { Language.English, "Select Custom Device" }
            },
            ["ImageNotExist"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "圖片文件不存在。" },
                { Language.SimplifiedChinese, "图片文件不存在。" },
                { Language.English, "Image file does not exist." }
            },
            ["DeleteFileFailed"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "刪除文件失敗：{0}" },
                { Language.SimplifiedChinese, "删除文件失败：{0}" },
                { Language.English, "Failed to delete file: {0}" }
            },
            ["ErrorOccurred"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "發生錯誤：{0}" },
                { Language.SimplifiedChinese, "发生错误：{0}" },
                { Language.English, "Error occurred: {0}" }
            },

            // ===== 設置對話框 =====
            ["SettingsTitle"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "設置" },
                { Language.SimplifiedChinese, "设置" },
                { Language.English, "Settings" }
            },
            ["LanguageLabel"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "語言 / Language：" },
                { Language.SimplifiedChinese, "语言 / Language：" },
                { Language.English, "Language：" }
            },
            ["OK"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "確定" },
                { Language.SimplifiedChinese, "确定" },
                { Language.English, "OK" }
            },
        };

        public static Language CurrentLanguage
        {
            get { return _currentLanguage; }
        }

        /// <summary>
        /// 獲取指定 key 的本地化字符串
        /// </summary>
        public static string GetString(string key)
        {
            if (Strings.TryGetValue(key, out var langDict))
            {
                if (langDict.TryGetValue(_currentLanguage, out var value))
                {
                    return value;
                }
                // 回退到繁體中文
                if (langDict.TryGetValue(Language.TraditionalChinese, out var fallback))
                {
                    return fallback;
                }
            }
            return key;
        }

        /// <summary>
        /// 獲取格式化的本地化字符串
        /// </summary>
        public static string GetString(string key, params object[] args)
        {
            return string.Format(GetString(key), args);
        }

        /// <summary>
        /// 設置當前語言
        /// </summary>
        public static void SetLanguage(Language language)
        {
            _currentLanguage = language;
            SaveLanguageSetting();
        }

        /// <summary>
        /// 從配置文件讀取語言設置
        /// </summary>
        public static void LoadLanguageSetting()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string content = File.ReadAllText(SettingsPath).Trim();
                    if (Enum.TryParse(content, out Language lang))
                    {
                        _currentLanguage = lang;
                    }
                }
            }
            catch
            {
                _currentLanguage = Language.TraditionalChinese;
            }
        }

        /// <summary>
        /// 保存語言設置到配置文件
        /// </summary>
        private static void SaveLanguageSetting()
        {
            try
            {
                File.WriteAllText(SettingsPath, _currentLanguage.ToString());
            }
            catch
            {
                // 忽略保存失敗
            }
        }
    }
}
