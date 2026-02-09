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
                inputForm.StartPosition = FormStartPosition.Manual;
                inputForm.Location = new Point(500, 300);
                inputForm.Width = 500;
                inputForm.Height = 450;
                inputForm.Text = LocalizationManager.GetString("SaveTemplateTitle");

                // 標籤：選擇目標資料夾
                Label labelFolder = new Label()
                {
                    Left = 10,
                    Top = 10,
                    Text = LocalizationManager.GetString("SelectTargetFolder"),
                    Width = 460,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                // TreeView 顯示現有的樹狀結構
                TreeView folderTree = new TreeView()
                {
                    Left = 10,
                    Top = 40,
                    Width = 460,
                    Height = 250,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                // 載入完整樹節點（資料夾 + 模板）
                LoadAllNodesToTreeView(folderTree, _templateLibraryData);

                // 若主 TreeView 有選中節點，則在彈窗中自動選中對應節點
                if (_treeView.SelectedNode != null)
                {
                    TreeNode preselectNode = FindNodeByPath(folderTree.Nodes, _treeView.SelectedNode.FullPath, _treeView.PathSeparator);
                    if (preselectNode != null)
                    {
                        folderTree.SelectedNode = preselectNode;
                        preselectNode.EnsureVisible();
                    }
                }

                // 標籤：模板名稱
                Label labelName = new Label()
                {
                    Left = 10,
                    Top = 300,
                    Text = LocalizationManager.GetString("TemplateName"),
                    Width = 80,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                TextBox textBox = new TextBox()
                {
                    Left = 90,
                    Top = 298,
                    Width = 380,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                Button confirmation = new Button()
                {
                    Text = LocalizationManager.GetString("Save"),
                    Left = 10,
                    Width = 225,
                    Top = 340,
                    Height = 40,
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
                    Text = LocalizationManager.GetString("Cancel"),
                    Left = 245,
                    Width = 225,
                    Top = 340,
                    Height = 40,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };
                cancelButton.Click += (s, ev) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

                inputForm.AcceptButton = confirmation;
                inputForm.CancelButton = cancelButton;
                inputForm.Controls.Add(labelFolder);
                inputForm.Controls.Add(folderTree);
                inputForm.Controls.Add(labelName);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(confirmation);
                inputForm.Controls.Add(cancelButton);

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
                inputForm.StartPosition = FormStartPosition.Manual;
                inputForm.Location = new Point(500, 300);
                inputForm.Width = 400;
                inputForm.Height = 180;
                inputForm.Text = LocalizationManager.GetString("SaveToFolder", parentNode.Text);

                Label label = new Label()
                {
                    Left = 10,
                    Top = 20,
                    Text = LocalizationManager.GetString("TemplateName"),
                    Width = 80,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                TextBox textBox = new TextBox()
                {
                    Left = 90,
                    Top = 18,
                    Width = 280,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };

                Button confirmation = new Button()
                {
                    Text = LocalizationManager.GetString("Save"),
                    Left = 10,
                    Width = 175,
                    Top = 70,
                    Height = 40,
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
                    Text = LocalizationManager.GetString("Cancel"),
                    Left = 195,
                    Width = 175,
                    Top = 70,
                    Height = 40,
                    Font = new Font("Microsoft YaHei UI", 10F)
                };
                cancelButton.Click += (s, ev) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

                inputForm.AcceptButton = confirmation;
                inputForm.CancelButton = cancelButton;
                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(confirmation);
                inputForm.Controls.Add(cancelButton);

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
