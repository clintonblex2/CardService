using CardService.Application.Common.Enums;

namespace CardService.Application.Common.Models.Responses
{
    public class PagedList
    {
        const int maxPageSize = 50;
        private int _pageSize = 10;
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public ResponseCodes ResponseCode { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > maxPageSize ? maxPageSize : value;
        }



        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;

        public PagedList()
        {

        }
    }

    public class PagedList<T> : PagedList
    {
        public PagedList()
        {
        }

        public static PagedList<T> Success(List<T> data, string message = null)
        {
            return new PagedList<T>()
            {
                IsSuccessful = true,
                Message = message ?? "Request was Successful",
                Data = data,
                ResponseCode = ResponseCodes.SUCCESS
            };
        }

        public static PagedList<T> Failure(string message = null)
        {
            return new PagedList<T>()
            {
                IsSuccessful = false,
                Message = message ?? "Request failed",
                ResponseCode = ResponseCodes.BAD_REQUEST
            };
        }

        internal PagedList(IQueryable<T> query, int page, int pageSize)
        {
            IsSuccessful = true;
            CurrentPage = page;
            PageSize = pageSize;
            TotalCount = query.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
            Data = query.Skip(page > 0 ? (page - 1) * pageSize : 0).Take(pageSize).ToList();
            Message = "Successful";
        }

        public List<T> Data { get; set; }
    }
}
