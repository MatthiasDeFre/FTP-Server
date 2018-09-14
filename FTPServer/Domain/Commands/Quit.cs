using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class Quit : Command
    {
        public Quit(ClientConnection clientConnection) : base(clientConnection)
        {
        }

        public override string Execute(string arguments)
        {
            return Response.Quit.Get();
        }
    }
}
