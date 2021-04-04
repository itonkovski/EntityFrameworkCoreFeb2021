namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var projects = new List<Project>();

            var validProjects = XmlConverter
                .Deserializer<XmlImportProjectDto>(xmlString, "Projects");

            foreach (var currentProject in validProjects)
            {
                if (!IsValid(currentProject))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime projectOpenDate;
                var validProjectOpenDate = DateTime.TryParseExact(
                    currentProject.OpenDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out projectOpenDate);

                if (!validProjectOpenDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? projectDueDate;

                if (!string.IsNullOrWhiteSpace(currentProject.DueDate))
                {
                    DateTime nonNullableDate;
                    var isDueDateProjectValid = DateTime.TryParseExact(
                        currentProject.DueDate,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out nonNullableDate);

                    if (!isDueDateProjectValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    projectDueDate = nonNullableDate;
                }
                else
                {
                    projectDueDate = null;
                }

                var project = new Project
                {
                    Name = currentProject.Name,
                    OpenDate = projectOpenDate,
                    DueDate = projectDueDate
                };

                //We need to validate the tasks as well

                foreach (var taskDto in currentProject.Tasks)
                {
                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskOpenDate;
                    var validTaskOpenDate = DateTime.TryParseExact(
                        taskDto.OpenDate,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out taskOpenDate);

                    if (!validTaskOpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskDueDate;
                    var validTaskDueDate = DateTime.TryParseExact(
                        taskDto.DueDate,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out taskDueDate);

                    if (!validTaskDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpenDate < projectOpenDate || taskDueDate > projectDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType
                    };

                    project.Tasks.Add(task);
                }

                projects.Add(project);
                sb
                    .AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count()));
            }
            context.Projects.AddRange(projects);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var employees = new List<Employee>();

            var validEmployees = JsonConvert
                .DeserializeObject<JsonImportEmployeeDto[]>(jsonString);

            foreach (var currentEmployee in validEmployees)
            {
                if (!IsValid(currentEmployee))
                {
                    sb
                        .AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee
                {
                    Username = currentEmployee.Username,
                    Email = currentEmployee.Email,
                    Phone = currentEmployee.Phone
                };

                foreach (var task in currentEmployee.Tasks.Distinct())
                {
                    var validTask = context.Tasks.FirstOrDefault(x => x.Id == task);

                    if (validTask == null)
                    {
                        sb
                            .AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask
                    {
                        Employee = employee,
                        Task = validTask
                    });
                }
                employees.Add(employee);
                sb
                    .AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count()));
            }
            context.Employees.AddRange(employees);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}