using System;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// MeetingUpdateItem structure
    /// </summary>
    [Serializable]
    [XmlRoot("MeetingUpdateItem")]
    public class MeetingUpdateItem : XmlDateTimeBase
    {
        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlAttribute("folder-id")]
        public string FolderId;

        [XmlElement("name")]
        public string Name;

        [XmlElement("description")]
        public string Description;

        [XmlElement("lang")]
        public string Language;

        [XmlElement("sco-tag")]
        public string ScoTag;

        [XmlElement]
        public string Email;

        [XmlElement("first-name")]
        public string FirstName;

        [XmlElement("last-name")]
        public string LastName;

        [XmlElement("url-path")]
        public string UrlPath;

        [XmlElement("type")]
        public SCOtype MeetingItemType = SCOtype.NotSet;
    }
}
