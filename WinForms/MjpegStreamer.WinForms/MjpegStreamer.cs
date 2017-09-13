//Project: MjpegStreamer.WinForms
//Filename: MjpegStreamer.cs
//Version: 20170913

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chantzaras.Media.Streaming.Mjpeg
{

    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class MjpegStreamer : IDisposable, IImageStreamer
    {
        public const int DEFAULT_INTERVAL = 50;

        private List<Socket> _Clients = new List<Socket>();
        private Thread _Thread;

        public MjpegStreamer(IEnumerable<Image> imagesSource)
        {
            _Thread = null;

            this.ImagesSource = imagesSource;
            this.Interval = DEFAULT_INTERVAL;
        }


        /// <summary>
        /// Gets or sets the source of images that will be streamed to the 
        /// any connected client.
        /// </summary>
        public IEnumerable<Image> ImagesSource { get; set; }

        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a collection of client sockets.
        /// </summary>
        public IEnumerable<Socket> Clients { get { return _Clients; } }

        /// <summary>
        /// Returns the status of the server. True means the server is currently 
        /// running and ready to serve any client requests.
        /// </summary>
        public bool IsRunning { get { return (_Thread != null && _Thread.IsAlive); } }

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {

            lock (this)
            {
                _Thread = new Thread(new ParameterizedThreadStart(ServerThread));
                _Thread.IsBackground = true;
                _Thread.Start(port);
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
                    _Thread.Join();
                    _Thread.Abort();
                }
                finally
                {

                    lock (_Clients)
                    {
                        
                        foreach (var s in _Clients)
                        {
                            try
                            {
                                s.Close();
                            }
                            catch(Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message);
                            }
                        }
                        _Clients.Clear();

                    }

                    _Thread = null;
                }
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="state"></param>
        private void ServerThread(object port)
        {

            try
            {
                Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Server.Bind(new IPEndPoint(IPAddress.Any,(int)port));
                Server.Listen(10);

                System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", port));
                
                foreach (Socket client in Server.IncommingConnections())
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
            
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            this.Stop();
        }

        /// <summary>
        /// Each client connection will be served by this thread.
        /// </summary>
        /// <param name="client"></param>
        private void ClientThread(object client)
        {

            Socket socket = (Socket)client;

            System.Diagnostics.Debug.WriteLine(string.Format("New client from {0}",socket.RemoteEndPoint.ToString()));

            lock (_Clients)
                _Clients.Add(socket);

            try
            {
                using (MjpegWriter wr = new MjpegWriter(new NetworkStream(socket, true)))
                {

                    // Writes the response header to the client.
                    wr.WriteHeader();

                    // Streams the images from the source to the client.
                    foreach (var imgStream in this.ImagesSource.JpegStreams())
                    {
                        if (this.Interval > 0)
                            Thread.Sleep(this.Interval);

                        wr.Write(imgStream);
                    }

                }
            }
            catch(Exception e)
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

    static class SocketExtensions
    {

        public static IEnumerable<Socket> IncommingConnections(this Socket server)
        {
            while(true)
                yield return server.Accept();
        }

    }

}
