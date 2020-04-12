using AdobeConnectSDK.Model;

namespace AdobeConnectSDK.Interfaces
{
    /// <summary>
    /// Communication provider.
    /// </summary>
    public interface ICommunicationProvider
    {
        ApiStatus ProcessRequest(string pAction, string qParams, ISdkSettings settings);
    }
}
