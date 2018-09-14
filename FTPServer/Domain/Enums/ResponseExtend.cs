using System;
using System.Collections.Generic;
using System.Text;

namespace FTPServer.Domain.Enums
{
    public static class ResponseExtend
    {
        public static string Get(this Response response)
        {
            return Get(response, Array.Empty<String>());
        }

        public static string Get(this Response response, string[] argument)
        {
            string output;
            switch (response)
            {
                case Response.User:
                    output = "331 Username ok, need password";
                    break;
                case Response.Password:
                    output = "210 User is logged in";
                    break;
                case Response.Directory:
                    output = "250 changed to a new directory";
                    break;
                case Response.PresentDirectory:
                    output = string.Format("257 Current directory is {0}", argument);
                    break;
                case Response.Quit:
                    output = "221 Service closing control connection.";
                    break;
                case Response.Port:
                    output = "200 Port ok";
                    break;
                case Response.Passive:
                    output = string.Format("227 Entering passive mode on ({0},{1},{2},{3},{4},{5})", argument);
                    break;
                case Response.Ok:
                    output = "200 Command succesfull";
                    break;
                case Response.InvalidUserOrPass:
                    output = "430 Invalid username or password";
                    break;
                case Response.InvalidDirectory:
                    output = "450 invalid directory";
                    break;
                case Response.NotSupported:
                    output = "504 Command not implemented";
                    break;
                default:
                    output = "not found";
                    break;
            }
            return output;
        }
    }
}
