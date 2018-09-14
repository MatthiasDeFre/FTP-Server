using System;
using FTPServer.Domain;

namespace FTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            FtpServer server = new FtpServer();
            server.Start();
            Console.ReadLine();
        }
    }
}
