using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Svg;
using _4._18;

[Serializable]
public class SvgDrawPicturePanel : Panel
{
    private SvgDocument _svgDocument; // 用于存储加载的 SVG
    private Panel _parentPanel; // 父级 Panel
    private MouseEventHandler _parentMouseWheelHandler; // 父级 Panel 的滚轮事件处理器
    private List<Panel> _dynamicPanels; // 动态面板列表
    public byte[] _svgData; // 用于存储传入的 SVG 字节数组
    private readonly float _aspectRatio; // 存储SVG原始宽高比
    private static readonly HashSet<string> _svgPanelLogged = new HashSet<string>();

    public SvgDrawPicturePanel(Point location, byte[] svgData, Panel parentPanel, MouseEventHandler parentMouseWheelHandler, List<Panel> dynamicPanels)
    {
        // 初始化成员变量
        this._parentPanel = parentPanel;
        this._parentMouseWheelHandler = parentMouseWheelHandler;
        this._dynamicPanels = dynamicPanels;
        this._svgData = svgData; // 存储传入的 SVG 数据

        // 设置初始位置和大小
        this.Location = location;
        this.BorderStyle = BorderStyle.None;
        this.Visible = true;
        // 使panel可获得焦点（新增）
        this.SetStyle(ControlStyles.Selectable, true);
        this.TabStop = true;

        // 加载 SVG 数据
        LoadSvgFromResource(svgData);

        // 存储SVG原始宽高比（注意要在SVG加载后赋值）
        if (_svgDocument != null && _svgDocument.Width > 0 && _svgDocument.Height > 0)
            _aspectRatio = (float)_svgDocument.Width / (float)_svgDocument.Height;
        else
            _aspectRatio = 1.0f;

        // 添加事件处理程序
        this.MouseDown += SvgDrawPicturePanel_MouseDown;
        this.MouseMove += SvgDrawPicturePanel_MouseMove;
        this.MouseWheel += SvgDrawPicturePanel_MouseWheel;
        this.MouseEnter += SvgDrawPicturePanel_MouseEnter;
        this.MouseLeave += SvgDrawPicturePanel_MouseLeave;
    }

    /// <summary>
    /// 从资源加载 SVG
    /// </summary>
    /// <param name="resourceData">资源中的 SVG 字节数组</param>
    public void LoadSvgFromResource(byte[] resourceData)
    {
        if (resourceData == null || resourceData.Length == 0)
        {
            LogSvgPanelIssue("LoadSvgFromResource: empty svgData");
            return;
        }

        try
        {
            using (var stream = new MemoryStream(resourceData))
            {
                _svgDocument = SvgDocument.Open<SvgDocument>(stream); // 从流加载 SVG
                // 设置初始尺寸
                if (_svgDocument != null)
                {
                    int width = (int)_svgDocument.Width;
                    int height = (int)_svgDocument.Height;
                    this.Size = new Size(width, height);
                    if (width <= 0 || height <= 0)
                    {
                        LogSvgPanelIssue($"LoadSvgFromResource: invalid size width={width}, height={height}");
                    }
                }
                else
                {
                    LogSvgPanelIssue("LoadSvgFromResource: SvgDocument is null");
                }
            }
        }
        catch (Exception ex)
        {
            LogSvgPanelIssue("LoadSvgFromResource exception: " + ex.Message);
        }

        this.Invalidate(); // 触发重绘
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics graphics = e.Graphics;

        if (_svgDocument != null)
        {
            // 如果加载了 SVG 文件，按 SVG 的方式绘制
            DrawSvg(graphics);
        }
    }

    private void DrawSvg(Graphics graphics)
    {
        // 启用抗锯齿以提高渲染质量
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // 计算缩放比例以适配 Panel 大小
        float scaleX = (float)this.Width / (float)_svgDocument.Width;
        float scaleY = (float)this.Height / (float)_svgDocument.Height;
        float scale = Math.Min(scaleX, scaleY); // 保持宽高比例

        // 创建缩放矩阵
        using (Matrix transform = new Matrix())
        {
            transform.Scale(scale, scale);
            graphics.Transform = transform;

            // 渲染 SVG 到 Panel
            _svgDocument.Draw(graphics);
        }
    }

