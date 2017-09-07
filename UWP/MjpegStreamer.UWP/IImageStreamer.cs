//Project: MjpegStreamer.UWP
//Filename: IImageStreamer.cs
//Version: 20170907

using System.Collections.Generic;
using Windows.Graphics.Imaging;
using Windows.Networking.Sockets;

namespace Chantzaras.Media.Streaming.Mjpeg
{
    public interface IImageStreamer
    {
        IEnumerable<SoftwareBitmap> ImagesSource { get; set; }
        int Interval { get; set; }

        bool IsRunning { get; }
        IEnumerable<StreamSocket> Clients { get; }

        void Dispose();
        void Start();
        void Start(int port);
        void Stop();
    }
}