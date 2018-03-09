using DGame.Data.Repository;
using DGame.DataModels;
using DGame.Web.Services.Contracts;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Linq;
using System.Numerics;
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
        private readonly string abi = @"[{'constant':true,'inputs':[],'name':'getBalance','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'transferFunds','outputs':[],'payable':true,'stateMutability':'payable','type':'function'},{'constant':false,'inputs':[{'name':'name','type':'string'},{'name':'addr','type':'address'}],'name':'addGame','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'addr','type':'address'}],'name':'getViews','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'name','type':'string'},{'name':'viewer','type':'address'}],'name':'addViewToGame','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'transfer','outputs':[],'payable':true,'stateMutability':'payable','type':'function'},{'constant':true,'inputs':[],'name':'owner','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'addr','type':'address'}],'name':'getFunds','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[],'payable':true,'stateMutability':'payable','type':'constructor'}]";
        
        HexBigInteger gasLimit = new HexBigInteger(0xC3500);
        HexBigInteger gasPrice = new HexBigInteger(0x4A817C800);
        HexBigInteger value = new HexBigInteger(0xB1A2BC2EC50000);

        public GameService(IGenericRepository<Game> gameRepository, IGenericRepository<User> userRepository,
            IGenericRepository<View> viewRepository)
        {
            this.gameRepository = gameRepository;
            this.userRepository = userRepository;
            this.viewRepository = viewRepository;
            this.web3 = new Nethereum.Web3.Web3();

            web3.TransactionManager.DefaultGasPrice = new BigInteger(20000000000);
            web3.TransactionManager.DefaultGas = new BigInteger(1);
            
        }

        public void Create(string name, string description, string ownerName)
        {
            var unlockResult = this.web3.Personal.UnlockAccount.SendRequestAsync(this.address, this.password, 100).Result;

            this.gameContract = web3.Eth.GetContract(abi, contractAddress);

            var createGameFunction = this.gameContract.GetFunction("addGame");
            var addViewFunction = this.gameContract.GetFunction("addViewToGame");

            var user = this.userRepository.All().First(u => u.UserName == ownerName);

            var res = createGameFunction.SendTransactionAsync(this.address, gasLimit, gasPrice, value, name, user.WalletAddress).ConfigureAwait(false).GetAwaiter().GetResult();

            var res2 = addViewFunction.SendTransactionAsync(this.address, gasLimit, gasPrice, value, name, user.WalletAddress).ConfigureAwait(false).GetAwaiter().GetResult();

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

            var res = addViewFunction.SendTransactionAsync(this.address, gasLimit, gasPrice, value, game.Name, user.WalletAddress).ConfigureAwait(false).GetAwaiter().GetResult();

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
