using System;
using System.ComponentModel.DataAnnotations;

namespace EFCoreCodeFirstDemo.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
