using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Security.Permissions;
using System.Web;
using _4._18.Properties;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.VisualStyles;
using Svg;




namespace _4._18
{
    public partial class Form1 : Form
    {
        //2.19
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
          IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

        //存放平臺名（圖名）
        public string platform_name;
        //存储下拉框选择的图片
        public static Image getImage;
        //寫一個路徑用於存放用戶個人裝置輸出地址
        public string custome_device_path;
        //存儲一個用於存放用戶自定義裝置的集合,用於上傳時每次添加
        public List<string> custom_device;
        //添加一個字典用於listbox1的雙擊事件添加用戶自繪裝置，用於每次在程序加載時瀏覽整個用戶文件添加
        public Dictionary<string, string> Custom_device;
        //存放預覽裝置
        private PictureBox previewDevice;

        //存儲一個bool變量用於判斷是否在超出listbox1的邊界時顯示光標右下角的圖片
        public bool isshownpreviousDevices = false;
        // 用于记录点击时固定的选中项
        private int fixedIndex = -1;

        //存儲一個變量用於判斷cursor是否離開了listbox1
        public bool isCursorleavelistbox1 = false;
        //存放一個變量來判斷panel1是否進行了右擊
        public bool isrightclick_while_cursor_leave_listbox1 = false;

        //存放discription1的字符串繪製picturebox
        public PictureBox discription1_previous;

        //存儲discription1的字符串的label
        public Label discription_previous;

        //2.12
        private ToolTip toolTip;
        private string selectedItemText;

        // 標籤文字的放大字體（2倍大小）
        private Font tagLabelFont;

        //2.13
        //存儲變量用於判斷是否在panel2上顯示tooltip
        private bool is_show_the_tootip = true;
        private Font toolTipFont;

        //2.18
        private bool isCapturing;
        private Point startPoint;
        private Point endPoint;
        private Rectangle rect;

        //2.20
        //存儲動態創建的panel
        private bool isSelecting = false;
        private Rectangle selectionRectangle;
        private List<Panel> dynamicPanels = new List<Panel>();
        private Point initialMouseDownLocation;


