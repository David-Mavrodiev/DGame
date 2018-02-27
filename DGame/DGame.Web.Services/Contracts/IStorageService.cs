namespace DGame.Web.Services.Contracts
{
    public interface IStorageService
    {
        byte[] GetFile();

        void SaveFile(string filename, byte[] fileData);
    }
}
