//Project: ScreenCaptureStreamer (WinForms)
//Filename: ScreenCapture.cs
//Version: 20170907

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chantzaras.Media.Capture
{

    static class ScreenCapture
    {

        public static IEnumerable<Image> Snapshots()
        {
            return Snapshots(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, true);
        }

        /// <summary>
        /// Returns a 
        /// </summary>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        public static IEnumerable<Image> Snapshots(int width, int height, bool showCursor, Func<bool> stop=null)
        {
            Size size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

            Bitmap srcImage = new Bitmap(size.Width, size.Height);
            Graphics srcGraphics = Graphics.FromImage(srcImage);

            bool scaled = (width != size.Width || height != size.Height);

            Bitmap dstImage = srcImage;
            Graphics dstGraphics = srcGraphics;

            if (scaled)
            {
                dstImage = new Bitmap(width, height);
                dstGraphics = Graphics.FromImage(dstImage);
            }

            Rectangle src = new Rectangle(0, 0, size.Width, size.Height);
            Rectangle dst = new Rectangle(0, 0, width, height);
            Size curSize = new Size(32, 32);

            while (true)
            {
                if ((stop != null) && stop())
                {
                    //cleanup
                    srcGraphics.Dispose();
                    dstGraphics.Dispose();
                    srcImage.Dispose();
                    dstImage.Dispose();

                    yield break;
                }

                srcGraphics.CopyFromScreen(0, 0, 0, 0, size);

                if (showCursor)
                    Cursors.Default.Draw(srcGraphics, new Rectangle(Cursor.Position, curSize));

                if (scaled)
                    dstGraphics.DrawImage(srcImage, dst, src, GraphicsUnit.Pixel);

                yield return dstImage;
            }

        }

    }

}