using System;
using System.Linq;
using System.Windows.Forms;

using Chantzaras.Media.Capture;
using Chantzaras.Media.Streaming.Mjpeg;

namespace ScreenStreamer
{
    public partial class MainForm : Form
    {
        //private DateTime time = DateTime.MinValue;

        const int port = 8080;
        private MjpegStreamer_Socket_Thread _Server;

        public MainForm()
        {
            InitializeComponent();
            this.linkLabel1.Text = string.Format("http://{0}:{1}", Environment.MachineName, port);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _Server = new MjpegStreamer_Socket_Thread(Chantzaras.Media.Capture.Screen.Snapshots(600, 450, true));
            _Server.Start(port);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int count = (_Server.Clients != null) ? _Server.Clients.Count() : 0;
            this.sts.Text = "Clients: " + count.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.linkLabel1.Text);
        }

    }


}
