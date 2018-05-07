using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace SocketProject_Client
{
    public partial class Form1 : Form
    {
        SocketProject_ClientAPI client;

        public Form1()
        {
            client = new SocketProject_ClientAPI();
            InitializeComponent();
        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            var deltaTime = client.GetTimeDeltaFromServer(new IPAddress(new byte[] { 192, 168, 1, 5 }), 3425);
            answerTextBox.Text = deltaTime.ToString("H:mm:ss, ffff");
        }
    }
}
