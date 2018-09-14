using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FTPServer.Domain
{
   
    public class FtpServer
    {
        private TcpListener _listener;

        public FtpServer()
        {
            
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 21);
            _listener.Start();

            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);

        }

        public void Stop()
        {
            _listener?.Stop();
        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            TcpClient client = _listener.EndAcceptTcpClient(result);
            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);

            ClientConnection connect = new ClientConnection(client);
            ThreadPool.QueueUserWorkItem(connect.HandleClient, client);
            /* NetworkStream stream = client.GetStream();
 
             using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
 
             using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
             {
                 writer.WriteLine("220 Connected");
                 writer.Flush();
 
                 string line = null;
                 while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                 {
                     writer.WriteLine("You said {0}", line);
                     writer.Flush();
                 }
             }*/
        }
    }
}
