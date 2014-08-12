using ensc_gurps.model.adventure;
using ensc_gurps.model.character;
using ensc_gurps.model.item;
using ensc_gurps.utils;
using System;
using System.Collections.Generic;

namespace ensc_gurps.model
{
    public class Model
    {
        // adventure
        public SituationList Adventure { get; set; }
        public AlternativeList Alternatives { get; set; }

        // world flavour
        public List<Trait> Traits { get; private set; }
        public List<Skill> Skills { get; private set; }
        public List<Class> Classes { get; private set; }
        public ClassInfluenceList ClassesInfluence { get; private set; }
        public List<Item> Items { get; set; }

        // people
        public List<Character> Characters { get; set; }

        public Model() {}

        public void LoadWorld()
        {
            Traits = (List<Trait>)XMLUtil.Unserialize(typeof(List<Trait>), PathUtil.GetDataPath("character/traits"));
            Skills = (List<Skill>)XMLUtil.Unserialize(typeof(List<Skill>), PathUtil.GetDataPath("character/skills"));
            Classes = (List<Class>)XMLUtil.Unserialize(typeof(List<Class>), PathUtil.GetDataPath("character/classes"));
            ClassesInfluence = (ClassInfluenceList) XMLUtil.Unserialize(typeof(ClassInfluenceList), PathUtil.GetDataPath("character/classes_influences"));
            Items = (List<Item>)XMLUtil.Unserialize(typeof(List<Item>), PathUtil.GetDataPath("items"));
            Characters = (List<Character>)XMLUtil.Unserialize(typeof(List<Character>), PathUtil.GetDataPath("characters"));

            Adventure = (SituationList)XMLUtil.Unserialize(typeof(SituationList), PathUtil.GetAdventurePath("situations"));
            Alternatives = (AlternativeList)XMLUtil.Unserialize(typeof(AlternativeList), PathUtil.GetAdventurePath("alternatives"));

            ConnectGraph();
        }

        private void ConnectGraph()
        {
            // situation -> choices
            foreach (Situation s in Adventure)
                foreach (string id in s.ChildrenIDs)
                    s.Children.Add(Alternatives[id]);

            // choices -> situation
            foreach (Alternative c in Alternatives)
            {
                if (c.SuccessID != null)
                    c.Success = Adventure[c.SuccessID];
                
                if (c.FailID != null)
                    c.Fail = Adventure[c.FailID];
            }
        }

        public void PrintState()
        {
            Console.WriteLine("\n List of traits:");
            foreach (var trait in Traits)
                Console.WriteLine("    " + trait.Name);

            Console.WriteLine(" List of skills:");
            foreach (var skill in Skills)
                Console.WriteLine("    " + skill.Name);

            Console.WriteLine(" List of classes:");
            foreach (var c in Classes)
                Console.WriteLine("    " + c.Name + " ( " +  c.Brief + " )");

            Console.WriteLine("\n List of class influences:\n" + ClassesInfluence);

            Console.WriteLine(" List of items:");
            foreach (var i in Items)
                Console.WriteLine("    " + i.Name);

            Console.WriteLine(" List of characters:");
            foreach (var c in Characters)
                Console.WriteLine("    " + c.CharacterID);
        }

        public Situation GetNode(string nodeID)
        {
            foreach (Situation s in Adventure)
                if (s.SituationID == nodeID)
                    return s;
            return null;
        }

        public Character AddPlayer()
        {
            Character p = new Character();
            p.CharacterID = "player";
            p.Traits = Traits;
            p.Money = 100.0f;

            Characters.Add(p);

            return p;
        }

        public NPC GetNPC(string id)
        {
            foreach (Character c in Characters)
                if (c is NPC && c.CharacterID == id)
                    return c as NPC;
            return null;
        }

