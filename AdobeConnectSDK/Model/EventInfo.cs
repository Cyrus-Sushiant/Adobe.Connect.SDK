using AdobeConnectSDK.Common;
using System;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// Event information 
    /// </summary>
    [Serializable]
    public class EventInfo : XmlDateTimeBase
    {
        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlAttribute("tree-id")]
        public int TreeId;

        [XmlIgnore]
        public SCOtype ItemType;

        [XmlAttribute("type")]
        public string ItemTypeRaw
        {
            get
            {
                return Helpers.EnumToString(this.ItemType);
            }
            set
            {
                this.ItemType = Helpers.ReflectEnum<SCOtype>(value);
            }
        }

        [XmlIgnore]
        public PermissionId PermissionId;

        [XmlAttribute("permission-id")]
        public string PermissionIdRaw
        {
            get { return Helpers.EnumToString(this.PermissionId); }
            set
            {
                this.PermissionId = Helpers.ReflectEnum<PermissionId>(value);
            }
        }

        [XmlElement("name")]
        public string Name;

        [XmlElement("domain-name")]
        public string DomainName;

        [XmlElement("url-path")]
        public string UrlPath;

        [XmlElement("expired")]
        public bool Expired;

        [XmlElement]
        public TimeSpan Duration;
    }
}
