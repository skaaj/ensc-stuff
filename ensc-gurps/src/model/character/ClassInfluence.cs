using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ensc_gurps.model.character
{
    public class ClassInfluence
    {
        [XmlAttribute("id")]
        public string ClassID { get; set; }

        public List<string> AvailableSkills { get; set; }

        public List<TraitModifier> TraitModifiers { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("    Class: {0}\n", ClassID));
            builder.Append("    Skills:\n");
            foreach (var skill in AvailableSkills)
            {
                builder.Append(string.Format("        {0} ({1})\n", skill, skill.GetType()));
            }
            builder.Append("    Traits modifiers:\n");
            foreach (TraitModifier tm in TraitModifiers)
            {
                builder.Append(string.Format("        {0} (starting_bonus: {1}, progression_factor: {2})\n", tm.TraitID, tm.StartingBonus, tm.Factor));
            }

            return builder.ToString();
        }
    }
}
