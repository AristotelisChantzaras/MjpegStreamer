using System.Collections.Generic;
using System.Drawing;

namespace Chantzaras.Media.Streaming.Mjpeg
{
    public interface IImageStreamer
    {
        IEnumerable<Image> ImagesSource { get; set; }
        int Interval { get; set; }
        bool IsRunning { get; }

        void Dispose();
        void Start();
        void Start(int port);
        void Stop();
    }
}