using System.Linq;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Chantzaras.Media.Capture;
using Chantzaras.Media.Streaming.Mjpeg;

namespace CameraStreamer.uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        const int port = 8080;

        private IImageStreamer _Server;

        #region Initialization

        public MainPage()
        {
            this.InitializeComponent();

            this.linkLabel1.Content = string.Format("http://{0}:{1}", GetLocalIp(), port);
        }

        #endregion

        #region Methods

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

        private void StartServer()
        {
            _Server = new MjpegStreamer_StreamSocket_Task(Camera.Snapshots(PreviewControl, 640, 480))
            {
                Interval = 500 //change this value if seeing too much lag (make sure it's not very small, try 100-500)
            };
            _Server.Start(port);
        }



        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private void linkLabel1_Click(System.Object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(this.linkLabel1.Content);
        }

        /*
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }
        */

        #endregion

    }

}
