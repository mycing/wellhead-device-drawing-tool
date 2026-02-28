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
        private ToolStripMenuItem menuItemCopyTemplate;
        private ToolStripMenuItem menuItemPasteTemplate;
        private TreeNode _copiedTemplateNode;

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
        private ToolStripMenuItem saveCurrentTemplateMenuItemPanel2;
        private ToolStripMenuItem deleteMenuItemPanel2;
        private ToolStripSeparator separatorPanel2;
        private ToolStripMenuItem importJsonMenuItemPanel2;
        private Control clickedControlOnPanel2;
        private CustomScrollBar hScrollBarPanel2;

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
        private Size _currentDrawBaseSize = Size.Empty;
        private Point _lastPreviewLocation = new Point(int.MinValue, int.MinValue);
        private int _lastHoverIndex = -1;
        private TemplateTreeNodeData _currentEditingTemplate;
        private static readonly HashSet<string> _svgParseLogged = new HashSet<string>();

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
            menuItemCopyTemplate = new ToolStripMenuItem(LocalizationManager.GetString("CopyNode"));
            menuItemCopyTemplate.Click += (s, ev) => { _copiedTemplateNode = treeViewTemplates.SelectedNode; };
            menuItemPasteTemplate = new ToolStripMenuItem(LocalizationManager.GetString("PasteAsChild"));
            menuItemPasteTemplate.Click += MenuItemPasteTemplate_Click;
            menuItemRename = new ToolStripMenuItem(LocalizationManager.GetString("Rename"));
            menuItemRename.Click += MenuItemRename_Click;
            menuItemDelete = new ToolStripMenuItem(LocalizationManager.GetString("Delete"));
            menuItemDelete.Click += MenuItemDelete_Click;
            contextMenuTemplates.Items.Add(menuItemAddToLibrary);
            contextMenuTemplates.Items.Add(menuItemCopyTemplate);
            contextMenuTemplates.Items.Add(menuItemPasteTemplate);
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
            saveCurrentTemplateMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("SaveCurrentTemplate"));
            saveCurrentTemplateMenuItemPanel2.Click += SaveCurrentTemplateMenuItem_Click_Panel2;
            saveSampleMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("AddSampleToLibrary"));
            saveSampleMenuItemPanel2.Click += SaveSampleMenuItem_Click_Panel2;
            contextMenuStripPanel2.Items.Add(autoAlignMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(deleteMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(separatorPanel2);
            contextMenuStripPanel2.Items.Add(autoCaptureMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(captureMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(clearCanvasMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(openSampleMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(saveCurrentTemplateMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(saveSampleMenuItemPanel2);
            contextMenuStripPanel2.Items.Add(new ToolStripSeparator());
            importJsonMenuItemPanel2 = new ToolStripMenuItem(LocalizationManager.GetString("ImportJson"));
            importJsonMenuItemPanel2.Click += ImportJsonMenuItem_Click_Panel2;
            contextMenuStripPanel2.Items.Add(importJsonMenuItemPanel2);
            MenuStyleHelper.Apply(contextMenuStripPanel2);
            contextMenuStripPanel2.Opening += (s, ev) =>
            {
                autoAlignMenuItemPanel2.Text = GetAutoAlignMenuText();
                UpdateSaveCurrentTemplateMenuState();
            };

            //panel2鼠標滾動處理事件
            panel2.MouseWheel += new MouseEventHandler(Panel2_MouseWheel);
            capturelist = new List<Bitmap>();

            // panel2 橫向滾動條（與縱向邏輯對齊）
            hScrollBarPanel2 = new CustomScrollBar();
            hScrollBarPanel2.Orientation = Orientation.Horizontal;
            hScrollBarPanel2.Minimum = 0;
            hScrollBarPanel2.SmallChange = 30;
            hScrollBarPanel2.LargeChange = 200;
            hScrollBarPanel2.TrackColor = vScrollBarPanel2.TrackColor;
            hScrollBarPanel2.ThumbColor = vScrollBarPanel2.ThumbColor;
            hScrollBarPanel2.BorderStyle = vScrollBarPanel2.BorderStyle;
            hScrollBarPanel2.Value = 0;
            hScrollBarPanel2.Scroll += hScrollBarPanel2_Scroll;
            panel1.Controls.Add(hScrollBarPanel2);
            hScrollBarPanel2.BringToFront();

            // 非 listBox1 區域的點擊/滾動，統一關閉預覽並取消選定
            textBoxTagSearch.MouseDown += NonListBoxArea_MouseDown;
            textBoxTemplateSearch.MouseDown += NonListBoxArea_MouseDown;
            listBox3Container.MouseDown += NonListBoxArea_MouseDown;
            vScrollBarPanel2.MouseDown += NonListBoxArea_MouseDown;
            hScrollBarPanel2.MouseDown += NonListBoxArea_MouseDown;
            textBoxTagSearch.MouseWheel += NonListBoxArea_MouseWheel;
            textBoxTemplateSearch.MouseWheel += NonListBoxArea_MouseWheel;
            treeViewTemplates.MouseWheel += NonListBoxArea_MouseWheel;
            panel1.MouseWheel += NonListBoxArea_MouseWheel;

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

            // 應用本地化
            ApplyLocalization();

            // 初始化工具欄按鈕
            InitToolbar();
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
                ClearPreviewDevice();
            }
        }

        private void textBoxTagSearch_TextChanged(object sender, EventArgs e)
        {
            tagTreeUserControl1.ApplyFilter(textBoxTagSearch.Text);
        }

        /// <summary>
        /// 顯示設置對話框
        /// </summary>
        private void ShowSettingsDialog()
        {
            using (Form settingsForm = new Form())
            {
                const float baselineDpi = 216f; // 225% 作為視覺基準
                int dpi = this.DeviceDpi > 0 ? this.DeviceDpi : (int)baselineDpi;
                Func<int, int> scale = v => (int)Math.Ceiling(v * dpi / baselineDpi);

                // ── 先算所有尺寸，最後反推 ClientSize ──────────────
                int spad  = scale(20);
                int sgap  = scale(10);
                int sfw   = scale(480);  // 控件區可用寬度
                int sLblW = scale(180);  // 左列標籤寬
                int sCmbW = sfw - sLblW - sgap;
                int sRowH = scale(42);   // 10pt font(30px) + 12px padding，各 DPI 安全
                int sBtnH = scale(46);
                int sBtnW = (sfw - sgap) / 2;
                int sLnkH = scale(30);

                // 測量底部鏈接文本寬度，確保窗體夠寬
                string repoDisplayText = "GitHub: https://github.com/mycing/wellhead-device-drawing-tool";
                using (Font infoFont = new Font("Microsoft YaHei UI", 8.5F))
                {
                    int textW = System.Windows.Forms.TextRenderer.MeasureText(repoDisplayText, infoFont).Width + spad;
                    if (textW > sfw)
                        sfw = textW;
                    // 重算依賴 sfw 的值
                    sCmbW = sfw - sLblW - sgap;
                    sBtnW = (sfw - sgap) / 2;
                }

                int row1Top  = spad;
                int row2Top  = row1Top + sRowH + sgap;
                int jsonBtnTop = row2Top + sRowH + scale(16);
                int btnTop   = jsonBtnTop + sBtnH + sgap;
                int lnkTop   = btnTop  + sBtnH + sgap;
                int infoTop  = lnkTop  + sLnkH + scale(6);
                int sformH   = infoTop + sLnkH * 2 + spad;   // 精確高度

                settingsForm.Text = LocalizationManager.GetString("SettingsTitle");
                settingsForm.StartPosition = FormStartPosition.CenterParent;
                settingsForm.AutoScaleMode = AutoScaleMode.None;
                settingsForm.ClientSize = new Size(sfw + spad * 2, sformH);
                settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                settingsForm.MaximizeBox = false;
                settingsForm.MinimizeBox = false;

                Label langLabel = new Label()
                {
                    Text = LocalizationManager.GetString("LanguageLabel"),
                    Left = spad, Top = row1Top, Width = sLblW, Height = sRowH,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                ComboBox langCombo = new ComboBox()
                {
                    Left = spad + sLblW + sgap, Top = row1Top + scale(2),
                    Width = sCmbW, Height = sRowH,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                langCombo.Items.AddRange(LanguageOptionMapper.GetDisplayNames().ToArray());
                langCombo.SelectedIndex = LanguageOptionMapper.GetIndex(LocalizationManager.CurrentLanguage);

                Label uncolorLabel = new Label()
                {
                    Text = LocalizationManager.GetString("UseUncoloredDeviceLabel"),
                    Left = spad, Top = row2Top, Width = sLblW, Height = sRowH,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                ComboBox uncolorCombo = new ComboBox()
                {
                    Left = spad + sLblW + sgap, Top = row2Top + scale(2),
                    Width = sCmbW, Height = sRowH,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                uncolorCombo.Items.Add(LocalizationManager.GetString("UseColoredDevice"));
                uncolorCombo.Items.Add(LocalizationManager.GetString("UseUncoloredDevice"));
                uncolorCombo.SelectedIndex = _useUncoloredDevices ? 1 : 0;

                Button importJsonBtn = new Button()
                {
                    Text = LocalizationManager.GetString("ImportJson"),
                    Left = spad, Top = jsonBtnTop, Width = sfw, Height = sBtnH,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(45, 120, 180),
                    ForeColor = Color.White
                };
                importJsonBtn.FlatAppearance.BorderSize = 0;
                importJsonBtn.Click += (s, ev) =>
                {
                    settingsForm.DialogResult = DialogResult.Cancel;
                    settingsForm.Close();
                    ImportJsonToCanvas();
                };

                Button okButton = new Button()
                {
                    Text = LocalizationManager.GetString("OK"),
                    Left = spad, Top = btnTop, Width = sBtnW, Height = sBtnH,
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
                    Left = spad + sBtnW + sgap, Top = btnTop, Width = sBtnW, Height = sBtnH,
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
                    AutoSize = false,
                    Left = spad, Top = lnkTop, Width = sfw, Height = sLnkH,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    LinkColor = Color.FromArgb(100, 100, 100),
                    ActiveLinkColor = Color.FromArgb(60, 63, 70)
                };
                helpLink.Click += (s, ev) =>
                {
                    using (HelpDialog dialog = new HelpDialog())
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        dialog.ShowDialog(settingsForm);
                    }
                };

                string repoUrl = "https://github.com/mycing/wellhead-device-drawing-tool";
                LinkLabel repoLink = new LinkLabel()
                {
                    Text = repoDisplayText,
                    Font = new Font("Microsoft YaHei UI", 8.5F),
                    AutoSize = false,
                    Left = spad, Top = infoTop, Width = sfw, Height = sLnkH,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(80, 80, 80),
                    LinkColor = Color.FromArgb(0, 102, 204),
                    ActiveLinkColor = Color.FromArgb(0, 70, 160),
                    VisitedLinkColor = Color.FromArgb(0, 102, 204)
                };
                repoLink.LinkArea = new LinkArea(8, repoUrl.Length);
                repoLink.Click += (s, ev) =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = repoUrl,
                        UseShellExecute = true
                    });
                };

                Label emailLabel = new Label()
                {
                    Text = "Email: 472070418@qq.com",
                    Font = new Font("Microsoft YaHei UI", 8.5F),
                    AutoSize = false,
                    Left = spad, Top = infoTop + sLnkH, Width = sfw, Height = sLnkH,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(80, 80, 80)
                };

                settingsForm.AcceptButton = okButton;
                settingsForm.CancelButton = cancelButton;
                settingsForm.Controls.AddRange(new Control[]
                {
                    langLabel, langCombo,
                    uncolorLabel, uncolorCombo,
                    importJsonBtn,
                    okButton, cancelButton,
                    helpLink,
                    repoLink, emailLabel
                });

                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    Language selectedLang = LanguageOptionMapper.GetLanguageByIndex(langCombo.SelectedIndex);

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

            // Panel2 右鍵菜單
            deleteMenuItemPanel2.Text = LocalizationManager.GetString("Delete");
            autoCaptureMenuItemPanel2.Text = LocalizationManager.GetString("AutoCapture");
            captureMenuItemPanel2.Text = LocalizationManager.GetString("Capture");
            clearCanvasMenuItemPanel2.Text = LocalizationManager.GetString("ClearCanvas");
            openSampleMenuItemPanel2.Text = LocalizationManager.GetString("OpenSample");
            autoAlignMenuItemPanel2.Text = GetAutoAlignMenuText();
            saveSampleMenuItemPanel2.Text = LocalizationManager.GetString("AddSampleToLibrary");
            saveCurrentTemplateMenuItemPanel2.Text = LocalizationManager.GetString("SaveCurrentTemplate");
            importJsonMenuItemPanel2.Text = LocalizationManager.GetString("ImportJson");

            // ListBox1 右鍵菜單
            addDeviceMenuItemListBox1.Text = LocalizationManager.GetString("AddCustomDevice");
            deleteMenuItemListBox1.Text = LocalizationManager.GetString("DeleteCurrentDevice");

            // 模板樹右鍵菜單
            menuItemDelete.Text = LocalizationManager.GetString("Delete");
            menuItemRename.Text = LocalizationManager.GetString("Rename");
            menuItemAddToLibrary.Text = LocalizationManager.GetString("AddSampleToLibrary");
            menuItemCopyTemplate.Text = LocalizationManager.GetString("CopyNode");
            menuItemPasteTemplate.Text = LocalizationManager.GetString("PasteAsChild");

            // 標籤樹右鍵菜單
            tagTreeUserControl1.ApplyLocalization();

            RefreshDeviceList();

            // 更新工具欄按鈕文字
            UpdateToolbarLocalization();

            // 重新調整按鈕佈局
            AdjustHelpButtonLayout();
            AdjustTagSearchLayout();
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
            // 只要在其他區域發生滾動，立即關閉 listBox1 預覽並取消選定
            DismissListBoxPreviewAndSelection();

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
            DismissListBoxPreviewAndSelection();
            // 計算最大滾動值
            int maxScrollValue = Math.Max(0, panel2.Height - panel2Container.ClientSize.Height);

            // 限制滾動範圍
            int scrollValue = Math.Max(0, Math.Min(maxScrollValue, e.NewValue));

            // panel2 在容器内滚动，Top 为负值表示向上滚动
            panel2.Top = -scrollValue;
        }

        private void hScrollBarPanel2_Scroll(object sender, ScrollEventArgs e)
        {
            DismissListBoxPreviewAndSelection();
            int maxScrollValue = Math.Max(0, panel2.Width - panel2Container.ClientSize.Width);
            int scrollValue = Math.Max(0, Math.Min(maxScrollValue, e.NewValue));
            panel2.Left = -scrollValue;
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
                        ClearPreviewDevice();
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
                    ClearPreviewDevice();
                }
                if (toolTip != null)
                {
                    toolTip.Hide(panel2);
                }

                if (listBox1.SelectedIndex != -1 && !tagTreeUserControl1.HasSelection)
                {
                    if (ShouldDrawSvgForCurrentSelection())
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

                // panel2 左鍵繪製後，立即關閉預覽並取消 listBox1 選定
                DismissListBoxPreviewAndSelection();
            }
        }
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isrightclick_while_cursor_leave_listbox1 == false && listBox1.SelectedIndex != -1)
            {
                ShowPreviewAtCursor(5, 5);
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
                    getImage = BuiltInDeviceCatalog.GetPreviewImage(idx, _useUncoloredDevices);
                    SvgData = _useUncoloredDevices ? BuiltInDeviceCatalog.GetSvgData(idx) : null;

                    if (ShouldDrawSvgForCurrentSelection())
                    {
                        _currentDrawBaseSize = TryGetSvgSize(SvgData);
                    }
                    else
                    {
                        _currentDrawBaseSize = getImage != null ? getImage.Size : Size.Empty;
                    }

                    if ((_currentDrawBaseSize.Width <= 0 || _currentDrawBaseSize.Height <= 0) && getImage != null)
                    {
                        _currentDrawBaseSize = getImage.Size;
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
                            _currentDrawBaseSize = getImage.Size;
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
            AdjustTagSearchLayout();
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
            int hScrollHeight = 26;

            // 設置滾動條位置和大小
            vScrollBarPanel2.Top = topY;
            vScrollBarPanel2.Height = visibleHeight - hScrollHeight;
            vScrollBarPanel2.Width = 48; // 明確設置寬度

            // 設置 panel2Container 位置和大小
            panel2Container.Top = topY;
            panel2Container.Height = visibleHeight - hScrollHeight;

            // panel2Container 右邊緊貼滾動條左邊
            panel2Container.Width = vScrollBarPanel2.Left - panel2Container.Left;

            // 橫向滾動條緊貼 panel2Container 底部
            if (hScrollBarPanel2 != null)
            {
                hScrollBarPanel2.Left = panel2Container.Left;
                hScrollBarPanel2.Top = panel2Container.Bottom;
                hScrollBarPanel2.Width = panel2Container.Width;
                hScrollBarPanel2.Height = hScrollHeight;
            }

            // 先按可視區域重置基準，最終尺寸由 EnsurePanel2ContentVisible 根據內容精確計算
            panel2.Width = panel2Container.ClientSize.Width;
            panel2.Height = panel2Container.ClientSize.Height;

            EnsurePanel2ContentVisible();
        }

        /// <summary>
        /// 保證 panel2 內現有內容可見：高度不夠則增高，左右超出則整體平移回可視區域
        /// </summary>
        private void EnsurePanel2ContentVisible()
        {
            if (panel2 == null || panel2Container == null) return;

            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxRight = 0;
            int maxBottom = 0;
            bool hasContent = false;

            foreach (Control c in panel2.Controls)
            {
                if (c == null || !c.Visible) continue;
                if (c is VScrollBar) continue;
                hasContent = true;
                minX = Math.Min(minX, c.Left);
                minY = Math.Min(minY, c.Top);
                maxRight = Math.Max(maxRight, c.Right);
                maxBottom = Math.Max(maxBottom, c.Bottom);
            }

            if (!hasContent)
            {
                panel2.Width = panel2Container.ClientSize.Width;
                panel2.Height = panel2Container.ClientSize.Height;
                vScrollBarPanel2.Value = 0;
                vScrollBarPanel2.Maximum = vScrollBarPanel2.LargeChange;
                panel2.Top = 0;
                if (hScrollBarPanel2 != null)
                {
                    hScrollBarPanel2.Value = 0;
                    hScrollBarPanel2.Maximum = hScrollBarPanel2.LargeChange;
                    panel2.Left = 0;
                }
                return;
            }

            const int padding = 12;
            // 內容超出左/上邊界時，整體回推到 padding 區域
            if (minX < padding)
            {
                int shift = padding - minX;
                foreach (Control c in panel2.Controls)
                {
                    if (c == null || !c.Visible) continue;
                    if (c is VScrollBar) continue;
                    c.Left += shift;
                }
                maxRight += shift;
            }
            if (minY < padding)
            {
                int shift = padding - minY;
                foreach (Control c in panel2.Controls)
                {
                    if (c == null || !c.Visible) continue;
                    if (c is VScrollBar) continue;
                    c.Top += shift;
                }
                maxBottom += shift;
            }

            // 每次按“當前內容”重算畫布尺寸（可放大也可縮小）
            panel2.Width = Math.Max(panel2Container.ClientSize.Width, maxRight + padding);
            panel2.Height = Math.Max(panel2Container.ClientSize.Height, maxBottom + padding);

            int maxScroll = Math.Max(0, panel2.Height - panel2Container.ClientSize.Height);
            vScrollBarPanel2.Maximum = maxScroll + vScrollBarPanel2.LargeChange;
            vScrollBarPanel2.Value = Math.Min(vScrollBarPanel2.Value, maxScroll);
            panel2.Top = -vScrollBarPanel2.Value;

            if (hScrollBarPanel2 != null)
            {
                int maxHScroll = Math.Max(0, panel2.Width - panel2Container.ClientSize.Width);
                hScrollBarPanel2.Maximum = maxHScroll + hScrollBarPanel2.LargeChange;
                hScrollBarPanel2.Value = Math.Min(hScrollBarPanel2.Value, maxHScroll);
                panel2.Left = -hScrollBarPanel2.Value;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustPanel2Layout();
            AdjustHelpButtonLayout();
            AdjustTagSearchLayout();
        }

        private void AdjustHelpButtonLayout()
        {
            // labelSettings 已合併到工具欄，此方法保留為空以避免修改調用點
        }

        /// <summary>
        /// 動態調整兩個搜索框的位置（跟隨 Form 的 AutoScale 基準）
        /// </summary>
        private void AdjustTagSearchLayout()
        {
            float designDpi = this.AutoScaleDimensions.Width > 0 ? this.AutoScaleDimensions.Width : 240f;
            float runtimeDpi = this.DeviceDpi > 0 ? this.DeviceDpi : designDpi;
            int gap = Math.Max(4, (int)Math.Round(8f * runtimeDpi / designDpi));

            // ── tagTreeUserControl1 上方的搜索框（label3 右側）──
            if (textBoxTagSearch != null && label3 != null && tagTreeUserControl1 != null)
            {
                int searchLeft = label3.Right + gap;
                int searchRight = tagTreeUserControl1.Right;
                textBoxTagSearch.Top = label3.Top;
                textBoxTagSearch.Left = searchLeft;
                textBoxTagSearch.Width = Math.Max(60, searchRight - searchLeft);
                textBoxTagSearch.Height = label3.Height;
                textBoxTagSearch.BringToFront();
            }

            // ── listBox3Container 上方的搜索框（label2 右側）──
            if (textBoxTemplateSearch != null && label2 != null && listBox3Container != null)
            {
                int searchLeft2 = label2.Right + gap;
                int searchRight2 = listBox3Container.Right;
                textBoxTemplateSearch.Top = label2.Top;
                textBoxTemplateSearch.Left = searchLeft2;
                textBoxTemplateSearch.Width = Math.Max(60, searchRight2 - searchLeft2);
                textBoxTemplateSearch.Height = label2.Height;
                textBoxTemplateSearch.BringToFront();
            }
        }

        /// <summary>
        /// 初始化工具欄按鈕
        /// </summary>
        private void InitToolbar()
        {
            flowLayoutToolbar.Controls.Clear();

            // 動態適配：讓工具欄高度跟隨 label1 的實際高度（已經過 DPI 縮放）
            int targetHeight = label1.Height + 4; // 上下各留 2px
            panelToolbar.Height = targetHeight;

            // 按鈕定義：(Unicode符號, 本地化鍵, 點擊事件)
            var buttons = new (string symbol, string locKey, EventHandler handler)[]
            {
                ("\u2795", "ImportJson",          (s, e) => ImportJsonToCanvas()),
                ("\u2702", "ClearCanvas",         (s, e) => ClearCanvasMenuItem_Click_Panel2(s, e)),
                ("\u25A4", "AutoAlign",           (s, e) => AutoAlignMenuItem_Click_Panel2(s, e)),
                ("\u25A3", "Capture",             (s, e) => CaptureMenuItem_Click_Panel2(s, e)),
                ("\u27F3", "AutoCapture",         (s, e) => AutoCaptureMenuItem_Click_Panel2(s, e)),
                ("\u25C7", "AddSampleToLibrary",  (s, e) => SaveSampleMenuItem_Click_Panel2(s, e)),
                ("\u25B7", "OpenSample",          (s, e) => OpenSampleMenuItem_Click_Panel2(s, e)),
                ("\u2699", "Settings",            (s, e) => ShowSettingsDialog()),
                ("\u2753", "Help",                (s, e) => { using (var d = new HelpDialog()) { d.StartPosition = FormStartPosition.CenterParent; d.ShowDialog(this); } }),
            };

            foreach (var (symbol, locKey, handler) in buttons)
            {
                flowLayoutToolbar.Controls.Add(CreateToolbarButton(symbol, locKey, handler));
            }
        }

        /// <summary>
        /// 創建工具欄按鈕
        /// </summary>
        private Button CreateToolbarButton(string symbol, string locKey, EventHandler onClick)
        {
            var btn = new Button();
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(210, 212, 215);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(190, 192, 195);
            btn.BackColor = Color.Transparent;
            // 使用與 label1 相同的字體大小，確保 DPI 下按鈕文字清晰可讀
            btn.Font = label1.Font;
            btn.AutoSize = true;
            btn.Height = label1.Height;
            btn.Margin = new Padding(2, 0, 2, 0);
            btn.Padding = new Padding(6, 0, 6, 0);
            btn.Cursor = Cursors.Hand;
            btn.Text = symbol + " " + LocalizationManager.GetString(locKey);
            btn.Click += onClick;
            btn.Tag = new string[] { symbol, locKey };
            return btn;
        }

        /// <summary>
        /// 更新工具欄按鈕的本地化文字
        /// </summary>
        private void UpdateToolbarLocalization()
        {
            if (flowLayoutToolbar == null) return;
            foreach (Control c in flowLayoutToolbar.Controls)
            {
                if (c is Button btn && btn.Tag is string[] parts && parts.Length == 2)
                {
                    btn.Text = parts[0] + " " + LocalizationManager.GetString(parts[1]);
                }
            }
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
                ClearPreviewDevice();
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
            dynamicPanels.Clear();
            panel2.Refresh();
            EnsurePanel2ContentVisible();
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
        /// panel2 右鍵菜單 - 保存到當前編輯模板
        /// </summary>
        private void SaveCurrentTemplateMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            TemplateTreeNodeData target = ResolveCurrentEditingTemplate();
            if (target == null || !target.IsTemplate)
            {
                return;
            }

            target.TemplateData = _panelManager.SaveAllPanels();
            SaveTemplateLibrary();
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
        /// 根據當前對齊模式返回對應的菜單文字
        /// </summary>
        public string GetAutoAlignMenuText()
        {
            if (autoAlignManager == null) return LocalizationManager.GetString("AutoAlign");
            return autoAlignManager.CurrentMode == 0
                ? LocalizationManager.GetString("AutoAlignCenter")
                : LocalizationManager.GetString("AutoAlignRight");
        }

        /// <summary>
        /// 公開方法 - 添加樣例到庫（供自定義控件調用）
        /// </summary>
        public void SaveSampleToLibrary()
        {
            _panelSampleLibrarySaver?.PromptAndSave(this);
        }

        /// <summary>
        /// 公開方法 - 匯入 JSON 到畫布（供自定義控件調用）
        /// </summary>
        public void ImportJsonToCanvas()
        {
            _currentEditingTemplate = null;
            var importer = new WellheadJsonImporter(panel2, dynamicPanels, tagLabelFont,
                Custom_device, storePictureaddress);
            var dialog = new JsonImportDialog(importer, Custom_device);
            if (dialog.ShowDialog(this) && importer.LastImportedSchemas != null && importer.LastImportedSchemas.Count > 0)
            {
                int batchIndex = 1;
                foreach (var schema in importer.LastImportedSchemas)
                {
                    if (schema == null || schema.Devices == null)
                    {
                        continue;
                    }

                    string rerenderError;
                    if (!importer.Import(JsonConvert.SerializeObject(schema), out rerenderError))
                    {
                        continue;
                    }

                    string groupName = !string.IsNullOrWhiteSpace(schema.Name)
                        ? schema.Name
                        : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    if (importer.LastImportedSchemas.Count > 1)
                    {
                        groupName += $" #{batchIndex}";
                    }
                    batchIndex++;

                    var childTags = new List<string>();
                    foreach (var device in schema.Devices)
                    {
                        string tag = "";
                        if (!string.IsNullOrWhiteSpace(device.Label))
                            tag = device.Label.Replace("\n", " ");
                        if (!string.IsNullOrWhiteSpace(device.Flange))
                        {
                            if (tag.Length > 0) tag += " | ";
                            tag += device.Flange.Replace("\n", " ");
                        }
                        if (!string.IsNullOrWhiteSpace(tag))
                            childTags.Add(tag);
                    }

                    tagTreeUserControl1.AddTagGroup(groupName, childTags);

                    // 同時保存到模板庫（listBox3 / treeViewTemplates）
                    List<PanelInfo> panelInfos = _panelManager.SaveAllPanels();
                    TemplateTreeNodeData newTemplate = new TemplateTreeNodeData(groupName, panelInfos);
                    templateLibraryData.Add(newTemplate);
                    TreeNode newNode = new TreeNode(groupName);
                    newNode.Tag = newTemplate;
                    treeViewTemplates.Nodes.Add(newNode);
                }
                SaveTemplateLibrary();
                AdjustPanel2Layout();
            }
            UpdateSaveCurrentTemplateMenuState();
        }

        private void ImportJsonMenuItem_Click_Panel2(object sender, EventArgs e)
        {
            ImportJsonToCanvas();
        }

        private void ClearPreviewDevice()
        {
            if (previewDevice == null)
            {
                return;
            }

            this.Controls.Remove(previewDevice);
            previewDevice.Dispose();
            previewDevice = null;
            _lastPreviewLocation = new Point(int.MinValue, int.MinValue);
        }

        private void DismissListBoxPreviewAndSelection()
        {
            isrightclick_while_cursor_leave_listbox1 = true;
            if (previewDevice != null)
            {
                ClearPreviewDevice();
            }
            listBox1.ClearSelected();
            fixedIndex = -1;
        }

        private void NonListBoxArea_MouseDown(object sender, MouseEventArgs e)
        {
            DismissListBoxPreviewAndSelection();
        }

        private void NonListBoxArea_MouseWheel(object sender, MouseEventArgs e)
        {
            DismissListBoxPreviewAndSelection();
        }

        private Size GetPreviewTargetSize(Image sourceImage)
        {
            if (sourceImage == null || sourceImage.Width <= 0 || sourceImage.Height <= 0)
            {
                return Size.Empty;
            }

            int idx = listBox1.SelectedIndex;
            bool isCustomDevice = idx >= LocalizationManager.BuiltInDeviceCount;

            // 用戶自定義裝置（索引 22 之後）固定使用原始圖片尺寸，和落圖完全一致
            Size targetSize = isCustomDevice ? sourceImage.Size : _currentDrawBaseSize;
            if (targetSize.Width <= 0 || targetSize.Height <= 0)
            {
                targetSize = sourceImage.Size;
            }
            if (targetSize.Width <= 0 || targetSize.Height <= 0)
            {
                return Size.Empty;
            }

            return targetSize;
        }

        private PictureBox CreateScaledPreviewPictureBox(Image sourceImage)
        {
            Size targetSize = GetPreviewTargetSize(sourceImage);
            if (targetSize.IsEmpty)
            {
                return null;
            }

            return new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = targetSize,
                Image = sourceImage,
                Visible = true
            };
        }

        private bool ShouldDrawSvgForCurrentSelection()
        {
            int idx = listBox1.SelectedIndex;
            return idx >= 0
                   && idx < LocalizationManager.BuiltInDeviceCount
                   && _useUncoloredDevices
                   && SvgData != null
                   && SvgData.Length > 0;
        }

        private Size TryGetSvgSize(byte[] svgData)
        {
            if (svgData == null || svgData.Length == 0)
            {
                LogSvgParseIssue("TryGetSvgSize: empty svgData");
                return Size.Empty;
            }

            try
            {
                using (var stream = new MemoryStream(svgData))
                {
                    var doc = SvgDocument.Open<SvgDocument>(stream);
                    if (doc != null)
                    {
                        int width = (int)Math.Round((float)doc.Width);
                        int height = (int)Math.Round((float)doc.Height);

                        // 某些 SVG 文档 Width/Height 会返回极小值，回退到 ViewBox。
                        if (width <= 2 || height <= 2)
                        {
                            try
                            {
                                if (doc.ViewBox.Width > 2 && doc.ViewBox.Height > 2)
                                {
                                    width = (int)Math.Round(doc.ViewBox.Width);
                                    height = (int)Math.Round(doc.ViewBox.Height);
                                }
                            }
                            catch
                            {
                                // Ignore and continue fallback below.
                            }
                        }

                        if (width > 0 && height > 0)
                        {
                            return new Size(width, height);
                        }

                        LogSvgParseIssue($"TryGetSvgSize: invalid size width={width}, height={height}");
                    }
                }
            }
            catch (Exception ex)
            {
                // SVG 無法解析時，回退到圖片尺寸
                LogSvgParseIssue("TryGetSvgSize exception: " + ex.Message);
            }

            return Size.Empty;
        }

        private void LogSvgParseIssue(string message)
        {
            try
            {
                string key = message ?? string.Empty;
                lock (_svgParseLogged)
                {
                    if (_svgParseLogged.Contains(key))
                    {
                        return;
                    }
                    _svgParseLogged.Add(key);
                }

                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "svg_parse.log");
                File.AppendAllText(logPath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch
            {
                // Never break UI flow because of diagnostics.
            }
        }

        private void ShowPreviewAtCursor(int offsetX, int offsetY)
        {
            Size targetSize = GetPreviewTargetSize(getImage);
            if (targetSize.IsEmpty || getImage == null)
            {
                ClearPreviewDevice();
                return;
            }

            if (previewDevice == null)
            {
                previewDevice = CreateScaledPreviewPictureBox(getImage);
                if (previewDevice == null)
                {
                    return;
                }
                this.Controls.Add(previewDevice);
            }
            else
            {
                if (!this.Controls.Contains(previewDevice))
                {
                    this.Controls.Add(previewDevice);
                }
                if (!ReferenceEquals(previewDevice.Image, getImage))
                {
                    previewDevice.Image = getImage;
                }
                if (previewDevice.Size != targetSize)
                {
                    previewDevice.Size = targetSize;
                }
                previewDevice.Visible = true;
            }

            Point cursorPosition = this.PointToClient(Cursor.Position);
            Point newLocation = new Point(cursorPosition.X + offsetX, cursorPosition.Y + offsetY);
            if (_lastPreviewLocation.X == int.MinValue ||
                Math.Abs(newLocation.X - _lastPreviewLocation.X) >= 2 ||
                Math.Abs(newLocation.Y - _lastPreviewLocation.Y) >= 2)
            {
                previewDevice.Location = newLocation;
                _lastPreviewLocation = newLocation;
            }
            previewDevice.BringToFront();
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

                // 只在 hover 的項目真正改變時才更新選中狀態（避免鼠標抖動導致閃爍）
                if (index != ListBox.NoMatches && index != _lastHoverIndex)
                {
                    _lastHoverIndex = index;
                    if (index != listBox1.SelectedIndex)
                    {
                        listBox1.SelectedIndex = index;
                    }
                }
                ShowPreviewAtCursor(0, 0);
            }
            else
            {
                ShowPreviewAtCursor(5, 5);
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
            _lastHoverIndex = -1;
            if (fixedIndex == -1 && previewDevice != null)
            {
                ClearPreviewDevice();
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
                    ClearPreviewDevice();
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
                ShowPreviewAtCursor(5, 5);
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
                ShowPreviewAtCursor(5, 5);
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
                ShowPreviewAtCursor(5, 5);
            }
        }

        private void TagTreeUserControl1_MouseDown(object sender, MouseEventArgs e)
        {
            bool hadPreview = previewDevice != null;
            DismissListBoxPreviewAndSelection();
            if (e.Button == MouseButtons.Right && hadPreview)
            {
                tagTreeUserControl1.SuppressNextContextMenu = true;
                return;
            }
            listBox1.ClearSelected();
            fixedIndex = -1;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            bool hadPreview = previewDevice != null;
            DismissListBoxPreviewAndSelection();

            if (e.Button == MouseButtons.Right && hadPreview)
            {
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                toolTip.Hide(panel1);
                tagTreeUserControl1.ClearSelection();
            }
            listBox1.ClearSelected();
            fixedIndex = -1;
        }

        private bool _suppressTemplatesContextMenu;

        private void treeViewTemplates_MouseDown(object sender, MouseEventArgs e)
        {
            bool hadPreview = previewDevice != null;
            DismissListBoxPreviewAndSelection();
            if (e.Button == MouseButtons.Right && hadPreview)
            {
                _suppressTemplatesContextMenu = true;
                return;
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
                vScrollBarPanel2.Value = 0;
                panel2.Top = 0;
                if (hScrollBarPanel2 != null)
                {
                    hScrollBarPanel2.Value = 0;
                    panel2.Left = 0;
                }

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
                EnsurePanel2ContentVisible();
                _currentEditingTemplate = ResolveTemplateNodeData(nodeData);
                UpdateSaveCurrentTemplateMenuState();
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
                vScrollBarPanel2.Value = 0;
                panel2.Top = 0;
                if (hScrollBarPanel2 != null)
                {
                    hScrollBarPanel2.Value = 0;
                    panel2.Left = 0;
                }

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
                EnsurePanel2ContentVisible();
                _currentEditingTemplate = ResolveTemplateNodeData(nodeData);
                UpdateSaveCurrentTemplateMenuState();
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
            if (e.Button == MouseButtons.Right && _suppressTemplatesContextMenu)
            {
                _suppressTemplatesContextMenu = false;
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = treeViewTemplates.GetNodeAt(e.Location);

                if (node == null)
                {
                    // 右鍵點擊空白區域：添加樣例到庫、粘貼為根節點
                    treeViewTemplates.SelectedNode = null;
                    menuItemAddToLibrary.Visible = true;
                    menuItemCopyTemplate.Visible = false;
                    menuItemPasteTemplate.Visible = true;
                    menuItemPasteTemplate.Enabled = _copiedTemplateNode != null;
                    menuItemRename.Visible = false;
                    menuItemDelete.Visible = false;
                }
                else
                {
                    // 右鍵點擊節點：複製、粘貼為子節點、重命名、刪除
                    treeViewTemplates.SelectedNode = node;
                    menuItemAddToLibrary.Visible = false;
                    menuItemCopyTemplate.Visible = true;
                    menuItemPasteTemplate.Visible = true;
                    menuItemPasteTemplate.Enabled = _copiedTemplateNode != null;
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
                    if (!IsTemplateNodeInLibrary(_currentEditingTemplate))
                    {
                        _currentEditingTemplate = null;
                    }
                    UpdateSaveCurrentTemplateMenuState();
                }
            }
        }

        /// <summary>
        /// 粘貼節點（作為選中節點的子節點，或作為根節點）
        /// </summary>
        private void MenuItemPasteTemplate_Click(object sender, EventArgs e)
        {
            if (_copiedTemplateNode == null)
                return;

            TemplateTreeNodeData sourceData = _copiedTemplateNode.Tag as TemplateTreeNodeData;
            if (sourceData == null)
                return;

            TemplateTreeNodeData clonedData = CloneTemplateData(sourceData);
            TreeNode clonedNode = ConvertDataToTreeNode(clonedData);

            TreeNode targetNode = treeViewTemplates.SelectedNode;
            if (targetNode != null)
            {
                // 粘貼為子節點
                TemplateTreeNodeData targetData = targetNode.Tag as TemplateTreeNodeData;
                if (targetData != null)
                    targetData.Children.Add(clonedData);
                targetNode.Nodes.Add(clonedNode);
                targetNode.Expand();
            }
            else
            {
                // 粘貼為根節點
                templateLibraryData.Add(clonedData);
                treeViewTemplates.Nodes.Add(clonedNode);
            }

            SaveTemplateLibrary();
        }

        /// <summary>
        /// 深拷貝 TemplateTreeNodeData（包含子節點和模板數據）
        /// </summary>
        private static TemplateTreeNodeData CloneTemplateData(TemplateTreeNodeData source)
        {
            TemplateTreeNodeData clone = new TemplateTreeNodeData();
            clone.Text = source.Text;
            clone.TemplateData = source.TemplateData != null
                ? new List<PanelInfo>(source.TemplateData)
                : null;
            clone.Children = new List<TemplateTreeNodeData>();
            foreach (TemplateTreeNodeData child in source.Children)
            {
                clone.Children.Add(CloneTemplateData(child));
            }
            return clone;
        }

        private TemplateTreeNodeData ResolveTemplateNodeData(TemplateTreeNodeData nodeData)
        {
            if (nodeData == null) return null;
            return nodeData.OriginalData ?? nodeData;
        }

        private TemplateTreeNodeData ResolveCurrentEditingTemplate()
        {
            TemplateTreeNodeData current = ResolveTemplateNodeData(_currentEditingTemplate);
            if (!IsTemplateNodeInLibrary(current))
            {
                _currentEditingTemplate = null;
                return null;
            }
            _currentEditingTemplate = current;
            return current;
        }

        private void UpdateSaveCurrentTemplateMenuState()
        {
            if (saveCurrentTemplateMenuItemPanel2 == null)
            {
                return;
            }
            saveCurrentTemplateMenuItemPanel2.Enabled = ResolveCurrentEditingTemplate() != null;
        }

        private bool IsTemplateNodeInLibrary(TemplateTreeNodeData node)
        {
            if (node == null || templateLibraryData == null)
            {
                return false;
            }

            return ContainsTemplateNodeRecursive(templateLibraryData, node);
        }

        private static bool ContainsTemplateNodeRecursive(List<TemplateTreeNodeData> nodes, TemplateTreeNodeData target)
        {
            if (nodes == null || target == null)
            {
                return false;
            }

            foreach (TemplateTreeNodeData node in nodes)
            {
                if (ReferenceEquals(node, target))
                {
                    return true;
                }
                if (ContainsTemplateNodeRecursive(node.Children, target))
                {
                    return true;
                }
            }

            return false;
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
