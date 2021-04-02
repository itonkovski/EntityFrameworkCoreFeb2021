using System;
using System.Xml.Serialization;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray]
        public ExportPurchaseDto[] Purchases { get; set; }


        public decimal TotalSpent { get; set; }
    }
}
