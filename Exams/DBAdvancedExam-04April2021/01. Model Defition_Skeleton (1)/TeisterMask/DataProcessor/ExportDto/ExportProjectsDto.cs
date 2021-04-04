using System;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ExportProjectsDto
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        public string ProjectName { get; set; }

        public string HasEndDate { get; set; }

        [XmlArray("Tasks")]
        public TaskDto[] Tasks { get; set; }
    }

    [XmlType("Task")]
    public class TaskDto
    {
        public string Name { get; set; }

        public string Label { get; set; }
    }
}
