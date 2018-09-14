using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class Password : Command
    {
        public Password(ClientConnection clientConnection) : base(clientConnection)
        {
        }

        public override string Execute(string arguments)
        {
            //Check db for password;
            if (true)
                return Response.Password.Get();
            else
                return Response.InvalidUserOrPass.Get();

        }
    }
}
