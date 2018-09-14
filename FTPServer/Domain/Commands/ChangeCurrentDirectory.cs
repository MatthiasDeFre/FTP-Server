using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class ChangeCurrentDirectory : Command
    {
        private string _changedTo;
        public ChangeCurrentDirectory(ClientConnection clientConnection, string changedTo) : base(clientConnection)
        {
            _changedTo = changedTo;
        }

        public ChangeCurrentDirectory(ClientConnection clientConnection) : this(clientConnection, "")
        {
            
        }
        public override string Execute(string arguments)
        {
            //check if valid
            if (arguments == String.Empty)
                arguments = _changedTo;
            ClientConnection.CurrentDirectory = arguments;
            if (true)
                return Response.Directory.Get();
            else
                return Response.InvalidDirectory.Get();
        }
    }
}
