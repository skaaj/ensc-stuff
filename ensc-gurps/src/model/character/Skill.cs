using System.Xml.Serialization;

namespace ensc_gurps
{
    public class Skill
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("strike")]
        public int StrikeValue { get; set; }
        [XmlAttribute("heal")]
        public int HealValue { get; set; }

        public Skill() {}

        public Skill(string name) : this()
        {
            Name = name;
        }
    }
}
