using AdobeConnectSDK.Interfaces;
using AdobeConnectSDK.Model;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace AdobeConnectSDK.Common
{
    public class HttpCommunicationProvider : ICommunicationProvider
    {
        private string m_SessionInfo = string.Empty;
        private string m_SessionDomain = string.Empty;

        private HttpWebRequest CreateWebRequest(string pAction, string qParams, string serviceURL)
        {
            HttpWebRequest HttpWReq = null;

            HttpWReq = WebRequest.Create(serviceURL + string.Format(@"?action={0}&{1}", pAction, qParams)) as HttpWebRequest;

            return HttpWReq;
        }

        private WebProxy SetProxyCredencials(string proxyUrl, string proxyUser, string proxyPassword, string proxyDomain)
        {
            WebProxy proxy = new WebProxy();
            try
            {
                if (!string.IsNullOrEmpty(proxyUrl))
                {
                    if (!string.IsNullOrEmpty(proxyUser) && !string.IsNullOrEmpty(proxyPassword))
                    {
                        proxy = new WebProxy(proxyUrl, true)
                        {
                            Credentials = new NetworkCredential(proxyUser, proxyPassword, proxyDomain)
                        };
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return proxy;
        }

        /// <summary>
        /// Sets the Http header fields Timeout, Accept and KeepAlive.
        /// Override this method in order to have access to the <c>HttpwebRequest</c> object before it gets send to Adobe Connection.
        /// You can override this method to further configure the proxy, cookies, etc. 
        /// </summary>
        /// <returns>
        /// <see cref="HttpWebRequest" />        
        /// </returns>        
        public virtual HttpWebRequest SetHttpConfiguration(HttpWebRequest webRequest)
        {
            webRequest.Timeout = 20000 * 60;
            webRequest.Accept = "*/*";
            webRequest.KeepAlive = false;
            return webRequest;
        }

        private CookieContainer SetCookieContainer(CookieContainer cookieContainer, bool useSessionParam)
        {
            if (!useSessionParam)
            {
                if (!string.IsNullOrEmpty(m_SessionInfo) && !string.IsNullOrEmpty(m_SessionDomain))
                    cookieContainer.Add(new Cookie("BREEZESESSION", this.m_SessionInfo, "/", this.m_SessionDomain));
            }
            return cookieContainer;
        }

        public ApiStatus ProcessRequest(string pAction, string qParams, ISdkSettings settings)
        {

            ApiStatus operationApiStatus = new ApiStatus
            {
                Code = StatusCodes.NotSet
            };

            if (qParams == null)
                qParams = string.Empty;


            HttpWebRequest HttpWReq = CreateWebRequest(pAction, qParams, settings.ServiceURL);

            HttpWReq.Proxy = SetProxyCredencials(settings.ProxyUrl, settings.ProxyUser, settings.ProxyPassword, settings.ProxyDomain);

            HttpWReq.CookieContainer = SetCookieContainer(new CookieContainer(), settings.UseSessionParam);

            HttpWReq = SetHttpConfiguration(HttpWReq);

            HttpWebResponse HttpWResp = null;

            try
            {
                //FIX: Invalid SSL passing behavior
                //(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                ServicePointManager.ServerCertificateValidationCallback = delegate
                {
                    return true;
                };

                HttpWResp = HttpWReq.GetResponse() as HttpWebResponse;

                if (settings.UseSessionParam)
                {
                    if (HttpWResp.Cookies["BREEZESESSION"] != null)
                    {
                        this.m_SessionInfo = HttpWResp.Cookies["BREEZESESSION"].Value;
                        this.m_SessionDomain = HttpWResp.Cookies["BREEZESESSION"].Domain;
                    }
                }

                Stream receiveStream = HttpWResp.GetResponseStream();
                if (receiveStream == null)
                    return null;

                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    operationApiStatus = Helpers.ResolveOperationStatusFlags(new XmlTextReader(readStream));
                }

                if (settings.UseSessionParam)
                {
                    operationApiStatus.SessionInfo = this.m_SessionInfo;
                }

                return operationApiStatus;
            }
            catch (Exception ex)
            {
                HttpWReq.Abort();
                throw;
            }
            finally
            {
                if (HttpWResp != null)
                    HttpWResp.Close();
            }
        }
    }
}