using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace FTPServer.Domain
{
    public class ClientConnection
    {
        private TcpClient _controlClient;
        private TcpClient _dataClient;
        private NetworkStream _controlStream;
        private StreamWriter _controlWriter;
        private StreamReader _controlReader;
        private TcpListener _passiveListener;
        private DataConnectionType _dataConnectionType = DataConnectionType.Active;
        private IPEndPoint _dataEndPoint;
        private StreamWriter _dataWriter;
        private StreamReader _dataReader;
        private NetworkStream _dataStream;

        public string Username;
        private string _transerferType;
        private string _currentDirectory;


        public ClientConnection(TcpClient tcpClient)
        {
            _controlClient = tcpClient;
            _controlStream = _controlClient.GetStream();
            _controlWriter = new StreamWriter(_controlStream);
            _controlReader = new StreamReader(_controlStream);

        }

        public void HandleClient(object obj)
        {
            _controlWriter.WriteLine("220 Connected");
            _controlWriter.Flush();

            string line;

            try
            {
                while (!string.IsNullOrWhiteSpace(line = _controlReader.ReadLine()))
                {
                    string response = null;
                    line = line.Trim();
                    string[] command = line.Split(" ");
                    string cmd = command[0].ToUpperInvariant();
                    string arguments = command.Length > 1 ? line.Substring(command[0].Length + 1) : null;
                    if (response == null)
                    {
                        switch (cmd)
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
                        }
                    }
                    if (_controlClient == null || !_controlClient.Connected)
                    {
                        break;
                    }
                    else
                    {
                        _controlWriter.WriteLine(response);
                        _controlWriter.Flush();
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
            pathname = new DirectoryInfo(Path.Combine(_currentDirectory, pathname)).FullName;

            if (_dataConnectionType == DataConnectionType.Active)
            {
                _dataClient = new TcpClient();
                _dataEndPoint = (IPEndPoint) _dataClient.Client.LocalEndPoint;
                _dataClient.BeginConnect(_dataEndPoint.Address, _dataEndPoint.Port, DoList, pathname);
            }
            else
            {
                _passiveListener.BeginAcceptTcpClient(DoList, pathname);
            }
            return "150 opening";
        }

        private void DoList(IAsyncResult result)
        {
            if (_dataConnectionType ==DataConnectionType.Active)
            {
                _dataClient.EndConnect(result);
            }
            else
            {
                _dataClient = _passiveListener.EndAcceptTcpClient(result);
            }
            string pathname = (string) result.AsyncState;
            _dataStream = _dataClient.GetStream();

            _dataReader = new StreamReader(_dataStream, Encoding.ASCII);
            _dataWriter = new StreamWriter(_dataStream, Encoding.ASCII);

        }

        private string Passive(string arguments)
        {
            IPAddress localAddress = ((IPEndPoint) _controlClient.Client.LocalEndPoint).Address;
            _passiveListener = new TcpListener(localAddress, 0);
            _passiveListener.Start();

            IPEndPoint localEndPoint = ((IPEndPoint) _passiveListener.LocalEndpoint);
            byte[] address = localEndPoint.Address.GetAddressBytes();
            short port = (short) localEndPoint.Port;

            byte[] portArray = BitConverter.GetBytes(port);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(portArray);
            _dataConnectionType = DataConnectionType.Passive;
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
            _currentDirectory = directory;
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

        private String Type(string typecode, string formatcontrol)
        {
            string response = "";
            switch (typecode)
            {
                case "A":
                case "I":
                    _transerferType = typecode;
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
        }
        
    }
}
