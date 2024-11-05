using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Kev.Xrm.Utilities
{
    public class SerializerHelper
    {
        public static T ReadObject<T>(Stream content)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            });

            return (T)serializer.ReadObject(content);
        }

        public static string WriteObject<T>(T content)
        {
            var serializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            });

            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, content);

            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}