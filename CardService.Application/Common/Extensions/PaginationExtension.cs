using CardService.Application.Common.Models.Responses;

namespace CardService.Application.Common.Extensions
{
    public static class PaginationExtension
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 20;

            return new PagedList<T>(query, pageIndex, pageSize);
        }
    }
}
