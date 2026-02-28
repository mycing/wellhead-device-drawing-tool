using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _4._18
{
    internal sealed class HelpDialog : Form
    {
        private readonly TreeView _tree;
        private readonly RichTextBox _content;
        private readonly Label _titleLabel;
        private readonly Dictionary<string, string> _sections;
        private bool _isRtl;

        // Shown 事件中需要重新縮放的控件
        private SplitContainer _split;
        private Label _navHeader;
        private Panel _titleBar;
        private Panel _accent;

        // ── 配色 ──────────────────────────────────────────────
        private static readonly Color NavBg        = Color.FromArgb(232, 235, 240);
        private static readonly Color NavHeaderBg  = Color.FromArgb(55, 75, 100);
        private static readonly Color TitleBarBg   = Color.FromArgb(35, 65, 100);
        private static readonly Color AccentLine   = Color.FromArgb(70, 145, 215);
        private static readonly Color ContentBg    = Color.FromArgb(252, 253, 255);
        private static readonly Color HeadingColor = Color.FromArgb(25, 80, 155);
        private static readonly Color BodyColor    = Color.FromArgb(40, 40, 45);
        private static readonly Color BulletColor  = Color.FromArgb(50, 50, 55);
        private static readonly Color NoteColor    = Color.FromArgb(150, 100, 20);
        private static readonly Color WarnColor    = Color.FromArgb(180, 50, 30);

        public HelpDialog()
        {
            Text = LocalizationManager.GetString("HelpTitle");
            const float helpBaseline = 216f;

            // 幫助窗口全屏打開——內容多，最大化最合適
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(200, 203, 210);

            _isRtl = LocalizationManager.IsRtl;
            if (_isRtl)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }

            _sections = BuildSections();

            // ── 主分割容器 ──────────────────────────────────────
            _split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterWidth = 3,
                BackColor = Color.FromArgb(200, 203, 210)
            };

            // ── 左侧导航面板 ────────────────────────────────────
            Panel navPanel = new Panel { Dock = DockStyle.Fill, BackColor = NavBg };

            _navHeader = new Label
            {
                Text = LocalizationManager.GetString("HelpNavHeader"),
                Dock = DockStyle.Top,
                Height = 52,          // 在 Shown 裡用真實 DPI 修正
                Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = NavHeaderBg,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _tree = new TreeView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft YaHei UI", 11F),
                BackColor = NavBg,
                ForeColor = Color.FromArgb(35, 35, 35),
                ItemHeight = 48,      // 在 Shown 裡用真實 DPI 修正；11pt@216dpi=33px，需要足夠間距
                Indent = 18,
                ShowLines = false,
                ShowPlusMinus = true,
                HotTracking = true,
                FullRowSelect = true
            };

            navPanel.Controls.Add(_tree);
            navPanel.Controls.Add(_navHeader);

            // ── 右侧内容面板 ────────────────────────────────────
            Panel contentOuter = new Panel { Dock = DockStyle.Fill, BackColor = ContentBg };

            // 标题栏
            _titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 76,          // 在 Shown 裡用真實 DPI 修正；18pt@216dpi=54px
                BackColor = TitleBarBg,
                Padding = new Padding(28, 0, 28, 0)
            };
            _titleLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            _titleBar.Controls.Add(_titleLabel);

            // 蓝色装饰线
            _accent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,           // 在 Shown 裡用真實 DPI 修正
                BackColor = AccentLine
            };

            // 内容 RichTextBox
            _content = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = ContentBg,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true,
                Padding = new Padding(36, 24, 36, 24)
            };

            contentOuter.Controls.Add(_content);
            contentOuter.Controls.Add(_accent);
            contentOuter.Controls.Add(_titleBar);

            _split.Panel1.Controls.Add(navPanel);
            _split.Panel2.Controls.Add(contentOuter);
            Controls.Add(_split);

            BuildTree();
            _tree.AfterSelect += Tree_AfterSelect;

            if (_tree.Nodes.Count > 0)
            {
                _tree.SelectedNode = _tree.Nodes[0];
                ShowContent(_tree.Nodes[0]);
            }

            Shown += (s, e) =>
            {
                // Shown 時 DeviceDpi 已是真實值，重新精確縮放所有 pixel 值
                int realDpi = DeviceDpi > 0 ? DeviceDpi : (int)helpBaseline;
                Func<int, int> rs = v => (int)Math.Ceiling(v * realDpi / helpBaseline);

                // 控件高度修正（11pt 字體在 realDpi 下的像素 = 11*realDpi/72）
                _navHeader.Height       = rs(52);
                _tree.ItemHeight        = rs(48);  // 11pt@216=33px，48px → 7.5px each side
                _titleBar.Height        = rs(76);  // 18pt@216=54px，76px → 11px each side
                _accent.Height          = rs(4);

                // 最小尺寸
                MinimumSize = new Size(rs(900), rs(600));

                // 分割線位置：左側目錄寬 280px（baseline），適中不擁擠
                _split.Panel1MinSize    = rs(200);
                _split.Panel2MinSize    = rs(400);
                _split.SplitterDistance = rs(280);

                // 確保內容從頭顯示
                _content.SelectionStart = 0;
                _content.ScrollToCaret();
            };
        }

        // ── 建树 ──────────────────────────────────────────────
        private void BuildTree()
        {
            _tree.Nodes.AddRange(new[]
            {
                new TreeNode(LocalizationManager.GetString("HelpNavQuick"))      { Tag = "quick" },
                new TreeNode(LocalizationManager.GetString("HelpNavDevice"))     { Tag = "device" },
                new TreeNode(LocalizationManager.GetString("HelpNavTags"))       { Tag = "tags" },
                new TreeNode(LocalizationManager.GetString("HelpNavCanvas"))     { Tag = "canvas" },
                new TreeNode(LocalizationManager.GetString("HelpNavTemplate"))   { Tag = "template" },
                new TreeNode(LocalizationManager.GetString("HelpNavCapture"))    { Tag = "capture" },
                new TreeNode(LocalizationManager.GetString("HelpNavJsonImport")) { Tag = "json_import" },
                new TreeNode(LocalizationManager.GetString("HelpNavData"))       { Tag = "data" }
            });
        }

        // ── 节点选中 ──────────────────────────────────────────
        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowContent(e.Node);
        }

        private void ShowContent(TreeNode node)
        {
            string key = node?.Tag as string;
            _titleLabel.Text = node?.Text ?? string.Empty;

            _content.Clear();

            if (key == null || !_sections.TryGetValue(key, out string rawText))
                return;

            RenderContent(rawText);
        }

        // ── 富文本渲染 ────────────────────────────────────────
        private void RenderContent(string rawText)
        {
            _content.SuspendLayout();

            // 修复 LocalizationManager 中使用字面 \r\n 而非真实换行的问题
            rawText = rawText.Replace("\\r\\n", "\r\n");

            string[] lines = rawText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            bool skipFirst = true;

            // 开头加一行空白边距
            AppendSpacing(10);

            foreach (string rawLine in lines)
            {
                string trimmed = rawLine.Trim();

                // 跳过首行的【标题】行（已在标题栏显示）
                if (skipFirst)
                {
                    skipFirst = false;
                    if (trimmed.StartsWith("【") && trimmed.Contains("】"))
                        continue;
                }

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    AppendSpacing(6);
                    continue;
                }

                if (IsWarning(trimmed))
                    AppendWarning(trimmed);
                else if (IsNote(trimmed))
                    AppendNote(trimmed);
                else if (IsSubheading(trimmed))
                    AppendSubheading(trimmed);
                else if (IsBullet(trimmed))
                    AppendBullet(StripBulletPrefix(trimmed));
                else if (IsNumberedStep(trimmed))
                    AppendStep(trimmed);
                else
                    AppendBody(trimmed);
            }

            _content.SelectionStart = 0;
            _content.ScrollToCaret();
            _content.ResumeLayout();
        }

        // ── 行类型检测 ────────────────────────────────────────
        private static bool IsWarning(string s) =>
            s.StartsWith("注意：") || s.StartsWith("注意事項：") ||
            s.StartsWith("Warning:") || s.StartsWith("Attention:");

        private static bool IsNote(string s) =>
            s.StartsWith("提示：") || s.StartsWith("Note:") ||
            s.StartsWith("Tip:") || s.StartsWith("Conseil:");

        private static bool IsSubheading(string s)
        {
            if (s.Length > 45 || s.StartsWith("•") || s.StartsWith("·") || s.StartsWith("▸")) return false;
            if (s.Length > 0 && char.IsDigit(s[0])) return false;
            return s.EndsWith("：") || (s.EndsWith(":") && s.IndexOf(' ') == -1);
        }

        private static bool IsBullet(string s) =>
            s.StartsWith("• ") || s.StartsWith("· ") || s.StartsWith("▸ ") ||
            s.StartsWith("- ") || s.StartsWith("* ");

        private static bool IsNumberedStep(string s) =>
            s.Length > 2 && char.IsDigit(s[0]) &&
            (s[1] == '.' || s[1] == '、' || s[1] == ')');

        private static string StripBulletPrefix(string s)
        {
            foreach (string prefix in new[] { "• ", "· ", "▸ ", "- ", "* " })
                if (s.StartsWith(prefix)) return s.Substring(prefix.Length);
            return s;
        }

        // ── 各类型追加方法 ────────────────────────────────────
        private HorizontalAlignment ContentAlign =>
            _isRtl ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        private void AppendSubheading(string text)
        {
            AppendSpacing(4);
            _content.SelectionFont      = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            _content.SelectionColor     = HeadingColor;
            _content.SelectionIndent    = 0;
            _content.SelectionAlignment = ContentAlign;
            _content.AppendText(text + "\n");
        }

        private void AppendBullet(string text)
        {
            _content.SelectionFont      = new Font("Microsoft YaHei UI", 13F);
            _content.SelectionColor     = BulletColor;
            _content.SelectionAlignment = ContentAlign;
            _content.SelectionIndent    = _isRtl ? 0 : 24;
            _content.SelectionRightIndent = _isRtl ? 24 : 0;
            _content.AppendText((_isRtl ? "  •" : "•  ") + text + "\n");
            _content.SelectionIndent      = 0;
            _content.SelectionRightIndent = 0;
        }

        private void AppendStep(string text)
        {
            _content.SelectionFont      = new Font("Microsoft YaHei UI", 13F);
            _content.SelectionColor     = BulletColor;
            _content.SelectionAlignment = ContentAlign;
            _content.SelectionIndent    = _isRtl ? 0 : 24;
            _content.SelectionRightIndent = _isRtl ? 24 : 0;
            _content.AppendText(text + "\n");
            _content.SelectionIndent      = 0;
            _content.SelectionRightIndent = 0;
        }

        private void AppendNote(string text)
        {
            AppendSpacing(4);
            _content.SelectionFont      = new Font("Microsoft YaHei UI", 12F, FontStyle.Italic);
            _content.SelectionColor     = NoteColor;
            _content.SelectionIndent    = 0;
            _content.SelectionAlignment = ContentAlign;
            _content.AppendText(text + "\n");
        }

        private void AppendWarning(string text)
        {
            AppendSpacing(4);
            _content.SelectionFont      = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            _content.SelectionColor     = WarnColor;
            _content.SelectionIndent    = 0;
            _content.SelectionAlignment = ContentAlign;
            _content.AppendText("⚠  " + text + "\n");
        }

        private void AppendBody(string text)
        {
            _content.SelectionFont      = new Font("Microsoft YaHei UI", 13F);
            _content.SelectionColor     = BodyColor;
            _content.SelectionIndent    = 0;
            _content.SelectionAlignment = ContentAlign;
            _content.AppendText(text + "\n");
        }

        private void AppendSpacing(int size)
        {
            _content.SelectionFont  = new Font("Microsoft YaHei UI", size > 0 ? size : 5, FontStyle.Regular);
            _content.SelectionColor = ContentBg;
            _content.AppendText("\n");
        }

        // ── 内容数据 ──────────────────────────────────────────
        private Dictionary<string, string> BuildSections()
        {
            return new Dictionary<string, string>
            {
                { "quick",       LocalizationManager.GetString("HelpContentQuick") },
                { "device",      LocalizationManager.GetString("HelpContentDevice") },
                { "tags",        LocalizationManager.GetString("HelpContentTags") },
                { "canvas",      LocalizationManager.GetString("HelpContentCanvas") },
                { "template",    LocalizationManager.GetString("HelpContentTemplate") },
                { "capture",     LocalizationManager.GetString("HelpContentCapture") },
                { "json_import", LocalizationManager.GetString("HelpContentJsonImport") },
                { "data",        LocalizationManager.GetString("HelpContentData") }
            };
        }

    }
}
