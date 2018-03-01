using DGame.Data.Contracts;
using DGame.Data.Migrations;
using DGame.DataModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DGame.Data
{
    public class DGameDbContext : IdentityDbContext<User>, IDGameDbContext
    {
        public DGameDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DGameDbContext, Configuration>("DefaultConnection"));
        }

        public IDbSet<Game> Games { get; set; }

        public IDbSet<View> Views { get; set; }

        public IDbSet<Advert> Adverts { get; set; }

        public static DGameDbContext Create()
        {
            return new DGameDbContext();
        }
    }
}
