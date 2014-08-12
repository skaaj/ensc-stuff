using System.Collections.Generic;
using System.Text;

namespace ensc_gurps.model.character
{
    public class ClassInfluenceList : List<ClassInfluence>
    {
        public ClassInfluence this[string ClassID]
        {
            get
            {
                int i = GetIndex(ClassID);
                return (i == -1) ? null : this[i];
            }
        }

        private int GetIndex(string ClassID)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].ClassID == ClassID)
                    return i;

            return -1;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (ClassInfluence influence in this)
                builder.Append(influence + "\n");

            return builder.ToString();
        }
    }
}
