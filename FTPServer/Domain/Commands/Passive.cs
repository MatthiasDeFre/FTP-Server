using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class Passive : Command
    {
        public Passive(ClientConnection clientConnection) : base(clientConnection)
        {
        }

        public override string Execute(string arguments)
        {
            ClientConnection.DataConnectionType = DataConnectionType.Passive;

            IPAddress localAddress = ((IPEndPoint)ClientConnection.ControlClient.Client.LocalEndPoint).Address;
            ClientConnection.PassiveListener = new TcpListener(localAddress, 0);
            ClientConnection.PassiveListener.Start();

            IPEndPoint localEndPoint = ((IPEndPoint)ClientConnection.PassiveListener.LocalEndpoint);
            byte[] address = localEndPoint.Address.GetAddressBytes();
            short port = (short)localEndPoint.Port;

            byte[] portArray = BitConverter.GetBytes(port);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(portArray);
            return Response.Passive.Get(portArray.Select(x => x.ToString()).ToArray());
        }
    }
}
