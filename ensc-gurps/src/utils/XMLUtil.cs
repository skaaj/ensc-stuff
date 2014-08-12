using ensc_gurps.model.character;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ensc_gurps.utils
{
    public class XMLUtil
    {
        static public void Serialize(object o, string file)
        {
            XmlSerializer xs = new XmlSerializer(o.GetType());
            using (StreamWriter wr = new StreamWriter(string.Format("{0}.xml", file)))
            {
                xs.Serialize(wr, o);
            }
        }

        static public object Unserialize(System.Type type, string file)
        {
            object o;

            XmlSerializer xs = new XmlSerializer(type);
            using (StreamReader rd = new StreamReader(string.Format("{0}.xml", file)))
            {
                o = xs.Deserialize(rd);
            }

            return o;
        }
    }
}
