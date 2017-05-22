using System.Net;
using FeisTaim.Droid.Helpers;
using FeisTaim.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkHelper))]
namespace FeisTaim.Droid.Helpers
{
    public class NetworkHelper : INetworkHelper
    {
        public string GetLocalIpAddress()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());

            if (adresses != null && adresses[0] != null)
            {
                return adresses[0].ToString();
            }
            else
            {
                return "";
            }
        }
    }
}