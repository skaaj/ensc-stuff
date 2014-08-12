using ensc_gurps.model.adventure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ensc_gurps.model.adventure
{
    [XmlInclude(typeof(TraitAlternative))]
    [XmlInclude(typeof(NPCAlternative))]
    public class Alternative
    {
        public string AlternativeID { get; set; }
        public string Text { get; set; }

        [XmlIgnore]
        public Situation Success { get; set; }
        public string SuccessID { get; set; }

        [XmlIgnore]
        public Situation Fail { get; set; }
        public string FailID { get; set; }
    }
}
