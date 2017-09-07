//Project: CameraCaptureStreamer.UWP
//Filename: CameraCapture.cs
//Version: 20170907

using System.Collections.Generic;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using System;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;

namespace Chantzaras.Media.Capture
{

    public static class CameraCapture
    {
        static MediaCapture _mediaCapture;
        static VideoEncodingProperties _previewProperties;

        /// <summary>
        /// Returns a 
        /// </summary>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        public static IEnumerable<SoftwareBitmap> Snapshots(CaptureElement previewControl, int width, int height, Func<bool> stop=null)
        {
            _mediaCapture = new MediaCapture();
            _mediaCapture.Failed += _mediaCapture_Failed;
            _mediaCapture.InitializeAsync().GetAwaiter().GetResult();

            previewControl.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () => {
                previewControl.Source = _mediaCapture;
            }).GetAwaiter().GetResult();

            _mediaCapture.StartPreviewAsync().GetAwaiter().GetResult();

            _previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
            width = (int)_previewProperties.Width; //ignore passed-in width and height and get the ones from the preview
            height = (int)_previewProperties.Height;

            while (true)
            {
                if ((stop != null) && stop())
                {
                    //cleanup
                    _mediaCapture.StopPreviewAsync().GetAwaiter().GetResult();
                    _mediaCapture = null;
                    previewControl.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        previewControl.Source = null;
                    }).GetAwaiter().GetResult();

                    yield break;
                }

                yield return CameraImage(previewControl, width, height);
            }
        }

        private static void _mediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("Failed to capture");
        }

        public static SoftwareBitmap CameraImage(CaptureElement previewControl, int width = 0, int height = 0) //sample from https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/get-a-preview-frame
        {
            while (true)
            {
                try
                {
                    if (width == 0)
                        width = (int)previewControl.Width;

                    if (height == 0)
                        height = (int)previewControl.Height;

                    // Create a video frame in the desired format for the preview frame
                    VideoFrame videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, width, height);
                    VideoFrame previewFrame = _mediaCapture.GetPreviewFrameAsync(videoFrame).GetAwaiter().GetResult();
                    SoftwareBitmap bitmap = previewFrame.SoftwareBitmap;
                    //System.Diagnostics.Debug.WriteLine("Captured frame OK");
                    return bitmap;
                }
                catch (Exception e)
                {
                    //error occured (e.g. trying to capture too fast), skip frame
                    System.Diagnostics.Debug.WriteLine("Skiping frame: " + e.Message);
                }
            }
        }

    }

}