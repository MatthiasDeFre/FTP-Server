using System;
using System.Collections.Generic;
using System.Text;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain.Commands
{
    public class Type : Command
    {
        public Type(ClientConnection clientConnection) : base(clientConnection)
        {
        }

        public override string Execute(string arguments)
        {
            string[] splitargs = arguments.Split(" ");
            string typecode = splitargs[0];
            string formatcontrol = splitargs.Length > 1 ? splitargs[1] : null;

            Response response;    
            switch (typecode)
            {
                case "A":
                    ClientConnection.TranserferType = TransferType.Ascii;
                    response = Response.Ok;
                    break;
                case "I":
                    ClientConnection.TranserferType = TransferType.Image;
                    response = Response.Ok;
                    break;
                default:
                    response = Response.NotSupported;
                    break;
            }
            if (formatcontrol != null)
            {

                switch (formatcontrol)
                {
                    case "N":
                        response = Response.Ok;
                        break;
                    default:
                        response = Response.NotSupported;
                        break;
                }

            }
            return response.Get();
        }
    }
}
