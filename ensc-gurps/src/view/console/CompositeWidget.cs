using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public class CompositeWidget : Widget
    {
        public enum OrientationEnum { Horizontal, Vertical };

        public List<Widget> Widgets { get; set; }
        public OrientationEnum Orientation { get; set; }

        public CompositeWidget(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Orientation = OrientationEnum.Vertical;
            Widgets = new List<Widget>();
        }

        public CompositeWidget()
            : base()
        {
            Orientation = OrientationEnum.Vertical;
            Widgets = new List<Widget>();
        }

        public override void Draw()
        {
            base.Draw();

            foreach (Widget w in Widgets)
                w.Draw();
        }

        public void Refresh()
        {
            int nbWidgets = Widgets.Count;

            int x = Frame.Left;
            int y = Frame.Top;
            int width = Frame.Width;
            int height = Frame.Height;

            int i = 0;
            foreach (Widget widget in Widgets)
            {
                if (Orientation == OrientationEnum.Horizontal)
                {
                    widget.Frame.Width = width / nbWidgets;
                    widget.Frame.Height = height;
                    widget.Frame.Left = x + (i * widget.Frame.Width);
                    widget.Frame.Top = y;
                }
                else if (Orientation == OrientationEnum.Vertical)
                {
                    widget.Frame.Width = width;
                    widget.Frame.Height = height / nbWidgets;
                    widget.Frame.Left = x;
                    widget.Frame.Top = y + (i * widget.Frame.Height);
                }

                if (widget is CompositeWidget)
                {
                    CompositeWidget cw = widget as CompositeWidget;
                    cw.Refresh();
                }

                i++;
            }
        }

        public virtual void Add(Widget w)
        {
            Widgets.Add(w);
            this.Refresh();
        }

        public virtual void CatchFocus()
        {
            // Nothing FIXME
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            foreach (Widget w in Widgets)
                w.OnKeyPressed(keyInfo);
        }
    }
}
