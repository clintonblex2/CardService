using CardService.Domain.Enums;

namespace CardService.Domain.Entities
{
    public class CardEntity : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public Status Status { get; set; }
        public long UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
