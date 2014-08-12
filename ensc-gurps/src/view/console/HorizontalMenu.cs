using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public delegate void ChoiceMadeHandler(string choice);

    public class HorizontalMenu : Widget
    {
        public List<string> Values { get; set; }
        public int Selected { get; set; }
        private string _placeHolder;

        public event ChoiceMadeHandler ChoiceMade;
        public event ChoiceMadeHandler ChoiceChanged;

        public HorizontalMenu()
        {
            Values = new List<string>();

            _placeHolder = "Vide";
            Values.Add(_placeHolder);

            Focusable = true;
        }

        public void Add(string value)
        {
            if (Values[0].Equals(_placeHolder))
            {
                Values.Clear();
                Selected = 0;
            }

            Values.Add(value);
        }

        public void Clear()
        {
            Values.Clear();
            Values.Add(_placeHolder);
        }

        public bool IsNotSet()
        {
            return Values[Selected] == _placeHolder;
        }

        public void Previous()
        {
            if (Selected == 0)
                Selected = Values.Count - 1;
            else
                Selected = Selected - 1;
            Draw();
        }

        public void Next()
        {
            Selected = (Selected + 1) % Values.Count;
            Draw();
        }

        public override void Draw()
        {
            base.Draw();

            if (Values.Count == 0) return;

            int y = (Frame.Height / 2);
            int x = (Frame.Width / 2) - (Values[Selected].Length / 2);

            y = Frame.Top + y;
            x = Frame.Left + x;

            Console.SetCursorPosition(x, y);
            Console.Write(string.Format("◄- {0} -►", Values[Selected]));

            Console.CursorVisible = false;
            RestoreBackgroundColor();
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    Previous();
                    if (ChoiceChanged != null)
                        ChoiceChanged(Values[Selected]);
                    break;
                case ConsoleKey.RightArrow:
                    Next();
                    if (ChoiceChanged != null)
                        ChoiceChanged(Values[Selected]);
                    break;
                case ConsoleKey.Enter:
                    if(ChoiceMade != null && Values[Selected] != _placeHolder)
                        ChoiceMade(Values[Selected]);
                    break;
                default:
                    break;
            }
        }
    }
}
