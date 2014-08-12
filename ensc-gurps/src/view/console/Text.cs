using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public class Text : Widget
    {
        public string Value { get; set; }

        public Text(string value)
        {
            Value = value;
            Focusable = false;
        }

        public override void Draw()
        {
            base.Draw();

            int x, y;
            ConsoleUtils.Align(ConsoleUtils.Alignment.Center, out x, out y, Value.Length, 0, Frame.GetPaddingFrame());

            Console.SetCursorPosition(x, y);
            Console.Write(Value);
            Console.CursorVisible = false;
            RestoreBackgroundColor();
        }
    }
}
