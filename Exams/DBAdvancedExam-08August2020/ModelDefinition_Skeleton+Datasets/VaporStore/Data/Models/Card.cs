using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Purchases = new HashSet<Purchase>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        //text, which consists of 4 pairs of 4 digits,
        //separated by spaces (ex. “1234 5678 9012 3456”) 
        public string Number { get; set; }

        [Required]
        //which consists of 3 digits (ex. “123”) 
        public string Cvc { get; set; }

        public CardType Type { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }
}