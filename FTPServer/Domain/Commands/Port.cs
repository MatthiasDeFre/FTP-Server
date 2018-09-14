using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FTPServer.Domain.Enums;
using FTPServer.Domain.Extensions;

namespace FTPServer.Domain.Commands
{
    public class Port : Command
    {
        public Port(ClientConnection clientConnection) : base(clientConnection)
        {
        }

        public override string Execute(string arguments)
        {
            ClientConnection.DataConnectionType = DataConnectionType.Active;
            byte[] ipAndPort = Array.ConvertAll(arguments.Split(","), byte.Parse);

            byte[] ipAddress = ipAndPort.SubArray(0, 4);
            byte[] port = ipAndPort.SubArray(4, 2);

            if(BitConverter.IsLittleEndian)
                Array.Reverse(port);

            ClientConnection.DataEndPoint = new IPEndPoint(new IPAddress(ipAddress), BitConverter.ToInt16(port, 0));
            return Response.Port.Get();
        }
    }
}
