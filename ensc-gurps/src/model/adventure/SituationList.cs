using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.model.adventure
{
    public class SituationList : List<Situation>
    {
        public Situation this[string SituationID]
        {
            get
            {
                int i = GetIndex(SituationID);
                return (i == -1) ? null : this[i];
            }
        }

        private int GetIndex(string SituationID)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].SituationID == SituationID)
                    return i;

            return -1;
        }
    }
}
