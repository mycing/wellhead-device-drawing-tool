using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using _4._18;

[Serializable]
public class ImagePicturePanel : Panel
{
    public Image _image; // 用于存储加载的普通图像
    private Panel _parentPanel; // 父级 Panel
    private MouseEventHandler _parentMouseWheelHandler; // 父级 Panel 的滚轮事件处理器
    private List<Panel> _dynamicPanels; // 动态面板列表
    private readonly float _aspectRatio; // 存储原始图片宽高比

    public ImagePicturePanel(Point location, Image image, Panel parentPanel, MouseEventHandler parentMouseWheelHandler, List<Panel> dynamicPanels)
    {
        // 初始化成员变量
        this._parentPanel = parentPanel;
        this._parentMouseWheelHandler = parentMouseWheelHandler;
        this._dynamicPanels = dynamicPanels;

        // 设置初始位置和外观
        this.Location = location;
        this.BorderStyle = BorderStyle.None;
        this.Visible = true;
        this._image = image;

        // 存储原始图片宽高比
        if (image == null)
            throw new ArgumentNullException(nameof(image), "Image 对象不能为空！");
        _aspectRatio = (float)image.Width / image.Height;

        // 加载普通图像
        LoadImageResource(image);

        // 添加事件处理程序
        this.MouseDown += ImagePicturePanel_MouseDown;
        this.MouseMove += ImagePicturePanel_MouseMove;
        this.MouseWheel += ImagePicturePanel_MouseWheel;
        this.MouseEnter += ImagePicturePanel_MouseEnter;
        this.MouseLeave += ImagePicturePanel_MouseLeave;
    }

    /// <summary>
    /// 加载普通图像
    /// </summary>
    /// <param name="image">要加载的 Image 对象</param>
    private void LoadImageResource(Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image), "Image 对象不能为空！");
        }

        _image = image;

        // 设置控件的大小为图片大小
        this.Size = new Size(image.Width, image.Height);

        // 设置控件的背景图片
        this.BackgroundImage = image;
        this.BackgroundImageLayout = ImageLayout.Stretch;
    }

    /// <summary>
    /// 将 Image 转换为二进制数据
    /// </summary>
    /// <param name="image">要转换的 Image 对象</param>
    /// <returns>图像的二进制数据</returns>
    public byte[] ImageToBinaryData(Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image), "Image对象不能为空！");
        }

        using (MemoryStream memoryStream = new MemoryStream())
        {
            image.Save(memoryStream, image.RawFormat);
            return memoryStream.ToArray();
        }
    }

    /// <summary>
    /// 将二进制数据转换为 Image 对象
    /// </summary>
    /// <param name="binaryData">图像的二进制数据</param>
    /// <returns>转换后的 Image 对象</returns>
    public Image BinaryDataToImage(byte[] binaryData)
    {
        if (binaryData == null || binaryData.Length == 0)
        {
            throw new ArgumentNullException(nameof(binaryData), "二进制数据不能为空！");
        }

        using (MemoryStream memoryStream = new MemoryStream(binaryData))
        {
            return Image.FromStream(memoryStream);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_image != null)
        {
            Graphics graphics = e.Graphics;

            // 启用高质量插值模式
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // 绘制背景图片
            graphics.DrawImage(_image, new Rectangle(0, 0, this.Width, this.Height));
        }
        else
        {
            // 如果未加载图片，显示提示信息
            e.Graphics.DrawString("未加载图像", this.Font, Brushes.Red, new PointF(10, 10));
        }
    }

    private void ImagePicturePanel_MouseEnter(object sender, EventArgs e)
    {
        // 禁用父控件的滚轮事件
        if (_parentPanel != null && _parentMouseWheelHandler != null)
        {
            _parentPanel.MouseWheel -= _parentMouseWheelHandler;
        }
        this.Focus(); // 聚焦当前面板
    }

    private void ImagePicturePanel_MouseLeave(object sender, EventArgs e)
    {
        // 恢复父控件的滚轮事件
        if (_parentPanel != null && _parentMouseWheelHandler != null)
        {
            _parentPanel.MouseWheel += _parentMouseWheelHandler;
        }
        _parentPanel?.Focus(); // 聚焦父级 Panel
    }

    private void ImagePicturePanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            ContextMenuStrip contextMenu = CanvasContextMenuFactory.Create(this, () =>
            {
                _parentPanel?.Controls.Remove(this);
                _dynamicPanels?.Remove(this);
            }, includeImportJson: false);
            this.ContextMenuStrip = contextMenu;
            contextMenu.Show(this, e.Location);
        }
        else if (e.Button == MouseButtons.Left)
        {
            // 记录鼠标按下时的位置
            this.Tag = e.Location;
        }
    }

    private void ImagePicturePanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && this.Tag != null)
        {
            // 计算并更新面板的新位置
            Point oldLocation = (Point)this.Tag;
            Point newLocation = this.Location;
            newLocation.Offset(e.Location.X - oldLocation.X, e.Location.Y - oldLocation.Y);
            this.Location = newLocation;
        }
    }

    private void ImagePicturePanel_MouseWheel(object sender, MouseEventArgs e)
    {
        if (_image == null) return;

        // 计算缩放比例
        float scaleFactor = (e.Delta > 0) ? 1.05f : 0.95f;

        // 只缩放宽度，高度根据原始比例自动算
        int minWidth = 10, maxWidth = 10000;
        int newWidth = Math.Max(minWidth, Math.Min((int)(this.Width * scaleFactor), maxWidth));
        int newHeight = (int)(newWidth / _aspectRatio);

        this.Size = new Size(newWidth, newHeight);

        // 触发重绘以应用新的缩放
        this.Invalidate();
    }

    // ----------- 新增：WndProc 攔截滾輪消息，防止消息傳給父Panel -------------
    protected override void WndProc(ref Message m)
    {
        const int WM_MOUSEWHEEL = 0x020A;
        if (m.Msg == WM_MOUSEWHEEL)
        {
            // 只要鼠標在本Panel，攔截消息，不傳遞給父Panel
            // 用 ToInt64 保證64位兼容
            short delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);

            // 只缩放宽度，高度根据原始比例自动算
            float scaleFactor = (delta > 0) ? 1.05f : 0.95f;
            int minWidth = 10, maxWidth = 10000;
            int newWidth = Math.Max(minWidth, Math.Min((int)(this.Width * scaleFactor), maxWidth));
            int newHeight = (int)(newWidth / _aspectRatio);

            if (newWidth == this.Width && newHeight == this.Height) return;
            this.Size = new Size(newWidth, newHeight);
            this.Invalidate();
            return; // 不調用 base，阻止傳遞
        }
        base.WndProc(ref m);
    }
}
