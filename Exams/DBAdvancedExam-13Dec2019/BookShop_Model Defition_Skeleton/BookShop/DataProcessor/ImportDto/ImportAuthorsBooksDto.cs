using System;
using Newtonsoft.Json;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorsBooksDto
    {
        [JsonProperty("Id")]
        public int? BookId { get; set; }
    }
}
