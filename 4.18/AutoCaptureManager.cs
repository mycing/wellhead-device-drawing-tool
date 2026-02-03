using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _4._18
{
    /// <summary>
    /// 自動截圖管理器 - 自動計算 panel2 中所有控件的邊界並截取
    /// </summary>
    public class AutoCaptureManager
    {
        private Panel targetPanel;      // panel2
        private const int PADDING = 10; // 邊界外擴展的像素

        public AutoCaptureManager(Panel panel, Panel container)
        {
            targetPanel = panel;
        }

        /// <summary>
        /// 執行自動截圖
        /// </summary>
        public void Execute()
        {
            if (targetPanel.Controls.Count == 0)
            {
                MessageBox.Show("畫布上沒有任何內容可以截取。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 計算所有控件的邊界（在 panel2 坐標系中）
            Rectangle contentBounds = CalculateContentBounds();

            if (contentBounds.Width <= 0 || contentBounds.Height <= 0)
            {
                MessageBox.Show("無法計算內容邊界。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 添加 padding
            int left = Math.Max(0, contentBounds.Left - PADDING);
            int top = Math.Max(0, contentBounds.Top - PADDING);
            int right = Math.Min(targetPanel.Width, contentBounds.Right + PADDING);
            int bottom = Math.Min(targetPanel.Height, contentBounds.Bottom + PADDING);

            int captureWidth = right - left;
            int captureHeight = bottom - top;

            try
            {
                // 使用 DrawToBitmap 截取 panel2 的完整內容
                Bitmap fullBitmap = new Bitmap(targetPanel.Width, targetPanel.Height);
                targetPanel.DrawToBitmap(fullBitmap, new Rectangle(0, 0, targetPanel.Width, targetPanel.Height));

                // 裁剪出需要的區域
                Bitmap croppedBitmap = new Bitmap(captureWidth, captureHeight);
                using (Graphics g = Graphics.FromImage(croppedBitmap))
                {
                    g.DrawImage(fullBitmap,
                        new Rectangle(0, 0, captureWidth, captureHeight),
                        new Rectangle(left, top, captureWidth, captureHeight),
                        GraphicsUnit.Pixel);
                }

                fullBitmap.Dispose();

                Clipboard.SetImage(croppedBitmap);
                MessageBox.Show("截圖已複製到剪貼簿。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"截圖失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 計算所有控件的邊界
        /// </summary>
        private Rectangle CalculateContentBounds()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (Control ctrl in targetPanel.Controls)
            {
                if (ctrl.Visible)
                {
                    minX = Math.Min(minX, ctrl.Left);
                    minY = Math.Min(minY, ctrl.Top);
                    maxX = Math.Max(maxX, ctrl.Right);
                    maxY = Math.Max(maxY, ctrl.Bottom);
                }
            }

            if (minX == int.MaxValue)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
