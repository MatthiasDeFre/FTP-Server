using System;
using System.Collections.Generic;
using System.Text;

namespace FTPServer.Domain.Enums
{
    public static class ResponseExtend
    {
        public string Get(this Response response)
        {
            string output;
            switch (response)
            {
                case Response.User:
                    output = "331 Username ok, need password";
                    break;
                default:
                    output = "not found";
                    break;
            }
            return output;
        }
    }
}
