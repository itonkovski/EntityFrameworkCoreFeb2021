using System;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("Category")]
    public class CategoriesInputModel
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
