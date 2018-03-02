using DGame.DataModels;
using System.Linq;

namespace DGame.Web.Services.Contracts
{
    public interface IAdvertService
    {
        void Create(string transactionHash, string creatorName, string filename, string link);

        bool IsAdvertPayed(string transactionHash);

        IQueryable<Advert> GetAll();
    }
}
