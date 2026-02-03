using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace _4._18
{
    /// <summary>
    /// 可序列化的樹節點數據類
    /// </summary>
    [Serializable]
    public class TreeNodeData
    {
        public string Text { get; set; }
        public List<TreeNodeData> Children { get; set; }

        public TreeNodeData()
        {
            Children = new List<TreeNodeData>();
        }

        public TreeNodeData(string text) : this()
        {
            Text = text;
        }
    }

    public partial class TagTreeUserControl : UserControl
    {
        // 右键菜单
        private ContextMenuStrip menuBlank;
        private ContextMenuStrip menuNode;

        // 懸停提示
        private ToolTip toolTip;
        private Font toolTipFont;
        private TreeNode lastHoveredNode;

        // 數據持久化路徑
        private string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tagtree_items.bin");

        // 節點選中事件
        public event EventHandler NodeSelected;
        private List<TreeNodeData> _fullData = new List<TreeNodeData>();
        private string _currentFilter = string.Empty;

        public TagTreeUserControl()
        {
            InitializeComponent();

            // 假设你的treeview名字叫treeViewTag
            treeViewTag.LabelEdit = true;

            // 初始化菜单
            menuBlank = new ContextMenuStrip();
            menuBlank.Items.Add("添加根节点", null, (s, e) => AddRootNode());

            menuNode = new ContextMenuStrip();
            menuNode.Items.Add("添加子节点", null, (s, e) => AddChildNode());
            menuNode.Items.Add("重命名", null, (s, e) => RenameNode());
            menuNode.Items.Add("删除当前节点", null, (s, e) => DeleteNode());

            treeViewTag.MouseUp += TreeViewTag_MouseUp;
            treeViewTag.AfterSelect += TreeViewTag_AfterSelect;
            treeViewTag.AfterLabelEdit += TreeViewTag_AfterLabelEdit;

            // 初始化懸停提示
            toolTip = new ToolTip();
            toolTip.OwnerDraw = true;
            toolTip.Draw += ToolTip_Draw;
            toolTip.Popup += ToolTip_Popup;
            toolTipFont = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            treeViewTag.MouseMove += TreeViewTag_MouseMove;
            treeViewTag.MouseLeave += TreeViewTag_MouseLeave;
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            using (Graphics g = e.AssociatedControl.CreateGraphics())
            {
                string text = toolTip.GetToolTip(e.AssociatedControl);
                SizeF textSize = g.MeasureString(text, toolTipFont);
                e.ToolTipSize = new Size((int)textSize.Width + 10, (int)textSize.Height + 6);
            }
        }

        private void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 225)), e.Bounds);
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, toolTipFont, Brushes.Black, new PointF(5, 3));
        }

        private void TreeViewTag_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 當節點被選中時觸發事件
            NodeSelected?.Invoke(this, EventArgs.Empty);
        }

        private void TreeViewTag_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // 如果用戶取消編輯或新名稱為空，則取消
            if (e.Label == null)
            {
                return;
            }

            if (IsFilterActive())
            {
                // 在過濾模式下，需要同步更新 _fullData 中對應的節點
                string oldName = e.Node.Text;
                string newName = e.Label;
                UpdateNodeInFullData(e.Node, oldName, newName);
            }
            else
            {
                SnapshotTreeToFullData();
            }
        }

        /// <summary>
        /// 在過濾模式下更新 _fullData 中對應的節點名稱
        /// </summary>
        private void UpdateNodeInFullData(TreeNode node, string oldName, string newName)
        {
            // 構建節點路徑（使用舊名稱，因為 TreeNode.Text 還沒更新）
            List<string> path = new List<string>();
            TreeNode current = node.Parent;
            while (current != null)
            {
                path.Insert(0, current.Text);
                current = current.Parent;
            }

            // 在 _fullData 中查找並更新對應節點
            TreeNodeData targetData = FindNodeDataByPath(_fullData, path, oldName);
            if (targetData != null)
            {
                targetData.Text = newName;
            }
        }

        /// <summary>
        /// 根據路徑在數據結構中查找節點
        /// </summary>
        private TreeNodeData FindNodeDataByPath(List<TreeNodeData> nodes, List<string> parentPath, string nodeName)
        {
            if (nodes == null) return null;

            if (parentPath.Count == 0)
            {
                // 在當前層級查找目標節點
                return nodes.Find(n => n.Text == nodeName);
            }

            // 找到路徑中的第一個父節點
            string firstParent = parentPath[0];
            TreeNodeData parentData = nodes.Find(n => n.Text == firstParent);
            if (parentData == null || parentData.Children == null)
            {
                return null;
            }

            // 遞歸查找剩餘路徑
            List<string> remainingPath = parentPath.GetRange(1, parentPath.Count - 1);
            return FindNodeDataByPath(parentData.Children, remainingPath, nodeName);
        }

        private void TreeViewTag_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果有選中的節點，始終顯示選中節點的文字
            if (treeViewTag.SelectedNode != null)
            {
                toolTip.Hide(treeViewTag);
                toolTip.Show(treeViewTag.SelectedNode.Text, treeViewTag, e.X + 10, e.Y + 10);
            }
            else
            {
                // 沒有選中節點時，顯示懸停節點的文字
                TreeNode node = treeViewTag.GetNodeAt(e.X, e.Y);

                if (node != null)
                {
                    if (node != lastHoveredNode)
                    {
                        lastHoveredNode = node;
                        toolTip.Hide(treeViewTag);
                        toolTip.Show(node.Text, treeViewTag, e.X + 10, e.Y + 10);
                    }
                }
                else
                {
                    if (lastHoveredNode != null)
                    {
                        lastHoveredNode = null;
                        toolTip.Hide(treeViewTag);
                    }
                }
            }
        }

        private void TreeViewTag_MouseLeave(object sender, EventArgs e)
        {
            lastHoveredNode = null;
            // 離開控件時隱藏本控件的提示（Form1 會接管顯示）
            toolTip.Hide(treeViewTag);
        }

        private void TreeViewTag_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = treeViewTag.GetNodeAt(e.X, e.Y);

                if (node == null)
                {
                    // 右鍵點擊空白處，清除選中狀態並隱藏提示
                    ClearSelection();
                    menuBlank.Show(treeViewTag, e.Location);
                }
                else
                {
                    treeViewTag.SelectedNode = node;
                    menuNode.Show(treeViewTag, e.Location);
                }
            }
        }

        /// <summary>
        /// 清除選中狀態並隱藏提示框（供外部調用）
        /// </summary>
        public void ClearSelection()
        {
            treeViewTag.SelectedNode = null;
            lastHoveredNode = null;
            toolTip.Hide(treeViewTag);
        }

        /// <summary>
        /// 獲取當前選中節點的文字，如果沒有選中則返回 null
        /// </summary>
        public string SelectedNodeText
        {
            get { return treeViewTag.SelectedNode?.Text; }
        }

        /// <summary>
        /// 是否有選中的節點
        /// </summary>
        public bool HasSelection
        {
            get { return treeViewTag.SelectedNode != null; }
        }
        /// <summary>
        /// 根據關鍵字過濾節點（只顯示匹配項）
        /// </summary>
        public void ApplyFilter(string query)
        {
            EnsureFullData();
            string trimmed = query?.Trim() ?? string.Empty;
            _currentFilter = trimmed;

            treeViewTag.BeginUpdate();
            treeViewTag.Nodes.Clear();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                foreach (TreeNodeData data in _fullData)
                {
                    treeViewTag.Nodes.Add(ConvertToTreeNode(data));
                }
                treeViewTag.EndUpdate();
                return;
            }

            foreach (TreeNodeData data in _fullData)
            {
                TreeNodeData filtered = FilterNodeData(data, trimmed);
                if (filtered != null)
                {
                    treeViewTag.Nodes.Add(ConvertToTreeNode(filtered));
                }
            }

            treeViewTag.ExpandAll();
            treeViewTag.EndUpdate();
        }

        private void AddRootNode()
        {
            TreeNode node = new TreeNode("新根节点");
            treeViewTag.Nodes.Add(node);
            treeViewTag.SelectedNode = node;
            node.BeginEdit();
            if (IsFilterActive())
            {
                return;
            }
            SnapshotTreeToFullData();
        }

        private void AddChildNode()
        {
            TreeNode node = treeViewTag.SelectedNode;
            if (node != null)
            {
                TreeNode child = new TreeNode("新子节点");
                node.Nodes.Add(child);
                node.Expand();
                treeViewTag.SelectedNode = child;
                child.BeginEdit();
                if (IsFilterActive())
                {
                    return;
                }
                SnapshotTreeToFullData();
            }
        }

        private void RenameNode()
        {
            TreeNode node = treeViewTag.SelectedNode;
            if (node != null)
            {
                node.BeginEdit();
            }
        }

        private void DeleteNode()
        {
            TreeNode node = treeViewTag.SelectedNode;
            if (node != null)
                node.Remove();
            if (IsFilterActive())
            {
                return;
            }
            SnapshotTreeToFullData();
        }

        #region 數據持久化

        /// <summary>
        /// 保存樹結構到文件
        /// </summary>
        public void SaveTreeData()
        {
            try
            {
                EnsureFullData();
                List<TreeNodeData> rootNodes = _fullData;

                using (FileStream stream = new FileStream(dataFilePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, rootNodes);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("保存樹結構失敗: " + ex.Message);
            }
        }

        /// <summary>
        /// 從文件讀取樹結構
        /// </summary>
        public void LoadTreeData()
        {
            try
            {
                if (File.Exists(dataFilePath) && new FileInfo(dataFilePath).Length > 0)
                {
                    using (FileStream stream = new FileStream(dataFilePath, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        List<TreeNodeData> rootNodes = (List<TreeNodeData>)formatter.Deserialize(stream);

                        // 確保反序列化後的數據完整性
                        EnsureChildrenInitialized(rootNodes);

                        treeViewTag.Nodes.Clear();
                        foreach (TreeNodeData nodeData in rootNodes)
                        {
                            treeViewTag.Nodes.Add(ConvertToTreeNode(nodeData));
                        }
                        _fullData = rootNodes;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("讀取樹結構失敗: " + ex.Message);
            }
        }

        /// <summary>
        /// 確保所有節點的 Children 列表已初始化（修復反序列化問題）
        /// </summary>
        private void EnsureChildrenInitialized(List<TreeNodeData> nodes)
        {
            if (nodes == null) return;
            foreach (TreeNodeData node in nodes)
            {
                if (node.Children == null)
                {
                    node.Children = new List<TreeNodeData>();
                }
                EnsureChildrenInitialized(node.Children);
            }
        }

        /// <summary>
        /// 將 TreeNode 轉換為可序列化的 TreeNodeData
        /// </summary>
        private TreeNodeData ConvertToTreeNodeData(TreeNode node)
        {
            TreeNodeData data = new TreeNodeData(node.Text);
            foreach (TreeNode child in node.Nodes)
            {
                data.Children.Add(ConvertToTreeNodeData(child));
            }
            return data;
        }

        /// <summary>
        /// 將 TreeNodeData 轉換為 TreeNode
        /// </summary>
        private TreeNode ConvertToTreeNode(TreeNodeData data)
        {
            TreeNode node = new TreeNode(data.Text);
            // 確保 Children 不為 null（防止反序列化問題）
            if (data.Children != null)
            {
                foreach (TreeNodeData childData in data.Children)
                {
                    node.Nodes.Add(ConvertToTreeNode(childData));
                }
            }
            return node;
        }

        private void SnapshotTreeToFullData()
        {
            List<TreeNodeData> rootNodes = new List<TreeNodeData>();
            foreach (TreeNode node in treeViewTag.Nodes)
            {
                rootNodes.Add(ConvertToTreeNodeData(node));
            }
            _fullData = rootNodes;
        }

        private void EnsureFullData()
        {
            if (_fullData != null && _fullData.Count > 0)
            {
                return;
            }

            // 先嘗試從當前 TreeView 快照
            if (treeViewTag.Nodes.Count > 0)
            {
                SnapshotTreeToFullData();
                return;
            }

            // 如果 TreeView 也為空，嘗試從文件重新加載
            try
            {
                if (File.Exists(dataFilePath) && new FileInfo(dataFilePath).Length > 0)
                {
                    using (FileStream stream = new FileStream(dataFilePath, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        _fullData = (List<TreeNodeData>)formatter.Deserialize(stream);
                        // 確保反序列化後的數據完整性
                        EnsureChildrenInitialized(_fullData);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EnsureFullData 加載失敗: " + ex.Message);
                _fullData = new List<TreeNodeData>();
            }
        }

        private bool IsFilterActive()
        {
            return !string.IsNullOrWhiteSpace(_currentFilter);
        }

        private TreeNodeData FilterNodeData(TreeNodeData data, string query)
        {
            if (data == null) return null;

            bool selfMatch = data.Text != null && data.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
            List<TreeNodeData> filteredChildren = new List<TreeNodeData>();

            // 確保 Children 不為 null（防止反序列化問題）
            if (data.Children != null)
            {
                foreach (TreeNodeData child in data.Children)
                {
                    TreeNodeData filtered = FilterNodeData(child, query);
                    if (filtered != null)
                    {
                        filteredChildren.Add(filtered);
                    }
                }
            }

            if (selfMatch || filteredChildren.Count > 0)
            {
                TreeNodeData result = new TreeNodeData(data.Text);
                result.Children.AddRange(filteredChildren);
                return result;
            }

            return null;
        }

        #endregion
    }
}

