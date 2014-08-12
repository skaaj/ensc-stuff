using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public class Frame
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get { return Left + Width; } }
        public int Bottom { get { return Top + Height; } }

        public int Width { get; set; }
        public int Height { get; set; }

        public int PaddingLeft { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingBottom { get; set; }

        public Frame() { }

        public Frame(int x, int y, int w, int h)
        {
            Left = x;
            Top  = y;
            Width  = w;
            Height = h;
        }

        public Frame GetPaddingFrame()
        {
            Frame padding = new Frame();

            padding.Left = this.Left + PaddingLeft;
            padding.Top = this.Top + PaddingTop;
            padding.Width = this.Width - PaddingLeft - PaddingRight;
            padding.Height = this.Height - PaddingTop - PaddingBottom;

            return padding;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", Left, Top, Width, Height);
        }
    }
}
