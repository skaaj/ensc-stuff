using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ensc_gurps.utils;
using ensc_gurps.model.character;

namespace ensc_gurps.view.console
{
    public class NewScreen : Screen
    {
        // Graphic
        private Text _mainTitle;

        // Nom
        private Text _labelName;
        private TextBox _inputName;

        // Classes
        private Text _labelClass;
        private HorizontalMenu _menuClass;

        private Text _labelPts;

        private Button _buttonNext;

        private List<CompositeWidget> _layoutsTrait;
        private List<Trait> _traits;
        private float initialPoints;
        private Character _player;
        private List<Class> _classes;

        public NewScreen(ConsoleView context, Frame border) : base(context, border) { }

        public override void InitUI()
        {
            _mainTitle = new Text("Nouveau personnage");

            // nom
            CompositeWidget layoutName = new CompositeWidget();
            layoutName.Orientation = OrientationEnum.Horizontal;

            _labelName = new Text("Nom ?");
            _inputName = new TextBox("...");

            layoutName.Add(_labelName);
            layoutName.Add(_inputName);

            // class
            CompositeWidget layoutClass = new CompositeWidget();
            layoutClass.Orientation = OrientationEnum.Horizontal;

            _labelClass = new Text("Classe ?");
            _menuClass = new HorizontalMenu();

            layoutClass.Add(_labelClass);
            layoutClass.Add(_menuClass);

            Add(_mainTitle);
            Add(layoutName);
            Add(layoutClass);
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

        public void SetData(List<Class> characterClasses, Character player, string difficulty)
        {
            _context.Controller.OnDifficultyChosen(difficulty);

            _player = player;
            _classes = characterClasses;

            foreach (Class c in characterClasses)
                _menuClass.Add(c.Name);

            _layoutsTrait = new List<CompositeWidget>();
            foreach (Trait t in player.Traits)
            {
                CompositeWidget layout = new CompositeWidget();
                layout.Orientation = OrientationEnum.Horizontal;

                Text label = new Text(string.Format("{0} (coût : {1})", t.Name, t.Cost));
                initialPoints += t.Value * t.Cost;
                NumericTextBox input = new NumericTextBox(t.Value.ToString(), t.TraitID);
                input.ValueChanged += ValueChanged;
                layout.Add(label);
                layout.Add(input);
                _layoutsTrait.Add(layout);
                Add(layout);
            }

            _traits = player.Traits;

            initialPoints += player.Points;
            _labelPts = new Text(player.Points.ToString() + " points restants");
            Add(_labelPts);

            _buttonNext = new Button("Continuer", "launch_game");
            _buttonNext.ClickPerformed += LaunchGame;
            Add(_buttonNext);
        }

        public void LaunchGame(string action)
        {
            float sum = GetTraitSum();
            if (GetTraitSum() <= 0)
            {
                _buttonNext.Value = "Continuer - Vous ne pouvez pas dépenser trop de points";
                _buttonNext.Draw();
            }
            else
            {
                _player.Name = _inputName.Value;

                foreach (Class c in _classes)
                    if (c.Name == _menuClass.Values[_menuClass.Selected])
                        _player.Class = c;

                for(int i = 0; i < _traits.Count; i++)
                    _player.Traits[i].Value = int.Parse(_layoutsTrait[i].Widgets[1].ToString());

                _context.Controller.StartGameLoop(_player);
            }
        }

        public float GetTraitSum()
        {
            int sum = 0;
            for (int i = 0; i < _traits.Count; i++)
            {
                sum += _traits[i].Cost * int.Parse(_layoutsTrait[i].Widgets[1].ToString());
            }
            return sum;
        }

        public void ValueChanged(string valueID, string value)
        {
            float sum = GetTraitSum();
            _labelPts.Value = (initialPoints - sum) + " points restants";
            _labelPts.Draw();
        }
    }
}
