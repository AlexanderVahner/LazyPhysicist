using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Xml
    {
        public static void ReadXmlToObject<T>(string path, ref T obj)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
            StreamReader file = new StreamReader(path);
            obj = (T)reader.Deserialize(file);
            file.Close();
        }
        public static void WriteXmlFromObject<T>(string path, T obj)
        {
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var wfile = new StreamWriter(path);
            writer.Serialize(wfile, obj);
            wfile.Close();
        }
    }
}
