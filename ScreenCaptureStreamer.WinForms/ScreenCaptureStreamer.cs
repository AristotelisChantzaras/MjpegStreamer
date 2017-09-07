using Chantzaras.Media.Streaming.Mjpeg;
using Chantzaras.Media.Capture;

namespace Chantzaras.Media.Streaming.Screencast
{
    public class ScreenCaptureStreamer : MjpegStreamer
    {

        public ScreenCaptureStreamer(int width, int height, bool showCursor) : base((ScreenCapture.Snapshots(width, height, showCursor))) //TODO: pass a Func<bool> here for stopping the enumerable (override Stop to set some boolean)
        {
        }

    }
}
