using System;
using System.IO;
using System.Net;
using System.Text;

namespace AdobeConnectSDK.Common
{
    internal class HttpUtilsInternal
    {
        /// <summary>
        /// Retrieves remote http contents
        /// </summary>
        /// <param Name="url">Url to the source</param>
        /// <param Name="accessCredentials">optional Credentials</param>
        /// <param Name="proxyUrl">option proxy Url</param>
        /// <returns></returns>
        public static string HttpGetContents(string url, NetworkCredential accessCredentials, string proxyUrl)
        {
            if (!(WebRequest.Create(url) is HttpWebRequest HttpWReq)) return null;

            if (accessCredentials != null)
            {
                HttpWReq.Credentials = accessCredentials;
            }
            else
            {
                HttpWReq.Credentials = CredentialCache.DefaultCredentials;
            }

            if (!string.IsNullOrEmpty(proxyUrl))
            {
                HttpWReq.Proxy = new WebProxy(proxyUrl, true);

                if (accessCredentials != null)
                {
                    HttpWReq.Proxy.Credentials = accessCredentials;
                }
                else
                {
                    HttpWReq.Proxy.Credentials = CredentialCache.DefaultCredentials;
                }
            }

            HttpWReq.Timeout = 20000 * 60;
            HttpWReq.Accept = "*/*";
            HttpWReq.KeepAlive = false;
            HttpWReq.CookieContainer = new CookieContainer();

            HttpWebResponse HttpWResp = null;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate
                {
                    return true;
                };

                HttpWResp = HttpWReq.GetResponse() as HttpWebResponse;
                Stream receiveStream = HttpWResp.GetResponseStream();
                if (receiveStream == null) return null;
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (HttpWResp != null)
                    HttpWResp.Close();
            }

        }

        public static string UrlEncode(string str)
        {
            return UrlEncode(str, Encoding.UTF8);
        }

        public static string UrlEncode(string s, Encoding Enc)
        {
            if (s == null)
                return null;

            if (s == string.Empty)
                return string.Empty;

            byte[] bytes = Enc.GetBytes(s);
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, 0, bytes.Length));
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            return UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;

            if (str == string.Empty)
                return new byte[0];

            byte[] bytes = e.GetBytes(str);
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return new byte[0];

            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;
            if (len == 0)
                return new byte[0];

            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || count > len - offset)
                throw new ArgumentOutOfRangeException("count");

            MemoryStream result = new MemoryStream(count);
            int end = offset + count;
            for (int i = offset; i < end; i++)
                UrlEncodeChar((char)bytes[i], result, false);

            return result.ToArray();
        }

        public static string UrlEncodeUnicode(string str)
        {
            if (str == null)
                return null;

            return Encoding.ASCII.GetString(UrlEncodeUnicodeToBytes(str));
        }

        static void UrlEncodeChar(char c, Stream result, bool isUnicode)
        {
            char[] hexChars = "0123456789abcdef".ToCharArray();
            string notEncoded = "!'()*-._";

            if (c > 255)
            {
                int idx;
                int i = (int)c;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');
                idx = i >> 12;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                return;
            }

            if (c > ' ' && notEncoded.IndexOf(c) != -1)
            {
                result.WriteByte((byte)c);
                return;
            }
            if (c == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }
            if ((c < '0') ||
                (c < 'A' && c > '9') ||
                (c > 'Z' && c < 'a') ||
                (c > 'z'))
            {
                if (isUnicode && c > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                    result.WriteByte((byte)'%');

                int idx = ((int)c) >> 4;
                result.WriteByte((byte)hexChars[idx]);
                idx = ((int)c) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
                result.WriteByte((byte)c);
        }

        static byte[] UrlEncodeUnicodeToBytes(string str)
        {
            if (str == null)
                return null;

            if (str == string.Empty)
                return new byte[0];

            MemoryStream result = new MemoryStream(str.Length);
            foreach (char c in str)
            {
                UrlEncodeChar(c, result, true);
            }
            return result.ToArray();
        }
    }
}
