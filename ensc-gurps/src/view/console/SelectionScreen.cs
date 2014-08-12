using ensc_gurps.utils;
using System;
using System.Collections.Generic;

namespace ensc_gurps.view.console
{
    public class SelectionScreen : Screen
    {
        // Graphic
        private Text _mainTitle;
        private CompositeWidget _layoutUniverse;
        private Text _labelUniverse;
        private HorizontalMenu _pickerUniverse;

        private CompositeWidget _layoutAdventure;
        private Text _labelAdventure;
        private HorizontalMenu _pickerAdventure;

        // Difficulté
        private Text _labelDiff;
        private HorizontalMenu _menuDiff;

        private string nextScreen;

        private Button _buttonNext;

        public SelectionScreen(ConsoleView context, Frame border) : base(context, border) { }

        public override void InitUI()
        {
            _mainTitle = new Text("ENSC-GURPS");

            // UNIVERSE
            _layoutUniverse = new CompositeWidget();
            _layoutUniverse.Orientation = OrientationEnum.Horizontal;

            _labelUniverse = new Text("Choisissez un univers");
            _pickerUniverse = new HorizontalMenu();
            foreach (string universe in PathUtil.GetUniverses())
                _pickerUniverse.Add(universe);  

            _layoutUniverse.Add(_labelUniverse);
            _layoutUniverse.Add(_pickerUniverse);
            _pickerUniverse.ChoiceChanged += OnUniverseChosen;

            // ADVENTURE
            _layoutAdventure = new CompositeWidget();
            _layoutAdventure.Orientation = OrientationEnum.Horizontal;

            _labelAdventure = new Text("Choisissez une aventure");
            _pickerAdventure = new HorizontalMenu();

            _layoutAdventure.Add(_labelAdventure);
            _layoutAdventure.Add(_pickerAdventure);

            _buttonNext = new Button("Continuer", "go");
            _buttonNext.ClickPerformed += NextScreen;

            Add(_mainTitle);
            Add(_layoutUniverse);
            Add(_layoutAdventure);
        }

        public void SetNextScreen(string action)
        {
            nextScreen = action;
            if (action == "new_game")
            {
                CompositeWidget layoutDiff = new CompositeWidget();
                layoutDiff.Orientation = OrientationEnum.Horizontal;
                _labelDiff = new Text("Difficulté ?");
                _menuDiff = new HorizontalMenu();

                layoutDiff.Add(_labelDiff);
                layoutDiff.Add(_menuDiff);

                Add(layoutDiff);
            }
            Add(_buttonNext);
        }

        public void LoadDifficulties(Dictionary<string, int> difficulty)
        {
            foreach (string diff in difficulty.Keys)
                _menuDiff.Add(diff);
        }

        public void OnUniverseChosen(string u)
        {
            _context.Controller.OnUniverseChosen(u);

            _pickerAdventure.Clear();
            foreach (string adv in PathUtil.GetAdventures())
                _pickerAdventure.Add(adv);

            Draw();
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

        public void NextScreen(string a)
        {
            if (_pickerAdventure.IsNotSet()) return;

            _context.Controller.OnAdventureChosen(_pickerAdventure.Values[_pickerAdventure.Selected]);
            if (nextScreen == "new_game")
                _context.DisplayNewScreen(_menuDiff.Values[_menuDiff.Selected]);
            else
                _context.DisplayLoadScreen();
        }
    }
}
