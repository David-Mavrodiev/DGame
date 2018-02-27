using DGame.Web.Services.Contracts;

namespace DGame.Web.Services
{
    public class StorageService : IStorageService
    {
        public byte[] GetFile()
        {
            return null;
        }

        public void SaveFile(string filename, byte[] fileData)
        {
            //if (!string.IsNullOrEmpty(remoteHostIP))
            //{
            //    byte[] fileNameByte = Encoding.ASCII.GetBytes(shortFileName);
            //    byte[] fileData = File.ReadAllBytes(longFileName);
            //    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
            //    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
            //    fileNameLen.CopyTo(clientData, 0);
            //    fileNameByte.CopyTo(clientData, 4); fileData.CopyTo(clientData, 4 + fileNameByte.Length);
            //    TcpClient clientSocket = new TcpClient(remoteHostIP, remoteHostPort);
            //    NetworkStream networkStream = clientSocket.GetStream();
            //    networkStream.Write(clientData, 0, clientData.GetLength(0));
            //    networkStream.Close();
            //}
        }
    }
}
