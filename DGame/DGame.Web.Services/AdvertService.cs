using DGame.Data.Repository;
using DGame.DataModels;
using DGame.Web.Services.Contracts;
using System.Linq;

namespace DGame.Web.Services
{
    public class AdvertService : IAdvertService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly IGenericRepository<Advert> advertRepository;

        public AdvertService(IGenericRepository<User> userRepository, IGenericRepository<Advert> advertRepository)
        {
            this.userRepository = userRepository;
            this.advertRepository = advertRepository;
        }

        public void Create(string transactionHash, string creatorName)
        {
            var user = this.userRepository.All().First(u => u.UserName == creatorName);

            var advert = new Advert()
            {
                CreatorId = user.Id,
                TransactionHash = transactionHash
            };

            this.advertRepository.Add(advert);
            this.advertRepository.SaveChanges();
        }
    }
}
