using System;
using System.Drawing;
using System.Windows.Forms;

namespace _4._18
{
    /// <summary>
    /// 自定義滾動條控件，支持自定義顏色樣式
    /// </summary>
    public class CustomScrollBar : Panel
    {
        private Panel thumbPanel;
        private bool isDragging = false;
        private int dragStartY;
        private int thumbStartTop;

        private int minimum = 0;
        private int maximum = 100;
        private int value = 0;
        private int largeChange = 10;
        private int smallChange = 1;

        // 顏色設置 - 與 listBox1 保持一致
        private Color trackColor = Color.FromArgb(230, 232, 235);
        private Color thumbColor = Color.FromArgb(180, 182, 185);
        private Color thumbHoverColor = Color.FromArgb(160, 162, 165);
        private Color borderColor = Color.FromArgb(200, 202, 205);

        public event ScrollEventHandler Scroll;

        public CustomScrollBar()
        {
            this.DoubleBuffered = true;
            this.BackColor = trackColor;
            this.BorderStyle = BorderStyle.FixedSingle;

            // 創建滾動塊
            thumbPanel = new Panel
            {
                BackColor = thumbColor,
                Cursor = Cursors.Hand
            };
            thumbPanel.MouseDown += ThumbPanel_MouseDown;
            thumbPanel.MouseMove += ThumbPanel_MouseMove;
            thumbPanel.MouseUp += ThumbPanel_MouseUp;
            thumbPanel.MouseEnter += (s, e) => { thumbPanel.BackColor = thumbHoverColor; };
            thumbPanel.MouseLeave += (s, e) => { if (!isDragging) thumbPanel.BackColor = thumbColor; };

            this.Controls.Add(thumbPanel);
            this.Resize += CustomScrollBar_Resize;
            this.MouseClick += CustomScrollBar_MouseClick;

            UpdateThumbSize();
        }

        public int Minimum
        {
            get => minimum;
            set { minimum = value; UpdateThumbSize(); }
        }

        public int Maximum
        {
            get => maximum;
            set { maximum = value; UpdateThumbSize(); }
        }

        public int Value
        {
            get => value;
            set
            {
                int newValue = Math.Max(minimum, Math.Min(GetMaxScrollValue(), value));
                if (this.value != newValue)
                {
                    this.value = newValue;
                    UpdateThumbPosition();
                    OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, newValue));
                }
            }
        }

        public int LargeChange
        {
            get => largeChange;
            set { largeChange = value; UpdateThumbSize(); }
        }

        public int SmallChange
        {
            get => smallChange;
            set => smallChange = value;
        }

        public Color TrackColor
        {
            get => trackColor;
            set { trackColor = value; this.BackColor = value; }
        }

        public Color ThumbColor
        {
            get => thumbColor;
            set { thumbColor = value; thumbPanel.BackColor = value; }
        }

        private int GetMaxScrollValue()
        {
            return Math.Max(0, maximum - largeChange);
        }

        private void UpdateThumbSize()
        {
            if (this.Height <= 0 || maximum <= minimum) return;

            int trackHeight = this.Height - 4; // 留出邊距
            int range = maximum - minimum;

            // 計算滾動塊高度（最小50像素）
            int thumbHeight = Math.Max(50, (int)((float)largeChange / range * trackHeight));
            thumbHeight = Math.Min(thumbHeight, trackHeight);

            thumbPanel.Width = this.Width - 4;
            thumbPanel.Height = thumbHeight;
            thumbPanel.Left = 2;

            UpdateThumbPosition();
        }

        private void UpdateThumbPosition()
        {
            if (this.Height <= 0) return;

            int trackHeight = this.Height - 4;
            int thumbHeight = thumbPanel.Height;
            int scrollableHeight = trackHeight - thumbHeight;
            int maxScrollValue = GetMaxScrollValue();

            if (maxScrollValue > 0 && scrollableHeight > 0)
            {
                int thumbTop = 2 + (int)((float)value / maxScrollValue * scrollableHeight);
                thumbPanel.Top = Math.Max(2, Math.Min(thumbTop, trackHeight - thumbHeight + 2));
            }
            else
            {
                thumbPanel.Top = 2;
            }
        }

        private void ThumbPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartY = e.Y;
                thumbStartTop = thumbPanel.Top;
                thumbPanel.BackColor = thumbHoverColor;
            }
        }

        private void ThumbPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int trackHeight = this.Height - 4;
                int thumbHeight = thumbPanel.Height;
                int scrollableHeight = trackHeight - thumbHeight;

                int newTop = thumbStartTop + (e.Y - dragStartY);
                newTop = Math.Max(2, Math.Min(newTop, scrollableHeight + 2));

                int maxScrollValue = GetMaxScrollValue();
                if (scrollableHeight > 0 && maxScrollValue > 0)
                {
                    int newValue = (int)((float)(newTop - 2) / scrollableHeight * maxScrollValue);
                    Value = newValue;
                }
            }
        }

        private void ThumbPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            thumbPanel.BackColor = thumbColor;
        }

        private void CustomScrollBar_MouseClick(object sender, MouseEventArgs e)
        {
            // 點擊軌道時跳轉
            if (e.Y < thumbPanel.Top)
            {
                Value -= largeChange;
            }
            else if (e.Y > thumbPanel.Bottom)
            {
                Value += largeChange;
            }
        }

        private void CustomScrollBar_Resize(object sender, EventArgs e)
        {
            UpdateThumbSize();
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                Value -= smallChange * 5;
            }
            else
            {
                Value += smallChange * 5;
            }
        }
    }
}
