using ensc_gurps.model.adventure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ensc_gurps.model.adventure
{
    public class TraitAlternative : Alternative
    {
        public string TraitID { get; set; }
        public int Goal { get; set; }
    }
}
