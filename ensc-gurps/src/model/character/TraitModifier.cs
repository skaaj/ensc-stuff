using System.Xml.Serialization;

namespace ensc_gurps.model.character
{
    public class TraitModifier
    {
        [XmlAttribute("id")]
        public string TraitID { get; set; }
        public int StartingBonus { get; set; }
        public float Factor { get; set; }

        public TraitModifier() {}

        public TraitModifier(string traitID, int startingBonus, float factor)
        {
            TraitID = traitID;
            StartingBonus = startingBonus;
            Factor = factor;
        }
    }
}
