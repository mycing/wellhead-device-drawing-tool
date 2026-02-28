using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _4._18
{
    internal sealed class PanelSampleLibrarySaver
    {
        private readonly PanelManager _panelManager;
        private readonly List<TemplateTreeNodeData> _templateLibraryData;
        private readonly TreeView _treeView;
        private readonly Action _saveAction;

        private static int ScaleByDpi(IWin32Window owner, int value)
        {
            const float baselineDpi = 216f; // 225% 作為視覺基準
            if (owner is Control c && c.DeviceDpi > 0)
            {
                return (int)Math.Ceiling(value * c.DeviceDpi / baselineDpi);
            }
            return value;
        }

        public PanelSampleLibrarySaver(
            PanelManager panelManager,
            List<TemplateTreeNodeData> templateLibraryData,
            TreeView treeView,
            Action saveAction)
        {
            _panelManager = panelManager ?? throw new ArgumentNullException(nameof(panelManager));
            _templateLibraryData = templateLibraryData ?? throw new ArgumentNullException(nameof(templateLibraryData));
            _treeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
            _saveAction = saveAction;
        }

        /// <summary>
        /// 顯示對話框讓用戶選擇保存位置和輸入名稱（保存為根節點或選擇資料夾）
        /// </summary>
        public void PromptAndSave(IWin32Window owner)
        {
            using (Form inputForm = new Form())
            {
                // ── 先用 ScaleByDpi 算出所有尺寸，最後反推 ClientSize ──
                int pad      = ScaleByDpi(owner, 14);
                int gap      = ScaleByDpi(owner, 10);
                int formW    = ScaleByDpi(owner, 560);
                int fw       = formW - pad * 2;
                // 10pt 字體在 baseline(216 DPI) = 30px，控件高度必須 > 30
                int lblFolH  = ScaleByDpi(owner, 38); // 30px font + 8px padding
                int treeH    = ScaleByDpi(owner, 280);
                int lblNameW = ScaleByDpi(owner, 110);
                int nameH    = ScaleByDpi(owner, 42); // TextBox 高由字體決定，此值用於定位
                int btnH     = ScaleByDpi(owner, 46);
                int btnW     = (fw - gap) / 2;

                int treeTop  = pad + lblFolH + gap;
                int nameTop  = treeTop + treeH + gap;
                int btnTop   = nameTop + nameH + gap;
                int formH    = btnTop + btnH + pad;   // 精確高度，不截斷

                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.AutoScaleMode = AutoScaleMode.None;
                inputForm.ClientSize = new Size(formW, formH);
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.Text = LocalizationManager.GetString("SaveTemplateTitle");

                // 標籤：選擇目標資料夾
                Label labelFolder = new Label()
                {
                    Left = pad, Top = pad, Width = fw, Height = lblFolH,
                    Text = LocalizationManager.GetString("SelectTargetFolder"),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                // TreeView 顯示現有的樹狀結構
                TreeView folderTree = new TreeView()
                {
                    Left = pad, Top = treeTop, Width = fw, Height = treeH,
                    Font = new Font("Microsoft YaHei UI", 10F),
                    ShowLines = true,
                    ShowPlusMinus = true,
                    ShowRootLines = true
                };

                // 載入完整樹節點（資料夾 + 模板）
                LoadAllNodesToTreeView(folderTree, _templateLibraryData);

                // 預設不選中任何節點，讓用戶手動選擇
                folderTree.SelectedNode = null;
                // 防止 TreeView 獲得焦點時自動選中第一個節點
                bool userHasSelected = false;
                folderTree.BeforeSelect += (bs, be) =>
                {
                    if (!userHasSelected && be.Action == TreeViewAction.Unknown)
                    {
                        be.Cancel = true;
                    }
                };
                folderTree.NodeMouseClick += (mc, me) => { userHasSelected = true; };
                folderTree.KeyDown += (kd, ke) => { userHasSelected = true; };

                // 標籤：模板名稱
                Label labelName = new Label()
                {
                    Left = pad, Top = nameTop,
                    Width = lblNameW, Height = nameH,
                    Text = LocalizationManager.GetString("TemplateName"),
                    Font = new Font("Microsoft YaHei UI", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                TextBox textBox = new TextBox()
                {
                    Left = pad + lblNameW + gap, Top = nameTop,
                    Width = fw - lblNameW - gap, Height = nameH,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                Button confirmation = new Button()
                {
                    Left = pad, Top = btnTop, Width = btnW, Height = btnH,
                    Text = LocalizationManager.GetString("Save"),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };
                confirmation.Click += (s, ev) =>
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        MessageBox.Show(inputForm, LocalizationManager.GetString("TemplateNameEmpty"), LocalizationManager.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox.Focus();
                        return;
                    }
                    inputForm.DialogResult = DialogResult.OK;
                    inputForm.Close();
                };

                Button cancelButton = new Button()
                {
                    Left = pad + btnW + gap, Top = btnTop, Width = btnW, Height = btnH,
                    Text = LocalizationManager.GetString("Cancel"),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };
                cancelButton.Click += (s, ev) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

                inputForm.AcceptButton = confirmation;
                inputForm.CancelButton = cancelButton;
                inputForm.Controls.AddRange(new Control[]
                {
                    labelFolder, folderTree,
                    labelName, textBox,
                    confirmation, cancelButton
                });

                if (inputForm.ShowDialog(owner) == DialogResult.OK)
                {
                    string templateName = textBox.Text;
                    List<PanelInfo> panelInfos = _panelManager.SaveAllPanels();
                    TemplateTreeNodeData newTemplate = new TemplateTreeNodeData(templateName, panelInfos);

                    if (folderTree.SelectedNode != null)
                    {
                        // 保存到選中的節點作為子節點
                        TemplateTreeNodeData parentData = folderTree.SelectedNode.Tag as TemplateTreeNodeData;

                        if (parentData != null)
                        {
                            parentData.Children.Add(newTemplate);

                            // 在主 TreeView 中找到對應的節點並添加
                            TreeNode parentNode = FindNodeByPath(_treeView.Nodes, folderTree.SelectedNode.FullPath, folderTree.PathSeparator);
                            if (parentNode == null || parentNode.Tag != parentData)
                            {
                                parentNode = FindNodeByData(_treeView.Nodes, parentData);
                            }
                            if (parentNode != null)
                            {
                                TreeNode newNode = new TreeNode(templateName);
                                newNode.Tag = newTemplate;
                                parentNode.Nodes.Add(newNode);
                                parentNode.Expand();
                            }
                        }
                        else
                        {
                            // 無法定位父節點則保存為根節點
                            _templateLibraryData.Add(newTemplate);
                            TreeNode newNode = new TreeNode(templateName);
                            newNode.Tag = newTemplate;
                            _treeView.Nodes.Add(newNode);
                        }
                    }
                    else
                    {
                        // 未選擇則保存為根節點
                        _templateLibraryData.Add(newTemplate);
                        TreeNode newNode = new TreeNode(templateName);
                        newNode.Tag = newTemplate;
                        _treeView.Nodes.Add(newNode);
                    }

                    _saveAction?.Invoke();
                }
            }
        }

        /// <summary>
        /// 直接保存到指定的資料夾節點
        /// </summary>
        public void PromptAndSaveToFolder(IWin32Window owner, TreeNode parentNode, TemplateTreeNodeData parentData)
        {
            using (Form inputForm = new Form())
            {
                // ── 先算所有尺寸，最後反推 ClientSize ──────────────
                int p2    = ScaleByDpi(owner, 16);
                int gap2  = ScaleByDpi(owner, 10);
                int fw2   = ScaleByDpi(owner, 428); // 460 - 2*16
                int lblW  = ScaleByDpi(owner, 110);
                int nameH2 = ScaleByDpi(owner, 42); // 10pt font(30px) + 12px padding
                int btnH2  = ScaleByDpi(owner, 46);
                int btnW2  = (fw2 - gap2) / 2;

                int nameTop2 = p2;
                int btnTop2  = nameTop2 + nameH2 + gap2;
                int formH2   = btnTop2 + btnH2 + p2;   // 精確高度

                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.AutoScaleMode = AutoScaleMode.None;
                inputForm.ClientSize = new Size(ScaleByDpi(owner, 460), formH2);
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.Text = LocalizationManager.GetString("SaveToFolder", parentNode.Text);

                Label label = new Label()
                {
                    Left = p2, Top = nameTop2,
                    Width = lblW, Height = nameH2,
                    Text = LocalizationManager.GetString("TemplateName"),
                    Font = new Font("Microsoft YaHei UI", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                TextBox textBox = new TextBox()
                {
                    Left = p2 + lblW + gap2, Top = nameTop2,
                    Width = fw2 - lblW - gap2, Height = nameH2,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                Button confirmation = new Button()
                {
                    Left = p2, Top = btnTop2, Width = btnW2, Height = btnH2,
                    Text = LocalizationManager.GetString("Save"),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };
                confirmation.Click += (s, ev) =>
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        MessageBox.Show(inputForm, LocalizationManager.GetString("TemplateNameEmpty"), LocalizationManager.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox.Focus();
                        return;
                    }
                    inputForm.DialogResult = DialogResult.OK;
                    inputForm.Close();
                };

                Button cancelButton = new Button()
                {
                    Left = p2 + btnW2 + gap2, Top = btnTop2, Width = btnW2, Height = btnH2,
                    Text = LocalizationManager.GetString("Cancel"),
                    Font = new Font("Microsoft YaHei UI", 10F)
                };
                cancelButton.Click += (s, ev) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

                inputForm.AcceptButton = confirmation;
                inputForm.CancelButton = cancelButton;
                inputForm.Controls.AddRange(new Control[] { label, textBox, confirmation, cancelButton });

                if (inputForm.ShowDialog(owner) == DialogResult.OK)
                {
                    string templateName = textBox.Text;
                    List<PanelInfo> panelInfos = _panelManager.SaveAllPanels();
                    TemplateTreeNodeData newTemplate = new TemplateTreeNodeData(templateName, panelInfos);

                    // 添加到數據結構
                    parentData.Children.Add(newTemplate);

                    // 添加到 TreeView
                    TreeNode newNode = new TreeNode(templateName);
                    newNode.Tag = newTemplate;
                    parentNode.Nodes.Add(newNode);
                    parentNode.Expand();

                    _saveAction?.Invoke();
                }
            }
        }

        /// <summary>
        /// 將完整樹狀節點載入到 TreeView（資料夾 + 模板）
        /// </summary>
        private void LoadAllNodesToTreeView(TreeView treeView, List<TemplateTreeNodeData> dataList)
        {
            treeView.Nodes.Clear();
            foreach (TemplateTreeNodeData data in dataList)
            {
                treeView.Nodes.Add(CreateNode(data));
            }
        }

        /// <summary>
        /// 遞歸創建樹狀節點（資料夾 + 模板）
        /// </summary>
        private TreeNode CreateNode(TemplateTreeNodeData data)
        {
            TreeNode node = new TreeNode(data.Text);
            node.Tag = data;
            foreach (TemplateTreeNodeData child in data.Children)
            {
                node.Nodes.Add(CreateNode(child));
            }
            return node;
        }

        /// <summary>
        /// 在 TreeView 節點集合中找到與指定數據對應的節點
        /// </summary>
        private TreeNode FindNodeByData(TreeNodeCollection nodes, TemplateTreeNodeData targetData)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == targetData)
                {
                    return node;
                }
                TreeNode found = FindNodeByData(node.Nodes, targetData);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        /// <summary>
        /// 按 FullPath 在 TreeView 中查找節點
        /// </summary>
        private TreeNode FindNodeByPath(TreeNodeCollection nodes, string fullPath, string pathSeparator)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }

            string[] parts = fullPath.Split(new[] { pathSeparator }, StringSplitOptions.None);
            return FindNodeByPathParts(nodes, parts, 0);
        }

        private TreeNode FindNodeByPathParts(TreeNodeCollection nodes, string[] parts, int index)
        {
            if (parts == null || index >= parts.Length)
            {
                return null;
            }

            foreach (TreeNode node in nodes)
            {
                if (string.Equals(node.Text, parts[index], StringComparison.Ordinal))
                {
                    if (index == parts.Length - 1)
                    {
                        return node;
                    }
                    return FindNodeByPathParts(node.Nodes, parts, index + 1);
                }
            }
            return null;
        }

    }
}
