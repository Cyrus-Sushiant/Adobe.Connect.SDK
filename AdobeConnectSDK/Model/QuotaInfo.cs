using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// Quota information.
    /// </summary>
    [Serializable]
    [XmlRoot("report-quotas")]
    public class QuotaInfo
    {
        [XmlElement("quota")]
        public List<Quota> QuotaList { get; set; }
    }

    [Serializable]
    public class Quota : XmlDateTimeBase
    {
        [XmlAttribute("acl-id")]
        public long AclId { get; set; }

        [XmlAttribute("quota-id")]
        public string QuotaId { get; set; }

        [XmlAttribute("used")]
        public long Used { get; set; }

        [XmlIgnore]
        public long Limit { get; set; }

        [XmlAttribute("limit")]
        internal string LimitRaw
        {
            get { return this.Limit.ToString(CultureInfo.InvariantCulture); }
            set { this.Limit = (value == "unlimited") ? 0 : long.Parse(value, CultureInfo.InvariantCulture); }
        }

        [XmlAttribute("soft-limit")]
        public long SoftLimit { get; set; }
    }
}
