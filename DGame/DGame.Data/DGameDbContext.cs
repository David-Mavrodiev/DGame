using DGame.Data.Contracts;
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
        }

        public IDbSet<Peer> Peers { get; set; }

        public static DGameDbContext Create()
        {
            return new DGameDbContext();
        }
    }
}
