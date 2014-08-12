using ensc_gurps.model.adventure;
using ensc_gurps.model.item;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ensc_gurps.model.character
{
    [XmlInclude(typeof(NPC))]
    public class Character
    {
        public string CharacterID { get; set; }
        public string Name { get; set; }
        public float Money { get; set; }
        public float Experience { get; set; }
        public int Points { get; set; }
        public string CurrentNode { get; set; }
        public Class Class { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Status> Status { get; set; }
        public List<Item> Inventory { get; set; }

        [XmlIgnore]
        public string ClassID
        {
            get { return Class.Name; }
        }

        public Character()
        {
            CharacterID = "unknown";
            Name = CharacterID;
            Money = 0.0f;
            Experience = 0.0f;
            Points = 0;

            Class = new Class();
            Traits = new List<Trait>();
            Skills = new List<Skill>();
            Status = new List<Status>();
            Inventory = new List<Item>();

            Status.Add(new Status("HP", 100));
        }

        public Trait GetTrait(string id)
        {
            foreach (Trait t in Traits)
                if (t.TraitID == id)
                    return t;

            return null;
        }

        public Character(string name, Class characterClass) : this()
        {
            Name = name;
            Class = characterClass;
        }

        public void Learn(Skill s)
        {
            Skills.Add(s);
        }

        public float GetStrike()
        {
            if (Traits.Count == 0)
                return 5.0f + (this.Experience / 5.0f);

            float sum = 0.0f;
            foreach (Trait t in Traits)
                sum += t.Value;

            return sum / Traits.Count;
        }
    }
}
