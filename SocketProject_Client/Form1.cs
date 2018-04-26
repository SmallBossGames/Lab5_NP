using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketProject_Client
{
    public partial class Form1 : Form
    {
        SocketProject_ClientAPI client;

        public Form1()
        {
            client = new SocketProject_ClientAPI();
            client.StartConnecting();
            InitializeComponent();
        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            var deltaTime = client.GetTimeDeltaFromServer();
            answerTextBox.Text = deltaTime.ToString("H:mm:ss, ffff");
        }
    }
}
