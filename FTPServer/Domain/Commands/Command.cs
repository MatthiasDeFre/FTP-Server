using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public abstract class Command
    {
        protected ClientConnection ClientConnection;
        public Command(ClientConnection clientConnection)
        {
            ClientConnection = clientConnection;
        }
        public abstract Response Execute(string arguments);
    }
}