        private void Debug_SerializeAll()
        {
            //// (1) skills
            //Skills = new List<Skill>();
            //Skills.Add(new Skill("Fireball"));
            //Skills.Add(new Skill("Strike"));
            //Skills.Add(new Skill("Steal"));

            //XMLUtil.Serialize(Skills, PathUtil.GetDataPath("character/skills"));

            //// (2) traits
            //Traits = new List<Trait>();
            //Traits.Add(new Trait("S", "Strength", 10, 10));
            //Traits.Add(new Trait("IQ", "Intelligence", 10, 20));
            //Traits.Add(new Trait("DX", "Dexterity", 10, 20));

            //XMLUtil.Serialize(Traits, PathUtil.GetDataPath("character/traits"));

            //// (3) classes influences
            //ClassesInfluence = new ClassInfluenceList();
            //ClassesInfluence.Add(new ClassInfluence()
            //{
            //    ClassID = "Mage",
            //    AvailableSkills = new List<string>() { "Fireball" },
            //    TraitModifiers = new List<TraitModifier>() { 
            //        new TraitModifier("IQ", 2, 1.5f)
            //    }
            //});

            //ClassesInfluence.Add(new ClassInfluence()
            //{
            //    ClassID = "Warrior",
            //    AvailableSkills = new List<string>() { "Strike" },
            //    TraitModifiers = new List<TraitModifier>() { 
            //        new TraitModifier("S", 2, 1.5f)
            //    }
            //});

            //ClassesInfluence.Add(new ClassInfluence()
            //{
            //    ClassID = "Rogue",
            //    AvailableSkills = new List<string>() { "Strike", "Steal" },
            //    TraitModifiers = new List<TraitModifier>() { 
            //        new TraitModifier("DX", 2, 1.5f),
            //        new TraitModifier("F", 1, 1.2f)
            //    }
            //});

            //XMLUtil.Serialize(ClassesInfluence, PathUtil.GetDataPath("character/classes_influences"));

            //// (4) classes
            //Classes = new List<Class>();
            //Classes.Add(new Class("Mage", "A mage"));
            //Classes.Add(new Class("Warrior", "A warrior"));
            //Classes.Add(new Class("Rogue", "A rogue"));

            //XMLUtil.Serialize(Classes, PathUtil.GetDataPath("character/classes"));

            //// (5) items
            //Items = new List<Item>();
            //Items.Add(new Item() { Name = "Deagle" });
            //Items.Add(new Item() { Name = "AK-47" });

            //XMLUtil.Serialize(Items, PathUtil.GetDataPath("items"));

            //// (5) items
            //Characters = new List<Character>();
            //Characters.Add(new NPC() { CharacterID = "ulfric", Name = "Ulfric Sombrage", ClassID = "Warrior" });
            //Characters.Add(new NPC() { CharacterID = "maven", Name = "Maven Roncenoire", ClassID = "Rogue" });

            //XMLUtil.Serialize(Characters, PathUtil.GetDataPath("characters"));

            // (5) story
            //Adventure = new SituationList();

            //Situation root = new Situation();
            //root.SituationID = "root";
            //root.Text = "Choisissez un type de noeud";

            //Situation ok = new Situation();
            //ok.SituationID = "end";
            //ok.Text = "Bravo! Fin du jeu.";

            //Situation loose = new Situation();
            //loose.SituationID = "loose";
            //loose.Text = "Dommage! Fin du jeu.";

            //root.ChildrenIDs.Add("default");
            //root.ChildrenIDs.Add("trait");
            //root.ChildrenIDs.Add("npc");

            //Adventure.Add(root);
            //Adventure.Add(ok);
            //Adventure.Add(loose);

            //Alternative def = new Alternative()
            //{
            //    AlternativeID = "default",
            //    Text = "Finir le jeu directement",
            //    SuccessID = "end"
            //};

            //TraitAlternative trait = new TraitAlternative()
            //    {
            //        AlternativeID = "trait",
            //        Text = "être malin",
            //        SuccessID = "end",
            //        FailID = "loose",
            //        TraitID = "IQ",
            //        Goal = 14
            //    };

            //NPCAlternative npc = new NPCAlternative()
            //    {
            //        AlternativeID = "npc",
            //        Text = "taper le npc",
            //        SuccessID = "end",
            //        FailID = "loose",
            //        NPCID = "ulfric"
            //    };

            //Alternatives = new AlternativeList();
            //Alternatives.Add(def);
            //Alternatives.Add(trait);
            //Alternatives.Add(npc);

            //XMLUtil.Serialize(Adventure, PathUtil.GetDataPath("adventures/debug/situations"));
            //XMLUtil.Serialize(Alternatives, PathUtil.GetDataPath("adventures/debug/alternatives"));
        }
    }
}
