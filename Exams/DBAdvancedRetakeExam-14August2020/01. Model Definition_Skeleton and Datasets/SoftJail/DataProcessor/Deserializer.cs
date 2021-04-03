namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System; 
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var departments = new List<Department>();

            var departmentsCells = JsonConvert
                .DeserializeObject<IEnumerable<ImportDepartmentAndCellDto>>(jsonString);

            foreach (var departmentCell in departmentsCells)
            {
                if (!IsValid(departmentCell) ||
                    !departmentCell.Cells.All(IsValid) ||
                    !departmentCell.Cells.Any())
                {
                    sb
                        .AppendLine("Invalid Data");
                    continue;
                }

                //Simple way of validating the cells collection
                //foreach (var item in departmentCell.Cells)
                //{
                //    if (!IsValid(item))
                //    {
                //        sb
                //            .AppendLine("Invalid Data");
                //    }
                //}

                var department = new Department
                {
                    Name = departmentCell.Name,
                    Cells = departmentCell.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    })
                    .ToList()
                };

                departments.Add(department);

                sb
                    .AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var prisoners = new List<Prisoner>();

            var prisonersMails = JsonConvert
                .DeserializeObject<IEnumerable<ImportPrisonersAndMailsDto>>(jsonString);

            foreach (var currentPrisoner in prisonersMails)
            {
                if (!IsValid(currentPrisoner) ||
                    !currentPrisoner.Mails.All(IsValid))
                {
                    sb
                        .AppendLine("Invalid Data");
                    continue;
                }

                var isValidReleaseDate = DateTime
                    .TryParseExact(currentPrisoner.ReleaseDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime releaseDate);

                var incarcerationDate = DateTime
                    .ParseExact(currentPrisoner.IncarcerationDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);

                var prisoner = new Prisoner
                {
                    FullName = currentPrisoner.FullName,
                    Nickname = currentPrisoner.Nickname,
                    Age = currentPrisoner.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = isValidReleaseDate ? (DateTime?)releaseDate : null,
                    Bail = currentPrisoner.Bail,
                    CellId = currentPrisoner.CellId,
                    Mails = currentPrisoner.Mails.Select(m => new Mail
                    {
                        Description = m.Description,
                        Sender = m.Sender,
                        Address = m.Address
                    })
                    .ToList()
                };

                prisoners.Add(prisoner);

                sb
                    .AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(prisoners); 
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var validOfficers = new List<Officer>();

            var officerPrisoners = XmlConverter
                .Deserializer<ImportOfficersPrisonersDto>(xmlString, "Officers");

            foreach (var currentOfficer in officerPrisoners)
            {
                if (!IsValid(currentOfficer))
                 {
                    sb
                        .AppendLine("Invalid Data");
                    continue;
                }

                var officer = new Officer
                {
                    FullName = currentOfficer.Name,
                    Salary = currentOfficer.Money,
                    Position = Enum.Parse<Position>(currentOfficer.Position),
                    Weapon = Enum.Parse<Weapon>(currentOfficer.Weapon),
                    DepartmentId = currentOfficer.DepartmentId,
                    OfficerPrisoners = currentOfficer.Prisoners.Select(x => new OfficerPrisoner
                    {
                        PrisonerId = x.Id
                    })
                    .ToList()
                };

                validOfficers.Add(officer);

                sb
                    .AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }
            context.Officers.AddRange(validOfficers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}