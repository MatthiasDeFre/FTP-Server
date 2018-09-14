using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class PresentDirectory : Command
    {
        public PresentDirectory(ClientConnection clientConnection) : base(clientConnection)
        {
        }

        public override string Execute(string arguments)
        {
            return Response.PresentDirectory.Get(new []{ClientConnection.CurrentDirectory});
        }
    }
}
