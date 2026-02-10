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
        private readonly Dictionary<string, string> _sections;

        public HelpDialog()
        {
            Text = LocalizationManager.GetString("HelpTitle");
            Size = new Size(980, 680);
            MinimumSize = new Size(820, 520);
            BackColor = Color.FromArgb(235, 237, 240);

            if (LocalizationManager.IsRtl)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }

            _sections = BuildSections();

            SplitContainer container = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 200,
                Panel1MinSize = 160,
                BackColor = BackColor
            };

            _tree = new TreeView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 232, 235),
                ForeColor = Color.FromArgb(35, 35, 35)
            };

            _content = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular),
                BackColor = Color.FromArgb(252, 252, 252),
                ForeColor = Color.FromArgb(35, 35, 35)
            };

            BuildTree();
            _tree.AfterSelect += Tree_AfterSelect;

            container.Panel1.Controls.Add(_tree);
            container.Panel2.Controls.Add(_content);

            Controls.Add(container);

            if (_tree.Nodes.Count > 0)
            {
                _tree.SelectedNode = _tree.Nodes[0];
            }

            Shown += (s, e) =>
            {
                container.SplitterDistance = 200;
            };
        }

        private void BuildTree()
        {
            TreeNode quick = new TreeNode(LocalizationManager.GetString("HelpNavQuick")) { Tag = "quick" };

            TreeNode device = new TreeNode(LocalizationManager.GetString("HelpNavDevice"))
            {
                Nodes =
                {
                    new TreeNode(LocalizationManager.GetString("HelpNavDeviceSelect")) { Tag = "device_select" },
                    new TreeNode(LocalizationManager.GetString("HelpNavDevicePreview")) { Tag = "device_preview" },
                    new TreeNode(LocalizationManager.GetString("HelpNavDeviceCustom")) { Tag = "device_custom" },
                    new TreeNode(LocalizationManager.GetString("HelpNavDeviceColor")) { Tag = "device_color" }
                }
            };

            TreeNode tags = new TreeNode(LocalizationManager.GetString("HelpNavTags"))
            {
                Nodes =
                {
                    new TreeNode(LocalizationManager.GetString("HelpNavTagCreate")) { Tag = "tag_create" },
                    new TreeNode(LocalizationManager.GetString("HelpNavTagPlace")) { Tag = "tag_place" },
                    new TreeNode(LocalizationManager.GetString("HelpNavTagSearch")) { Tag = "tag_search" }
                }
            };

            TreeNode canvas = new TreeNode(LocalizationManager.GetString("HelpNavCanvas"))
            {
                Nodes =
                {
                    new TreeNode(LocalizationManager.GetString("HelpNavCanvasMove")) { Tag = "canvas_move" },
                    new TreeNode(LocalizationManager.GetString("HelpNavCanvasDelete")) { Tag = "canvas_delete" },
                    new TreeNode(LocalizationManager.GetString("HelpNavCanvasAlign")) { Tag = "canvas_align" },
                    new TreeNode(LocalizationManager.GetString("HelpNavCanvasClear")) { Tag = "canvas_clear" }
                }
            };

            TreeNode template = new TreeNode(LocalizationManager.GetString("HelpNavTemplate"))
            {
                Nodes =
                {
                    new TreeNode(LocalizationManager.GetString("HelpNavTemplateSave")) { Tag = "template_save" },
                    new TreeNode(LocalizationManager.GetString("HelpNavTemplateLoad")) { Tag = "template_load" },
                    new TreeNode(LocalizationManager.GetString("HelpNavTemplateFolder")) { Tag = "template_folder" },
                    new TreeNode(LocalizationManager.GetString("HelpNavTemplateSearch")) { Tag = "template_search" }
                }
            };

            TreeNode capture = new TreeNode(LocalizationManager.GetString("HelpNavCapture"))
            {
                Nodes =
                {
                    new TreeNode(LocalizationManager.GetString("HelpNavCaptureManual")) { Tag = "capture_manual" },
                    new TreeNode(LocalizationManager.GetString("HelpNavCaptureAuto")) { Tag = "capture_auto" },
                    new TreeNode(LocalizationManager.GetString("HelpNavCaptureLong")) { Tag = "capture_long" }
                }
            };

            TreeNode sample = new TreeNode(LocalizationManager.GetString("HelpNavSample")) { Tag = "sample" };
            TreeNode data = new TreeNode(LocalizationManager.GetString("HelpNavData")) { Tag = "data" };

            _tree.Nodes.Add(quick);
            _tree.Nodes.Add(device);
            _tree.Nodes.Add(tags);
            _tree.Nodes.Add(canvas);
            _tree.Nodes.Add(template);
            _tree.Nodes.Add(capture);
            _tree.Nodes.Add(sample);
            _tree.Nodes.Add(data);

            _tree.ExpandAll();
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string key = e.Node.Tag as string;
            if (key != null && _sections.TryGetValue(key, out string value))
            {
                _content.Text = value;
            }
            else
            {
                _content.Text = string.Empty;
            }
        }

        private Dictionary<string, string> BuildSections()
        {
            if (LocalizationManager.CurrentLanguage != Language.TraditionalChinese)
            {
                return new Dictionary<string, string>
                {
                    { "quick", LocalizationManager.GetString("HelpContentQuick") },
                    { "device_select", LocalizationManager.GetString("HelpContentDeviceSelect") },
                    { "device_preview", LocalizationManager.GetString("HelpContentDevicePreview") },
                    { "device_custom", LocalizationManager.GetString("HelpContentDeviceCustom") },
                    { "device_color", LocalizationManager.GetString("HelpContentDeviceColor") },
                    { "tag_create", LocalizationManager.GetString("HelpContentTagCreate") },
                    { "tag_place", LocalizationManager.GetString("HelpContentTagPlace") },
                    { "tag_search", LocalizationManager.GetString("HelpContentTagSearch") },
                    { "canvas_move", LocalizationManager.GetString("HelpContentCanvasMove") },
                    { "canvas_delete", LocalizationManager.GetString("HelpContentCanvasDelete") },
                    { "canvas_align", LocalizationManager.GetString("HelpContentCanvasAlign") },
                    { "canvas_clear", LocalizationManager.GetString("HelpContentCanvasClear") },
                    { "template_save", LocalizationManager.GetString("HelpContentTemplateSave") },
                    { "template_load", LocalizationManager.GetString("HelpContentTemplateLoad") },
                    { "template_folder", LocalizationManager.GetString("HelpContentTemplateFolder") },
                    { "template_search", LocalizationManager.GetString("HelpContentTemplateSearch") },
                    { "capture_manual", LocalizationManager.GetString("HelpContentCaptureManual") },
                    { "capture_auto", LocalizationManager.GetString("HelpContentCaptureAuto") },
                    { "capture_long", LocalizationManager.GetString("HelpContentCaptureLong") },
                    { "sample", LocalizationManager.GetString("HelpContentSample") },
                    { "data", LocalizationManager.GetString("HelpContentData") }
                };
            }
            return new Dictionary<string, string>
            {
                {
                    "quick",
                    "【快速開始】\r\n\r\n" +
                    "本軟件用於繪製井口裝置示意圖，以下是基本操作流程：\r\n\r\n" +
                    "第一步：放置裝置\r\n" +
                    "  • 在左側「井口裝置選擇」列表中點擊選中一個裝置\r\n" +
                    "  • 將鼠標移到右側畫布，點擊左鍵放置裝置\r\n" +
                    "  • 可重複點擊放置多個相同裝置\r\n\r\n" +
                    "第二步：添加文字標籤\r\n" +
                    "  • 在中間「標籤管理」樹中點擊選中一個標籤\r\n" +
                    "  • 將鼠標移到畫布，點擊左鍵放置文字\r\n\r\n" +
                    "第三步：調整位置和大小\r\n" +
                    "  • 左鍵拖動：移動裝置或文字的位置\r\n" +
                    "  • 滾輪滾動：放大或縮小裝置/文字\r\n" +
                    "  • 右鍵菜單「自動對齊」：一鍵整理排列\r\n\r\n" +
                    "第四步：保存或截圖\r\n" +
                    "  • 右鍵菜單「添加樣例到庫」：保存為模板\r\n" +
                    "  • 右鍵菜單「自動截圖」：一鍵截圖到剪貼簿\r\n\r\n" +
                    "提示：畫布上右鍵點擊空白處或控件可打開功能菜單。"
                },
                {
                    "device_select",
                    "【選擇與放置裝置】\r\n\r\n" +
                    "選擇裝置：\r\n" +
                    "  • 在左側「井口裝置選擇」列表中單擊選中裝置\r\n" +
                    "  • 選中後鼠標會顯示裝置預覽圖\r\n\r\n" +
                    "放置裝置：\r\n" +
                    "  • 將鼠標移到右側畫布區域\r\n" +
                    "  • 在想要的位置點擊左鍵即可放置\r\n" +
                    "  • 可連續點擊放置多個相同裝置\r\n\r\n" +
                    "取消選擇：\r\n" +
                    "  • 在畫布空白處點擊右鍵\r\n" +
                    "  • 或點擊其他區域（標籤樹、模板庫等）\r\n\r\n" +
                    "內置裝置包括：\r\n" +
                    "  萬能防噴器、閘板防噴器、雙閘板防噴器、\r\n" +
                    "  套管頭、轉盤面、臨時井口頭、井口平台、\r\n" +
                    "  單層套管、雙層套管、三層套管、升高立管、\r\n" +
                    "  密封盤根升高短節、鑽井四通、分流器、\r\n" +
                    "  旋轉控制頭、喇叭口、變徑法蘭 等"
                },
                {
                    "device_preview",
                    "【裝置預覽】\r\n\r\n" +
                    "當您在裝置列表中移動鼠標時：\r\n" +
                    "  • 鼠標懸停在裝置名稱上會自動顯示預覽圖\r\n" +
                    "  • 預覽圖跟隨鼠標位置移動\r\n" +
                    "  • 移開鼠標後預覽圖自動消失\r\n\r\n" +
                    "預覽圖幫助您：\r\n" +
                    "  • 快速識別各種裝置的外觀\r\n" +
                    "  • 在放置前確認選擇了正確的裝置\r\n\r\n" +
                    "注意：右鍵點擊時預覽圖會暫時隱藏。"
                },
                {
                    "device_custom",
                    "【添加自定義裝置】\r\n\r\n" +
                    "除了內置裝置，您還可以添加自己的裝置圖片。\r\n\r\n" +
                    "添加方法：\r\n" +
                    "  1. 在裝置列表的空白處或任意位置點擊右鍵\r\n" +
                    "  2. 選擇「添加自繪裝置」\r\n" +
                    "  3. 在彈出的對話框中選擇圖片文件\r\n" +
                    "  4. 圖片會自動添加到列表末尾\r\n\r\n" +
                    "支持的圖片格式：\r\n" +
                    "  JPG、JPEG、PNG、BMP、GIF、SVG\r\n\r\n" +
                    "刪除自定義裝置：\r\n" +
                    "  • 右鍵點擊您添加的自定義裝置\r\n" +
                    "  • 選擇「刪除當前裝置」\r\n" +
                    "  • 注意：內置裝置無法刪除\r\n\r\n" +
                    "提示：建議使用透明背景的 PNG 或 SVG 格式。"
                },
                {
                    "device_color",
                    "【上色/未上色切換】\r\n\r\n" +
                    "部分裝置提供兩種顯示樣式：\r\n" +
                    "  • 上色版本：彩色填充的裝置圖\r\n" +
                    "  • 未上色版本：線條輪廓的裝置圖\r\n\r\n" +
                    "切換方法：\r\n" +
                    "  • 勾選左上角「使用未上色裝置」：使用線條版\r\n" +
                    "  • 取消勾選：使用彩色填充版\r\n\r\n" +
                    "應用場景：\r\n" +
                    "  • 彩色版本：適合演示、彙報使用\r\n" +
                    "  • 線條版本：適合技術文檔、打印使用\r\n\r\n" +
                    "注意：此設置影響新放置的裝置，已放置的裝置不會改變。"
                },
                {
                    "tag_create",
                    "【創建與編輯標籤】\r\n\r\n" +
                    "標籤用於在畫布上添加文字說明。\r\n\r\n" +
                    "添加根標籤：\r\n" +
                    "  • 在標籤樹空白處點擊右鍵\r\n" +
                    "  • 選擇「添加根節點」\r\n" +
                    "  • 輸入標籤名稱後按 Enter 確認\r\n\r\n" +
                    "添加子標籤：\r\n" +
                    "  • 右鍵點擊已有的標籤節點\r\n" +
                    "  • 選擇「添加子節點」\r\n" +
                    "  • 輸入名稱後按 Enter 確認\r\n\r\n" +
                    "重命名標籤：\r\n" +
                    "  • 右鍵點擊標籤，選擇「重命名」\r\n" +
                    "  • 直接修改文字後按 Enter 確認\r\n" +
                    "  • 按 Esc 取消修改\r\n\r\n" +
                    "刪除標籤：\r\n" +
                    "  • 右鍵點擊標籤，選擇「删除當前節點」\r\n\r\n" +
                    "提示：標籤支持多層級結構，方便分類管理。"
                },
                {
                    "tag_place",
                    "【放置文字標籤】\r\n\r\n" +
                    "選擇標籤：\r\n" +
                    "  • 在標籤樹中單擊選中一個標籤\r\n" +
                    "  • 選中的標籤會高亮顯示\r\n\r\n" +
                    "放置文字：\r\n" +
                    "  • 將鼠標移到畫布上\r\n" +
                    "  • 在想要的位置點擊左鍵\r\n" +
                    "  • 文字會以較大字號顯示在畫布上\r\n\r\n" +
                    "調整文字：\r\n" +
                    "  • 拖動：按住左鍵拖動可移動文字位置\r\n" +
                    "  • 縮放：滾動滾輪可放大或縮小文字\r\n" +
                    "  • 刪除：右鍵點擊文字，選擇「刪除」\r\n\r\n" +
                    "提示：懸停在標籤上會顯示完整標籤名稱的提示框。"
                },
                {
                    "tag_search",
                    "【搜索標籤】\r\n\r\n" +
                    "當標籤較多時，可使用搜索功能快速定位。\r\n\r\n" +
                    "搜索方法：\r\n" +
                    "  • 在標籤樹上方的搜索框中輸入關鍵字\r\n" +
                    "  • 輸入時會即時過濾顯示匹配的標籤\r\n" +
                    "  • 匹配的標籤及其父級節點都會顯示\r\n\r\n" +
                    "清除搜索：\r\n" +
                    "  • 刪除搜索框中的所有文字\r\n" +
                    "  • 標籤樹會恢復顯示全部內容\r\n\r\n" +
                    "搜索特點：\r\n" +
                    "  • 不區分大小寫\r\n" +
                    "  • 支持搜索任意層級的標籤\r\n" +
                    "  • 搜索結果會自動展開顯示"
                },
                {
                    "canvas_move",
                    "【移動與縮放】\r\n\r\n" +
                    "移動控件：\r\n" +
                    "  • 將鼠標移到裝置或文字上\r\n" +
                    "  • 按住左鍵拖動到新位置\r\n" +
                    "  • 鬆開左鍵完成移動\r\n\r\n" +
                    "縮放控件：\r\n" +
                    "  • 將鼠標移到裝置或文字上\r\n" +
                    "  • 向上滾動滾輪：放大\r\n" +
                    "  • 向下滾動滾輪：縮小\r\n" +
                    "  • 縮放時保持原始比例不變形\r\n\r\n" +
                    "滾動畫布：\r\n" +
                    "  • 在畫布空白處滾動滾輪\r\n" +
                    "  • 或拖動右側滾動條\r\n" +
                    "  • 可查看畫布上下方的內容\r\n\r\n" +
                    "提示：當鼠標在控件上時，滾輪用於縮放該控件；\r\n" +
                    "      當鼠標在空白處時，滾輪用於滾動畫布。"
                },
                {
                    "canvas_delete",
                    "【刪除控件】\r\n\r\n" +
                    "刪除單個控件：\r\n" +
                    "  • 右鍵點擊要刪除的裝置或文字\r\n" +
                    "  • 在彈出菜單中選擇「刪除」\r\n" +
                    "  • 該控件會立即從畫布移除\r\n\r\n" +
                    "清空所有控件：\r\n" +
                    "  • 右鍵點擊畫布任意位置\r\n" +
                    "  • 選擇「清空畫布」\r\n" +
                    "  • 畫布上所有內容會被清除\r\n\r\n" +
                    "注意：刪除操作無法撤銷，請謹慎使用。"
                },
                {
                    "canvas_align",
                    "【自動對齊】\r\n\r\n" +
                    "自動對齊功能可以快速整理畫布上的控件。\r\n\r\n" +
                    "使用方法：\r\n" +
                    "  • 右鍵點擊畫布或任意控件\r\n" +
                    "  • 選擇「自動對齊」\r\n\r\n" +
                    "對齊效果（兩種模式交替）：\r\n\r\n" +
                    "  模式一（第一次點擊）：\r\n" +
                    "    • 所有裝置垂直排列並水平居中\r\n" +
                    "    • 文字標籤居中顯示在對應裝置上\r\n\r\n" +
                    "  模式二（第二次點擊）：\r\n" +
                    "    • 裝置保持垂直排列並水平居中\r\n" +
                    "    • 文字標籤顯示在裝置右側\r\n\r\n" +
                    "提示：連續點擊可在兩種模式間切換，選擇合適的排版。"
                },
                {
                    "canvas_clear",
                    "【清空畫布】\r\n\r\n" +
                    "當需要重新開始繪製時，可以清空畫布。\r\n\r\n" +
                    "操作方法：\r\n" +
                    "  • 右鍵點擊畫布任意位置\r\n" +
                    "  • 選擇「清空畫布」\r\n" +
                    "  • 所有裝置和文字都會被移除\r\n\r\n" +
                    "注意事項：\r\n" +
                    "  • 清空操作無法撤銷\r\n" +
                    "  • 如果需要保留當前內容，請先保存為模板\r\n" +
                    "  • 清空不會影響已保存的模板"
                },
                {
                    "template_save",
                    "【保存模板】\r\n\r\n" +
                    "可以將當前畫布內容保存為模板，以便日後重複使用。\r\n\r\n" +
                    "保存到根目錄：\r\n" +
                    "  • 右鍵點擊畫布，選擇「添加樣例到庫」\r\n" +
                    "  • 在彈出對話框中輸入模板名稱\r\n" +
                    "  • 點擊「保存」按鈕\r\n\r\n" +
                    "保存到指定資料夾：\r\n" +
                    "  • 在模板庫中右鍵點擊目標資料夾\r\n" +
                    "  • 選擇「添加裝置到此資料夾」\r\n" +
                    "  • 輸入模板名稱並保存\r\n\r\n" +
                    "保存內容包括：\r\n" +
                    "  • 所有裝置的類型、位置、大小\r\n" +
                    "  • 所有文字標籤的內容、位置、大小\r\n\r\n" +
                    "提示：模板會自動保存到本地文件，關閉程序後不會丟失。"
                },
                {
                    "template_load",
                    "【載入模板】\r\n\r\n" +
                    "從模板庫中載入已保存的模板到畫布。\r\n\r\n" +
                    "載入方法：\r\n" +
                    "  • 在模板庫樹中找到要載入的模板\r\n" +
                    "  • 單擊該模板即可載入\r\n" +
                    "  • 模板內容會顯示在畫布上\r\n\r\n" +
                    "注意事項：\r\n" +
                    "  • 載入模板會清空當前畫布內容\r\n" +
                    "  • 如需保留當前內容，請先保存為新模板\r\n" +
                    "  • 資料夾節點無法載入，只有模板節點可以\r\n\r\n" +
                    "載入後可以：\r\n" +
                    "  • 繼續編輯修改\r\n" +
                    "  • 另存為新模板\r\n" +
                    "  • 直接截圖使用"
                },
                {
                    "template_folder",
                    "【管理資料夾】\r\n\r\n" +
                    "使用資料夾可以更好地組織和管理模板。\r\n\r\n" +
                    "新增資料夾：\r\n" +
                    "  • 右鍵點擊模板庫空白處\r\n" +
                    "  • 選擇「添加裝置到庫」可在根目錄創建\r\n" +
                    "  • 或右鍵點擊現有資料夾，選擇「新增子資料夾」\r\n\r\n" +
                    "重命名：\r\n" +
                    "  • 右鍵點擊資料夾或模板\r\n" +
                    "  • 選擇「重命名」\r\n" +
                    "  • 輸入新名稱後按 Enter 確認\r\n\r\n" +
                    "刪除：\r\n" +
                    "  • 右鍵點擊要刪除的資料夾或模板\r\n" +
                    "  • 選擇「刪除」\r\n" +
                    "  • 確認後刪除（資料夾會連同內容一起刪除）\r\n\r\n" +
                    "提示：建議按項目或類型分類存放模板，便於查找。"
                },
                {
                    "template_search",
                    "【搜索模板】\r\n\r\n" +
                    "當模板較多時，可使用搜索功能快速定位。\r\n\r\n" +
                    "搜索方法：\r\n" +
                    "  • 在模板庫上方的搜索框中輸入關鍵字\r\n" +
                    "  • 輸入時會即時過濾顯示匹配的項目\r\n" +
                    "  • 匹配的模板及其所在資料夾都會顯示\r\n\r\n" +
                    "清除搜索：\r\n" +
                    "  • 刪除搜索框中的所有文字\r\n" +
                    "  • 模板庫會恢復顯示全部內容\r\n\r\n" +
                    "搜索特點：\r\n" +
                    "  • 不區分大小寫\r\n" +
                    "  • 同時搜索資料夾名和模板名\r\n" +
                    "  • 搜索結果會自動展開顯示"
                },
                {
                    "capture_manual",
                    "【手動截圖】\r\n\r\n" +
                    "手動截圖可以自由選擇截取畫布的任意區域。\r\n\r\n" +
                    "操作步驟：\r\n" +
                    "  1. 右鍵點擊畫布，選擇「截圖」\r\n" +
                    "  2. 鼠標變為十字形，進入截圖模式\r\n" +
                    "  3. 按住左鍵，從左上角拖動到右下角\r\n" +
                    "  4. 鬆開左鍵完成截圖\r\n" +
                    "  5. 截圖自動複製到系統剪貼簿\r\n\r\n" +
                    "取消截圖：\r\n" +
                    "  • 在截圖模式下點擊右鍵即可取消\r\n\r\n" +
                    "使用截圖：\r\n" +
                    "  • 在 Word、PPT、微信等軟件中按 Ctrl+V 粘貼\r\n" +
                    "  • 或在畫圖軟件中粘貼後保存為圖片文件"
                },
                {
                    "capture_auto",
                    "【自動截圖】\r\n\r\n" +
                    "自動截圖會智能識別畫布內容並一鍵完成截圖。\r\n\r\n" +
                    "操作步驟：\r\n" +
                    "  1. 右鍵點擊畫布，選擇「自動截圖」\r\n" +
                    "  2. 程序自動計算所有控件的範圍\r\n" +
                    "  3. 自動截取包含所有內容的最小區域\r\n" +
                    "  4. 截圖自動複製到系統剪貼簿\r\n" +
                    "  5. 顯示「截圖已複製到剪貼簿」提示\r\n\r\n" +
                    "適用場景：\r\n" +
                    "  • 快速獲取完整的裝置示意圖\r\n" +
                    "  • 不需要手動選擇截圖區域\r\n" +
                    "  • 自動排除多餘的空白區域\r\n\r\n" +
                    "注意：畫布為空時會提示「畫布上沒有任何內容可以截取」。"
                },
                {
                    "capture_long",
                    "【長圖截取】\r\n\r\n" +
                    "當畫布內容超出一屏時，可以使用長圖截取功能。\r\n\r\n" +
                    "操作步驟：\r\n" +
                    "  1. 右鍵點擊畫布，選擇「截圖」進入截圖模式\r\n" +
                    "  2. 從左上角按住左鍵開始拖動\r\n" +
                    "  3. 拖到畫布底部時，保持左鍵按住不放\r\n" +
                    "  4. 滾動滾輪向下滾動畫布\r\n" +
                    "  5. 繼續滾動直到需要的位置\r\n" +
                    "  6. 鬆開左鍵完成截圖\r\n" +
                    "  7. 程序自動拼接成完整長圖\r\n\r\n" +
                    "注意事項：\r\n" +
                    "  • 截圖時請從左上角向右下角方向拖動\r\n" +
                    "  • 滾動過程中保持左鍵按住\r\n" +
                    "  • 最終會自動合併為一張完整的長圖"
                },
                {
                    "sample",
                    "【裝置樣例】\r\n\r\n" +
                    "提供裝置繪製的參考樣例圖。\r\n\r\n" +
                    "查看方法：\r\n" +
                    "  • 右鍵點擊畫布，選擇「打開裝置樣例」\r\n" +
                    "  • 會自動打開系統畫圖程序\r\n" +
                    "  • 顯示上色版和未上色版的樣例圖\r\n\r\n" +
                    "樣例用途：\r\n" +
                    "  • 了解標準井口裝置的繪製方式\r\n" +
                    "  • 參考裝置的排列組合方式\r\n" +
                    "  • 對照樣例進行繪製\r\n\r\n" +
                    "提示：樣例圖可以作為繪製井口裝置示意圖的參考模板。"
                },
                {
                    "data",
                    "【數據說明】\r\n\r\n" +
                    "本軟件的數據保存在程序所在目錄下。\r\n\r\n" +
                    "數據文件：\r\n" +
                    "  • template_library.bin - 模板庫數據\r\n" +
                    "  • tagtree_items.bin - 標籤樹數據\r\n" +
                    "  • pictures 資料夾 - 自定義裝置圖片\r\n\r\n" +
                    "自動保存：\r\n" +
                    "  • 標籤和模板的修改會自動保存\r\n" +
                    "  • 關閉程序時也會自動保存\r\n" +
                    "  • 無需手動保存操作\r\n\r\n" +
                    "備份建議：\r\n" +
                    "  • 定期備份程序目錄下的數據文件\r\n" +
                    "  • 重裝系統前請備份整個程序資料夾\r\n\r\n" +
                    "注意事項：\r\n" +
                    "  • 請勿手動刪除或修改數據文件\r\n" +
                    "  • 數據文件損壞可能導致內容丟失\r\n" +
                    "  • 遷移到其他電腦時請複製整個程序資料夾"
                }
            };
        }
    }
}
