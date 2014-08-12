using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    abstract public class Screen : CompositeWidget
    {
        private List<Widget> _focusableWidgets;
        protected int _focusIndex;
        protected ConsoleView _context;

        public Screen(ConsoleView context, Frame frame)
        {
            _context = context;
            Frame = frame;

            _focusableWidgets = new List<Widget>();
            _focusIndex = -1;

            InitUI();
        }

        abstract public void InitUI();

        public override sealed void CatchFocus()
        {
            int nbWidgets = _focusableWidgets.Count;
            int startIndex = (_focusIndex + 1) % nbWidgets;

            foreach (Widget w in _focusableWidgets)
                w.Focused = false;

            for (int i = startIndex; i < nbWidgets; i++)
            {
                if (_focusableWidgets[i].Focusable)
                {
                    _focusIndex = i;
                    _focusableWidgets[i].OnFocus();
                    if (_focusableWidgets[i].Focused)
                        break;
                }
            }
        }

        public void GiveFocus(int i)
        {
            if (i < 0)
            {
                i = _focusableWidgets.Count - 1;
            }
            else
            {
                i = i % _focusableWidgets.Count;
            }

            foreach (Widget w in _focusableWidgets)
                w.Focused = false;

            _focusableWidgets[i].Focused = true;
            _focusIndex = i;

            Draw();
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            _focusableWidgets[_focusIndex].OnKeyPressed(keyInfo);
        }

        public override sealed void Add(Widget w)
        {
            base.Add(w);

            if (w is CompositeWidget)
            {
                CompositeWidget cw = w as CompositeWidget;
                foreach (Widget item in cw.Widgets)
                    if (item.Focusable)
                        _focusableWidgets.Add(item);
            }
            else
            {
                if (w.Focusable)
                    _focusableWidgets.Add(w);
            }
        }

    }
}
