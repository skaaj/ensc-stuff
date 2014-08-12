using ensc_gurps.model.adventure;
using ensc_gurps.model.character;
using System.Collections.Generic;

namespace ensc_gurps.view
{
    public abstract class View
    {
        protected string _title;

        public View(string title)
        {
            _title = title;
        }

        public abstract void DisplayPlayerCreation(Character player, List<Class> characterClasses, ClassInfluenceList classesInfluence, Dictionary<string, int> difficulty);
        public abstract string ChooseUniverseScreen(List<string> universes);
        public abstract string ChooseAdventureScreen(List<string> adventures);
        public abstract void PrintPlayerProfile(Character player);
        public abstract Alternative DisplaySituation(Situation s);

        public abstract void Main();
    }
}
