using DGame.Web.Services.Contracts;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DGame.Web.Services
{
    public class StorageService : IStorageService
    {
        private readonly string remoteHostIP = "192.168.0.104";
        private readonly int targetPort = 333;

        public string TempStoragePath { get; set; }

        public void StartFileListener()
        {
            string hostName = Dns.GetHostName();
            var ips = Dns.GetHostByName(hostName).AddressList;
            IPAddress ip = ips[ips.Length - 1];
            int port = 444;

            TcpListener tcpListener = new TcpListener(ip, port);
            tcpListener.Start();
            while (true)
            {
                Socket handlerSocket = tcpListener.AcceptSocket();
                if (handlerSocket.Connected)
                {
                    string fileName = string.Empty;
                    NetworkStream networkStream = new NetworkStream(handlerSocket);
                    int thisRead = 0;
                    int blockSize = 1024;
                    Byte[] dataByte = new Byte[blockSize];
                    lock (this)
                    {
                        string folderPath = $@"{this.TempStoragePath}\";
                        int receivedBytesLen = handlerSocket.Receive(dataByte);

                        int fileNameLen = BitConverter.ToInt32(dataByte, 0);
                        fileName = Encoding.ASCII.GetString(dataByte, 4, fileNameLen);

                        if (!File.Exists(folderPath + fileName))
                        {
                            Stream fileStream = File.OpenWrite(folderPath + fileName);
                            fileStream.Write(dataByte, 4 + fileNameLen, (1024 - (4 + fileNameLen)));
                            while (true)
                            {
                                thisRead = networkStream.Read(dataByte, 0, blockSize);
                                fileStream.Write(dataByte, 0, thisRead);
                                if (thisRead == 0)
                                    break;
                            }
                            fileStream.Close();
                            break;
                        }
                    }

                    handlerSocket = null;
                }
            }
        }

        public byte[] GetFile(string name)
        {
            string hostName = Dns.GetHostName();
            var ips = Dns.GetHostByName(hostName).AddressList;
            IPAddress ip = ips[ips.Length - 1];
            byte[] data = Encoding.UTF8.GetBytes($"get-file -address {ip.ToString()}:{444} -name {name}");
            TcpClient clientSocket = new TcpClient(remoteHostIP, targetPort);
            NetworkStream networkStream = clientSocket.GetStream();
            networkStream.Write(data, 0, data.GetLength(0));

            //byte[] fileData = new byte[1024];
            //networkStream.Read(fileData, 0, 1024);

            //if (fileData.Length > 0)
            //{
            //    var path = $@"{storagePath}\{name}";
            //    File.Create(path).Close();
            //    File.WriteAllBytes(storagePath + name, fileData);

            //    string extractPath = $@"{storagePath}\Extract";
            //    ZipFile.ExtractToDirectory(path, extractPath);
            //}

            networkStream.Close();

            return null;
        }

        public void SaveFile(string filename, string path)
        {
            if (!string.IsNullOrEmpty(remoteHostIP))
            {
                byte[] fileNameByte = Encoding.ASCII.GetBytes(filename);
                byte[] fileData = File.ReadAllBytes(path);
                byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
                fileNameLen.CopyTo(clientData, 0);
                fileNameByte.CopyTo(clientData, 4); fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                TcpClient clientSocket = new TcpClient(remoteHostIP, targetPort);
                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Write(clientData, 0, clientData.GetLength(0));
                networkStream.Close();
            }
        }
    }
}
