using DGame.Data.Repository;
using DGame.DataModels;
using DGame.Web.Services.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Linq;

namespace DGame.Web.Services
{
    public class AdvertService : IAdvertService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly IGenericRepository<Advert> advertRepository;
        private readonly Web3 web3;

        public AdvertService(IGenericRepository<User> userRepository, IGenericRepository<Advert> advertRepository)
        {
            this.userRepository = userRepository;
            this.advertRepository = advertRepository;
            this.web3 = new Web3("https://ropsten.infura.io/zRqjiU4v9RI7ixoO2rvh");
        }

        public void Create(string transactionHash, string creatorName, string filename, string link)
        {
            var user = this.userRepository.All().First(u => u.UserName == creatorName);

            var advert = new Advert()
            {
                CreatorId = user.Id,
                TransactionHash = transactionHash,
                FileName = filename,
                Link = link
            };

            this.advertRepository.Add(advert);
            this.advertRepository.SaveChanges();
        }

        public IQueryable<Advert> GetAll()
        {
            this.RemoveExpiredAdverts();

            return this.advertRepository.All().Where(a => !a.IsExpired);
        }

        private void RemoveExpiredAdverts()
        {
            var adverts = this.advertRepository.All().Where(a => !a.IsExpired).ToList();

            foreach (var advert in adverts)
            {
                if ((DateTime.Now - advert.DateCreated).TotalDays > 30)
                {
                    var expiredAdvert = this.advertRepository.GetById(advert.Id);
                    expiredAdvert.IsExpired = true;
                    this.advertRepository.Update(expiredAdvert);
                    this.advertRepository.SaveChanges();
                }
            }
        }

        public bool IsAdvertPayed(string transactionHash)
        {
            if (transactionHash == null || transactionHash == string.Empty)
            {
                return false;
            }

            var receipt = this.web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;
            System.Numerics.BigInteger flag = new System.Numerics.BigInteger(1);
            HexBigInteger success = new HexBigInteger(flag);

            if (receipt != null && receipt.Status.Value == success.Value)
            {
                return true;
            }

            return false;
        }
    }
}
