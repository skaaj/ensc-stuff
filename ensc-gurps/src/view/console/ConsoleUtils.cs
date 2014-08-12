using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.view.console
{
    public class ConsoleUtils
    {
        public enum Alignment { Center, Left, Top, Right, Bottom };

        public static void NewLine()
        {
            Console.Write(Environment.NewLine);
        }

        public static int InputInt(string message, int min, int max)
        {
            int input;
            bool pass;
            do{
                Console.Write(message);
                pass = int.TryParse(Console.ReadLine(), out input);
            }while(!pass || input < min || input > max);

            return input;
        }

        public static void MatrixWrite(string str)
        {
            foreach (var c in str)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(100);
            }
        }

        public static void Align(Alignment a, out int x, out int y, int w, int h, Frame f)
        {
            switch (a)
            {
                case Alignment.Center:
                    y = f.Top + (f.Height / 2);
                    x = f.Left + ((f.Width / 2) - (w / 2));
                    break;
                case Alignment.Left:
                    y = f.Top + (f.Height / 2);
                    x = f.Left;
                    break;
                case Alignment.Top:
                    y = f.Top;
                    x = f.Left + ((f.Width / 2) - (w / 2));
                    break;
                case Alignment.Right:
                    y = f.Top + (f.Height / 2);
                    x = f.Right - w;
                    break;
                case Alignment.Bottom:
                    y = f.Bottom - 1;
                    x = f.Left + ((f.Width / 2) - (w / 2));
                    break;
                default:
                    y = f.Top;
                    x = f.Left;
                    break;
            }
        }

        public static void ClearRect(int x, int y, int w, int h)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < w; i++)
                builder.Append(' ');

            string emptyLine = builder.ToString();

            for (int i = 0; i < h; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(emptyLine);
            }
        }
    }
}
