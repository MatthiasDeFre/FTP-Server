using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using FTPServer.Domain.Commands;
using FTPServer.Domain.Enums;

namespace FTPServer.Domain
{
    public class ClientConnection
    {
        public TcpClient ControlClient;
        public TcpClient DataClient;
        public NetworkStream ControlStream;
        public StreamWriter ControlWriter;
        public StreamReader ControlReader;
        public TcpListener PassiveListener;
        public DataConnectionType DataConnectionType = DataConnectionType.Active;
        public IPEndPoint DataEndPoint;
        public StreamWriter StreamWriter;
        public StreamReader StreamReader;
        public NetworkStream NetworkStream;

        public string Username;
        public TransferType TranserferType;
        public string CurrentDirectory;

        private Dictionary<string,Command> _commands;


        public ClientConnection(TcpClient tcpClient)
        {
            ControlClient = tcpClient;
            ControlStream = ControlClient.GetStream();
            ControlWriter = new StreamWriter(ControlStream);
            ControlReader = new StreamReader(ControlStream);
            _commands = new Dictionary<string, Command>();

            //TO DO Lazy load
            _commands.Add("USER", new User(this));
            _commands.Add("PASS", new Password(this));
            _commands.Add("CWD", new ChangeCurrentDirectory(this));
            _commands.Add("CDUP", new ChangeCurrentDirectory(this, ".."));
        }

        public void HandleClient(object obj)
        {
            ControlWriter.WriteLine("220 Connected");
            ControlWriter.Flush();

            string line;

            try
            {
                while (!string.IsNullOrWhiteSpace(line = ControlReader.ReadLine()))
                {
                    string response = null;
                    line = line.Trim();
                    string[] command = line.Split(" ");
                    string cmd = command[0].ToUpperInvariant();
                    string arguments = command.Length > 1 ? line.Substring(command[0].Length + 1) : null;
                    Command commandEx;
                    if (response == null)
                    {
                        if (_commands.TryGetValue(cmd, out commandEx))
                        {
                            response =commandEx.Execute(arguments);
                        }
                        else
                        {
                            response = "504 not found";
                        }
                       /* switch (cmd)
                        {
                            case "USER":
                                response = User(arguments);
                                break;
                            case "PASS":
                                response = Password(arguments);
                                break;
                            case "CWD":
                                response = ChangeCurrentWorkingDirectory(arguments);
                                break;
                            case "CDUP":
                                response = ChangeCurrentWorkingDirectory("..");
                                break;
                            case "PWD":
                                response = "257 \"/\" is current directory";
                                break;
                            case "QUIT":
                                response = "221 server closing connection";
                                break;
                            case "TYPE":
                                string[] splitargs = arguments.Split(" ");
                                response = Type(splitargs[0], splitargs.Length > 1 ? splitargs[1] : null);
                                break;
                            case "PORT":
                                response = Port(arguments);
                                break;
                            case "PASV":
                                response = Passive(arguments);
                                break;
                            case "LIST":
                                response = ListDirectory(arguments);
                                break;
                            default:
                                response = "Not found";
                                break;
                        }*/
                    }
                    if (ControlClient == null || !ControlClient.Connected)
                    {
                        break;
                    }
                    else
                    {
                        ControlWriter.WriteLine(response);
                        ControlWriter.Flush();
                        if (response.StartsWith("221"))
                            break;


                    }
                }
            }
            catch (Exception ex)
            {
              // Console.WriteLine(ex);
            }
        }

        private string ListDirectory(string pathname)
        {
            if (pathname == null)
                pathname = string.Empty;
            pathname = new DirectoryInfo(Path.Combine(CurrentDirectory, pathname)).FullName;

            if (DataConnectionType == DataConnectionType.Active)
            {
                DataClient = new TcpClient();
                DataEndPoint = (IPEndPoint) DataClient.Client.LocalEndPoint;
                DataClient.BeginConnect(DataEndPoint.Address, DataEndPoint.Port, DoList, pathname);
            }
            else
            {
                PassiveListener.BeginAcceptTcpClient(DoList, pathname);
            }
            return "150 opening";
        }

        private void DoList(IAsyncResult result)
        {
            if (DataConnectionType ==DataConnectionType.Active)
            {
                DataClient.EndConnect(result);
            }
            else
            {
                DataClient = PassiveListener.EndAcceptTcpClient(result);
            }
            string pathname = (string) result.AsyncState;
            NetworkStream = DataClient.GetStream();

            StreamReader = new StreamReader(NetworkStream, Encoding.ASCII);
            StreamWriter = new StreamWriter(NetworkStream, Encoding.ASCII);

        }

        private string Passive(string arguments)
        {
            IPAddress localAddress = ((IPEndPoint) ControlClient.Client.LocalEndPoint).Address;
            PassiveListener = new TcpListener(localAddress, 0);
            PassiveListener.Start();

            IPEndPoint localEndPoint = ((IPEndPoint) PassiveListener.LocalEndpoint);
            byte[] address = localEndPoint.Address.GetAddressBytes();
            short port = (short) localEndPoint.Port;

            byte[] portArray = BitConverter.GetBytes(port);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(portArray);
            DataConnectionType = DataConnectionType.Passive;
            return string.Format("227 Entering passive mode on ({0},{1},{2},{3},{4},{5})", address[0], address[1], address[2], address[3],
                portArray[0], portArray[1]);
           
        }

        private string Port(string arguments)
        {
            byte[] port =Array.ConvertAll(arguments.Split(","), Byte.Parse);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(port);
            BitConverter.ToInt16(port, 0);
            return "200 ok";
        }

        private string ChangeCurrentWorkingDirectory(string directory)
        {
            CurrentDirectory = directory;
            return "250 changed to new directory";
        }

        private string User(string username)
        {
            Username = username;

            return "331 Username ok, need password";
        }

        private string Password(string password)
        {
            //Check for corrrect password
            return $"210 User logged in {Username}";
     
        }

     /*   private String Type(string typecode, string formatcontrol)
        {
            string response = "";
            switch (typecode)
            {
                case "A":
                case "I":
                    TranserferType = typecode;
                    response = "200 ok";
                    break;
                default:
                    response = "Not supported";
                    break;
            }
            if (formatcontrol != null)
            {

            switch (formatcontrol)
            {
                case "N":
                    response = "200 ok";
                    break;
                default:
                    response = "not supported";
                    break;
            }

            }
            return response;
        }*/
        
    }
}
