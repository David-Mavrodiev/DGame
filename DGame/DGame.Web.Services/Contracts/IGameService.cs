using DGame.DataModels;
using System;
using System.Linq;

namespace DGame.Web.Services.Contracts
{
    public interface IGameService
    {
        void Create(string name, string description, string owner);

        Game Get(Guid id);

        void AddView(Guid gameId, string userName);

        IQueryable<Game> GetTop(int count);
    }
}
