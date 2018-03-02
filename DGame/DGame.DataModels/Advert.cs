using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGame.DataModels
{
    public class Advert
    {
        public Advert()
        {
            this.DateCreated = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public virtual User Creator { get; set; }

        public DateTime DateCreated { get; set; }

        public string TransactionHash { get; set; }

        public string FileName { get; set; }

        public string Link { get; set; }

        public bool IsExpired { get; set; }
    }
}
