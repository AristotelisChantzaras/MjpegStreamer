//Project: MjpegStreamer.UWP
//Filename: JpegStreaming.cs
//Version: 20170907

using System.Collections.Generic;
using System.IO;
using Windows.Graphics.Imaging;

namespace Chantzaras.Media.Streaming.Mjpeg
{

    public static class JpegStreaming
    {

        public static IEnumerable<MemoryStream> JpegStreams(this IEnumerable<SoftwareBitmap> source)
        {
            MemoryStream ms = new MemoryStream();

            foreach (var img in source)
            {
                ms.SetLength(0);
                img.WriteJpeg(ms);
                yield return ms;
            }

            ms.Dispose(); //ms.Close() not available
            ms = null;

            yield break;
        }

        public static void WriteJpeg(this SoftwareBitmap img, MemoryStream ms)
        {
            var data = MjpegWriter.EncodedBytes(img, BitmapEncoder.JpegEncoderId).GetAwaiter().GetResult();
            ms.Write(data, 0, data.Length); //TODO: do we need to also call Flush()?
        }

    }

}