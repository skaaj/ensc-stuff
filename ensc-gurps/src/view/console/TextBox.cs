using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public class TextBox : Text
    {
        protected string _placeHolder;

        public TextBox(string placeholder) : base(placeholder)
        {
            _placeHolder = placeholder;
            Focusable = true;
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            char key = keyInfo.KeyChar;

            if (Value == _placeHolder)
                Value = "";

            if (key == '\b')
            {
                if(Value.Length > 0)
                    Value = Value.Remove(Value.Length - 1);
            }
            else if(key >= 32 && key <= 175)
            {
                Value = string.Concat(Value, keyInfo.KeyChar);
            }

            Draw();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            Draw();
        }

        public override void Draw()
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            base.Draw();

            Console.ForegroundColor = color;
            Console.CursorVisible = true;
        }
    }
}
