using AdobeConnectSDK.Interfaces;

namespace AdobeConnectSDK.Common
{
    public class SdkSettings : ISdkSettings
    {
        public string ServiceURL { get; set; }

        public string ProxyUrl { get; set; }

        public string ProxyUser { get; set; }

        public string ProxyPassword { get; set; }

        public string ProxyDomain { get; set; }

        public string NetUser { get; set; }

        public string NetPassword { get; set; }

        public string NetDomain { get; set; }

        public bool UseSessionParam { get; set; }
    }
}