using System;
using System.ComponentModel.DataAnnotations;

namespace DGame.DataModels
{
    public class Peer
    {
        [Key]
        public Guid Id { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }
    }
}
