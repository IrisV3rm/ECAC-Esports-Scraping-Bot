using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ECAC_eSports.Classes
{
    public class Util
    {
        public static T DeepClone<T>(T from)
        {
            using MemoryStream s = new();
            BinaryFormatter f = new();
            f.Serialize(s, from);
            s.Position = 0;
            object clone = f.Deserialize(s);

            return (T)clone;
        }
    }
}
