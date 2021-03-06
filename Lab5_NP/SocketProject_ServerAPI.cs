﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Lab5_NP
{
    class SocketProject_ServerAPI
    {
        Socket socketListner;

        public SocketProject_ServerAPI()
        {
            socketListner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening(IPAddress address, int port)
        {
            var endPoint = new IPEndPoint(address, port);
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
            Console.WriteLine($"Такты: {deltaTime}");
            return BitConverter.GetBytes(deltaTime);
        }

        public static DateTime GetTime()
        {
            const string ntpServer = "time.windows.com";

            var ntpData = new byte[48];

            ntpData[0] = 0x1B;

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            var pingDuration = Stopwatch.GetTimestamp();

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                pingDuration = Stopwatch.GetTimestamp();
                socket.Receive(ntpData);

                pingDuration = Stopwatch.GetTimestamp() - pingDuration;
                socket.Close();
            }

            const byte serverReplyTime = 40;

            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.AddTicks(pingDuration).ToLocalTime();
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