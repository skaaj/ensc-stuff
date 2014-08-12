using System;
using System.Threading;

namespace ensc_gurps.view.console
{
    public class ConsoleKeyListener
    {
        private ConsoleView _view;
        private Thread _listener;

        public ConsoleKeyListener(ConsoleView view)
        {
            _view = view;

            _listener = new Thread(new ThreadStart(StartListening));
            _listener.Start();
        }

        public void StartListening()
        {
            bool exit = false;
            while (!exit)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0 && keyInfo.Key == ConsoleKey.F4)
                    exit = true;
                else
                    _view.OnKeyPressed(keyInfo);
            }
        }
    }
}
