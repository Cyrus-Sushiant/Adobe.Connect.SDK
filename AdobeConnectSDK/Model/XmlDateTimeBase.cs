using AdobeConnectSDK.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// Shared serialization properties.
    /// </summary>
    [Serializable]
    public class XmlDateTimeBase
    {
        [XmlIgnore]
        public DateTime DateBegin { get; set; }

        [XmlElement(ElementName = "date-begin")]
        internal string DateBeginRaw
        {
            get { return this.DateBegin.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
            set { this.DateBegin = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
        }

        [XmlIgnore]
        public DateTime DateEnd { get; set; }

        [XmlElement(ElementName = "date-end")]
        internal string DateEndRaw
        {
            get { return this.DateEnd.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
            set { this.DateEnd = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
        }

        [XmlIgnore]
        public DateTime DateModified { get; set; }

        [XmlElement(ElementName = "date-modified")]
        internal string DateModifiedRaw
        {
            get { return this.DateModified.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
            set { this.DateModified = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
        }

        [XmlIgnore]
        public DateTime DateCreated { get; set; }

        [XmlElement(ElementName = "date-created")]
        internal string DateCreatedRaw
        {
            get { return this.DateCreated.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
            set { this.DateCreated = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
        }

        [XmlIgnore]
        public DateTime DateClosed { get; set; }

        [XmlElement(ElementName = "date-closed")]
        internal string DateClosedRaw
        {
            get { return this.DateClosed.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
            set { this.DateClosed = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
        }
    }
}
