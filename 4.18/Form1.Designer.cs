using System.Windows.Forms;

namespace _4._18
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tagTreeUserControl1 = new _4._18.TagTreeUserControl();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox3Container = new System.Windows.Forms.Panel();
            this.treeViewTemplates = new System.Windows.Forms.TreeView();
            this.textBoxTemplateSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2Container = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.vScrollBarPanel2 = new _4._18.CustomScrollBar();
            this.labelSettings = new System.Windows.Forms.Label();
            this.textBoxTagSearch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1Container = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.listBox3Container.SuspendLayout();
            this.panel2Container.SuspendLayout();
            this.listBox1Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(222)))), ((int)(((byte)(225)))));
            this.panel1.Controls.Add(this.tagTreeUserControl1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.listBox3Container);
            this.panel1.Controls.Add(this.textBoxTemplateSearch);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.panel2Container);
            this.panel1.Controls.Add(this.vScrollBarPanel2);
            this.panel1.Controls.Add(this.labelSettings);
            this.panel1.Controls.Add(this.textBoxTagSearch);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.listBox1Container);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1924, 2108);
            this.panel1.TabIndex = 19;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // tagTreeUserControl1
            // 
            this.tagTreeUserControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.tagTreeUserControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tagTreeUserControl1.Location = new System.Drawing.Point(655, 63);
            this.tagTreeUserControl1.Margin = new System.Windows.Forms.Padding(12);
            this.tagTreeUserControl1.Name = "tagTreeUserControl1";
            this.tagTreeUserControl1.Size = new System.Drawing.Size(816, 948);
            this.tagTreeUserControl1.TabIndex = 52;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label4.Location = new System.Drawing.Point(1476, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 40);
            this.label4.TabIndex = 53;
            this.label4.Text = "防噴器配置";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBox3Container
            // 
            this.listBox3Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.listBox3Container.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox3Container.Controls.Add(this.treeViewTemplates);
            this.listBox3Container.Location = new System.Drawing.Point(655, 1061);
            this.listBox3Container.Margin = new System.Windows.Forms.Padding(2, 8, 2, 8);
            this.listBox3Container.Name = "listBox3Container";
            this.listBox3Container.Size = new System.Drawing.Size(816, 852);
            this.listBox3Container.TabIndex = 57;
            // 
            // treeViewTemplates
            // 
            this.treeViewTemplates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.treeViewTemplates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTemplates.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewTemplates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.treeViewTemplates.LabelEdit = true;
            this.treeViewTemplates.Location = new System.Drawing.Point(0, 0);
            this.treeViewTemplates.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewTemplates.Name = "treeViewTemplates";
            this.treeViewTemplates.Size = new System.Drawing.Size(814, 850);
            this.treeViewTemplates.TabIndex = 50;
            this.treeViewTemplates.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewTemplates_AfterLabelEdit);
            this.treeViewTemplates.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTemplates_NodeMouseClick);
            this.treeViewTemplates.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTemplates_NodeMouseDoubleClick);
            this.treeViewTemplates.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeViewTemplates_MouseMove);
            this.treeViewTemplates.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewTemplates_MouseUp);
            // 
            // textBoxTemplateSearch
            // 
            this.textBoxTemplateSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.textBoxTemplateSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTemplateSearch.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxTemplateSearch.Location = new System.Drawing.Point(870, 1016);
            this.textBoxTemplateSearch.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxTemplateSearch.Multiline = true;
            this.textBoxTemplateSearch.Name = "textBoxTemplateSearch";
            this.textBoxTemplateSearch.Size = new System.Drawing.Size(601, 53);
            this.textBoxTemplateSearch.TabIndex = 59;
            this.textBoxTemplateSearch.TextChanged += new System.EventHandler(this.textBoxTemplateSearch_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label2.Location = new System.Drawing.Point(655, 1016);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 50);
            this.label2.TabIndex = 49;
            this.label2.Text = "已保存模板";
            // 
            // panel2Container
            // 
            this.panel2Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.panel2Container.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2Container.Controls.Add(this.panel2);
            this.panel2Container.Location = new System.Drawing.Point(1476, 63);
            this.panel2Container.Margin = new System.Windows.Forms.Padding(0);
            this.panel2Container.Name = "panel2Container";
            this.panel2Container.Size = new System.Drawing.Size(418, 1850);
            this.panel2Container.TabIndex = 55;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(416, 4000);
            this.panel2.TabIndex = 19;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            this.panel2.MouseEnter += new System.EventHandler(this.panel2_MouseEnter);
            this.panel2.MouseLeave += new System.EventHandler(this.panel2_MouseLeave);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseUp);
            // 
            // vScrollBarPanel2
            // 
            this.vScrollBarPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBarPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.vScrollBarPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.vScrollBarPanel2.LargeChange = 200;
            this.vScrollBarPanel2.Location = new System.Drawing.Point(1894, 55);
            this.vScrollBarPanel2.Maximum = 4000;
            this.vScrollBarPanel2.Minimum = 0;
            this.vScrollBarPanel2.Name = "vScrollBarPanel2";
            this.vScrollBarPanel2.Size = new System.Drawing.Size(28, 1858);
            this.vScrollBarPanel2.SmallChange = 50;
            this.vScrollBarPanel2.TabIndex = 54;
            this.vScrollBarPanel2.ThumbColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(182)))), ((int)(((byte)(185)))));
            this.vScrollBarPanel2.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.vScrollBarPanel2.Value = 0;
            this.vScrollBarPanel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarPanel2_Scroll);
            // 
            // labelSettings
            // 
            this.labelSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.labelSettings.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSettings.Location = new System.Drawing.Point(1596, 10);
            this.labelSettings.Margin = new System.Windows.Forms.Padding(0);
            this.labelSettings.Name = "labelSettings";
            this.labelSettings.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelSettings.Size = new System.Drawing.Size(20, 40);
            this.labelSettings.TabIndex = 61;
            this.labelSettings.Text = "設置";
            this.labelSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelSettings.Click += new System.EventHandler(this.labelSettings_Click);
            // 
            // textBoxTagSearch
            // 
            this.textBoxTagSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.textBoxTagSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTagSearch.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxTagSearch.Location = new System.Drawing.Point(1115, 83);
            this.textBoxTagSearch.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxTagSearch.Multiline = true;
            this.textBoxTagSearch.Name = "textBoxTagSearch";
            this.textBoxTagSearch.Size = new System.Drawing.Size(636, 53);
            this.textBoxTagSearch.TabIndex = 58;
            this.textBoxTagSearch.TextChanged += new System.EventHandler(this.textBoxTagSearch_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label3.Location = new System.Drawing.Point(655, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(170, 50);
            this.label3.TabIndex = 9;
            this.label3.Text = "標籤管理";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(237)))), ((int)(((byte)(240)))));
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 50);
            this.label1.TabIndex = 4;
            this.label1.Text = "井口装置選擇";
            // 
            // listBox1Container
            // 
            this.listBox1Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.listBox1Container.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1Container.Controls.Add(this.listBox1);
            this.listBox1Container.Location = new System.Drawing.Point(10, 63);
            this.listBox1Container.Margin = new System.Windows.Forms.Padding(2, 8, 2, 8);
            this.listBox1Container.Name = "listBox1Container";
            this.listBox1Container.Size = new System.Drawing.Size(640, 1850);
            this.listBox1Container.TabIndex = 56;
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 50;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Margin = new System.Windows.Forms.Padding(0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(638, 1848);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            this.listBox1.MouseEnter += new System.EventHandler(this.listBox1_MouseEnter);
            this.listBox1.MouseLeave += new System.EventHandler(this.listBox1_MouseLeave);
            this.listBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseMove);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1924, 2108);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 8, 2, 8);
            this.Name = "Form1";
            this.Text = "井口装置绘图工具                           ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.listBox3Container.ResumeLayout(false);
            this.panel2Container.ResumeLayout(false);
            this.listBox1Container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTagSearch;
        private System.Windows.Forms.Panel panel2;
        private TreeView treeViewTemplates;
        private System.Windows.Forms.TextBox textBoxTemplateSearch;
        private Label label2;
        private TagTreeUserControl tagTreeUserControl1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelSettings;
        private _4._18.CustomScrollBar vScrollBarPanel2;
        private System.Windows.Forms.Panel panel2Container;
        private System.Windows.Forms.Panel listBox1Container;
        private System.Windows.Forms.Panel listBox3Container;
    }
}

