//Project: ScreenCaptureStreamer (WinForms)
//Filename: ScreenCapture.cs
//Version: 20170907

using Chantzaras.Media.Streaming.Mjpeg;
using Chantzaras.Media.Capture;
using System;

namespace Chantzaras.Media.Streaming.Screencast
{
    public class ScreenCaptureStreamer : MjpegStreamer
    {

        public ScreenCaptureStreamer(int width, int height, bool showCursor, Func<bool> stop=null) : base((ScreenCapture.Snapshots(width, height, showCursor, stop)))
        {
        }

    }
}
