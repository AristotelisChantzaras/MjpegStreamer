//Project: MjpegStreamer.WinForms
//Filename: IImageStreamer.cs
//Version: 20170907

using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;

namespace Chantzaras.Media.Streaming.Mjpeg
{
    public interface IImageStreamer
    {
        IEnumerable<Image> ImagesSource { get; set; }
        int Interval { get; set; }

        bool IsRunning { get; }
        IEnumerable<Socket> Clients { get; }

        void Dispose();
        void Start();
        void Start(int port);
        void Stop();
    }
}