using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace AdobeConnectSDK.Common
{
    internal static class XmlSerializerHelpersGeneric
    {
        static readonly object locker = new object();

        public static T FromXML<T>(XmlReader docReader, XmlRootAttribute xmlRootAttribute)
        {
            Type underlingType = typeof(T);

            XmlAttributes attrs = new XmlAttributes
            {
                XmlRoot = xmlRootAttribute
            };

            var xmlAttributeOverrides = new XmlAttributeOverrides();
            xmlAttributeOverrides.Add(underlingType, attrs);

            XmlSerializer serializer = GetSerializerInstance(underlingType, xmlAttributeOverrides);
            if (serializer == null)
                return default(T);

            return (T)serializer.Deserialize(docReader);
        }


        public static T FromXML<T>(XmlReader docReader, XmlAttributeOverrides xmlAttributeOverrides)
        {
            Type UnderlingType = typeof(T);
            XmlSerializer serializer = GetSerializerInstance(UnderlingType, xmlAttributeOverrides);
            if (serializer == null)
                return default(T);

            return (T)serializer.Deserialize(docReader);
        }

        public static T FromXML<T>(XmlReader docReader)
        {
            Type UnderlingType = typeof(T);
            XmlSerializer serializer = GetSerializerInstance(UnderlingType);
            if (serializer == null)
                return default(T);

            return (T)serializer.Deserialize(docReader);
        }

        public static T FromXML<T>(string xmlSource)
        {
            Type UnderlingType = typeof(T);
            XmlSerializer serializer = GetSerializerInstance(UnderlingType);
            if (serializer == null)
                return default(T);

            using (XmlTextReader reader = new XmlTextReader(xmlSource, XmlNodeType.Document, null))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T FromXmlImplicit<T>(Type underlingType, string xmlSource)
        {
            XmlSerializer serializer = GetSerializerInstance(underlingType);
            if (serializer == null)
                return default(T);

            using (XmlTextReader reader = new XmlTextReader(xmlSource, XmlNodeType.Document, null))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static object FromXML(Type underlingType, string xmlSource)
        {
            XmlSerializer serializer = GetSerializerInstance(underlingType);
            if (serializer == null)
                return null;

            using (XmlTextReader reader = new XmlTextReader(xmlSource, XmlNodeType.Document, null))
            {
                return serializer.Deserialize(reader);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        public static string ToXML(object sourceDataObject)
        {
            if (sourceDataObject == null)
                throw new ArgumentNullException("sourceDataObject");
            return ToXML(sourceDataObject, true, true);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        public static string ToXML(object sourceDataObject, bool omitXmlDeclaration, bool omitXmlNamespaces)
        {
            if (sourceDataObject == null)
                throw new ArgumentNullException("sourceDataObject");
            if (!sourceDataObject.GetType().IsSerializable)
                throw new ArgumentException("The specified object cannot be serialized.", "sourceDataObject");

            Type srcType = sourceDataObject.GetType();
            bool isIList = sourceDataObject is IList;
            if (isIList && srcType.IsGenericType)
            {
                if ((isIList && srcType.IsGenericType) && srcType.GetGenericArguments().Length > 0)
                {
                    srcType = srcType.GetGenericArguments()[0];
                }
                else
                    throw new ArgumentException("Empty list cannot be serialized.", "sourceDataObject");

            }

            XmlSerializer serializer = GetSerializerInstance(srcType);
            if (serializer == null)
                throw new ArgumentException("Specified Type not initialized.", "sourceDataObject");

            StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            XmlWriter xmlWriter = null;
            if (omitXmlDeclaration)
            {
                xmlWriter = new XmlFragmentWriter(stringWriter);
            }
            else
            {
                xmlWriter = XmlWriter.Create(stringWriter);
                if (omitXmlNamespaces)
                    ns.Add("", "");
            }

            try
            {
                if (sourceDataObject.GetType().GetGenericArguments().Length > 0)
                {
                    int listCount = (int)sourceDataObject.GetType().GetProperty("Count").GetValue(sourceDataObject, null);
                    for (int index = 0; index < listCount; index++)
                    {
                        object item = sourceDataObject.GetType().GetMethod("get_Item").Invoke(sourceDataObject, new object[] { index });
                        serializer.Serialize(xmlWriter, item, ns);
                    }
                }
                else
                    serializer.Serialize(xmlWriter, sourceDataObject, ns);
            }
            catch { throw; }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
                xmlWriter = null;
            }

            stringWriter.Flush();
            return stringWriter.ToString();
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        public static IXPathNavigable ToXMLDocument(object sourceDataObject, bool omitXmlNamespaces)
        {
            if (sourceDataObject == null)
                throw new ArgumentNullException("sourceDataObject");
            if (!sourceDataObject.GetType().IsSerializable)
                throw new ArgumentException("The specified object cannot be serialized.", "sourceDataObject");

            Type srcType = sourceDataObject.GetType();
            bool isIList = sourceDataObject is IList;
            if (isIList && srcType.IsGenericType)
            {
                if ((isIList && srcType.IsGenericType) && srcType.GetGenericArguments().Length > 0)
                {
                    srcType = srcType.GetGenericArguments()[0];
                }
                else
                    throw new ArgumentException("Empty list cannot be serialized.", "sourceDataObject");
            }

            XmlSerializer serializer = GetSerializerInstance(srcType);
            if (serializer == null)
                throw new ArgumentException("Specified Type not initialized.", "sourceDataObject");

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            XmlDocument xmlDoc = new XmlDocument();
            XmlWriter xmlWriter = xmlDoc.CreateNavigator().AppendChild();
            if (omitXmlNamespaces)
                ns.Add("", "");

            try
            {
                if (sourceDataObject.GetType().GetGenericArguments().Length > 0)
                {
                    int listCount = (int)sourceDataObject.GetType().GetProperty("Count").GetValue(sourceDataObject, null);
                    for (int index = 0; index < listCount; index++)
                    {
                        object item = sourceDataObject.GetType().GetMethod("get_Item").Invoke(sourceDataObject, new object[] { index });
                        serializer.Serialize(xmlWriter, item, ns);
                    }
                }
                else
                    serializer.Serialize(xmlWriter, sourceDataObject, ns);
            }
            catch { throw; }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
                xmlWriter = null;
            }

            return xmlDoc;
        }

        #region XmlSerializerFactory Type Instance caching
        private static volatile Dictionary<Tuple<Type, string>, XmlSerializer> m_serializers;

        private static XmlSerializer GetSerializerInstance(Type underlingType)
        {
            XmlSerializer cachedSerializer = null;
            lock (locker)
            {
                if (m_serializers == null)
                {
                    m_serializers = new Dictionary<Tuple<Type, string>, XmlSerializer>();
                }

                var cacheKey = Tuple.Create(underlingType, string.Empty);

                if (!m_serializers.TryGetValue(cacheKey, out cachedSerializer))
                {
                    cachedSerializer = new XmlSerializerFactory().CreateSerializer(underlingType);
                    m_serializers.Add(cacheKey, cachedSerializer);
                }
            }

            return cachedSerializer;
        }

        private static XmlSerializer GetSerializerInstance(Type underlingType, XmlAttributeOverrides xmlAttributeOverrides)
        {
            XmlSerializer cachedSerializer = null;
            lock (locker)
            {
                if (m_serializers == null)
                {
                    m_serializers = new Dictionary<Tuple<Type, string>, XmlSerializer>();
                }

                Tuple<Type, string> cacheKey = Tuple.Create(underlingType, string.Empty);

                var xmlRootObject = xmlAttributeOverrides[underlingType].XmlRoot;

                if (xmlRootObject != null)
                {
                    cacheKey = Tuple.Create(underlingType, xmlRootObject.ElementName);
                }

                if (!m_serializers.TryGetValue(cacheKey, out cachedSerializer))
                {
                    cachedSerializer = new XmlSerializerFactory().CreateSerializer(underlingType, xmlAttributeOverrides);
                    m_serializers.Add(cacheKey, cachedSerializer);
                }
            }

            return cachedSerializer;
        }


        #endregion
    }
}
