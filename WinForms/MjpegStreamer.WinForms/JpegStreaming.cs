//Project: MjpegStreamer.WinForms
//Filename: JpegStreaming.cs
//Version: 20170907

using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Chantzaras.Media.Streaming.Mjpeg
{

    public static class JpegStreaming
    {

        public static IEnumerable<MemoryStream> JpegStreams(this IEnumerable<Image> source)
        {
            MemoryStream ms = new MemoryStream();

            foreach (var img in source)
            {
                ms.SetLength(0);
                img.WriteJpeg(ms);
                yield return ms;
            }

            ms.Close();
            ms = null;

            yield break;
        }

        public static void WriteJpeg(this Image img, MemoryStream ms)
        {
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

    }

}