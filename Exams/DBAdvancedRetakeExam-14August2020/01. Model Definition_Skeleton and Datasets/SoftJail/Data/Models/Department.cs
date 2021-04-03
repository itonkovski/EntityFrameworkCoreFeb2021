using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Department
    {
        public Department()
        {
            this.Cells = new HashSet<Cell>();
        }

        [Key]
        public int Id { get; set; }

        //text with min length 3 and max length 25
        [Required]
        public string Name { get; set; }

        public ICollection<Cell> Cells { get; set; }

        // TODO Add prisoners collection
    }
}
