using System;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// Meeting structure
    /// </summary>
    [Serializable]
    [XmlRoot("sco")]
    public class MeetingDetail : XmlDateTimeBase
    {
        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlAttribute("account-id")]
        public string AccountId;

        [XmlAttribute("folder-id")]
        public string FolderId;

        [XmlAttribute("lang")]
        public string Language;

        [XmlElement("name")]
        public string Name;

        [XmlElement("description")]
        public string Description;

        [XmlElement("url-path")]
        public string UrlPath;

        [NonSerialized]
        public string FullUrl;

        [XmlElement("passing-score")]
        public int PassingScore;

        /// <summary>
        /// The length of time needed to View or play the SCO, in milliseconds.
        /// </summary>
        [XmlElement("duration")]
        public int Duration;

        [XmlElement("section-count")]
        public int SectionCount;
    }
}
