using DGame.Data.Repository;
using DGame.DataModels;
using DGame.Web.Services.Contracts;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DGame.Web.Services
{
    public class GameService : IGameService
    {
        private readonly IGenericRepository<Game> gameRepository;
        private readonly IGenericRepository<User> userRepository;
        private readonly IGenericRepository<View> viewRepository;
        private readonly Web3 web3;
        private Contract gameContract;
        private readonly string address = WebConfigurationManager.AppSettings["address"];
        private readonly string password = WebConfigurationManager.AppSettings["password"];
        private readonly string contractAddress = WebConfigurationManager.AppSettings["contract"];
        private readonly string abi = @"[{'constant':false,'inputs':[],'name':'transferFunds','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'name','type':'string'},{'name':'addr','type':'address'}],'name':'addGame','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'name','type':'string'},{'name':'viewer','type':'address'}],'name':'addViewToGame','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'transfer','outputs':[],'payable':true,'stateMutability':'payable','type':'function'},{'constant':true,'inputs':[],'name':'owner','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'addr','type':'address'}],'name':'getFunds','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[],'payable':true,'stateMutability':'payable','type':'constructor'}]";

        public GameService(IGenericRepository<Game> gameRepository, IGenericRepository<User> userRepository,
            IGenericRepository<View> viewRepository)
        {
            this.gameRepository = gameRepository;
            this.userRepository = userRepository;
            this.viewRepository = viewRepository;
            this.web3 = new Web3();
        }

        public void Create(string name, string description, string ownerName)
        {
            var unlockResult = this.web3.Personal.UnlockAccount.SendRequestAsync(this.address, this.password, 100).Result;

            this.gameContract = web3.Eth.GetContract(abi, contractAddress);

            var createGameFunction = this.gameContract.GetFunction("addGame");
            var addViewFunction = this.gameContract.GetFunction("addViewToGame");

            var user = this.userRepository.All().First(u => u.UserName == ownerName);

            createGameFunction.SendTransactionAsync(this.address, name, user.WalletAddress);
            addViewFunction.SendTransactionAsync(this.address, name, user.WalletAddress);

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
            var unlockResult = this.web3.Personal.UnlockAccount.SendRequestAsync(this.address, this.password, 100).Result;

            this.gameContract = this.web3.Eth.GetContract(abi, contractAddress);

            var game = this.gameRepository.GetById(gameId);
            var user = this.userRepository.All().First(u => u.UserName == userName);

            var addViewFunction = this.gameContract.GetFunction("addViewToGame");

            addViewFunction.SendTransactionAsync(this.address, game.Name, user.WalletAddress);

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

        public IQueryable<Game> GetByPattern(string pattern)
        {
            return this.gameRepository.All().Where(g => g.Name.Contains(pattern));
        }

        public int GetCountOfNewViews(string userId)
        {
            var games = this.gameRepository.All().Where(g => g.OwnerId == userId).ToList();
            int count = 0;

            foreach (var game in games)
            {
                count += game.Views.Where(v => !v.IsUsed).Count();
            }

            return count;
        }
    }
}
