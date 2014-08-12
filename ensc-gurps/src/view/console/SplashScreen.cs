using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ensc_gurps.view.console;

namespace ensc_gurps.view.console
{
    public class SplashScreen : Screen
    {
        private Text _mainTitle;
        private Button _buttonNew;
        private Button _buttonLoad;
        private Button _buttonExit;

        public SplashScreen(ConsoleView context, Frame border) : base(context, border) { }

        public override void InitUI()
        {
            _mainTitle = new Text("ENSC-GURPS");
            _buttonNew = new Button("Nouvelle partie", "new_game");
            _buttonNew.ClickPerformed += OnClick;
            _buttonLoad = new Button("Charger une partie", "load_game");
            _buttonLoad.ClickPerformed += OnClick;
            _buttonExit = new Button("Quitter", "exit");
            _buttonExit.ClickPerformed += OnClick;

            Add(_mainTitle);
            Add(_buttonNew);
            Add(_buttonLoad);
            Add(_buttonExit);
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            ConsoleKey key = keyInfo.Key;
            if (key == ConsoleKey.UpArrow)
            {
                GiveFocus(_focusIndex - 1);
            }
            else if (key == ConsoleKey.DownArrow)
            {
                GiveFocus(_focusIndex + 1);
            }
            else
            {
                base.OnKeyPressed(keyInfo);
            }
        }

        public void OnClick(string action)
        {
            if (action == "exit")
                System.Environment.Exit(0);

            _context.DisplaySelectionScreen(action);
        }
    }
}
