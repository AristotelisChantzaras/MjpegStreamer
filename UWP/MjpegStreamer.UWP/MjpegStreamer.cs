//Project: MjpegStreamer.UWP
//Filename: MjpegStreamer.cs
//Version: 20170913

using System;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Threading;
using Windows.Networking.Sockets;
using System.IO; //for WindowsRuntimeStreamExtensions - see https://msdn.microsoft.com/en-us/library/system.io.windowsruntimestreamextensions(v=vs.110).aspx 
using Chantzaras.Tasks;

namespace Chantzaras.Media.Streaming.Mjpeg
{

    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class MjpegStreamer : IImageStreamer, IDisposable
    {
        public const int DEFAULT_INTERVAL = 50;

        private StreamSocketListener socketListener;
        private List<StreamSocket> _Clients = new List<StreamSocket>();

        public MjpegStreamer(IEnumerable<SoftwareBitmap> imagesSource)
        {
            ImagesSource = imagesSource;
            Interval = DEFAULT_INTERVAL;
        }

        /// <summary>
        /// Gets or sets the source of images that will be streamed to the 
        /// any connected client.
        /// </summary>
        public IEnumerable<SoftwareBitmap> ImagesSource { get; set; }

        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a collection of client sockets.
        /// </summary>
        public IEnumerable<StreamSocket> Clients { get { return _Clients; } }

        /// <summary>
        /// Returns the status of the server. True means the server is currently 
        /// running and ready to serve any client requests.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the server to accepts any new connections on the default port (8080).
        /// </summary>
        public void Start()
        {
            this.Start(8080);
        }

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            lock (this) //TODO: check if lock is needed
            {
                ActionItem.Schedule(ServerTask, port);
            }

        }

        public void Stop()
        {
            if (!IsRunning) return;

            try
            {
                DisposeSocketListener();
            }
            finally
            {
                lock (_Clients)
                {
                    foreach (var s in _Clients)
                    {
                        try
                        {
                            s.Dispose();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }
                    }
                    _Clients.Clear();
                }

                IsRunning = false;
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="state"></param>
        private void ServerTask(object port)
        {

            try
            {
                DisposeSocketListener();
                
                //Create a StreamSocketListener to start listening for TCP connections.
                socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                socketListener.BindServiceNameAsync(port.ToString()).GetAwaiter().GetResult();
                //socketListener.BindServiceNameAsync(state.ToString(), SocketProtectionLevel.PlainSocket, GetIPadapter()).GetAwaiter().GetResult();

                IsRunning = true;

                System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", port));
            }
            catch (Exception e)
            {
                IsRunning = false;

                System.Diagnostics.Debug.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Handles new client connections.
        /// </summary>
        /// <param name="client"></param>
        private void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            ActionItem.Schedule(ClientTask, args.Socket);
        }

        /// <summary>
        /// Each client connection will be served by this thread.
        /// </summary>
        /// <param name="client"></param>
        private void ClientTask(object client)
        {
            StreamSocket socket = (StreamSocket)client;

            System.Diagnostics.Debug.WriteLine(string.Format("New client from {0}", socket.Information.RemoteAddress.ToString()));

            lock (_Clients) //TODO: see if needed, Add may be thread-safe already
                _Clients.Add(socket);

            try
            {
                using (MjpegWriter wr = new MjpegWriter(socket.OutputStream.AsStreamForWrite()))
                {

                    // Writes the response header to the client.
                    wr.WriteHeader();

                    // Streams the images from the source to the client.
                    foreach (var imgStream in this.ImagesSource.JpegStreams())
                    {
                        if (this.Interval > 0)
                            Task.Delay(this.Interval).Wait(); //see https://stackoverflow.com/questions/12641223/thread-sleep-replacement-in-net-for-windows-store

                        wr.Write(imgStream);
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                socket.Dispose();

                lock (_Clients) //TODO: see if needed, Remove may be thread-safe already
                    _Clients.Remove(socket);
            }
        }


        #region Cleanup

        private void DisposeSocketListener()
        {
            if (socketListener != null)
            {
                socketListener.CancelIOAsync().GetAwaiter().GetResult();
                socketListener.Dispose();
                socketListener = null;
            }
        }

        public void Dispose() //IDisposable
        {
            Stop();
        }

        #endregion
    }

}
