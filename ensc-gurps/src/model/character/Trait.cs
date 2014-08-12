using System.Xml.Serialization;

namespace ensc_gurps
{
    public class Trait
    {
        [XmlAttribute("id")]
        public string TraitID { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public int Cost { get; set; }

        public Trait() {}

        public Trait(string id, string name, int value, int cost)
        {
            TraitID = id;
            Name = name;
            Value = value;
            Cost = cost;
        }
    }
}
