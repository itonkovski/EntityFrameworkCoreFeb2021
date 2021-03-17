using System;
using System.Linq;
using Demo.Models;

namespace Demo
{
    //Microsoft.EntityFrameworkCore.SqlServer
    //Microsoft.EntityFrameworkCore.Design
    //dotnet ef dbcontext scaffold "Server=localhost;User Id = SA;Password = Qawsed12;Database=SoftUni" Microsoft.EntityFrameworkCore.SqlServer -o Models
    //dotnet tool install --global dotnet-ef

    public class StartUp
    {
        static void Main(string[] args)
        {
            var db = new SoftUniContext();

            //db.Towns.Add(new Town { Name = "Pernik" });
            //db.SaveChanges();

            var employees = db.Employees
                .Where(x => x.FirstName.StartsWith("N"))
                .OrderByDescending(x => x.Salary)
                .Select(x => new { x.FirstName, x.LastName, x.Salary })
                .ToList();

            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName} - {employee.Salary}");
            }

            var departments = db.Employees
                .GroupBy(x => x.Department.Name)
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .ToList();

            foreach (var department in departments)
            {
                Console.WriteLine($"{department.Name} => {department.Count}");
            }
        }
    }
}
