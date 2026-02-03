using System;
using System.Collections.Generic;
using System.Windows.Forms;
//一個備用類，用來在需要的時候在listbox2上進行展示
public class RecursiveTreeView : TreeView
{
    // 用來存儲所有節點名稱的List
    public List<string> NodeNames { get; private set; } = new List<string>();

    public RecursiveTreeView()
    {
        // 綁定右鍵點擊事件
        this.NodeMouseClick += RecursiveTreeView_NodeMouseClick;
        // 綁定鼠標移動事件
        this.NodeMouseHover += RecursiveTreeView_NodeMouseHover;
    }

    // 遞歸遍歷TreeView，存儲所有節點名稱
    public void RefreshNodeNames()
    {
        NodeNames.Clear();
        foreach (TreeNode node in this.Nodes)
        {
            TraverseNode(node);
        }
    }

    private void TraverseNode(TreeNode node)
    {
        NodeNames.Add(node.Text);
        foreach (TreeNode child in node.Nodes)
        {
            TraverseNode(child);
        }
    }

    // 右鍵點擊節點
    private void RecursiveTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // 這裡可以彈出菜單或處理其他右鍵功能
            MessageBox.Show($"右鍵點擊：{e.Node.Text}");
        }
    }

    // 鼠標移動到節點上
    private void RecursiveTreeView_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
    {
        // 這裡可以顯示節點提示，或自定義行為
        this.SelectedNode = e.Node;
        ToolTip tt = new ToolTip();
        tt.SetToolTip(this, $"懸停於節點：{e.Node.Text}");
    }
}