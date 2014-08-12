using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public class Widget
    {
        public Frame Frame { get; set; }

        public ConsoleColor BackgroundColor { get; set; }
        protected ConsoleColor _savedColor;

        public bool Focusable { get; protected set; }
        public bool Focused { get; set; }

        public ConsoleUtils.Alignment Alignment { get; set; }

        public Widget()
        {
            Frame = new Frame();
            BackgroundColor = ConsoleColor.Black;
            Focusable = false;
        }

        public Widget(int x, int y, int width, int height)
            : this()
        {
            Frame.Left = x;
            Frame.Top = y;
            Frame.Width = width;
            Frame.Height = height;
        }

        public virtual void Draw()
        {
            if(Focused)
                SetBackgroundColor(ConsoleColor.DarkCyan);
            else
                SetBackgroundColor(BackgroundColor);

            ConsoleUtils.ClearRect(Frame.Left, Frame.Top, Frame.Width, Frame.Height);
        }

        protected void SetBackgroundColor(ConsoleColor bgCol)
        {
            _savedColor = Console.BackgroundColor;
            Console.BackgroundColor = bgCol;
        }

        protected void RestoreBackgroundColor()
        {
            Console.BackgroundColor = _savedColor;
        }

        public virtual void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            // Nothing to do.
        }

        public virtual void OnFocus()
        {
            Focused = Focusable;
        }
    }
}
