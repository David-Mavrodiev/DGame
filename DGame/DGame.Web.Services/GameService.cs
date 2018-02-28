using DGame.Data.Repository;
using DGame.DataModels;
using DGame.Web.Services.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DGame.Web.Services
{
    public class GameService : IGameService
    {
        private readonly IGenericRepository<Game> gameRepository;
        private readonly IGenericRepository<User> userRepository;
        private readonly IGenericRepository<View> viewRepository;

        public GameService(IGenericRepository<Game> gameRepository, IGenericRepository<User> userRepository,
            IGenericRepository<View> viewRepository)
        {
            this.gameRepository = gameRepository;
            this.userRepository = userRepository;
            this.viewRepository = viewRepository;
        }

        public void Create(string name, string description, string ownerName)
        {
            var user = this.userRepository.All().First(u => u.UserName == ownerName);

            var game = new Game()
            {
                Name = name,
                OwnerId = user.Id,
                Description = description
            };

            this.gameRepository.Add(game);
            this.gameRepository.SaveChanges();
        }

        public Game Get(Guid id)
        {
            var game = this.gameRepository.GetById(id);

            return game;
        }

        public void AddView(Guid gameId, string userName)
        {
            var view = new View()
            {
                GameId = gameId,
                UserName = userName
            };

            this.viewRepository.Add(view);
            this.viewRepository.SaveChanges();
        }

        public IQueryable<Game> GetTop(int count)
        {
            var games = this.gameRepository.All().OrderBy(g => g.Views.Count).Take(count);

            return games;
        }
    }
}
