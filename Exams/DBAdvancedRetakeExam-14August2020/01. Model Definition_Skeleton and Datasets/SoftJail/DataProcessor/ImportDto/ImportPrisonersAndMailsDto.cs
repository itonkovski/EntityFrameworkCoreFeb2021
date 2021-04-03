using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonersAndMailsDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        //text with min length 3 and max length 20 
        public string FullName { get; set; }

        [Required]
        [RegularExpression("^(The\\s)([A-Z]{1}[a-z]*)$")]
        //text starting with "The " and a single word only of letters with an uppercase letter for beginning
        //(example: The Prisoner)
        public string Nickname { get; set; }

        [Range(18,65)]
        //integer in the range [18, 65]
        public int Age { get; set; }

        [Required]
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        [Range(typeof(decimal), "0.0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }

        public int CellId { get; set; }

        public IEnumerable<MailInputModel> Mails { get; set; }
    }

    public class MailInputModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"^([A-z0-9\s]+ str.)$")]
        // text, consisting only of letters, spaces and numbers,
        //which ends with “ str.”
        //(required) (Example: “62 Muir Hill str.“)
        public string Address { get; set; }
    }
}
