using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportCardDto
    {
        [Required]
        [RegularExpression("[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}")]
        //text, which consists of 4 pairs of 4 digits,
        //separated by spaces (ex. “1234 5678 9012 3456”) 
        public string Number { get; set; }

        [Required]
        [RegularExpression("[0-9]{3}")]
        //which consists of 3 digits (ex. “123”) 
        public string CVC { get; set; }

        [Required]
        public CardType? Type { get; set; }
    }
}