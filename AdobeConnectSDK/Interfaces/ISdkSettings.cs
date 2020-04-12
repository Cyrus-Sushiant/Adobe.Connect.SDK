namespace AdobeConnectSDK.Interfaces
{
    public interface ISdkSettings
    {
        string ServiceURL { get; set; }
        string NetUser { get; set; }
        string NetPassword { get; set; }
        string NetDomain { get; set; }
        bool UseSessionParam { get; set; }
        string ProxyUrl { get; set; }
        string ProxyUser { get; set; }
        string ProxyPassword { get; set; }
        string ProxyDomain { get; set; }
    }
}
