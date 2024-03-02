namespace CardService.Domain.Entities
{
    public abstract class BaseEntity
    {
        long _Id;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public virtual long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
    }
}
