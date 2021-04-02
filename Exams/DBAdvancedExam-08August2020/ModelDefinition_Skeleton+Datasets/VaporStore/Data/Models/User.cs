using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
    public class User
    {
        public User()
        {
            this.Cards = new HashSet<Card>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        //text with length [3, 20]
        public string Username { get; set; }

        //two words, consisting of Latin letters.
        //Both start with an upper letter and are followed by lower letters.
        //The two words are separated by a single space (ex. "John Smith") (required)
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        //integer in the range [3, 103] 
        public int Age { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
