using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// Recording information.
    /// </summary>
    [Serializable]
    [XmlRoot("recording")]
    public class RecordingItem : XmlDateTimeBase
    {
        [XmlElement("name")]
        public string MeetingName;

        [XmlElement("url-path")]
        public string UrlPath;

        [XmlElement("is-seminar")]
        public bool IsSeminar;
    }
}
