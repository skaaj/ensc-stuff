using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ensc_gurps.view.console;

namespace ensc_gurps.view.console
{
    public delegate void ClickHandler(string action);

    public class Button : Text
    {
        private string _actionID;
        public event ClickHandler ClickPerformed;

        public Button(string text, string action) : base(text)
        {
            _actionID = action;
            Focusable = true;
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    if (ClickPerformed != null)
                        ClickPerformed(_actionID);
                    break;
                default:
                    break;
            }
        }
    }
}