    private void SvgDrawPicturePanel_MouseEnter(object sender, EventArgs e)
    {
        // 禁用父控件的滚轮事件
        if (_parentPanel != null && _parentMouseWheelHandler != null)
        {
            _parentPanel.MouseWheel -= _parentMouseWheelHandler;
        }

        // 聚焦当前面板
        if (this.Parent != null && this.Parent.Focused)
        {
            this.Focus();
        }

        // 聚焦当前面板
        this.Focus();
    }

    private void SvgDrawPicturePanel_MouseLeave(object sender, EventArgs e)
    {
        // 恢复父控件的滚轮事件
        if (_parentPanel != null && _parentMouseWheelHandler != null)
        {
            _parentPanel.MouseWheel += _parentMouseWheelHandler;
        }

        // 聚焦父级 Panel，避免焦点丢失
        if (_parentPanel != null)
        {
            _parentPanel.Focus();
        }
        // 失去焦点时聚焦父控件
        this.Parent?.Focus();
    }

    private void SvgDrawPicturePanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            ContextMenuStrip contextMenu = CanvasContextMenuFactory.Create(this, () =>
            {
                if (_parentPanel != null)
                {
                    _parentPanel.Controls.Remove(this);
                }
                if (_dynamicPanels != null)
                {
                    _dynamicPanels.Remove(this);
                }
            }, includeImportJson: true);
            this.ContextMenuStrip = contextMenu;
            contextMenu.Show(this, e.Location);
        }
        else if (e.Button == MouseButtons.Left)
        {
            // 记录鼠标按下时的位置
            this.Tag = e.Location;
        }
    }

    private void SvgDrawPicturePanel_MouseMove(object sender, MouseEventArgs e)
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

    private void SvgDrawPicturePanel_MouseWheel(object sender, MouseEventArgs e)
    {
        if (_svgDocument == null) return;

        // 计算缩放比例
        float scaleFactor = (e.Delta > 0) ? 1.05f : 0.95f; // 放大或缩小的比例因子

        // 只缩放宽度，高度根据原始比例自动算
        int minWidth = 10, maxWidth = 10000;
        int newWidth = Math.Max(minWidth, Math.Min((int)(this.Width * scaleFactor), maxWidth));
        int newHeight = (int)(newWidth / _aspectRatio);

        this.Size = new Size(newWidth, newHeight);

        // 触发重绘以应用新的缩放
        this.Invalidate();
    }

    // 新增：拦截WM_MOUSEWHEEL，父panel将不再响应滚轮
    protected override void WndProc(ref Message m)
    {
        const int WM_MOUSEWHEEL = 0x020A;
        if (m.Msg == WM_MOUSEWHEEL)
        {
            // 处理自己的缩放逻辑
            int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
            float scaleFactor = (delta > 0) ? 1.05f : 0.95f;
            int minWidth = 10, maxWidth = 10000;
            int newWidth = Math.Max(minWidth, Math.Min((int)(this.Width * scaleFactor), maxWidth));
            int newHeight = (int)(newWidth / _aspectRatio);
            this.Size = new Size(newWidth, newHeight);
            this.Invalidate();
            return; // 不调用base，阻止消息上传到父panel
        }
        base.WndProc(ref m);
    }

    private static void LogSvgPanelIssue(string message)
    {
        try
        {
            string key = message ?? string.Empty;
            lock (_svgPanelLogged)
            {
                if (_svgPanelLogged.Contains(key))
                {
                    return;
                }
                _svgPanelLogged.Add(key);
            }

            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "svg_parse.log");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch
        {
            // Diagnostics should never impact runtime behavior.
        }
    }
}
