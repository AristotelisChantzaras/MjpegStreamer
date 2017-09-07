//Project: CameraStreamer (UWP)
//Filename: MainPage.cs
//Version: 20170907

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MjpegStreamerTest.uwp;
using Chantzaras.Media.Streaming.Cameracast;

namespace CameraStreamer.uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Constants

        const int port = 8080;

        #endregion

        #region Fields

        private CameraCaptureStreamer _Server;

        #endregion

        #region Initialization

        public MainPage()
        {
            this.InitializeComponent();

            this.linkLabel1.Content = string.Format("http://{0}:{1}", NetworkInfo.GetLocalIp(), port);
        }

        #endregion
        
        #region Methods

        private void StartServer()
        {
            _Server = new CameraCaptureStreamer(PreviewControl, 640, 480)
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
