using System;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        // text, consisting only of letters, spaces and numbers,
        //which ends with “ str.”
        //(required) (Example: “62 Muir Hill str.“)
        [Required]
        public string Address { get; set; }

        public int PrisonerId { get; set; }

        public Prisoner Prisoner { get; set; }
    }
}
