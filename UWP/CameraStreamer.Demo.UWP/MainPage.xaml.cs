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

        public MainPage()
        {
            this.InitializeComponent();

            this.linkLabel1.Content = string.Format("http://{0}:{1}", Environment_MachineName(), port);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //StartServer();
        }

        private void StartServer()
        {
            _Server = new MjpegStreamer_StreamSocket_Task(Camera.Snapshots(PreviewControl, 640, 480))
            {
                Interval = 500 //change this value if seeing too much lag (make sure it's not very small, try 100-500)
            };
            _Server.Start(port);
        }

        private void linkLabel1_Click(System.Object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(this.linkLabel1.Content);
        }

        private string Environment_MachineName() //see https://stackoverflow.com/questions/32876966/how-to-get-local-host-name-in-c-sharp-on-a-windows-10-universal-app
        {
            var hostNames = NetworkInformation.GetHostNames();
            return hostNames.FirstOrDefault(name => name.Type == HostNameType.Ipv4)?.DisplayName ?? "???"; //use DomainName to get machine name in local network
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }
    }

}
