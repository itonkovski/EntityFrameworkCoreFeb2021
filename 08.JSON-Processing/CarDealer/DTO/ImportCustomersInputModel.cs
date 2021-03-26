using System;
namespace CarDealer.DTO
{
    public class ImportCustomersInputModel
    {
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public bool IsYoungDriver { get; set; }
    }
}
