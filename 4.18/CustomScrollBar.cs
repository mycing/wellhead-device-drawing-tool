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
        private int dragStartPos;
        private int thumbStartPos;

        private int minimum = 0;
        private int maximum = 100;
        private int value = 0;
        private int largeChange = 10;
        private int smallChange = 1;
        private Orientation orientation = Orientation.Vertical;

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
                Cursor = Cursors.Default
            };
            this.Cursor = Cursors.Default;
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

        public Orientation Orientation
        {
            get => orientation;
            set
            {
                if (orientation == value) return;
                orientation = value;
                UpdateThumbSize();
            }
        }

        private int GetMaxScrollValue()
        {
            return Math.Max(0, maximum - largeChange);
        }

        private void UpdateThumbSize()
        {
            int trackLength = (orientation == Orientation.Vertical ? this.Height : this.Width) - 4;
            if (trackLength <= 0 || maximum <= minimum) return;

            int range = maximum - minimum;

            // 計算滾動塊長度（最小50像素）
            int thumbLength = Math.Max(50, (int)((float)largeChange / range * trackLength));
            thumbLength = Math.Min(thumbLength, trackLength);

            if (orientation == Orientation.Vertical)
            {
                thumbPanel.Width = this.Width - 4;
                thumbPanel.Height = thumbLength;
                thumbPanel.Left = 2;
            }
            else
            {
                thumbPanel.Height = this.Height - 4;
                thumbPanel.Width = thumbLength;
                thumbPanel.Top = 2;
            }

            UpdateThumbPosition();
        }

        private void UpdateThumbPosition()
        {
            int trackLength = (orientation == Orientation.Vertical ? this.Height : this.Width) - 4;
            if (trackLength <= 0) return;

            int thumbLength = orientation == Orientation.Vertical ? thumbPanel.Height : thumbPanel.Width;
            int scrollableLength = trackLength - thumbLength;
            int maxScrollValue = GetMaxScrollValue();

            if (maxScrollValue > 0 && scrollableLength > 0)
            {
                int thumbPos = 2 + (int)((float)value / maxScrollValue * scrollableLength);
                thumbPos = Math.Max(2, Math.Min(thumbPos, trackLength - thumbLength + 2));
                if (orientation == Orientation.Vertical)
                {
                    thumbPanel.Top = thumbPos;
                }
                else
                {
                    thumbPanel.Left = thumbPos;
                }
            }
            else
            {
                if (orientation == Orientation.Vertical)
                {
                    thumbPanel.Top = 2;
                }
                else
                {
                    thumbPanel.Left = 2;
                }
            }
        }

        private void ThumbPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                Point p = this.PointToClient(Cursor.Position);
                dragStartPos = orientation == Orientation.Vertical ? p.Y : p.X;
                thumbStartPos = orientation == Orientation.Vertical ? thumbPanel.Top : thumbPanel.Left;
                thumbPanel.Capture = true;
                thumbPanel.BackColor = thumbHoverColor;
            }
        }

        private void ThumbPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int trackLength = (orientation == Orientation.Vertical ? this.Height : this.Width) - 4;
                int thumbLength = orientation == Orientation.Vertical ? thumbPanel.Height : thumbPanel.Width;
                int scrollableLength = trackLength - thumbLength;

                Point p = this.PointToClient(Cursor.Position);
                int currentPos = orientation == Orientation.Vertical ? p.Y : p.X;
                int newPos = thumbStartPos + (currentPos - dragStartPos);
                newPos = Math.Max(2, Math.Min(newPos, scrollableLength + 2));

                int maxScrollValue = GetMaxScrollValue();
                if (scrollableLength > 0 && maxScrollValue > 0)
                {
                    int newValue = (int)((float)(newPos - 2) / scrollableLength * maxScrollValue);
                    Value = newValue;
                }
            }
        }

        private void ThumbPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            thumbPanel.Capture = false;
            thumbPanel.BackColor = thumbColor;
        }

        private void CustomScrollBar_MouseClick(object sender, MouseEventArgs e)
        {
            // 點擊軌道時跳轉
            int clickPos = orientation == Orientation.Vertical ? e.Y : e.X;
            int thumbStart = orientation == Orientation.Vertical ? thumbPanel.Top : thumbPanel.Left;
            int thumbEnd = orientation == Orientation.Vertical ? thumbPanel.Bottom : thumbPanel.Right;

            if (clickPos < thumbStart)
            {
                Value -= largeChange;
            }
            else if (clickPos > thumbEnd)
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
