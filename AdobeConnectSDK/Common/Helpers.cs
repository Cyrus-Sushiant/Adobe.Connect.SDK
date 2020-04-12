using AdobeConnectSDK.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Common
{
    internal static class Helpers
    {
        public static ApiStatus WrapStatusException(StatusCodes code, StatusSubCodes subCode, Exception exInfo)
        {
            return new ApiStatus
            {
                Code = code,
                SubCode = subCode,
                InnerException = exInfo
            };
        }

        public static ApiStatus ResolveOperationStatusFlags(XmlReader reader)
        {
            ApiStatus operationApiStatus = new ApiStatus();

            XDocument doc = XDocument.Load(reader);

            var statusNode = doc.Descendants("status").FirstOrDefault();

            if (statusNode == null)
            {
                return null;
            }

            operationApiStatus.Code = Helpers.ReflectEnum<StatusCodes>(statusNode.Attribute("code").Value);

            var subStatusNode = statusNode.Descendants("invalid").FirstOrDefault();

            if (subStatusNode != null)
            {
                operationApiStatus.SubCode = Helpers.ReflectEnum<StatusSubCodes>(subStatusNode.Attribute("subcode").Value);
                operationApiStatus.InvalidField = subStatusNode.Attribute("field")?.Value;
            }

            var exceptionNode = statusNode.Descendants("exception").FirstOrDefault();

            if (exceptionNode != null)
            {
                operationApiStatus.Exception = exceptionNode.Value;
            }

            if (statusNode.ElementsAfterSelf().Any())
            {
                var resultDoc = new XDocument();
                resultDoc.Add(new XElement("resultroot", statusNode.ElementsAfterSelf()));
                operationApiStatus.ResultDocument = resultDoc;
            }

            return operationApiStatus;
        }

        public static string EnumToString(Enum en)
        {
            try
            {
                var memInfo = en.GetType().GetMember(en.ToString());

                if (memInfo.Length < 1)
                    return en.ToString();

                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : en.ToString().ToLowerInvariant();
            }
            catch
            {
                throw;
            }
        }

        public static Enum ReflectEnum(Type PrimaryType, string EnumField)
        {
            try
            {
                EnumField = EnumField.Replace('-', '_');
                return (Enum)Enum.Parse(PrimaryType, EnumField, true);
            }
            catch
            {
                throw;
            }
        }

        public static T ReflectEnum<T>(string enumField)
        {
            if (string.IsNullOrWhiteSpace(enumField))
                return default(T);

            try
            {
                enumField = enumField.Replace("-", string.Empty);
                return (T)Enum.Parse(typeof(T), enumField, true);
            }
            catch
            {
                throw;
            }
        }

        public static string StructToQueryString(object pSetup, bool XmlElementAttributeOverride)
        {
            if (pSetup == null)
                return null;

            StringBuilder cmdParams = new StringBuilder();
            foreach (FieldInfo fi in pSetup.GetType().GetFields())
            {
                if (!fi.IsPublic)
                    continue;

                object _fieldValue = fi.GetValue(pSetup);
                if (_fieldValue == null)
                    continue;
                if (_fieldValue.GetType().Equals(typeof(bool)))
                    _fieldValue = (_fieldValue.Equals(true)) ? 1 : 0;
                else
                  if (_fieldValue.GetType().Equals(typeof(DateTime)))
                {
                    if (_fieldValue.Equals(DateTime.MinValue))
                        continue;
                    _fieldValue = ((DateTime)_fieldValue).ToString(Constants.DateFormatString);
                }
                else
                    if (_fieldValue.GetType().Equals(typeof(TimeSpan)))
                {
                    if (_fieldValue.Equals(TimeSpan.Zero))
                        continue;
                    _fieldValue = ((TimeSpan)_fieldValue).TotalMinutes;
                }
                else
                      if (_fieldValue.GetType().BaseType.Equals(typeof(Enum)))
                {
                    _fieldValue = EnumToString((Enum)_fieldValue);
                }


                string _filedName = fi.Name.Replace('_', '-').ToLower();

                if (XmlElementAttributeOverride)
                {
                    if (fi.GetCustomAttributes(typeof(XmlElementAttribute), false) is XmlElementAttribute[] xmlElementAttributes && xmlElementAttributes.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(xmlElementAttributes[0].ElementName))
                            _filedName = xmlElementAttributes[0].ElementName;
                    }

                    if (fi.GetCustomAttributes(typeof(XmlAttributeAttribute), false) is XmlAttributeAttribute[] xmlAttributes && xmlAttributes.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(xmlAttributes[0].AttributeName))
                            _filedName = xmlAttributes[0].AttributeName;
                    }
                }

                cmdParams.AppendFormat("&{0}={1}", _filedName, HttpUtilsInternal.UrlEncode(_fieldValue.ToString()));
            }
            return cmdParams.ToString();
        }

        public static void CopySharedProperties(object sourceObject, object targetObject)
        {
            var properties = sourceObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var propertyInfo = targetObject.GetType().GetProperty(prop.Name);

                if (propertyInfo == null)
                {
                    continue;
                }

                var value = prop.GetValue(sourceObject);
                propertyInfo.SetValue(targetObject, value, null);
            }
        }

        public static T WrapBaseStatusInfo<T>(BaseStatusInfo source) where T : BaseStatusInfo
        {
            var target = Activator.CreateInstance(typeof(T));

            CopySharedProperties(source, target);

            return target as T;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
                yield return cur;

            yield return value;
        }
    }
}
