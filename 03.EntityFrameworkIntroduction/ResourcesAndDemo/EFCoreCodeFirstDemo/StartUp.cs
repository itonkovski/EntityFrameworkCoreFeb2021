using System;
using EFCoreCodeFirstDemo.Models;

namespace EFCoreCodeFirstDemo
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new SliDoDbContext();
            db.Database.EnsureCreated();
        }
    }
}
