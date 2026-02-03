using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

public class ScreenCaptureManager
{
    private Control targetControl;
    private Point startPoint;          // 截图起点
    private Point endPoint;            // 截图终点
    private Point rectStartPoint;      // 显示矩形的起点（固定不变）
    private Point rectEndPoint;        // 显示矩形的终点（动态调整）
    private Rectangle captureRectangle; // 当前显示的矩形区域
    private List<Bitmap> captureList;  // 存储每次截图的结果
    private bool isCapturing;
    private bool drawOverlay = true;   // 控制是否绘制黑色边框

    public ScreenCaptureManager(Control target)
    {
        targetControl = target;
        captureList = new List<Bitmap>();
        isCapturing = false;
    }

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(
        IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

    public void StartCapture(Point start)
    {
        startPoint = start;
        endPoint = start;
        rectStartPoint = start;
        rectEndPoint = start;
        captureRectangle = new Rectangle(start, new Size(0, 0));
        isCapturing = true;
    }

    public void UpdateCapture(Point currentPoint)
    {
        if (!isCapturing) return;

        rectEndPoint = currentPoint;
        captureRectangle = GetRectangleFromPoints(rectStartPoint, rectEndPoint);
        targetControl.Invalidate();
        endPoint = currentPoint;
    }

    public void HandleScroll(int delta, int moveStep)
    {
        if (!isCapturing) return;

        // 1. 不畫邊框
        drawOverlay = false;
        targetControl.Invalidate();
        targetControl.Update(); // 強制立即重繪（確保不畫邊框）

        // 2. 截圖
        CaptureAndAddToList();

        // 3. 恢復畫邊框
        drawOverlay = true;
        targetControl.Invalidate();

        // 4. 更新區域
        startPoint = new Point(startPoint.X, endPoint.Y);
        endPoint = new Point(endPoint.X, endPoint.Y + (delta > 0 ? -moveStep : moveStep));
        rectEndPoint = new Point(rectEndPoint.X, rectEndPoint.Y + (delta > 0 ? -moveStep : moveStep));
        captureRectangle = GetRectangleFromPoints(rectStartPoint, rectEndPoint);
    }

    public void FinalizeCapture(Point finalEndPoint)
    {
        if (!isCapturing) return;

        // 不畫邊框
        drawOverlay = false;
        targetControl.Invalidate();
        targetControl.Update();

        endPoint = finalEndPoint;
        CaptureAndAddToList();

        // 恢復畫邊框
        drawOverlay = true;
        targetControl.Invalidate();

        Bitmap finalImage = MergeBitmaps(captureList);
        Clipboard.SetImage(finalImage);

        isCapturing = false;
        captureList.Clear();
        targetControl.Invalidate();
    }

    public void DrawOverlay(Graphics graphics)
    {
        if (isCapturing && drawOverlay)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                graphics.DrawRectangle(pen, captureRectangle);
            }
        }
    }

    private void CaptureAndAddToList()
    {
        int x1 = Math.Min(startPoint.X, endPoint.X);
        int y1 = Math.Min(startPoint.Y, endPoint.Y);
        int x2 = Math.Max(startPoint.X, endPoint.X);
        int y2 = Math.Max(startPoint.Y, endPoint.Y);

        int width = x2 - x1;
        int height = y2 - y1;

        if (width > 0 && height > 0)
        {
            Bitmap bitmap = CaptureControlArea(
                targetControl,
                x1,
                y1,
                width,
                height
            );
            if (bitmap != null)
            {
                captureList.Add(bitmap);
            }
        }
    }

    private Bitmap CaptureControlArea(Control control, int x, int y, int width, int height)
    {
        if (width <= 0 || height <= 0) return null;

        Bitmap bitmap = new Bitmap(width, height);
        using (Graphics gDest = Graphics.FromImage(bitmap))
        {
            IntPtr hdcDest = gDest.GetHdc();
            using (Graphics gSrc = control.CreateGraphics())
            {
                IntPtr hdcSrc = gSrc.GetHdc();
                BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, CopyPixelOperation.SourceCopy);
                gSrc.ReleaseHdc(hdcSrc);
            }
            gDest.ReleaseHdc(hdcDest);
        }

        return bitmap;
    }

    private Rectangle GetRectangleFromPoints(Point start, Point end)
    {
        int x = Math.Min(start.X, end.X);
        int y = Math.Min(start.Y, end.Y);
        int width = Math.Abs(end.X - start.X);
        int height = Math.Abs(end.Y - start.Y);
        return new Rectangle(x, y, width, height);
    }

    public static Bitmap MergeBitmaps(List<Bitmap> bitmaps)
    {
        if (bitmaps == null || bitmaps.Count == 0)
        {
            throw new ArgumentException("The bitmap list is empty.");
        }

        int totalHeight = 0;
        int maxWidth = 0;
        foreach (var bitmap in bitmaps)
        {
            totalHeight += bitmap.Height;
            if (bitmap.Width > maxWidth)
            {
                maxWidth = bitmap.Width;
            }
        }

        Bitmap mergedBitmap = new Bitmap(maxWidth, totalHeight);
        using (Graphics g = Graphics.FromImage(mergedBitmap))
        {
            int currentHeight = 0;
            foreach (var bitmap in bitmaps)
            {
                g.DrawImage(bitmap, 0, currentHeight);
                currentHeight += bitmap.Height;
            }
        }

        return mergedBitmap;
    }
}