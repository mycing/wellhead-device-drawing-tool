namespace _4._18
{
    partial class TagTreeUserControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.treeViewTag = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewTag
            // 
            this.treeViewTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.treeViewTag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewTag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTag.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewTag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.treeViewTag.Location = new System.Drawing.Point(0, 0);
            this.treeViewTag.Name = "treeViewTag";
            this.treeViewTag.Size = new System.Drawing.Size(179, 171);
            this.treeViewTag.TabIndex = 0;
            // 
            // TagTreeUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewTag);
            this.Name = "TagTreeUserControl";
            this.Size = new System.Drawing.Size(179, 171);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewTag;
    }
}
