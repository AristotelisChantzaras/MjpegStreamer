using System.Collections.Generic;
using Windows.Graphics.Imaging;

namespace Chantzaras.Media.Streaming.Mjpeg
{
    public interface IImageStreamer
    {
        IEnumerable<SoftwareBitmap> ImagesSource { get; set; }
        int Interval { get; set; }
        bool IsRunning { get; }

        void Dispose();
        void Start();
        void Start(int port);
        void Stop();
    }
}