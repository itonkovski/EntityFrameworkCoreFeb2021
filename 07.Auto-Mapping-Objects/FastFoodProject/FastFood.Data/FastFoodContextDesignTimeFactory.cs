using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FastFood.Data
{
    public class FastFoodContextDesignTimeFactory : IDesignTimeDbContextFactory<FastFoodContext>
    {
        public FastFoodContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FastFoodContext>();

            builder.UseSqlServer("Server=localhost;Database=FastFood;User Id = SA;Password = Qawsed12;MultipleActiveResultSets=true");

            return new FastFoodContext(builder.Options);
        }
    }
}
