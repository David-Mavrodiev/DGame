namespace DGame.Web.Services.Contracts
{
    public interface IStorageService
    {
        string TempStoragePath { get; set; }

        byte[] GetFile(string filename);

        void SaveFile(string filename, string path);

        void StartFileListener();
    }
}
