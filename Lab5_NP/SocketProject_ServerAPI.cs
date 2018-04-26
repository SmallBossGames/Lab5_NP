using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Lab5_NP
{
    class SocketProject_ServerAPI
    {
        Socket socketListner;

        public SocketProject_ServerAPI()
        {
            socketListner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListeing()
        {
            var endPoint = new IPEndPoint(new IPAddress(new byte[] { 192, 168, 1, 5 }), 3425);
            socketListner.Bind(endPoint);
            socketListner.Listen(1);
            Console.WriteLine(endPoint.Address);

            while (true)
            {
                var poolSocket = socketListner.Accept();
                var buffer = new byte[sizeof(Int64)];
                var count = poolSocket.Receive(buffer);
                poolSocket.Send(GetDeltaTimeWithClient(buffer));
                poolSocket.Close();
            }
        }

        public byte[] GetDeltaTimeWithClient(byte[] data)
        {
            var inputTime = BitConverter.ToInt64(data, 0);
            var deltaTime = Abs(GetTime().Ticks - inputTime);
            Console.WriteLine(deltaTime);
            return BitConverter.GetBytes(deltaTime);
        }

        public static DateTime GetTime()
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                //Stops code hang if NTP is blocked
                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        public Int64 Abs(Int64 val) => val > 0 ? val : -val;
    }
}