        //3.18 for read and write json to json listbox3
        public Dictionary<string, List<PanelInfo>> panelinfos_to_listbox3;
        //3.27
        public string listbox3_binary_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "listbox3_items.bin");
        public string listbox3_json_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "listbox3_items.json");

        // 模板庫樹狀結構數據
        public List<TemplateTreeNodeData> templateLibraryData;
        public string templateLibraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template_library.bin");
        private string _currentTemplateFilter = string.Empty;

        //3.31 treeViewTemplates 右鍵菜單
        private ContextMenuStrip contextMenuTemplates;
        private ToolStripMenuItem menuItemDelete;
        private ToolStripMenuItem menuItemRename;
        private ToolStripMenuItem menuItemAddToLibrary;

        //listBox1 右鍵菜單
        private ContextMenuStrip contextMenuStripListBox1;
        private ToolStripMenuItem addDeviceMenuItemListBox1;
        private ToolStripMenuItem deleteMenuItemListBox1;

        //panel2 右鍵菜單
        private ContextMenuStrip contextMenuStripPanel2;
        private ToolStripMenuItem autoCaptureMenuItemPanel2;
        private ToolStripMenuItem captureMenuItemPanel2;
        private ToolStripMenuItem clearCanvasMenuItemPanel2;
        private ToolStripMenuItem openSampleMenuItemPanel2;
        private ToolStripMenuItem autoAlignMenuItemPanel2;
        private ToolStripMenuItem saveSampleMenuItemPanel2;
        private ToolStripMenuItem deleteMenuItemPanel2;
        private ToolStripSeparator separatorPanel2;
        private Control clickedControlOnPanel2;

        // 自動對齊管理器
        private AutoAlignManager autoAlignManager;

        // 自動截圖管理器
        private AutoCaptureManager autoCaptureManager;

        //4.5 capture picture from panel2
        private int totalScrollDistance = 0;

        //4.8 captureing mousewhell
        private bool captureingmousewheel = false;

        // combain picture from panel2_captire
        private List<Bitmap> capturelist;

        //store picture address
        public string storePictureaddress = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pictures");

        //5.7 設置svg公共變量用於存儲與getimage相同的需要繪製的圖片
        public byte[] SvgData;
        private bool _useUncoloredDevices = true;

        //創建panelmanager管理類
        private PanelManager _panelManager;
        private PanelSampleLibrarySaver _panelSampleLibrarySaver;

        //創建screenCaptureManager
        private ScreenCaptureManager captureManager;
        /// <summary>
        ///类窗体1的构造方法
        /// </summary>
        public Form1()
        {
            // 先載入語言設置，再初始化控件
            LocalizationManager.LoadLanguageSetting();

            InitializeComponent();

            // 統一文字與 ToolTip 的字體大小
            toolTipFont = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            tagLabelFont = toolTipFont;

            // 配置 ToolTip 使用自定義繪製
            toolTip = new ToolTip();
            toolTip.OwnerDraw = true;
            toolTip.Draw += ToolTip_Draw;
            toolTip.Popup += ToolTip_Popup;

            custom_device = new List<string>();
            _useUncoloredDevices = true;
            //在窗體加載時在listbox1中加載本地化裝置名稱
            Device alldevice = new Device();
            foreach (string name in LocalizationManager.GetAllDeviceNames())
            {
                listBox1.Items.Add(name);
            }

            // 检查文件夹是否存在，如果不存在则创建它
            if (!Directory.Exists(storePictureaddress))
            {
                Directory.CreateDirectory(storePictureaddress);
            }
            //讀取圖片名字到listbox，
            alldevice.RetrieveImagesFromDirectory(storePictureaddress);
            //將字典中的key即裝置名添加到listbox1中
            foreach (var key in alldevice.Costumer_device.Keys)
            {
                listBox1.Items.Add(key);
            }
            Custom_device = alldevice.custome_device;

            //load listbox3 (保留用於數據遷移)
            panelinfos_to_listbox3 = new Dictionary<string, List<PanelInfo>>();
            LoadPanelInfosDictionaryBinary();

            // 載入模板庫樹狀結構
            templateLibraryData = new List<TemplateTreeNodeData>();
            LoadTemplateLibrary();

            _panelManager = new PanelManager(panel2, new Font("宋体", 12, FontStyle.Regular), new List<Panel>(), listbox3_binary_path, panelinfos_to_listbox3);
            _panelSampleLibrarySaver = new PanelSampleLibrarySaver(_panelManager, templateLibraryData, treeViewTemplates, SaveTemplateLibrary);

            // 將樹狀數據載入到 TreeView
            LoadTemplateTreeView();

            //3.31 treeViewTemplates 右鍵菜單
            contextMenuTemplates = new ContextMenuStrip();
            menuItemAddToLibrary = new ToolStripMenuItem(LocalizationManager.GetString("AddSampleToLibrary"));
            menuItemAddToLibrary.Click += MenuItemAddToLibrary_Click;
            menuItemRename = new ToolStripMenuItem(LocalizationManager.GetString("Rename"));
            menuItemRename.Click += MenuItemRename_Click;
            menuItemDelete = new ToolStripMenuItem(LocalizationManager.GetString("Delete"));
            menuItemDelete.Click += MenuItemDelete_Click;
            contextMenuTemplates.Items.Add(menuItemAddToLibrary);
            contextMenuTemplates.Items.Add(menuItemRename);
            contextMenuTemplates.Items.Add(menuItemDelete);
            MenuStyleHelper.Apply(contextMenuTemplates);


            //listBox1 右鍵菜單：添加自繪裝置和刪除用戶自定義裝置
            contextMenuStripListBox1 = new ContextMenuStrip();
            addDeviceMenuItemListBox1 = new ToolStripMenuItem(LocalizationManager.GetString("AddCustomDevice"));
            addDeviceMenuItemListBox1.Click += AddDeviceMenuItem_Click_ListBox1;
            deleteMenuItemListBox1 = new ToolStripMenuItem(LocalizationManager.GetString("DeleteCurrentDevice"));
            deleteMenuItemListBox1.Click += DeleteMenuItem_Click_ListBox1;
            contextMenuStripListBox1.Items.Add(addDeviceMenuItemListBox1);
            contextMenuStripListBox1.Items.Add(deleteMenuItemListBox1);
            MenuStyleHelper.Apply(contextMenuStripListBox1);

            //panel2 右鍵菜單：自動截圖、截圖、清空畫布、打開裝置樣例、自動對齊、刪除
            contextMenuStripPanel2 = new ContextMenuStrip();
            deleteMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("Delete"));
            deleteMenuItemPanel2.Click += DeleteMenuItem_Click_Panel2;
            separatorPanel2 = new ToolStripSeparator();
            autoCaptureMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("AutoCapture"));
            autoCaptureMenuItemPanel2.Click += AutoCaptureMenuItem_Click_Panel2;
            captureMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("Capture"));
            captureMenuItemPanel2.Click += CaptureMenuItem_Click_Panel2;
            clearCanvasMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("ClearCanvas"));
            clearCanvasMenuItemPanel2.Click += ClearCanvasMenuItem_Click_Panel2;
            openSampleMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("OpenSample"));
            openSampleMenuItemPanel2.Click += OpenSampleMenuItem_Click_Panel2;
            autoAlignMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("AutoAlign"));
            autoAlignMenuItemPanel2.Click += AutoAlignMenuItem_Click_Panel2;
            saveSampleMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("AddSampleToLibrary"));
            saveSampleMenuItemPanel2.Click += SaveSampleMenuItem_Click_Panel2;
            contextMenuStripPanel2.Items.Add(autoAlignMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(deleteMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(separatorPanel2);
            contextMenuStripPanel2.Items.Add(autoCaptureMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(captureMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(clearCanvasMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(openSampleMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(saveSampleMenuItemPanel2);
            MenuStyleHelper.Apply(contextMenuStripPanel2);

            //panel2鼠標滾動處理事件
            panel2.MouseWheel += new MouseEventHandler(Panel2_MouseWheel);
            capturelist = new List<Bitmap>();

            //初始化截圖管理類
            captureManager = new ScreenCaptureManager(panel2);

            // 初始化自動對齊管理器
            autoAlignManager = new AutoAlignManager(panel2);

            // 初始化自動截圖管理器
            autoCaptureManager = new AutoCaptureManager(panel2, panel2Container);

            // 讀取 tagTreeUserControl1 的樹結構數據
            tagTreeUserControl1.LoadTreeData();

            // 訂閱 tagTreeUserControl1 的節點選中事件
            tagTreeUserControl1.NodeSelected += TagTreeUserControl1_NodeSelected;
            // 訂閱 tagTreeUserControl1 的 MouseMove 事件，用於維持預覽圖片
            tagTreeUserControl1.TreeViewMouseMove += TagTreeUserControl1_MouseMove;
            // 訂閱 tagTreeUserControl1 的 MouseDown 事件，用於右鍵取消預覽和選定
            tagTreeUserControl1.TreeViewMouseDown += TagTreeUserControl1_MouseDown;
            // 補充 treeViewTemplates 的 MouseDown 事件訂閱
            treeViewTemplates.MouseDown += treeViewTemplates_MouseDown;

            labelSettings.BringToFront();
            int settingsButtonWidth = TextRenderer.MeasureText(labelSettings.Text, labelSettings.Font).Width + 12;
            if (labelSettings.Width < settingsButtonWidth)
            {
                labelSettings.Width = settingsButtonWidth;
            }

            // 應用本地化
            ApplyLocalization();
        }

        /// <summary>
        /// 當 tagTreeUserControl1 中的節點被選中時
        /// </summary>
        private void TagTreeUserControl1_NodeSelected(object sender, EventArgs e)
        {
            // 取消 listBox1 的選定
            listBox1.ClearSelected();
            fixedIndex = -1;

            // 隱藏預覽圖片
            if (previewDevice != null)
            {
                this.Controls.Remove(previewDevice);
                previewDevice.Dispose();
                previewDevice = null;
            }
        }

        private void textBoxTagSearch_TextChanged(object sender, EventArgs e)
        {
            tagTreeUserControl1.ApplyFilter(textBoxTagSearch.Text);
        }

        private void labelSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsDialog();
        }

        /// <summary>
        /// 顯示設置對話框
        /// </summary>
        private void ShowSettingsDialog()
        {
            using (Form settingsForm = new Form())
            {
                settingsForm.Text = LocalizationManager.GetString("SettingsTitle");
                settingsForm.StartPosition = FormStartPosition.CenterParent;
                settingsForm.Width = 400;
                settingsForm.Height = 250;
                settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                settingsForm.MaximizeBox = false;
                settingsForm.MinimizeBox = false;

                Label langLabel = new Label()
                {
                    Text = LocalizationManager.GetString("LanguageLabel"),
                    Left = 20,
                    Top = 25,
                    Width = 160,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                ComboBox langCombo = new ComboBox()
                {
                    Left = 180,
                    Top = 23,
                    Width = 180,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                langCombo.Items.Add("English");
                langCombo.Items.Add("简体中文");
                langCombo.Items.Add("繁體中文");
                langCombo.Items.Add("Español");
                langCombo.Items.Add("Français");
                langCombo.Items.Add("Português");
                langCombo.Items.Add("Русский");
                langCombo.Items.Add("فارسی");
                langCombo.Items.Add("Norsk");
                langCombo.Items.Add("العربية");

                // 設置當前選中項
                switch (LocalizationManager.CurrentLanguage)
                {
                    case Language.English: langCombo.SelectedIndex = 0; break;
                    case Language.SimplifiedChinese: langCombo.SelectedIndex = 1; break;
                    case Language.TraditionalChinese: langCombo.SelectedIndex = 2; break;
                    case Language.Spanish: langCombo.SelectedIndex = 3; break;
                    case Language.French: langCombo.SelectedIndex = 4; break;
                    case Language.Portuguese: langCombo.SelectedIndex = 5; break;
                    case Language.Russian: langCombo.SelectedIndex = 6; break;
                    case Language.Persian: langCombo.SelectedIndex = 7; break;
                    case Language.Norwegian: langCombo.SelectedIndex = 8; break;
                    case Language.Arabic: langCombo.SelectedIndex = 9; break;
                }

                Label uncolorLabel = new Label()
                {
                    Text = LocalizationManager.GetString("UseUncoloredDeviceLabel"),
                    Left = 20,
                    Top = 65,
                    Width = 160,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                ComboBox uncolorCombo = new ComboBox()
                {
                    Left = 180,
                    Top = 63,
                    Width = 180,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                uncolorCombo.Items.Add(LocalizationManager.GetString("UseColoredDevice"));
                uncolorCombo.Items.Add(LocalizationManager.GetString("UseUncoloredDevice"));
                uncolorCombo.SelectedIndex = _useUncoloredDevices ? 1 : 0;

                Button okButton = new Button()
                {
                    Text = LocalizationManager.GetString("OK"),
                    Left = 20,
                    Top = 110,
                    Width = 167,
                    Height = 38,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(60, 63, 70),
                    ForeColor = Color.White
                };
                okButton.FlatAppearance.BorderSize = 0;
                okButton.Click += (s, ev) =>
                {
                    settingsForm.DialogResult = DialogResult.OK;
                    settingsForm.Close();
                };

                Button cancelButton = new Button()
                {
                    Text = LocalizationManager.GetString("Cancel"),
                    Left = 193,
                    Top = 110,
                    Width = 167,
                    Height = 38,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    FlatStyle = FlatStyle.Flat
                };
                cancelButton.FlatAppearance.BorderColor = Color.FromArgb(180, 184, 190);
                cancelButton.Click += (s, ev) =>
                {
                    settingsForm.DialogResult = DialogResult.Cancel;
                    settingsForm.Close();
                };

                LinkLabel helpLink = new LinkLabel()
                {
                    Text = LocalizationManager.GetString("Help"),
                    Font = new Font("Microsoft YaHei UI", 9F),
                    AutoSize = true,
                    Top = 162,
                    LinkColor = Color.FromArgb(100, 100, 100),
                    ActiveLinkColor = Color.FromArgb(60, 63, 70)
                };
                helpLink.Left = (settingsForm.ClientSize.Width - helpLink.PreferredWidth) / 2;
                helpLink.Click += (s, ev) =>
                {
                    using (HelpDialog dialog = new HelpDialog())
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        dialog.ShowDialog(settingsForm);
                    }
                };

                settingsForm.AcceptButton = okButton;
                settingsForm.CancelButton = cancelButton;
                settingsForm.Controls.Add(langLabel);
                settingsForm.Controls.Add(langCombo);
                settingsForm.Controls.Add(uncolorLabel);
                settingsForm.Controls.Add(uncolorCombo);
                settingsForm.Controls.Add(okButton);
                settingsForm.Controls.Add(cancelButton);
                settingsForm.Controls.Add(helpLink);

                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    Language selectedLang;
                    switch (langCombo.SelectedIndex)
                    {
                        case 0: selectedLang = Language.English; break;
                        case 1: selectedLang = Language.SimplifiedChinese; break;
                        case 2: selectedLang = Language.TraditionalChinese; break;
                        case 3: selectedLang = Language.Spanish; break;
                        case 4: selectedLang = Language.French; break;
                        case 5: selectedLang = Language.Portuguese; break;
                        case 6: selectedLang = Language.Russian; break;
                        case 7: selectedLang = Language.Persian; break;
                        case 8: selectedLang = Language.Norwegian; break;
                        case 9: selectedLang = Language.Arabic; break;
                        default: selectedLang = Language.English; break;
                    }

                    if (selectedLang != LocalizationManager.CurrentLanguage)
                    {
                        LocalizationManager.SetLanguage(selectedLang);
                        ApplyLocalization();
                    }

                    bool useUncolored = uncolorCombo.SelectedIndex == 1;
                    _useUncoloredDevices = useUncolored;
                }
            }
        }

        /// <summary>
        /// 應用本地化字符串到所有 UI 控件
        /// </summary>
        private void ApplyLocalization()
        {
            // 主窗體標題
            this.Text = LocalizationManager.GetString("FormTitle") + "                           ";

            // 頂部標籤
            label1.Text = LocalizationManager.GetString("DeviceSelection");
            label3.Text = LocalizationManager.GetString("TagManagement");
            label4.Text = LocalizationManager.GetString("BOPConfig");
            label2.Text = LocalizationManager.GetString("SavedTemplates");
            labelSettings.Text = LocalizationManager.GetString("Settings");

            // Panel2 右鍵菜單
            deleteMenuItemPanel2.Text = LocalizationManager.GetString("Delete");
            autoCaptureMenuItemPanel2.Text = LocalizationManager.GetString("AutoCapture");
            captureMenuItemPanel2.Text = LocalizationManager.GetString("Capture");
            clearCanvasMenuItemPanel2.Text = LocalizationManager.GetString("ClearCanvas");
            openSampleMenuItemPanel2.Text = LocalizationManager.GetString("OpenSample");
            autoAlignMenuItemPanel2.Text = LocalizationManager.GetString("AutoAlign");
            saveSampleMenuItemPanel2.Text = LocalizationManager.GetString("AddSampleToLibrary");

            // ListBox1 右鍵菜單
            addDeviceMenuItemListBox1.Text = LocalizationManager.GetString("AddCustomDevice");
            deleteMenuItemListBox1.Text = LocalizationManager.GetString("DeleteCurrentDevice");

            // 模板樹右鍵菜單
            menuItemDelete.Text = LocalizationManager.GetString("Delete");
            menuItemRename.Text = LocalizationManager.GetString("Rename");
            menuItemAddToLibrary.Text = LocalizationManager.GetString("AddSampleToLibrary");

            // 標籤樹右鍵菜單
            tagTreeUserControl1.ApplyLocalization();

            RefreshDeviceList();

            // 重新調整按鈕佈局
            AdjustHelpButtonLayout();
        }

        private void RefreshDeviceList()
        {
            int selectedIndex = listBox1.SelectedIndex;
            string selectedCustom = selectedIndex >= LocalizationManager.BuiltInDeviceCount && selectedIndex < listBox1.Items.Count
                ? listBox1.SelectedItem?.ToString()
                : null;

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            foreach (string name in LocalizationManager.GetAllDeviceNames())
            {
                listBox1.Items.Add(name);
            }
            if (Custom_device != null)
            {
                foreach (var key in Custom_device.Keys)
                {
                    listBox1.Items.Add(key);
                }
            }
            listBox1.EndUpdate();

            if (selectedIndex >= 0)
            {
                if (selectedIndex < LocalizationManager.BuiltInDeviceCount)
                {
                    listBox1.SelectedIndex = selectedIndex;
                }
                else if (!string.IsNullOrEmpty(selectedCustom))
                {
                    int idx = listBox1.Items.IndexOf(selectedCustom);
                    if (idx >= 0)
                    {
                        listBox1.SelectedIndex = idx;
                    }
                }
            }
        }
        private void Panel2_MouseWheel(object sender, MouseEventArgs e)
        {
            int moveStep = 50; // 每次滚动移动的像素数

            // 計算最大滾動值
            int maxScrollValue = Math.Max(0, panel2.Height - panel2Container.ClientSize.Height);

            if (e.Delta > 0)
            {
                // 向上滚动
                vScrollBarPanel2.Value = Math.Max(0, vScrollBarPanel2.Value - moveStep);
            }
            else
            {
                // 向下滚动
                vScrollBarPanel2.Value = Math.Min(maxScrollValue, vScrollBarPanel2.Value + moveStep);
            }

            // 根据滚动条值更新 panel2 位置（在容器内滚动）
            panel2.Top = -vScrollBarPanel2.Value;

            if (isCapturing)
            {
                captureManager.HandleScroll(e.Delta, moveStep);
            }
        }

        /// <summary>
        /// panel2 滚动条滚动事件
        /// </summary>
        private void vScrollBarPanel2_Scroll(object sender, ScrollEventArgs e)
        {
            // 計算最大滾動值
            int maxScrollValue = Math.Max(0, panel2.Height - panel2Container.ClientSize.Height);

            // 限制滾動範圍
            int scrollValue = Math.Max(0, Math.Min(maxScrollValue, e.NewValue));

            // panel2 在容器内滚动，Top 为负值表示向上滚动
            panel2.Top = -scrollValue;
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            isrightclick_while_cursor_leave_listbox1 = false;
            is_show_the_tootip = false;

            isSelecting = true;

            if (e.Button == MouseButtons.Right)
            {
                toolTip.Hide(panel1);
                toolTip.Hide(panel2);

                if (isCapturing)
                {
                    isCapturing = false;
                    this.Cursor = Cursors.Default;
                }

                // 先檢查是否點擊了 panel2 中的控件
                clickedControlOnPanel2 = null;
                foreach (Control ctrl in panel2.Controls)
                {
                    if (ctrl.Bounds.Contains(e.Location))
                    {
                        clickedControlOnPanel2 = ctrl;
                        break;
                    }
                }

                // 如果 listBox1 或 tagTreeUserControl1 有選中內容
                if (listBox1.SelectedIndex != -1 || tagTreeUserControl1.HasSelection)
                {
                    // 隱藏預覽圖片
                    if (previewDevice != null)
                    {
                        this.Controls.Remove(previewDevice);
                        previewDevice.Dispose();
                        previewDevice = null;
                    }
                    listBox1.ClearSelected();
                    fixedIndex = -1;
                    tagTreeUserControl1.ClearSelection();

                    // 如果點擊的是空白處，只取消選定不彈出菜單
                    if (clickedControlOnPanel2 == null)
                    {
                        return;
                    }
                }

                // 根據是否點擊了控件顯示/隱藏刪除選項和分隔線
                if (clickedControlOnPanel2 != null)
                {
                    deleteMenuItemPanel2.Visible = true;
                    separatorPanel2.Visible = true;
                }
                else
                {
                    deleteMenuItemPanel2.Visible = false;
                    separatorPanel2.Visible = false;
                }

                // 確保其他選項始終可見
                autoCaptureMenuItemPanel2.Visible = true;
                captureMenuItemPanel2.Visible = true;
                clearCanvasMenuItemPanel2.Visible = true;
                openSampleMenuItemPanel2.Visible = true;
                autoAlignMenuItemPanel2.Visible = true;
                saveSampleMenuItemPanel2.Visible = true;

                contextMenuStripPanel2.Show(panel2, e.Location);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (previewDevice != null)
                {
                    previewDevice.Dispose();
                }
                if (toolTip != null)
                {
                    toolTip.Hide(panel2);
                }

                if (listBox1.SelectedIndex != -1 && !tagTreeUserControl1.HasSelection)
                {
                    if (listBox1.SelectedIndex <= 21)
                    {
                        CreateSvgPanel(e.Location, SvgData);
                    }
                    else
                    {
                        CreateImagePanel(e.Location, getImage);
                    }
                }
                else if (tagTreeUserControl1.HasSelection)
                {
                    // 使用 tagTreeUserControl1 選中節點的文字創建文字面板
                    CreateTextPanel(e.Location, tagTreeUserControl1.SelectedNodeText);
                }
                else if (isCapturing)
                {
                    captureManager.StartCapture(e.Location);
                }
                else if (isSelecting)
                {
                    selectionRectangle = new Rectangle(e.Location, new Size(0, 0));
                }
            }
        }
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isrightclick_while_cursor_leave_listbox1 == false && listBox1.SelectedIndex != -1)
            {
                // 如果已经有一个 PictureBox，先移除并释放它
                if (previewDevice != null)
                {
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }
                // 新建一个 PictureBox 用来存储预览的装置
                previewDevice = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Image = getImage,
                    Visible = true
                };
                this.Controls.Add(previewDevice);

                // 设置 PictureBox 的图像和位置

                Point cursorPosition = this.PointToClient(Cursor.Position);
                previewDevice.Location = new Point(cursorPosition.X + 5, cursorPosition.Y + 5); // 调整位置以避免覆盖鼠标指针
                previewDevice.BringToFront();
            }

            if (is_show_the_tootip == true)
            {
                // 顯示 tagTreeUserControl1 選中節點的提示
                if (listBox1.SelectedIndex == -1 && tagTreeUserControl1.HasSelection)
                {
                    toolTip.Show(tagTreeUserControl1.SelectedNodeText, panel2, e.X + 10, e.Y + 10);
                }
            }

            if (isCapturing && e.Button == MouseButtons.Left)
            {
                captureManager.UpdateCapture(e.Location);
            }
        }
        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            if (isCapturing && e.Button == MouseButtons.Left)
            {
                captureManager.FinalizeCapture(e.Location);

            }
            if (isSelecting && e.Button == MouseButtons.Left)
            {
                isSelecting = false;
                // 选中所有在长方形中的Panel
                var selectedPanels = dynamicPanels.Where(p => selectionRectangle.Contains(p.Bounds)).ToList();
                if (selectedPanels.Any())
                {
                    // 添加拖动和缩放功能
                    foreach (var panel in selectedPanels)
                    {

                    }
                }
                selectionRectangle = Rectangle.Empty;
                panel2.Invalidate();
            }
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (isCapturing)
            {
                captureManager.DrawOverlay(e.Graphics);
            }
        }







        /// <summary>
        /// listbox1选择项改变时发生的事件，选择井口装置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tagTreeUserControl1.ClearSelection();
            if (listBox1.SelectedIndex != -1)
            {
                int idx = listBox1.SelectedIndex;
                if (idx < LocalizationManager.BuiltInDeviceCount)
                {
                    if (_useUncoloredDevices == false)
                    {
                        switch (idx)
                        {
                            case 0:  getImage = Properties.Resource.转盘面; break;
                            case 1:  getImage = Properties.Resource.喇叭口; break;
                            case 2:  getImage = Properties.Resource.密封盘根升高短节; break;
                            case 3:  getImage = Properties.Resource.旋转控制头; break;
                            case 4:  getImage = Properties.Resource.临时井口头; break;
                            case 5:  getImage = Properties.Resource.万能防喷器; break;
                            case 6:  getImage = Properties.Resource.闸板防喷器; break;
                            case 7:  getImage = Properties.Resource.双闸板防喷器; break;
                            case 8:  getImage = Properties.Resource.钻井四通; break;
                            case 9:  getImage = Properties.Resource.油管四通; break;
                            case 10: getImage = Properties.Resource.油管四通; break;
                            case 11: getImage = Properties.Resource.法兰; break;
                            case 12: getImage = Properties.Resource.法兰; break;
                            case 13: getImage = Properties.Resource.升高立管; break;
                            case 14: getImage = Properties.Resource.套管头; break;
                            case 15: getImage = Properties.Resource.井口平台; break;
                            case 16: getImage = Properties.Resource.分流器; break;
                            case 17: getImage = Properties.Resource.单层套管; break;
                            case 18: getImage = Properties.Resource.双层套管; break;
                            case 19: getImage = Properties.Resource.三层套管; break;
                            case 20: getImage = Properties.Resource.單筒雙井; break;
                            case 21: getImage = Properties.Resource.隔水導管; break;
                            case 22: getImage = Properties.Resource.節流壓井管匯; break;
                        }
                    }
                    else
                    {
                        switch (idx)
                        {
                            case 0:  getImage = Properties.Resource.轉盤面; SvgData = Properties.Resource.转盘面1; break;
                            case 1:  getImage = Properties.Resource.喇叭口1; SvgData = Properties.Resource.喇叭口2; break;
                            case 2:  getImage = Properties.Resource.密封盤根升高短節; SvgData = Properties.Resource.密封盘根升高短节1; break;
                            case 3:  getImage = Properties.Resource.旋轉控制頭; SvgData = Properties.Resource.精细控压旋转控制头; break;
                            case 4:  getImage = Properties.Resource.臨時京口頭; SvgData = Properties.Resource.临时井口头1; break;
                            case 5:  getImage = Properties.Resource.萬能; SvgData = Properties.Resource.环形防喷器; break;
                            case 6:  getImage = Properties.Resource.閘板防噴器; SvgData = Properties.Resource.闸板防喷器1; break;
                            case 7:  getImage = Properties.Resource.雙閘版防噴器; SvgData = Properties.Resource.双闸板防喷器1; break;
                            case 8:  getImage = Properties.Resource.鑽井四通; SvgData = Properties.Resource.钻井四通1; break;
                            case 9:  getImage = Properties.Resource.鑽井四通; SvgData = Properties.Resource.钻井四通1; break;
                            case 10: getImage = Properties.Resource.鑽井四通; SvgData = Properties.Resource.钻井四通1; break;
                            case 11: getImage = Properties.Resource.法蘭; SvgData = Properties.Resource.变径法兰; break;
                            case 12: getImage = Properties.Resource.法蘭; SvgData = Properties.Resource.变径法兰; break;
                            case 13: getImage = Properties.Resource.升高1; SvgData = Properties.Resource.升高立管1; break;
                            case 14: getImage = Properties.Resource.套管頭; SvgData = Properties.Resource.套管头1; break;
                            case 15: getImage = Properties.Resource.井口平臺; SvgData = Properties.Resource.井口平台1; break;
                            case 16: getImage = Properties.Resource.分流器11; SvgData = Properties.Resource.分流器2; break;
                            case 17: getImage = Properties.Resource.單層套管; SvgData = Properties.Resource.单层套管1; break;
                            case 18: getImage = Properties.Resource.雙層套管; SvgData = Properties.Resource.双层套管1; break;
                            case 19: getImage = Properties.Resource.三層套管; SvgData = Properties.Resource.三层套管1; break;
                            case 20: getImage = Properties.Resource.單筒雙井; SvgData = Properties.Resource.單筒雙井1; break;
                            case 21: getImage = Properties.Resource.隔水導管; SvgData = Properties.Resource.隔水導管1; break;
                            case 22: getImage = Properties.Resource.節流壓井管匯; SvgData = Properties.Resource.变径法兰; break;
                        }
                    }
                }
                else
                {
                    // 檢查文件是否存在
                    if (File.Exists(Path.Combine(storePictureaddress, listBox1.SelectedItem.ToString())))
                    {
                        using (Bitmap bitmap = new Bitmap(Path.Combine(storePictureaddress, listBox1.SelectedItem.ToString())))
                        {
                            getImage = new Bitmap(bitmap);
                        }
                    }
                    else
                    {
                        MessageBox.Show(LocalizationManager.GetString("ImageNotExist"), LocalizationManager.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }

        }

        /// <summary>
        /// 窗体1加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            panel2.BackColor = Color.White;
            panel2.Visible = true;

            // 動態調整 panel2Container 和滾動條的位置和大小
            AdjustPanel2Layout();
            AdjustHelpButtonLayout();
        }

        /// <summary>
        /// 調整 panel2Container 和滾動條的佈局，確保對齊
        /// </summary>
        private void AdjustPanel2Layout()
        {
            // 計算可視區域：從 listBox1Container 頂部到 listBox3Container 底部
            int topY = listBox1Container.Top;
            int bottomY = listBox3Container.Bottom;
            int visibleHeight = bottomY - topY;

            // 設置滾動條位置和大小
            vScrollBarPanel2.Top = topY;
            vScrollBarPanel2.Height = visibleHeight;
            vScrollBarPanel2.Width = 48; // 明確設置寬度

            // 設置 panel2Container 位置和大小
            panel2Container.Top = topY;
            panel2Container.Height = visibleHeight;

            // panel2Container 右邊緊貼滾動條左邊
            panel2Container.Width = vScrollBarPanel2.Left - panel2Container.Left;

            // panel2 寬度填滿容器（考慮邊框）
            panel2.Width = panel2Container.ClientSize.Width;

            // 設置滾動條的最大值：panel2高度 - 可視區域高度（考慮邊框）
            int maxScroll = Math.Max(0, panel2.Height - panel2Container.ClientSize.Height);
            vScrollBarPanel2.Maximum = maxScroll + vScrollBarPanel2.LargeChange;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustPanel2Layout();
            AdjustHelpButtonLayout();
        }

        private void AdjustHelpButtonLayout()
        {
            if (labelSettings == null || label4 == null)
            {
                return;
            }

            labelSettings.Font = label4.Font;
            labelSettings.Height = label4.Height;
            labelSettings.Top = label4.Top;
            labelSettings.BackColor = label4.BackColor;
            labelSettings.ForeColor = label4.ForeColor;
            labelSettings.TextAlign = label4.TextAlign;

            int settingsTextWidth = TextRenderer.MeasureText(labelSettings.Text, labelSettings.Font, new Size(int.MaxValue, labelSettings.Height), TextFormatFlags.SingleLine).Width;
            int settingsTargetWidth = settingsTextWidth + 32;
            labelSettings.Width = Math.Max(60, settingsTargetWidth);
            labelSettings.Left = (panel2Container != null)
                ? panel2Container.Right - labelSettings.Width
                : label4.Right + 4;
            labelSettings.BringToFront();
        }

        /// <summary>
        /// panel2 右鍵菜單 - 截圖
        /// </summary>
        /// <summary>
        /// panel2 右鍵菜單 - 自動截圖
        /// </summary>
        private void AutoCaptureMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            autoCaptureManager?.Execute();
        }

        private void CaptureMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            isCapturing = true;
            panel2.Cursor = Cursors.Cross;
            listBox1.SelectedIndex = -1;
            if (previewDevice != null)
            {
                getImage = null;
                previewDevice.Dispose();
            }
            tagTreeUserControl1.ClearSelection();
            if (toolTip != null)
            {
                toolTip.Hide(panel1);
                toolTip.Hide(panel2);
            }
        }

        /// <summary>
        /// panel2 右鍵菜單 - 清空畫布
        /// </summary>
        private void ClearCanvasMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            panel2.Refresh();
        }

        /// <summary>
        /// panel2 右鍵菜單 - 打開裝置樣例
        /// </summary>
        private void OpenSampleMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            try
            {
                string tempPath1 = Path.Combine(Path.GetTempPath(), "temp_image1.png");
                Properties.Resource.未上色圖片示例.Save(tempPath1);
                Process.Start("mspaint.exe", $"\"{tempPath1}\"");

                string tempPath2 = Path.Combine(Path.GetTempPath(), "temp_image2.png");
                Properties.Resource.上色圖片示例.Save(tempPath2);
                Process.Start("mspaint.exe", $"\"{tempPath2}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(LocalizationManager.GetString("ErrorOccurred", ex.Message), LocalizationManager.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 公開方法 - 開始截圖（供自定義控件調用）
        /// </summary>
        public void StartCapture()
        {
            CaptureMenuItem_Click_Panel2(null, EventArgs.Empty);
        }

        /// <summary>
        /// 公開方法 - 自動截圖（供自定義控件調用）
        /// </summary>
        public void StartAutoCapture()
        {
            autoCaptureManager?.Execute();
        }

        /// <summary>
        /// 公開方法 - 打開裝置樣例（供自定義控件調用）
        /// </summary>
        public void OpenDeviceSample()
        {
            OpenSampleMenuItem_Click_Panel2(null, EventArgs.Empty);
        }

        /// <summary>
        /// panel2 右鍵菜單 - 刪除選中的控件
        /// </summary>
        private void DeleteMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            if (clickedControlOnPanel2 != null)
            {
                panel2.Controls.Remove(clickedControlOnPanel2);
                clickedControlOnPanel2.Dispose();
                clickedControlOnPanel2 = null;
            }
        }

        /// <summary>
        /// panel2 右鍵菜單 - 自動對齊
        /// 將所有控件按原有上下順序重新排列，從頂端150px開始，居中且緊鄰
        /// </summary>
        private void AutoAlignMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            AutoAlignControls();
        }

        /// <summary>
        /// panel2 右鍵菜單 - 添加樣例到庫
        /// </summary>
        private void SaveSampleMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            _panelSampleLibrarySaver?.PromptAndSave(this);
        }

        /// <summary>
        /// 公開方法 - 自動對齊（供自定義控件調用）
        /// 使用 AutoAlignManager 管理器執行自動對齊
        /// </summary>
        public void AutoAlignControls()
        {
            autoAlignManager?.Execute();
        }

        /// <summary>
        /// 公開方法 - 添加樣例到庫（供自定義控件調用）
        /// </summary>
        public void SaveSampleToLibrary()
        {
            _panelSampleLibrarySaver?.PromptAndSave(this);
        }


        /// <summary>
        /// whe cursor move in listbox1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果右鍵取消過，不顯示預覽圖片
            if (isrightclick_while_cursor_leave_listbox1)
            {
                return;
            }

            if (fixedIndex == -1)
            {
                // 获取鼠标当前位置对应的 ListBox 项的索引
                int index = listBox1.IndexFromPoint(e.Location);

                // 如果索引有效且不同于当前选中的索引
                if (index != ListBox.NoMatches && index != listBox1.SelectedIndex)
                {
                    // 设置当前选中的项
                    listBox1.SelectedIndex = index;
                }
                // 如果已经有一个 PictureBox，先移除并释放它
                if (previewDevice != null)
                {
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }

                // 新建一个 PictureBox 用来存储预览的装置
                previewDevice = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Image = getImage,
                    Visible = true
                };
                this.Controls.Add(previewDevice);

                // 设置 PictureBox 的图像和位置

                Point cursorPosition = this.PointToClient(Cursor.Position);
                previewDevice.Location = new Point(cursorPosition.X, cursorPosition.Y); // 调整位置以避免覆盖鼠标指针
                previewDevice.BringToFront();
            }
            else
            {
                // 如果已经有一个 PictureBox，先移除并释放它
                if (previewDevice != null)
                {
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }
                // 新建一个 PictureBox 用来存储预览的装置
                previewDevice = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Image = getImage,
                    Visible = true
                };
                this.Controls.Add(previewDevice);

                // 设置 PictureBox 的图像和位置

                Point cursorPosition = this.PointToClient(Cursor.Position);
                previewDevice.Location = new Point(cursorPosition.X + 5, cursorPosition.Y + 5); // 调整位置以避免覆盖鼠标指针
                previewDevice.BringToFront();
            }




        }
        /// <summary>
        /// 黨鼠標離開listbox1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_MouseLeave(object sender, EventArgs e)
        {
            isCursorleavelistbox1 = true;
            if (fixedIndex == -1 && previewDevice != null)
            {
                previewDevice.Dispose();
            }
        }




        /// <summary>
        /// listbox1 mousedown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int clickedIndex = listBox1.IndexFromPoint(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                // 左鍵點擊：選擇裝置，重置右鍵標誌
                isrightclick_while_cursor_leave_listbox1 = false;
                fixedIndex = clickedIndex;
                if (fixedIndex != ListBox.NoMatches)
                {
                    listBox1.SelectedIndex = fixedIndex;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // 右鍵點擊：隱藏預覽圖片並取消選定
                isrightclick_while_cursor_leave_listbox1 = true;
                if (previewDevice != null)
                {
                    previewDevice.Visible = false;
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }
                listBox1.ClearSelected();
                fixedIndex = -1;

                // 根據點擊位置設置菜單項狀態
                if (clickedIndex == ListBox.NoMatches)
                {
                    // 空白區域：只顯示添加選項
                    addDeviceMenuItemListBox1.Visible = true;
                    deleteMenuItemListBox1.Visible = false;
                }
                else if (clickedIndex <= 22)
                {
                    // 內置裝置：添加可用，刪除灰色不可用
                    addDeviceMenuItemListBox1.Visible = true;
                    deleteMenuItemListBox1.Visible = true;
                    deleteMenuItemListBox1.Enabled = false;
                    listBox1.SelectedIndex = clickedIndex;
                }
                else
                {
                    // 用戶裝置：兩個選項都可用
                    addDeviceMenuItemListBox1.Visible = true;
                    deleteMenuItemListBox1.Visible = true;
                    deleteMenuItemListBox1.Enabled = true;
                    listBox1.SelectedIndex = clickedIndex;
                }
                contextMenuStripListBox1.Show(listBox1, e.Location);
            }
        }

        /// <summary>
        /// 刪除用戶自定義裝置
        /// </summary>
        private void DeleteMenuItem_Click_ListBox1(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index > 22 && index != ListBox.NoMatches)
            {
                string fileName = listBox1.Items[index].ToString();
                string filePath = Path.Combine(storePictureaddress, fileName);

                // 從列表中移除
                listBox1.Items.RemoveAt(index);
                custom_device.Remove(fileName);

                // 刪除本地文件
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(LocalizationManager.GetString("DeleteFileFailed", ex.Message), LocalizationManager.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // 清除選中狀態
                listBox1.ClearSelected();
                fixedIndex = -1;
            }
        }

        /// <summary>
        /// 添加自繪裝置（從右鍵菜單）
        /// </summary>
        private void AddDeviceMenuItem_Click_ListBox1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = LocalizationManager.GetString("ImageFileFilter");
                openFileDialog.Title = LocalizationManager.GetString("SelectCustomDevice");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string fileName = System.IO.Path.GetFileName(selectedFilePath);

                    custom_device.Add(fileName);
                    listBox1.Items.Add(fileName);

                    if (!System.IO.Directory.Exists(storePictureaddress))
                    {
                        System.IO.Directory.CreateDirectory(storePictureaddress);
                    }
                    custome_device_path = storePictureaddress;
                    System.IO.File.Copy(selectedFilePath, Path.Combine(storePictureaddress, Path.GetFileName(selectedFilePath)), true);
                }
            }
        }

        private void listBox1_MouseEnter(object sender, EventArgs e)
        {
            isCursorleavelistbox1 = false;
            isrightclick_while_cursor_leave_listbox1 = false;
            toolTip.Hide(listBox1);
            // 進入 listbox1 時清除 tagTreeUserControl1 的選中狀態
            tagTreeUserControl1.ClearSelection();
        }
        /// <summary>
        /// mousemove in form1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                // 如果已经有一个 PictureBox，先移除并释放它
                if (previewDevice != null)
                {
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }
                // 新建一个 PictureBox 用来存储预览的装置
                previewDevice = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Image = getImage,
                    Visible = true
                };
                this.Controls.Add(previewDevice);

                // 设置 PictureBox 的图像和位置

                Point cursorPosition = this.PointToClient(Cursor.Position);
                previewDevice.Location = new Point(cursorPosition.X + 5, cursorPosition.Y + 5); // 调整位置以避免覆盖鼠标指针
                previewDevice.BringToFront();
            }
            // 顯示 tagTreeUserControl1 選中節點的提示
            else if (tagTreeUserControl1.HasSelection)
            {
                toolTip.Show(tagTreeUserControl1.SelectedNodeText, this, e.X + 10, e.Y + 10);
            }

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isrightclick_while_cursor_leave_listbox1 == false && listBox1.SelectedIndex != -1) ;

            // 顯示 tagTreeUserControl1 選中節點的提示
            if (listBox1.SelectedIndex == -1 && tagTreeUserControl1.HasSelection)
            {
                toolTip.Hide(panel1);
                toolTip.Show(tagTreeUserControl1.SelectedNodeText, panel1, e.Location.X + 10, e.Location.Y + 10);
            }
        }




        private void treeViewTemplates_MouseMove(object sender, MouseEventArgs e)
        {
            if (isrightclick_while_cursor_leave_listbox1 == false && listBox1.SelectedIndex != -1)
            {
                // 如果已经有一个 PictureBox，先移除并释放它
                if (previewDevice != null)
                {
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }
                // 新建一个 PictureBox 用来存储预览的装置
                previewDevice = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Image = getImage,
                    Visible = true
                };
                this.Controls.Add(previewDevice);

                // 设置 PictureBox 的图像和位置

                Point cursorPosition = this.PointToClient(Cursor.Position);
                previewDevice.Location = new Point(cursorPosition.X + 5, cursorPosition.Y + 5); // 调整位置以避免覆盖鼠标指针
                previewDevice.BringToFront();
            }
            // 顯示 tagTreeUserControl1 選中節點的提示
            else if (listBox1.SelectedIndex == -1 && tagTreeUserControl1.HasSelection)
            {
                toolTip.Show(tagTreeUserControl1.SelectedNodeText, treeViewTemplates, e.Location.X + 10, e.Location.Y + 10);
            }
        }

        private void TagTreeUserControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isrightclick_while_cursor_leave_listbox1 == false && listBox1.SelectedIndex != -1)
            {
                // 如果已经有一个 PictureBox，先移除并释放它
                if (previewDevice != null)
                {
                    this.Controls.Remove(previewDevice);
                    previewDevice.Dispose();
                    previewDevice = null;
                }
                // 新建一个 PictureBox 用来存储预览的装置
                previewDevice = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Image = getImage,
                    Visible = true
                };
                this.Controls.Add(previewDevice);

                Point cursorPosition = this.PointToClient(Cursor.Position);
                previewDevice.Location = new Point(cursorPosition.X + 5, cursorPosition.Y + 5);
                previewDevice.BringToFront();
            }
        }

        private void TagTreeUserControl1_MouseDown(object sender, MouseEventArgs e)
        {
            isrightclick_while_cursor_leave_listbox1 = true;
            if (e.Button == MouseButtons.Right && previewDevice != null)
            {
                this.Controls.Remove(previewDevice);
                previewDevice.Dispose();
                previewDevice = null;
            }
            listBox1.ClearSelected();
            fixedIndex = -1;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isrightclick_while_cursor_leave_listbox1 = true;

            if (e.Button == MouseButtons.Right && previewDevice != null)
            {
                this.Controls.Remove(previewDevice);
                previewDevice.Dispose();
                previewDevice = null;
            }
            if (e.Button == MouseButtons.Right)
            {
                toolTip.Hide(panel1);
                tagTreeUserControl1.ClearSelection();
            }
            listBox1.ClearSelected();
            fixedIndex = -1;
        }

        private void treeViewTemplates_MouseDown(object sender, MouseEventArgs e)
        {
            isrightclick_while_cursor_leave_listbox1 = true;
            if (e.Button == MouseButtons.Right && previewDevice != null)
            {
                this.Controls.Remove(previewDevice);
                previewDevice.Dispose();
                previewDevice = null;
            }
            if (e.Button == MouseButtons.Right)
            {
                toolTip.Hide(treeViewTemplates);
                tagTreeUserControl1.ClearSelection();
            }
            listBox1.ClearSelected();
            fixedIndex = -1;
        }

        private void CreateSvgPanel(Point location, byte[] svgData)
        {

            // 创建 SvgDrawPicturePanel 并设置初始位置
            SvgDrawPicturePanel svgPanel = new SvgDrawPicturePanel(location, svgData, panel2, Panel2_MouseWheel, dynamicPanels);

            // 将 SvgDrawPicturePanel 添加到 panel2
            panel2.Controls.Add(svgPanel);

            // 确保新添加的面板在底层
            svgPanel.SendToBack();
        }
        private void CreateImagePanel(Point location, Image image)
        {
            // 创建 ImagePicturePanel 并设置初始位置
            ImagePicturePanel imagePanel = new ImagePicturePanel(location, image, panel2, Panel2_MouseWheel, dynamicPanels);

            // 将 ImagePicturePanel 添加到 panel2
            panel2.Controls.Add(imagePanel);

            // 确保新添加的面板在底层
            imagePanel.SendToBack();
        }

        private void CreateTextPanel(Point location, string text)
        {
            // 创建一个新的 DrawstringPanel，使用放大字體
            DrawstringPanel drawstringPanel = new DrawstringPanel(location, text, tagLabelFont);

            // 将 DrawstringPanel 添加到父控件
            panel2.Controls.Add(drawstringPanel);

            // 确保新添加的面板在顶层
            drawstringPanel.BringToFront();
        }

        /// <summary>
        /// ToolTip 彈出時設置大小
        /// </summary>
        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            // 使用放大字體計算大小
            using (Graphics g = e.AssociatedControl.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(toolTip.GetToolTip(e.AssociatedControl), toolTipFont);
                e.ToolTipSize = new Size((int)textSize.Width + 10, (int)textSize.Height + 6);
            }
        }

        /// <summary>
        /// ToolTip 自定義繪製
        /// </summary>
        private void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            // 繪製背景
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 225)), e.Bounds);
            e.DrawBorder();

            // 使用放大字體繪製文字
            e.Graphics.DrawString(e.ToolTipText, toolTipFont, Brushes.Black, new PointF(5, 3));
        }






        private void panel1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {

        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            is_show_the_tootip = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 保存模板庫樹狀結構
            SaveTemplateLibrary();

            // 保存 tagTreeUserControl1 的樹結構數據
            tagTreeUserControl1.SaveTreeData();
        }



        /// <summary>
        /// 二进制保存字典 panelinfos_to_listbox3
        /// </summary>
        public void SavePanelInfosDictionaryBinary()
        {
            using (FileStream stream = new FileStream(listbox3_binary_path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, panelinfos_to_listbox3);
            }
        }

        /// <summary>
        /// 二进制读取字典 panelinfos_to_listbox3
        /// </summary>
        public void LoadPanelInfosDictionaryBinary()
        {
            if (File.Exists(listbox3_binary_path) && new FileInfo(listbox3_binary_path).Length > 0)
            {
                using (FileStream stream = new FileStream(listbox3_binary_path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    panelinfos_to_listbox3 = (Dictionary<string, List<PanelInfo>>)formatter.Deserialize(stream);
                }
            }
        }

        #region 模板庫樹狀結構序列化

        /// <summary>
        /// 保存模板庫樹狀結構到二進制文件
        /// </summary>
        public void SaveTemplateLibrary()
        {
            try
            {
                if (!IsTemplateFilterActive())
                {
                    // 從 TreeView 更新數據結構
                    templateLibraryData = ConvertTreeViewToData();
                }

                using (FileStream stream = new FileStream(templateLibraryPath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, templateLibraryData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("保存模板庫失敗: " + ex.Message);
            }
        }

        /// <summary>
        /// 從二進制文件讀取模板庫樹狀結構，並處理數據遷移
        /// </summary>
        public void LoadTemplateLibrary()
        {
            try
            {
                if (File.Exists(templateLibraryPath) && new FileInfo(templateLibraryPath).Length > 0)
                {
                    // 從新格式文件讀取
                    using (FileStream stream = new FileStream(templateLibraryPath, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        templateLibraryData = (List<TemplateTreeNodeData>)formatter.Deserialize(stream);
                    }
                }
                else if (panelinfos_to_listbox3 != null && panelinfos_to_listbox3.Count > 0)
                {
                    // 從舊格式遷移數據
                    MigrateFromOldFormat();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("讀取模板庫失敗: " + ex.Message);
                templateLibraryData = new List<TemplateTreeNodeData>();
            }
        }

        /// <summary>
        /// 從舊的 listbox3 字典格式遷移到新的樹狀結構
        /// </summary>
        private void MigrateFromOldFormat()
        {
            templateLibraryData = new List<TemplateTreeNodeData>();
            foreach (var item in panelinfos_to_listbox3)
            {
                // 將舊數據轉換為模板節點（非資料夾）
                TemplateTreeNodeData nodeData = new TemplateTreeNodeData(item.Key, item.Value);
                templateLibraryData.Add(nodeData);
            }
            // 保存遷移後的數據
            SaveTemplateLibrary();
        }

        /// <summary>
        /// 將 TreeView 轉換為可序列化的數據結構
        /// </summary>
        private List<TemplateTreeNodeData> ConvertTreeViewToData()
        {
            List<TemplateTreeNodeData> rootNodes = new List<TemplateTreeNodeData>();
            foreach (TreeNode node in treeViewTemplates.Nodes)
            {
                rootNodes.Add(ConvertTreeNodeToData(node));
            }
            return rootNodes;
        }

        /// <summary>
        /// 將單個 TreeNode 轉換為 TemplateTreeNodeData
        /// </summary>
        private TemplateTreeNodeData ConvertTreeNodeToData(TreeNode node)
        {
            TemplateTreeNodeData existingData = node.Tag as TemplateTreeNodeData;
            if (existingData != null)
            {
                // 更新節點文字（可能被編輯過）
                existingData.Text = node.Text;
                // 更新子節點
                existingData.Children = new List<TemplateTreeNodeData>();
                foreach (TreeNode child in node.Nodes)
                {
                    existingData.Children.Add(ConvertTreeNodeToData(child));
                }
                return existingData;
            }

            // 如果沒有關聯數據，創建新的資料夾節點
            TemplateTreeNodeData data = new TemplateTreeNodeData(node.Text);
            foreach (TreeNode child in node.Nodes)
            {
                data.Children.Add(ConvertTreeNodeToData(child));
            }
            return data;
        }

        /// <summary>
        /// 將數據結構載入到 TreeView
        /// </summary>
        private void LoadTemplateTreeView()
        {
            treeViewTemplates.Nodes.Clear();
            if (templateLibraryData != null)
            {
                foreach (TemplateTreeNodeData nodeData in templateLibraryData)
                {
                    treeViewTemplates.Nodes.Add(ConvertDataToTreeNode(nodeData));
                }
            }
        }

        /// <summary>
        /// 將 TemplateTreeNodeData 轉換為 TreeNode
        /// </summary>
        private TreeNode ConvertDataToTreeNode(TemplateTreeNodeData data)
        {
            TreeNode node = new TreeNode(data.Text);
            node.Tag = data;
            foreach (TemplateTreeNodeData childData in data.Children)
            {
                node.Nodes.Add(ConvertDataToTreeNode(childData));
            }
            return node;
        }

        #endregion

        #region 模板庫搜索功能

        /// <summary>
        /// 模板搜索框文字改變事件
        /// </summary>
        private void textBoxTemplateSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyTemplateFilter(textBoxTemplateSearch.Text);
        }

        /// <summary>
        /// 根據關鍵字過濾模板節點
        /// </summary>
        private void ApplyTemplateFilter(string query)
        {
            string trimmed = query?.Trim() ?? string.Empty;
            _currentTemplateFilter = trimmed;

            treeViewTemplates.BeginUpdate();
            treeViewTemplates.Nodes.Clear();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                // 無搜索內容時顯示全部
                LoadTemplateTreeView();
                treeViewTemplates.EndUpdate();
                return;
            }

            // 過濾並顯示匹配的節點
            if (templateLibraryData != null)
            {
                foreach (TemplateTreeNodeData data in templateLibraryData)
                {
                    TreeNode filteredNode = BuildFilteredTemplateNode(data, trimmed);
                    if (filteredNode != null)
                    {
                        treeViewTemplates.Nodes.Add(filteredNode);
                    }
                }
            }

            treeViewTemplates.ExpandAll();
            treeViewTemplates.EndUpdate();
        }

        /// <summary>
        /// 遞歸過濾模板節點（保留原始數據引用）
        /// </summary>
        private TreeNode BuildFilteredTemplateNode(TemplateTreeNodeData data, string query)
        {
            if (data == null)
            {
                return null;
            }

            bool selfMatch = data.Text != null && data.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
            List<TreeNode> filteredChildren = new List<TreeNode>();

            if (data.Children != null)
            {
                foreach (TemplateTreeNodeData child in data.Children)
                {
                    TreeNode filtered = BuildFilteredTemplateNode(child, query);
                    if (filtered != null)
                    {
                        filteredChildren.Add(filtered);
                    }
                }
            }

            if (selfMatch || filteredChildren.Count > 0)
            {
                TreeNode node = new TreeNode(data.Text);
                node.Tag = data;
                foreach (TreeNode childNode in filteredChildren)
                {
                    node.Nodes.Add(childNode);
                }
                return node;
            }

            return null;
        }

        /// <summary>
        /// 檢查模板過濾是否激活
        /// </summary>
        private bool IsTemplateFilterActive()
        {
            return !string.IsNullOrWhiteSpace(_currentTemplateFilter);
        }

        #endregion








        /// <summary>
        /// TreeView 雙擊事件 - 加載選中模板
        /// </summary>
        private void treeViewTemplates_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 獲取節點關聯的數據
            TemplateTreeNodeData nodeData = e.Node.Tag as TemplateTreeNodeData;
            if (nodeData != null && nodeData.IsTemplate)
            {
                // 使用逆向循环遍历控件集合，以便在移除控件时不会影响循环
                for (int i = panel2.Controls.Count - 1; i >= 0; i--)
                {
                    var control = panel2.Controls[i];
                    if (control.Name != "vScrollBar1")
                    {
                        panel2.Controls.Remove(control);
                    }
                }
                _panelManager.RestorePanels(nodeData.TemplateData);
            }
        }

        /// <summary>
        /// TreeView 單擊事件 - 加載選中模板
        /// </summary>
        private void treeViewTemplates_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            TemplateTreeNodeData nodeData = e.Node.Tag as TemplateTreeNodeData;
            if (nodeData != null && nodeData.IsTemplate)
            {
                // 使用逆向循环遍历控件集合，以便在移除控件时不会影响循环
                for (int i = panel2.Controls.Count - 1; i >= 0; i--)
                {
                    var control = panel2.Controls[i];
                    if (control.Name != "vScrollBar1")
                    {
                        panel2.Controls.Remove(control);
                    }
                }
                _panelManager.RestorePanels(nodeData.TemplateData);
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        /// <summary>
        /// TreeView 右鍵菜單處理
        /// </summary>
        private void treeViewTemplates_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = treeViewTemplates.GetNodeAt(e.Location);

                if (node == null)
                {
                    // 右鍵點擊空白區域：添加樣例到庫
                    treeViewTemplates.SelectedNode = null;
                    menuItemAddToLibrary.Visible = true;
                    menuItemRename.Visible = false;
                    menuItemDelete.Visible = false;
                }
                else
                {
                    // 右鍵點擊節點：重命名、刪除
                    treeViewTemplates.SelectedNode = node;
                    menuItemAddToLibrary.Visible = false;
                    menuItemRename.Visible = true;
                    menuItemDelete.Visible = true;
                }
                contextMenuTemplates.Show(treeViewTemplates, e.Location);
            }
        }

        /// <summary>
        /// 添加樣例到庫（作為根節點）
        /// </summary>
        private void MenuItemAddToLibrary_Click(object sender, EventArgs e)
        {
            _panelSampleLibrarySaver?.PromptAndSave(this);
        }

        /// <summary>
        /// 重命名選中節點
        /// </summary>
        private void MenuItemRename_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewTemplates.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.BeginEdit();
            }
        }

        /// <summary>
        /// 節點重命名完成後更新數據
        /// </summary>
        private void treeViewTemplates_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // 如果用戶取消編輯或新名稱為空，則取消
            if (e.Label == null || string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            // 更新關聯的數據
            TemplateTreeNodeData nodeData = e.Node.Tag as TemplateTreeNodeData;
            if (nodeData != null)
            {
                nodeData.Text = e.Label;
                SaveTemplateLibrary();
            }
        }

        /// <summary>
        /// 刪除選中節點
        /// </summary>
        private void MenuItemDelete_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewTemplates.SelectedNode;
            if (selectedNode != null)
            {
                TemplateTreeNodeData nodeData = selectedNode.Tag as TemplateTreeNodeData;
                string message = nodeData.IsFolder ?
                    LocalizationManager.GetString("ConfirmDeleteFolder", selectedNode.Text) :
                    LocalizationManager.GetString("ConfirmDeleteTemplate", selectedNode.Text);

                if (MessageBox.Show(message, LocalizationManager.GetString("ConfirmDelete"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // 從數據結構中移除
                    if (selectedNode.Parent == null)
                    {
                        // 根節點
                        templateLibraryData.Remove(nodeData);
                    }
                    else
                    {
                        // 子節點
                        TemplateTreeNodeData parentData = selectedNode.Parent.Tag as TemplateTreeNodeData;
                        if (parentData != null)
                        {
                            parentData.Children.Remove(nodeData);
                        }
                    }

                    // 從 TreeView 中移除
                    selectedNode.Remove();
                    SaveTemplateLibrary();
                }
            }
        }



        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            // 设置panel2的滚动位置
            panel2.AutoScrollPosition = new Point(0, e.NewValue);
        }
        // Remove the probaematic line
        // panel2.MouseWheel += new MouseEventHandler(MouseWheel);

        // Add the MouseWheel event handler method

    }
}
