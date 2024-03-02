namespace CardService.Application.Common.Models.Requests
{
    public class CardFilter
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? SortBy { get; set; }
    }
}
