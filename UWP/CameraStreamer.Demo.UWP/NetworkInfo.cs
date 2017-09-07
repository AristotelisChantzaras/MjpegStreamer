//Project: CameraStreamer (UWP)
//Filename: NetworkInfo.cs
//Version: 20170907

using System.Linq;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace MjpegStreamerTest.uwp
{
    public static class NetworkInfo
    {

        public static string GetMachineName() //see https://stackoverflow.com/questions/32876966/how-to-get-local-host-name-in-c-sharp-on-a-windows-10-universal-app
        {
            var hostNames = NetworkInformation.GetHostNames();
            return hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";
        }

        public static string GetLocalIp(HostNameType hostNameType = HostNameType.Ipv4) //https://stackoverflow.com/questions/33770429/how-do-i-find-the-local-ip-address-on-a-win-10-uwp-project
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .FirstOrDefault(
                        hn =>
                            hn.Type == hostNameType &&
                            hn.IPInformation?.NetworkAdapter != null &&
                            hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }

    }
}
