using CardService.Domain.Enums;

namespace CardService.Domain.Entities
{
    public class UserEntity : BaseEntity
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Role Role { get; set; }
    }
}
