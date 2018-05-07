using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Lab5_NP
{
    class Program
    {
        static void Main(string[] args)
        {
            var hui = new SocketProject_ServerAPI();
            IPAddress address;
            int port;

            do
            {
                Console.WriteLine("Write the server IPAddress");
            } while (!IPAddress.TryParse(Console.ReadLine(), out address));

            do
            {
                Console.WriteLine("Write the port");
            } while (!Int32.TryParse(Console.ReadLine(), out port));

            hui.StartListeing(address, port);
        }
    }
}
