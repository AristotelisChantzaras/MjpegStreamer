//Project: CameraCaptureStreamer.UWP
//Filename: CameraCaptureStreamer.cs
//Version: 20170907

using Windows.UI.Xaml.Controls;

using Chantzaras.Media.Streaming.Mjpeg;
using Chantzaras.Media.Capture;

namespace Chantzaras.Media.Streaming.Cameracast
{
    public class CameraCaptureStreamer : MjpegStreamer
    {

        public CameraCaptureStreamer(CaptureElement previewControl, int width, int height) : base((CameraCapture.Snapshots(previewControl, width, height))) //TODO: pass a Func<bool> here for stopping the enumerable (override Stop to set some boolean)
        {
        }


    }
}
