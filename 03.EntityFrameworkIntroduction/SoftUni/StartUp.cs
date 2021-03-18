using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new SoftUniContext();

            //03.EmployeeFullInformation
            //Console.WriteLine(GetEmployeesFullInformation(context));

            //04.EmployeeswithSalaryOver50000
            //Console.WriteLine(GetEmployeesWithSalaryOver50000(context));

            //05.EmployeesFromResearchAndDevelopment
            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));

            //06.AddingANewAddressAndUpdatingEmployee
            //Console.WriteLine(AddNewAddressToEmployee(context));

            //07.EmployeesAndProjects
            //Console.WriteLine(GetEmployeesInPeriod(context));

            //08.AddressesByTown
            //Console.WriteLine(GetAddressesByTown(context));

            //09.Employee147
            //Console.WriteLine(GetEmployee147(context));

            //10.DepartmentsWithMoreThan5Employees
            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));

            //11.FindLatest10Projects
            //Console.WriteLine(GetLatestProjects(context));

            //12.IncreaseSalaries
            //Console.WriteLine(IncreaseSalaries(context));

            //13.FindEmployeesByFirstNameStartingWith"Sa"
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));

            //14.DeleteProjectById
            //Console.WriteLine(DeleteProjectById(context));

            //15.RemoveTown
            //Console.WriteLine(RemoveTown(context));
        }

        //03.EmployeeFullInformation
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    middleName = x.MiddleName,
                    jobTitle = x.JobTitle,
                    salary = x.Salary
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb
                    .AppendLine($"{employee.firstName} {employee.lastName} {employee.middleName} {employee.jobTitle} {employee.salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //04.EmployeeswithSalaryOver50000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Salary > 50000)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    salary = x.Salary
                })
                .OrderBy(x => x.firstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb
                    .AppendLine($"{employee.firstName} - {employee.salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //05.EmployeesFromResearchAndDevelopment
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    departmentName = x.Department.Name,
                    salary = x.Salary
                })
                .OrderBy(x => x.salary)
                .ThenByDescending(x => x.firstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb
                    .AppendLine($"{employee.firstName} {employee.lastName} from {employee.departmentName} - ${employee.salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //06.AddingANewAddressAndUpdatingEmployee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var employee = context.Employees
                .FirstOrDefault(x => x.LastName == "Nakov");

            employee.Address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.SaveChanges();

            var addressesToDisplay = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => new
                {
                    output = x.Address.AddressText
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var address in addressesToDisplay)
            {
                sb
                    .AppendLine($"{address.output}");
            }

            return sb.ToString().TrimEnd();
        }

        //07.EmployeesAndProjects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001
                                                         && p.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    empFirstName = x.FirstName,
                    empLastName = x.LastName,
                    managerFirstName = x.Manager.FirstName,
                    managerLastName = x.Manager.LastName,
                    projects = x.EmployeesProjects.Select(p => new
                    {
                        projectName = p.Project.Name,
                        startDate = p.Project.StartDate,
                        endDate = p.Project.EndDate
                    })
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb
                    .AppendLine($"{employee.empFirstName} {employee.empLastName} - Manager: {employee.managerFirstName} {employee.managerLastName}");

                foreach (var project in employee.projects)
                {
                    var projectsStartDate = project.startDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    var projectsEndDate = project.endDate is null ? "not finished" : project.endDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    sb
                        .AppendLine($"--{project.projectName} - {projectsStartDate} - {projectsEndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08.AddressesByTown
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(x => new
                {
                    adressText = x.AddressText,
                    townName = x.Town.Name,
                    empCount = x.Employees.Count
                })
                .OrderByDescending(x => x.empCount)
                .ThenBy(x => x.townName)
                .ThenBy(x => x.adressText)
                
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var address in addresses)
            {
                sb
                    .AppendLine($"{address.adressText}, {address.townName} - {address.empCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //09.Employee147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees
               .Select(x => new
               {
                   x.EmployeeId,
                   empFirstName = x.FirstName,
                   empLastName = x.LastName,
                   empJobTitle = x.JobTitle,
                   projects = x.EmployeesProjects
                    .Select(p => new
                    {
                        p.Project.Name
                    })
               })
               .FirstOrDefault(x => x.EmployeeId == 147);

            var sb = new StringBuilder();

            sb
                .AppendLine($"{employee147.empFirstName} {employee147.empLastName} - {employee147.empJobTitle}");

            foreach (var project in employee147.projects.OrderBy(x => x.Name))
            {
                sb
                    .AppendLine($"{project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        //10.DepartmentsWithMoreThan5Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    depName = x.Name,
                    managerFirstName = x.Manager.FirstName,
                    managerLastName = x.Manager.LastName,
                    employees = x.Employees
                        .Select( emp => new
                        {
                            employeeFirstName = emp.FirstName,
                            employeeLastName = emp.LastName,
                            employeeJobtitle = emp.JobTitle
                        })
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb
                    .AppendLine($"{dep.depName} - {dep.managerFirstName} {dep.managerLastName}");

                foreach (var emp in dep.employees
                    .OrderBy(e => e.employeeFirstName)
                    .ThenBy(e => e.employeeLastName))
                {
                    sb
                        .AppendLine($"{emp.employeeFirstName} {emp.employeeLastName} - {emp.employeeJobtitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //11.FindLatest10Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    projectName = p.Name,
                    projectDescript = p.Description,
                    projectStartDate = p.StartDate
                })
                .OrderBy(p => p.projectName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb
                    .AppendLine($"{project.projectName}")
                    .AppendLine($"{project.projectDescript}")
                    .AppendLine($"{project.projectStartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        //12.IncreaseSalaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var neededDepartments = new[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employees = context.Employees
                .Where(e => neededDepartments.Contains(e.Department.Name))
                .ToList();

            foreach (var emp in employees)
            {
                emp.Salary *= 1.12M;
            }

            context.SaveChanges();

            var employeesWithUpdatedSalaries = employees
                .Select(e => new
                {
                    empFirstName = e.FirstName,
                    empLastName = e.LastName,
                    empSalary = e.Salary
                })
                .OrderBy(e => e.empFirstName)
                .ThenBy(e => e.empLastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employeesWithUpdatedSalaries)
            {
                sb
                    .AppendLine($"{e.empFirstName} {e.empLastName} (${e.empSalary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //13.FindEmployeesByFirstNameStartingWith"Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.ToLower().StartsWith("sa"))
                .Select(e => new
                {
                    empFirstName = e.FirstName,
                    empLastName = e.LastName,
                    empJobTitle = e.JobTitle,
                    empSalary = e.Salary
                })
                .OrderBy(e => e.empFirstName)
                .ThenBy(e => e.empLastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb
                    .AppendLine($"{emp.empFirstName} {emp.empLastName} - {emp.empJobTitle} - (${emp.empSalary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //14.DeleteProjectById
        public static string DeleteProjectById(SoftUniContext context)
        {
            var idToDelete = 2;
            
            var projectToDeleteFromEmployeerojects = context
                .EmployeesProjects
                .Where(p => p.ProjectId == idToDelete)
                .ToList();

            context.EmployeesProjects
                .RemoveRange(projectToDeleteFromEmployeerojects);

            
            var projectToDeleteFromTableProjects = context
                .Projects
                .Where(x => x.ProjectId == idToDelete)
                .ToList();

            context.Projects
                .RemoveRange(projectToDeleteFromTableProjects);

            context.SaveChanges();
            
            var projects = context
                .Projects
                .Take(10)
                .Select(x => x.Name)
                .ToList();

            var sb = new StringBuilder();

            
            foreach (var p in projects)
            {
                sb.AppendLine($"{p}");
            }

            return sb.ToString().TrimEnd();
        }

        //15.RemoveTown
        public static string RemoveTown(SoftUniContext context)
        {
            var townName = "Seattle";

            var townToDelete = context
                .Towns
                .Where(t => t.Name == townName)
                .FirstOrDefault();

            var addresses = context
                .Addresses
                .Where(a => a.TownId == townToDelete.TownId)
                .ToList();

            foreach (var address in addresses)
            {
                var employees = context
                    .Employees
                    .Where(e => e.AddressId == address.AddressId)
                    .ToList();

                foreach (var emp in employees)
                {
                    emp.AddressId = null;
                }

                context.Addresses.Remove(address);
            }

            context.Towns.Remove(townToDelete);

            context.SaveChanges();

            return $"{addresses.Count()} addresses in {townName} were deleted";
        }
    }
}
