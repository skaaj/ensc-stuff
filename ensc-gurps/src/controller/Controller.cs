using ensc_gurps.model;
using ensc_gurps.model.adventure;
using ensc_gurps.model.character;
using ensc_gurps.utils;
using ensc_gurps.view.console;
using System;
using System.Collections.Generic;

namespace ensc_gurps.controller
{
    public class Controller
    {
        private Model _model;
        private ConsoleView  _view;

        private PlayerController _player;

        private string _universeName;
        private string _adventureName;
        private Dictionary<string, int> _difficulty;

        private Situation node;

        public Controller(Model model, ConsoleView view)
        {
            this._model = model;
            this._view = view;
            view.SetController(this);

            _difficulty = new Dictionary<string,int>();
            _difficulty.Add("Easy", 300);
            _difficulty.Add("Hard", 100);
        }

        public List<Class> GetClasses(){
            return _model.Classes;
        }

        public ClassInfluenceList GetClassesInfluence(){
            return _model.ClassesInfluence;
        }

        public Character AddPlayer()
        {
            return _model.AddPlayer();
        }

        public Character GetPlayer()
        {
            return _player.Player;
        }

        public Dictionary<string,int> GetDifficulties(){
            return _difficulty;
        }

        public void StartGameLoop(Character p)
        {
            if (_player == null)
            {
                _player = new PlayerController(p, _model.ClassesInfluence[p.Class.Name]);
                foreach(Skill s in _model.Skills)
                    _player.TryToLearn(s);

            }

            bool exit = false;

            if(node == null)
                node = _model.Adventure[0];
            Situation root = _model.Adventure[0];

            while (!exit)
            {

                Alternative a = _view.DisplaySituation(node);

                if (a != null)
                {
                    if (a is TraitAlternative)
                    {
                        TraitAlternative ta = a as TraitAlternative;
                        int rand = new Random().Next(10);

                        Console.Clear();
                        Console.WriteLine("\n Le jet de réussite fait : " + rand);

                        if (_player.ChallengeTrait(rand, ta.Goal, ta.TraitID))
                        {
                            Console.WriteLine(" C'est un succés !");
                            node = a.Success;
                        }
                        else
                        {
                            Console.WriteLine(" C'est un échec !");
                            node = a.Fail;
                        }
                    }
                    else if (a is NPCAlternative)
                    {
                        // lancer combat, pour l'instant c'est un rand
                        NPCAlternative npca = a as NPCAlternative;
                        NPC npce = _model.GetNPC(npca.NPCID);
                        PlayerController ennemy = new PlayerController(npce, _model.ClassesInfluence[npce.Class.Name]);

                        if (ennemy != null)
                        {
                            Console.Clear();
                            Console.WriteLine(" Vous affrontez " + npce.Name);

                            int turn = 0;

                            while (_player.IsAlive() && ennemy.IsAlive())
                            {
                                if (turn == 0)
                                {
                                    _player.FightAgainst(ennemy);
                                }
                                else
                                {
                                    ennemy.FightAgainst(_player);
                                }

                                Console.WriteLine("\n -> Fin du tour : ");
                                Console.WriteLine(" " + _player.Player.Name + " : " + _player.GetHP() + " points de vie.");
                                Console.WriteLine(" " + ennemy.Player.Name + " : " + ennemy.GetHP() + " points de vie.");
                                Console.Write("\n Appuyez sur une touche pour passer au tour suivant. ");
                                Console.ReadLine();
                                Console.Clear();

                                turn = (turn + 1) % 2;
                            }

                            if (_player.IsAlive())
                            {
                                Console.WriteLine(" Fin du combat, vous avez gagné !");
                                _player.LevelUp();
                                node = a.Success;
                            }
                            else
                            {
                                Console.WriteLine(" Fin du combat, vous avez perdu !");
                                node = root;
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(" En vous voyant votre ennemi a fui.");
                        }
                    }
                    else // narrative
                    {
                        node = a.Success;
                    }

                    Console.WriteLine(" Appuyez sur une touche pour continuer.");
                    Console.ReadLine();
                }
                else
                {
                    exit = true;
                    //node = root; // looping (debug)
                }
            }
        }

        public void Save()
        {
            _player.Player.CurrentNode = node.SituationID;
            string formattedName = _player.Player.Name.ToLower().Substring(0, _player.Player.Name.Length - 1);
            XMLUtil.Serialize(_player.Player, PathUtil.GetSavegamePath(_player.Player.CharacterID + "_" + formattedName));
        }

        public void Run()
        {
            _view.InitView();

            //OnUniverseChosen("wow");
            //OnAdventureChosen("debug");
            //_model.LoadWorld();
            //StartGameLoop(Debug_LoadPlayer());

            //StartGameLoop(Debug_CreatePlayer());

            /*
            _view.DisplayUniverseSelection(PathUtil.GetUniverses());

            _model.LoadWorld();

            if (debug)
                Debug_CreatePlayer();
            else
            {
                Character p = _model.AddPlayer();
                _view.DisplayPlayerCreation(p, _model.Classes, _model.ClassesInfluence, _difficulty);
                Player = new PlayerController(p, _model.ClassesInfluence[p.Class.Name]);
            }
             */

            
            //Console.ReadLine(); // end of the game
        }

        private Character Debug_CreatePlayer()
        {
            Character p = _model.AddPlayer();
            p.Name = "Debug Dummy";
            p.Class = _model.Classes[0];

            _player = new PlayerController(p, _model.ClassesInfluence[p.Class.Name]);

            _player.TryToLearn(_model.Skills[0]);
            _player.TryToLearn(_model.Skills[1]);
            _player.TryToLearn(_model.Skills[2]);
            _player.TryToLearn(_model.Skills[3]);

            return p;
        }

        public Character Debug_LoadPlayer()
        {
            Character p = (Character) XMLUtil.Unserialize(typeof(Character), PathUtil.GetSavegamePath("player"));
            _model.Characters.Add(p);
            node = _model.GetNode(p.CurrentNode);
            _player = new PlayerController(p, _model.ClassesInfluence[p.Class.Name]);

            return p;
        }

        public void Load(string savegame)
        {
            Character p = (Character)XMLUtil.Unserialize(typeof(Character), PathUtil.GetSavegamePath(savegame));
            _model.Characters.Add(p);
            node = _model.GetNode(p.CurrentNode);
            _player = new PlayerController(p, _model.ClassesInfluence[p.Class.Name]);
        }

        public void OnDifficultyChosen(string difficultyKey)
        {
            _model.Characters[_model.Characters.Count - 1].Points = _difficulty[difficultyKey];
        }

        public void OnUniverseChosen(string universe)
        {
            _universeName = universe;
            PathUtil.SetUniverse(_universeName);
        }

        public void OnAdventureChosen(string adventure)
        {
            _adventureName = adventure;
            PathUtil.SetAdventure(_adventureName);
            _model.LoadWorld();
        }
    }
}
