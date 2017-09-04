using Windows.UI.Xaml.Controls;

using Chantzaras.Media.Streaming.Mjpeg;
using Chantzaras.Media.Capture;

namespace Chantzaras.Media.Streaming.CameraCapture
{
    public class CameraCaptureStreamer : MjpegStreamer
    {

        public CameraCaptureStreamer(CaptureElement previewControl, int width, int height) : base((Camera.Snapshots(previewControl, width, height)))
        {
        }


    }
}
