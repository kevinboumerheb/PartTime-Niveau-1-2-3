using System.Runtime.Serialization;

namespace Kev.Xrm.Plugins.AppCode
{
    public class Feature
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "feature")]
        public string Name { get; set; }
    }

    [DataContract]
    public class PluginFeature
    {
        [DataMember]
        public Feature[] Features { get; set; }
    }
}