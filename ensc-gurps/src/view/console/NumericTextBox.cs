using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public delegate void ValueHandler(string valueID, string value);

    public class NumericTextBox : TextBox
    {
        public event ValueHandler ValueChanged;

        public string ValueID { get; set; }

        public NumericTextBox(string placeholder, string valueID)
            : base(placeholder)
        {
            ValueID = valueID;
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
                if (ValueChanged != null)
                    ValueChanged(ValueID, Value);
            }
            else if((key >= 48 && key <= 57) || (key == 45 && Value == ""))
            {
                Value = string.Concat(Value, keyInfo.KeyChar);
                if (ValueChanged != null)
                    ValueChanged(ValueID, Value);
            }

            Draw();
        }

        public override string ToString()
        {
            if (Value == "" || Value == "-")
                return "0";
            return Value;
        }
    }
}
