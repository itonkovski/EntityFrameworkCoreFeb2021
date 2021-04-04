namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context
                .Projects
                .ToArray()
                .Where(p => p.Tasks.Count > 0)
                .Select(p => new ExportProjectsDto
                {
                    TasksCount = p.Tasks.Count(),
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks.Select(t => new TaskDto
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToArray();

            var result = XmlConverter.Serialize(projects, "Projects");
            return result;
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var topEmployees = context
                .Employees
                .ToArray()
                .Where(x => x.EmployeesTasks.Any(y => y.Task.OpenDate >= date))
                .Select(emp => new
                {
                    Username = emp.Username,
                    Tasks = emp.EmployeesTasks
                    .Where(a => a.Task.OpenDate >= date)
                    .OrderByDescending(g => g.Task.DueDate)
                    .ThenBy(g => g.Task.Name)
                    .Select(et => new
                    {
                        TaskName = et.Task.Name,
                        OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = et.Task.LabelType.ToString(),
                        ExecutionType = et.Task.ExecutionType.ToString()
                    })
                    
                    .ToArray()
                })
                .OrderByDescending(e => e.Tasks.Length)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToArray();

            var result = JsonConvert
                .SerializeObject(topEmployees, Formatting.Indented);

            return result;
        }
    }
}