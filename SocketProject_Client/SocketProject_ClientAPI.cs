using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProject_Client
{
    class SocketProject_ClientAPI
    {
        Socket socketConnector;
        IPAddress ipAddress = new IPAddress(new byte[] { 192, 168, 1, 5 });
        Int32 port = 3425;

        public SocketProject_ClientAPI()
        {
            socketConnector = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void SendData(string data)
        {
            socketConnector.Send(Encoding.Default.GetBytes(data));
        }

        public DateTime GetTimeDeltaFromServer()
        {
            var byteDataPool = BitConverter.GetBytes(DateTime.Now.Ticks);

            using (var socketConnector = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socketConnector.Connect(ipAddress, port);

                socketConnector.Send(byteDataPool);
                socketConnector.Receive(byteDataPool);

                socketConnector.Shutdown(SocketShutdown.Both);
            }

            DateTime deltaTime = new DateTime(BitConverter.ToInt64(byteDataPool, 0));

            return deltaTime;
        }
    }
}
