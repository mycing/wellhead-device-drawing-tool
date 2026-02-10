using System;
using System.Drawing;
using System.Windows.Forms;
using _4._18;

[Serializable]
public class DrawstringPanel : Panel
{
    public string PanelString { get; set; } // 保存要绘制的字符串
    private Font _font; // 字体

    // 新增：只保存一个原始宽高比
    private readonly float _aspectRatio = 1.0f;

    public DrawstringPanel(Point location, string text, Font font)
    {
        this.PanelString = text;
        this._font = font;
        this.Location = location;
        this.BorderStyle = BorderStyle.None;
        this.Visible = true;

        // 根据文本和字体计算初始大小
        using (Graphics g = CreateGraphics())
        {
            SizeF textSize = g.MeasureString(PanelString, _font);
            this.Size = new Size((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
        }

        // 新增：记录初始宽高比
        if (this.Height != 0)
            _aspectRatio = (float)this.Width / this.Height;

        // 设置事件处理程序
        this.MouseDown += DrawstringPanel_MouseDown;
        this.MouseMove += DrawstringPanel_MouseMove;
        this.MouseWheel += DrawstringPanel_MouseWheel;
        this.MouseEnter += DrawstringPanel_MouseEnter;
        this.MouseLeave += DrawstringPanel_MouseLeave;
        this.Paint += DrawstringPanel_Paint;
    }

    private void DrawstringPanel_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.Clear(this.BackColor); // 清除之前的绘制内容

        // 根据面板尺寸动态调整字体大小
        using (Graphics g = e.Graphics)
        {
            SizeF textSize = g.MeasureString(PanelString, _font);
            float newFontSize = Math.Min(this.Width / textSize.Width * _font.Size, this.Height / textSize.Height * _font.Size);

            using (Font adjustedFont = new Font(_font.FontFamily, newFontSize))
            {
                g.DrawString(PanelString, adjustedFont, Brushes.Black, new RectangleF(0, 0, this.Width, this.Height));
            }
        }
    }

    private void DrawstringPanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // 创建右键菜单
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem deleteItem = new ToolStripMenuItem(LocalizationManager.GetString("Delete"), null, (s, ev) =>
            {
                if (this.Parent != null)
                {
                    this.Parent.Controls.Remove(this);
                }
            });
            contextMenu.Items.Add(deleteItem);

            ToolStripMenuItem autoAlignItem = new ToolStripMenuItem(LocalizationManager.GetString("AutoAlign"), null, (s, ev) =>
            {
                var form = this.FindForm() as _4._18.Form1;
                form?.AutoAlignControls();
            });
            contextMenu.Items.Add(autoAlignItem);

            ToolStripMenuItem autoCaptureItem = new ToolStripMenuItem(LocalizationManager.GetString("AutoCapture"), null, (s, ev) =>
            {
                var form = this.FindForm() as _4._18.Form1;
                form?.StartAutoCapture();
            });
            contextMenu.Items.Add(autoCaptureItem);
            contextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem captureItem = new ToolStripMenuItem(LocalizationManager.GetString("Capture"), null, (s, ev) =>
            {
                var form = this.FindForm() as _4._18.Form1;
                form?.StartCapture();
            });
            contextMenu.Items.Add(captureItem);

            ToolStripMenuItem clearItem = new ToolStripMenuItem(LocalizationManager.GetString("ClearCanvas"), null, (s, ev) =>
            {
                this.Parent?.Controls.Clear();
                (this.Parent as Panel)?.Refresh();
            });
            contextMenu.Items.Add(clearItem);

            ToolStripMenuItem openSampleItem = new ToolStripMenuItem(LocalizationManager.GetString("OpenSample"), null, (s, ev) =>
            {
                var form = this.FindForm() as _4._18.Form1;
                form?.OpenDeviceSample();
            });
            contextMenu.Items.Add(openSampleItem);

            ToolStripMenuItem saveSampleItem = new ToolStripMenuItem(LocalizationManager.GetString("AddSampleToLibrary"), null, (s, ev) =>
            {
                var form = this.FindForm() as _4._18.Form1;
                form?.SaveSampleToLibrary();
            });
            contextMenu.Items.Add(saveSampleItem);

            MenuStyleHelper.Apply(contextMenu);
            this.ContextMenuStrip = contextMenu;
            contextMenu.Show(this, e.Location);
        }
        else if (e.Button == MouseButtons.Left)
        {
            // 记录鼠标按下时的位置
            this.Tag = e.Location;
        }
    }

    private void DrawstringPanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && this.Tag != null)
        {
            // 拖动面板
            Point oldLocation = (Point)this.Tag;
            Point newLocation = this.Location;
            newLocation.Offset(e.Location.X - oldLocation.X, e.Location.Y - oldLocation.Y);
            this.Location = newLocation;
        }
    }

    private void DrawstringPanel_MouseWheel(object sender, MouseEventArgs e)
    {
        // 使用宽高比等比缩放
        float scale = e.Delta > 0 ? 1.1f : 1 / 1.1f;
        int minWidth = 10, maxWidth = 10000;

        int newWidth = Math.Max(minWidth, Math.Min((int)(this.Width * scale), maxWidth));
        int newHeight = (int)(newWidth / _aspectRatio);

        // 防止高度太小为0
        if (newHeight < 5) newHeight = 5;

        this.Size = new Size(newWidth, newHeight);

        // 触发重绘
        this.Invalidate();

        // 防止事件传递到父控件
        if (e is HandledMouseEventArgs handledEvent)
        {
            handledEvent.Handled = true;
        }
    }

    private void DrawstringPanel_MouseEnter(object sender, EventArgs e)
    {
        // 聚焦当前面板
        if (this.Parent != null && this.Parent.Focused)
        {
            this.Focus();
        }
    }

    private void DrawstringPanel_MouseLeave(object sender, EventArgs e)
    {
        // 失去焦点时聚焦父控件
        this.Parent?.Focus();
    }
}
