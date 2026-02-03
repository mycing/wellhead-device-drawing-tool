using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _4._18
{
    /// <summary>
    /// 自動對齊管理器
    /// 負責 panel2 中控件的自動排列功能
    /// </summary>
    public class AutoAlignManager
    {
        private Panel targetPanel;
        private int currentMode = 0;  // 0 = 文字居中於圖片，1 = 文字在圖片右側

        // 對齊參數
        private const int TOP_MARGIN = 150;      // 距離頂部的起始位置
        private const int TEXT_RIGHT_OFFSET = 30; // 模式1時文字在圖片右側的偏移量

        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="panel">目標面板</param>
        public AutoAlignManager(Panel panel)
        {
            targetPanel = panel;
        }

        /// <summary>
        /// 當前對齊模式
        /// </summary>
        public int CurrentMode
        {
            get { return currentMode; }
        }

        /// <summary>
        /// 執行自動對齊
        /// 循環兩種模式：
        /// 模式0：文字在對應圖片的正中間
        /// 模式1：文字垂直居中於圖片，水平在圖片右側30px處
        /// </summary>
        public void Execute()
        {
            if (targetPanel == null || targetPanel.Controls.Count == 0)
            {
                return;
            }

            // 分離圖片和文字控件
            List<Control> imageControls = new List<Control>();
            List<Control> textControls = new List<Control>();

            foreach (Control ctrl in targetPanel.Controls)
            {
                if (ctrl is DrawstringPanel)
                {
                    textControls.Add(ctrl);
                }
                else
                {
                    // SvgDrawPicturePanel 和 ImagePicturePanel 都視為圖片
                    imageControls.Add(ctrl);
                }
            }

            // 按照 Top 位置從上到下排序
            imageControls.Sort((a, b) => a.Top.CompareTo(b.Top));
            textControls.Sort((a, b) => a.Top.CompareTo(b.Top));

            // 排列圖片並獲取位置信息
            List<Rectangle> imageBounds = AlignImages(imageControls);

            // 根據當前模式排列文字
            AlignTexts(textControls, imageBounds);

            // 切換模式
            currentMode = (currentMode + 1) % 2;

            targetPanel.Refresh();
        }

        /// <summary>
        /// 排列圖片控件
        /// 從頂端開始，水平居中，上下緊鄰
        /// </summary>
        /// <param name="imageControls">圖片控件列表</param>
        /// <returns>每個圖片的邊界矩形列表</returns>
        private List<Rectangle> AlignImages(List<Control> imageControls)
        {
            List<Rectangle> imageBounds = new List<Rectangle>();
            int currentY = TOP_MARGIN;
            int panelCenterX = targetPanel.Width / 2;

            foreach (Control ctrl in imageControls)
            {
                // 計算居中的 X 位置
                int centerX = panelCenterX - (ctrl.Width / 2);

                // 設置新位置
                ctrl.Location = new Point(centerX, currentY);

                // 記錄圖片的邊界
                imageBounds.Add(new Rectangle(centerX, currentY, ctrl.Width, ctrl.Height));

                // 下一個圖片的 Y 位置緊鄰當前圖片底部
                currentY += ctrl.Height;
            }

            return imageBounds;
        }

        /// <summary>
        /// 排列文字控件
        /// 根據當前模式將文字定位到對應圖片的指定位置
        /// </summary>
        /// <param name="textControls">文字控件列表</param>
        /// <param name="imageBounds">圖片邊界列表</param>
        private void AlignTexts(List<Control> textControls, List<Rectangle> imageBounds)
        {
            for (int i = 0; i < textControls.Count && i < imageBounds.Count; i++)
            {
                Control textCtrl = textControls[i];
                Rectangle imgBounds = imageBounds[i];

                int textX, textY;

                // 垂直位置：始終在圖片的垂直中間
                textY = imgBounds.Y + (imgBounds.Height / 2) - (textCtrl.Height / 2);

                if (currentMode == 0)
                {
                    // 模式0：文字水平居中於圖片
                    textX = imgBounds.X + (imgBounds.Width / 2) - (textCtrl.Width / 2);
                }
                else
                {
                    // 模式1：文字在圖片右側
                    textX = imgBounds.X + imgBounds.Width + TEXT_RIGHT_OFFSET;
                }

                textCtrl.Location = new Point(textX, textY);
            }
        }

        /// <summary>
        /// 重置對齊模式為初始狀態
        /// </summary>
        public void ResetMode()
        {
            currentMode = 0;
        }
    }
}
