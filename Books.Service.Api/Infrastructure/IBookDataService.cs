using Books.Service.Internal.Api.Models;

namespace Books.Service.Internal.Api.Infrastructure
{
	public interface IBookDataService
    {

        Task<Result<PaginatedList<BookInfoModel>, ErrorDetails>> SearchBooks(string searchQuery, int? pageIndex, int? pageSize);
    }
}

