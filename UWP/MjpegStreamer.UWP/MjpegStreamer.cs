//Project: MjpegStreamer.UWP
//Filename: MjpegStreamer.cs
//Version: 20170907

using System;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Threading;
using Windows.Networking.Sockets;
using System.IO; //for WindowsRuntimeStreamExtensions - see https://msdn.microsoft.com/en-us/library/system.io.windowsruntimestreamextensions(v=vs.110).aspx 

namespace Chantzaras.Media.Streaming.Mjpeg
{

    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class MjpegStreamer : IDisposable, IImageStreamer
    {
        private Task _Task;
        private List<StreamSocket> _Clients;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationToken ct;

        public MjpegStreamer(IEnumerable<SoftwareBitmap> imagesSource)
        {
            _Clients = new List<StreamSocket>();

            this.ImagesSource = imagesSource;
            this.Interval = 50;
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
        public bool IsRunning { get { return (_Task != null && _Task.Status == TaskStatus.Running); } }

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            lock (this)
            {
                ct = tokenSource.Token;
                ActionItem.Schedule(ServerTask, port);
            }

        }

        /// <summary>
        /// Starts the server to accepts any new connections on the default port (8080).
        /// </summary>
        public void Start()
        {
            this.Start(8080);
        }

        public void Stop()
        {

            if (this.IsRunning)
            {
                try
                {
                    _Task.Wait(); //see https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/attached-and-detached-child-tasks
                    tokenSource.Cancel(); //see https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation and https://stackoverflow.com/questions/4359910/is-it-possible-to-abort-a-task-like-aborting-a-thread-thread-abort-method
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

                    _Task = null;
                }
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="state"></param>
        private void ServerTask(object state)
        {

            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                StreamSocketListener socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                socketListener.BindServiceNameAsync(state.ToString()).GetAwaiter().GetResult();
                //socketListener.BindServiceNameAsync(state.ToString(), SocketProtectionLevel.PlainSocket, GetIPadapter()).GetAwaiter().GetResult();

                System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", state));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            //this.Stop();
        }

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

            lock (_Clients)
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
                lock (_Clients)
                    _Clients.Remove(socket);
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion
    }

    static class ActionItem //see https://github.com/dstuckims/azure-relay-dotnet/commit/93777a9f8563bbdacc4b854afd9fb21a968196b9
    {
        public static Task Schedule(Action<object> action, object state, bool attachToParent = false)
        {
            // UWP doesn't support ThreadPool[.QueueUserWorkItem] so just use Task.Factory.StartNew
            return Task.Factory.StartNew(s => action(s), state, (attachToParent) ? TaskCreationOptions.AttachedToParent : TaskCreationOptions.DenyChildAttach);
        }
    }

}
