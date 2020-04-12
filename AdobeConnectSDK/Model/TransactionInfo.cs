using AdobeConnectSDK.Common;
using System;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// TransactionInfo structure
    /// </summary>
    [Serializable]
    [XmlRoot("row")]
    public class TransactionInfo : XmlDateTimeBase
    {
        [XmlAttribute("transaction-id")]
        public string TransactionId;

        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlIgnore]
        public SCOtype ItemType;

        [XmlAttribute("type")]
        internal string ItemTypeRaw
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

        [XmlAttribute("principal-id")]
        public string PrincipalId;

        [XmlAttribute("score")]
        public string Score;

        [XmlElement("name")]
        public string Name;

        [XmlElement("url")]
        public string Url;

        [XmlElement("login")]
        public string Login;

        [XmlElement("user-name")]
        public string UserName;

        [XmlElement("status")]
        public string Status;
    }
}
