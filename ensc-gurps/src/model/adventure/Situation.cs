using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ensc_gurps.model.adventure
{
    public class Situation
    {
        public string SituationID { get; set; }
        public string Text { get; set; }

        [XmlIgnore]
        public List<Alternative> Children { get; set; }
        public List<string> ChildrenIDs { get; set; }

        public Situation()
        {
            Children = new List<Alternative>();
            ChildrenIDs = new List<string>();
        }
    }
}
