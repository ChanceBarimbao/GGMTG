using System;
using System.ComponentModel.DataAnnotations;

namespace GGMTG.Server.Models
{
    public class Card
    {
        [Key]
        public Guid Id { get; set; } // Make sure it's marked as primary key with Guid type
        public string Name { get; set; }
        public string SetCode { get; set; }
        public string SetName { get; set; }
        public string ManaCost { get; set; }
        public string TypeLine { get; set; }
        public string OracleText { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string Rarity { get; set; }
        public string Artist { get; set; }
        public string ImageUrl { get; set; }
        public string ScryfallUri { get; set; }
        public DateTime CreatedAt { get; set; } // Timestamp field
    }

}
