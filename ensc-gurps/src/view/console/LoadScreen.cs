using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ensc_gurps.utils;
using ensc_gurps.model.character;

namespace ensc_gurps.view.console
{
    public class LoadScreen : Screen
    {
        private Text _mainTitle;

        private Text _label;
        private HorizontalMenu _menu;

        public LoadScreen(ConsoleView context, Frame border) : base(context, border) { }

        public override void InitUI()
        {
            _mainTitle = new Text("Chargement d'une partie");

            // nom
            CompositeWidget layout = new CompositeWidget();

            _label = new Text("Quelle partie souhaitez vous charger ?");
            _menu = new HorizontalMenu();
            _menu.ChoiceMade += LoadSaveGame;

            layout.Add(_label);
            layout.Add(_menu);

            Add(_mainTitle);
            Add(layout);
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

        public void SetData(List<string> savegames)
        {
            savegames = PathUtil.GetSaveGames();
            foreach (string s in savegames)
                _menu.Add(s);
        }

        private void LoadSaveGame(string savegame)
        {
            _context.Controller.Load(savegame);
            _context.Controller.StartGameLoop(null);
        }
    }
}
