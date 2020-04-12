using System.IO;
using System.Text;
using System.Xml;

namespace AdobeConnectSDK.Common
{
    internal class XmlFragmentWriter : XmlTextWriter
    {
        public XmlFragmentWriter(TextWriter writer) : base(writer) { }
        public XmlFragmentWriter(Stream writer, Encoding encoding) : base(writer, encoding) { }
        public XmlFragmentWriter(string fileName, Encoding encoding) :
            base(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None), encoding)
        { }

        bool _skip = false;

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            //omit namespaces
            if (prefix == "xmlns" && (localName == "xsd" || localName == "xsi"))
            {
                _skip = true;
                return;
            }
            base.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteString(string text)
        {
            if (_skip) return;
            base.WriteString(text);
        }

        public override void WriteEndAttribute()
        {
            if (_skip)
            {
                _skip = false;
                return;
            }
            base.WriteEndAttribute();
        }

        public override void WriteStartDocument()
        {
            //omit xml declaration
        }

    }
}
