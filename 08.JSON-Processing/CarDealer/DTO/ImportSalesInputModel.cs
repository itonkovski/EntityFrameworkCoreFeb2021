using System;
namespace CarDealer.DTO
{
    public class ImportSalesInputModel
    {
        public int CarId { get; set; }

        public int CustomerId { get; set; }

        public decimal Discount { get; set; }
    }
}
