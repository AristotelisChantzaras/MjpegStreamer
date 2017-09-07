//Project: ScreenCaptureStreamer (WinForms)
//Filename: ScreenCapture.csusing System;
//Version: 20170907

using System;
using System.Linq;
using System.Windows.Forms;

using Chantzaras.Media.Streaming.Screencast;

namespace ScreenStreamer
{
    public partial class MainForm : Form
    {

        #region Constants

        const int port = 8080;

        #endregion

        #region Fields

        private ScreenCaptureStreamer _Server;

        #endregion

        #region Initialization

        public MainForm()
        {
            InitializeComponent();
            this.linkLabel1.Text = string.Format("http://{0}:{1}", Environment.MachineName, port);
        }

        #endregion

        #region Methods

        public void StartServer()
        {
            _Server = new ScreenCaptureStreamer(600, 450, true);
            _Server.Start(port);
        }

        #endregion

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {
            StartServer();
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

        #endregion

    }


}
