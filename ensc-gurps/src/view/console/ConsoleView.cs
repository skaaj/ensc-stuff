using ensc_gurps.controller;
using ensc_gurps.model.adventure;
using ensc_gurps.model.character;
using System;
using System.Collections.Generic;
using System.Text;

namespace ensc_gurps.view.console
{
    public class ConsoleView //: View
    {
        private const int SCREEN_WIDTH = 122;
        private const int SCREEN_HEIGHT = 42;

        private Controller _ctrl;
        private ConsoleKeyListener _keyListener;

        public Controller Controller { get { return _ctrl; } }

        // Screens
        private Screen _currentScreen;
        private SelectionScreen _selectionScreen;
        private SplashScreen _splashScreen;
        private NewScreen _newScreen;
        private LoadScreen _loadScreen;

        public enum ScreenEnum { View, Splash, Select, New, Load };

        public ConsoleView(string title)
        {
            Console.Title = title;
            Console.SetWindowSize(SCREEN_WIDTH, SCREEN_HEIGHT);
            Console.SetBufferSize(SCREEN_WIDTH, SCREEN_HEIGHT);
        }

        public void InitView()
        {
            Frame screenFrame = new Frame(1, 1, SCREEN_WIDTH - 2, SCREEN_HEIGHT - 2);

            _selectionScreen = new SelectionScreen(this, screenFrame);
            _splashScreen = new SplashScreen(this, screenFrame);
            _newScreen = new NewScreen(this, screenFrame);
            _loadScreen = new LoadScreen(this, screenFrame);

            _currentScreen = _splashScreen;
            _currentScreen.CatchFocus();
            _currentScreen.Draw();
            _keyListener = new ConsoleKeyListener(this);
        }

        public void SetScreen(ScreenEnum se)
        {
            switch (se)
            {
                case ScreenEnum.Splash:
                    _currentScreen = _splashScreen;
                    break;
                case ScreenEnum.Select:
                    _currentScreen = _selectionScreen;
                    break;
                case ScreenEnum.New:
                    _currentScreen = _newScreen;
                    break;
                case ScreenEnum.Load:
                    _currentScreen = _loadScreen;
                    break;
                default:
                    _currentScreen = _splashScreen;
                    break;
            }

            _currentScreen.CatchFocus();
            _currentScreen.Draw();
        }

        public void SetController(Controller ctrl)
        {
            _ctrl = ctrl;
        }

        public void DisplaySelectionScreen(string action)
        {
            _selectionScreen.SetNextScreen(action);
            if (action == "new_game")
                _selectionScreen.LoadDifficulties(_ctrl.GetDifficulties());
            SetScreen(ScreenEnum.Select);
        }

        public void DisplayNewScreen(string difficulty)
        {
            _newScreen.SetData(_ctrl.GetClasses(), _ctrl.AddPlayer(), difficulty);
            SetScreen(ScreenEnum.New);
        }

        public void DisplayLoadScreen()
        {
            List<string> saves = new List<string>();
            saves.Add("test");
            saves.Add("beta");
            _loadScreen.SetData(saves);
            SetScreen(ScreenEnum.Load);
        }

        public void PrintPlayerProfile(Character player)
        {
            Console.Clear();
            Console.WriteLine("\n [ Profil ]");
            Console.WriteLine(" Nom: " + player.Name);
            Console.WriteLine(" Expérience: " + player.Experience);
            Console.WriteLine(" Classe: " + player.Class.Name);
            Console.WriteLine(" Argent: " + player.Money);
            Console.WriteLine(" Caractéristiques: ");
            foreach (Trait t in player.Traits)
                Console.WriteLine(string.Format("  {0}: {1}", t.TraitID, (int)t.Value));
            Console.WriteLine(" Compétences: ");
            foreach (Skill s in player.Skills)
                Console.WriteLine(string.Format("  {0}", s.Name));

            Console.Write(" \n Appuyez sur [Entr] pour continuer.");
            Console.ReadLine();
        }

        private string DisplayChoice(Alternative a, int index)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("    {0}. {1}", index, a.Text));

            TraitAlternative ta = a as TraitAlternative;
            if (ta != null)
                builder.Append(string.Format(" ({0}: {1})", ta.TraitID, ta.Goal));

            return builder.ToString();
        }

        public Alternative DisplaySituation(Situation s)
        {
            int input;
            do{
                Console.Clear();

                Console.WriteLine("\n [ " + s.Text + " ]\n");

                if (s.Children.Count == 0)
                    return null;

                int i;
                for (i = 0; i < s.Children.Count; i++)
                    Console.WriteLine(DisplayChoice(s.Children[i], i));

                Console.WriteLine(string.Format("\n    {0}. Consulter son profil", i++));
                Console.WriteLine(string.Format("    {0}. Sauvegarder", i));

                Console.Write("\n Que faites vous ? ");
                input = int.Parse(Console.ReadLine());

                if (input == s.Children.Count)
                {
                    PrintPlayerProfile(_ctrl.GetPlayer());
                }
                else if (input == s.Children.Count + 1)
                {
                    Console.Clear();
                    Console.WriteLine(" \n Sauvegarde en cours...");
                    _ctrl.Save();
                    Console.WriteLine(" Sauvegarde terminée.");

                    Console.Write(" \n Appuyez sur [Entr] pour continuer.");
                    Console.ReadLine();
                }
            }while(input < 0 || input >= s.Children.Count);

            return s.Children[input];
        }

        public void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Tab:
                    _currentScreen.CatchFocus();
                    _currentScreen.Draw();
                    break;
                default:
                    _currentScreen.OnKeyPressed(keyInfo);
                    break;
            }
        }

    }
}
