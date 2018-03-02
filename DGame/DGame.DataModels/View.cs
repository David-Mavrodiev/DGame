using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGame.DataModels
{
    public class View
    {
        public View()
        {
            this.Date = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public DateTime Date { get; set; }


        public Guid GameId { get; set; }

        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }

        public bool IsUsed { get; set; }
    }
}
