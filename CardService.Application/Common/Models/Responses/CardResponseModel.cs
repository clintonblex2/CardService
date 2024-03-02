using CardService.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CardService.Application.Common.Models.Responses
{
    public class CardResponseModel
    {
        public long CardId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
