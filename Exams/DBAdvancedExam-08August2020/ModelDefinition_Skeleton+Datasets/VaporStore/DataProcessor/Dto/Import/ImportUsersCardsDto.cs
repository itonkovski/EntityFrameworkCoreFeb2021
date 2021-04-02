using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUsersCardsDto
	{
        [Required]
        [RegularExpression("^[A-Z][a-z]{2,} [A-Z][a-z]{2,}$")]
        //two words, consisting of Latin letters.
        //Both start with an upper letter and are followed by lower letters.
        //The two words are separated by a single space (ex. "John Smith") (required)
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        //text with length [3, 20]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Range(3, 103)]
        //integer in the range [3, 103] 
        public int Age { get; set; }

        public IEnumerable<ImportCardDto> Cards { get; set; }

    }
}
