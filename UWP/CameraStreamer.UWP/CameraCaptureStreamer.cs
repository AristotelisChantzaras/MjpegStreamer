//Project: CameraCaptureStreamer.UWP
//Filename: CameraCaptureStreamer.cs
//Version: 20170907

using Windows.UI.Xaml.Controls;

using Chantzaras.Media.Streaming.Mjpeg;
using Chantzaras.Media.Capture;
using System;

namespace Chantzaras.Media.Streaming.Cameracast
{
    public class CameraCaptureStreamer : MjpegStreamer
    {

        public CameraCaptureStreamer(CaptureElement previewControl, int width, int height, Func<bool> stop=null) : base((CameraCapture.Snapshots(previewControl, width, height, stop)))
        {
        }


    }
}
