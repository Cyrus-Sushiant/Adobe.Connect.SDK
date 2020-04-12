using System;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// UserInfo structure
    /// </summary>
    [Serializable]
    [XmlRoot("user")]
    public class UserInfo
    {
        [XmlAttribute("user-id")]
        public string UserId;

        [XmlElement("name")]
        public string Name;

        [XmlElement("login")]
        public string Login;
    }
}
