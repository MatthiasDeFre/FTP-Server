using System;
using System.Collections.Generic;
using System.Text;

namespace FTPServer.Domain.Enums
{
    public enum Response
    {
        User,
        Password,
        Directory,
        PresentDirectory,
        Quit,
        Port,
        Passive,
        Type,
        Ok,
        InvalidUserOrPass,
        InvalidDirectory,
        NotSupported
    }
}
