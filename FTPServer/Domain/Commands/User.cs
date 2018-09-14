using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class User : Command
    {
        public User(ClientConnection clientConnection) : base(clientConnection)
        {
        }
        public override string Execute(string username)
        {
            ClientConnection.Username = username;

            return Response.User.Get();
        }

       
    }
}
