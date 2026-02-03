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
            Text = "操作說明";
            Size = new Size(980, 680);
            MinimumSize = new Size(820, 520);
            BackColor = Color.FromArgb(235, 237, 240);

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
            TreeNode quick = new TreeNode("快速開始") { Tag = "quick" };

            TreeNode device = new TreeNode("裝置與標籤")
            {
                Nodes =
                {
                    new TreeNode("裝置選擇") { Tag = "device" },
                    new TreeNode("標籤管理") { Tag = "tags" },
                    new TreeNode("搜索（標籤）") { Tag = "search_tags" }
                }
            };

            TreeNode canvas = new TreeNode("畫布操作")
            {
                Nodes =
                {
                    new TreeNode("基本操作") { Tag = "canvas_basic" },
                    new TreeNode("右鍵菜單") { Tag = "canvas_menu" },
                    new TreeNode("自動對齊") { Tag = "auto_align" }
                }
            };

            TreeNode template = new TreeNode("模板庫")
            {
                Nodes =
                {
                    new TreeNode("保存與載入") { Tag = "templates" },
                    new TreeNode("資料夾與重命名") { Tag = "template_manage" },
                    new TreeNode("搜索（模板）") { Tag = "search_templates" }
                }
            };

            TreeNode capture = new TreeNode("截圖")
            {
                Nodes =
                {
                    new TreeNode("基本截圖") { Tag = "capture_basic" },
                    new TreeNode("自動截圖") { Tag = "capture_auto" }
                }
            };

            TreeNode custom = new TreeNode("自定義裝置") { Tag = "custom_device" };
            TreeNode data = new TreeNode("數據保存") { Tag = "data_store" };

            _tree.Nodes.Add(quick);
            _tree.Nodes.Add(device);
            _tree.Nodes.Add(canvas);
            _tree.Nodes.Add(template);
            _tree.Nodes.Add(capture);
            _tree.Nodes.Add(custom);
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
            return new Dictionary<string, string>
            {
                {
                    "quick",
                    "快速開始\r\n" +
                    "1) 在左側「井口裝置選擇」中點選裝置。\r\n" +
                    "2) 移到右側畫布（panel2）點擊左鍵即可放置。\r\n" +
                    "3) 在「標籤管理」中選中標籤，再在畫布點擊可放置文字標籤。\r\n" +
                    "4) 用中間下方「已保存模板」保存或載入模板。\r\n" +
                    "5) 滾輪可上下查看畫布，右鍵打開功能菜單。"
                },
                {
                    "device",
                    "裝置選擇\r\n" +
                    "• 左側列表為內置/自定義裝置。\r\n" +
                    "• 點選後，移到畫布點擊左鍵即可放置。\r\n" +
                    "• 勾選「使用未上色裝置」可切換不同樣式（若有）。"
                },
                {
                    "tags",
                    "標籤管理\r\n" +
                    "• 右鍵空白處可添加根節點。\r\n" +
                    "• 右鍵節點可添加子節點或刪除。\r\n" +
                    "• 選中節點後，點擊畫布可添加文字標籤。"
                },
                {
                    "search_tags",
                    "搜索（標籤）\r\n" +
                    "• 上方搜索框輸入文字後只顯示匹配標籤。\r\n" +
                    "• 清空搜索可恢復全部標籤。"
                },
                {
                    "canvas_basic",
                    "畫布基本操作\r\n" +
                    "• 左鍵：放置選中的裝置或標籤。\r\n" +
                    "• 滾輪：上下移動畫布查看內容。\r\n" +
                    "• 右鍵：打開功能菜單或取消當前選中。"
                },
                {
                    "canvas_menu",
                    "畫布右鍵菜單\r\n" +
                    "• 截圖：進入截圖模式。\r\n" +
                    "• 自動截圖：程序自動完成截圖流程。\r\n" +
                    "• 清空畫布：刪除畫布上全部內容。\r\n" +
                    "• 打開裝置樣例：查看示例圖片。\r\n" +
                    "• 自動對齊：按規則排列畫布控件。\r\n" +
                    "• 添加樣例到庫：保存為模板。"
                },
                {
                    "auto_align",
                    "自動對齊\r\n" +
                    "• 會將畫布控件按順序自動排列並居中對齊。\r\n" +
                    "• 適合整理完成後的畫布。"
                },
                {
                    "templates",
                    "模板庫\r\n" +
                    "• 右鍵畫布「添加樣例到庫」可保存模板。\r\n" +
                    "• 單擊模板節點即可載入到畫布。\r\n" +
                    "• 模板保存在程序運行目錄的本地文件中。"
                },
                {
                    "template_manage",
                    "資料夾與重命名\r\n" +
                    "• 右鍵模板區空白可新增資料夾。\r\n" +
                    "• 右鍵資料夾可新增子資料夾或添加模板。\r\n" +
                    "• 右鍵節點可重命名或刪除。"
                },
                {
                    "search_templates",
                    "搜索（模板）\r\n" +
                    "• 模板區上方搜索框支持過濾模板樹。\r\n" +
                    "• 清空搜索可恢復完整樹結構。"
                },
                {
                    "capture_basic",
                    "基本截圖\r\n" +
                    "• 右鍵畫布選擇「截圖」進入截圖模式。\r\n" +
                    "• 按住左鍵拖拽選區，鬆開完成截圖。\r\n" +
                    "• 右鍵可取消截圖模式。"
                },
                {
                    "capture_auto",
                    "自動截圖\r\n" +
                    "• 右鍵畫布選擇「自動截圖」。\r\n" +
                    "• 程序會按當前畫布內容自動處理。"
                },
                {
                    "custom_device",
                    "自定義裝置\r\n" +
                    "• 在左側裝置列表右鍵可「添加自繪裝置」。\r\n" +
                    "• 上傳後會出現在裝置列表中。"
                },
                {
                    "data_store",
                    "數據保存\r\n" +
                    "• 標籤與模板保存在程序運行目錄。\r\n" +
                    "• 請勿刪除程序目錄中的數據文件，否則會丟失個人內容。"
                }
            };
        }
    }
}
