using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ensc_gurps.model.adventure
{
    public class AlternativeList : List<Alternative>
    {
        public Alternative this[string ChoiceID]
        {
            get
            {
                int i = GetIndex(ChoiceID);
                return (i == -1) ? null : this[i];
            }
        }

        private int GetIndex(string ChoiceID)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].AlternativeID == ChoiceID)
                    return i;

            return -1;
        }
    }
}
