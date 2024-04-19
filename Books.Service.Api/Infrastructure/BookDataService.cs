using System;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Books.Service.Internal.Api.Infrastructure.Services;
using AutoMapper;
using Books.Service.Internal.Api.Models;
using Books.Service.Internal.Api.Interceptors;

namespace Books.Service.Internal.Api.Infrastructure
{
    public class BookDataService : IBookDataService
    {
        private static string _apiKey;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        private readonly ILogger<BookDataService> _logger;
        private string _apikey;

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public BookDataService(HttpClient client, ICacheService cacheService, IMapper mapper, IOptions<APISetting> options, ILogger<BookDataService> logger)
        {
            _client = client;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;
            _apikey = options?.Value?.apikey;
        }

        public async Task<Result<PaginatedList<BookInfoModel>, ErrorDetails>> SearchBooks(string searchQuery, int? pageIndex, int? pageSize)
        { 
            try
            {
                pageIndex = pageIndex.HasValue ? pageIndex : 1;
                pageSize = pageSize.HasValue ? (pageSize.Value > 10 ? 10 : pageSize) : 10;

                var cacheData = GetBooksFromCache();

                if (cacheData == null)
                {
                    try
                    {
                        await semaphore.WaitAsync();

                        cacheData = GetBooksFromCache();

                        if (cacheData == null)
                        {
                            var queryText = pageIndex.Value > 1 ? $"q={searchQuery}&startIndex={(pageIndex-1)*pageSize}&maxResults={pageSize}" : $"q={searchQuery}";
                            //_client.BaseAddress = new Uri(_client.BaseAddress.AbsolutePath + queryText;
                            var result = await _client.GetFromJsonAsync<Volumes>($"?key={_apikey}&{queryText}");

                            if (result != null)
                            {
                                cacheData = result;
                            }

                            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                            _ = _cacheService.SetData<Volumes>($"books-{searchQuery}-{(pageIndex - 1)}", cacheData, expirationTime);
                        }
                    }
                    finally { semaphore.Release(); }

                }
                var bookResults = cacheData?.Items?.Take(pageSize.Value).ToList();

                var mappedResults = _mapper.Map<List<BookInfoModel>>(bookResults);

                var bookResultsCount = cacheData.TotalItems.Value;
                var totalPages = (int)Math.Ceiling(bookResultsCount / (double)pageSize);

                return new PaginatedList<BookInfoModel>(mappedResults, pageIndex.Value, totalPages, bookResultsCount);


                Volumes GetBooksFromCache()
                {
                    return _cacheService.GetData<Volumes>($"books-{searchQuery}-{pageIndex - 1}") ?? null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal Error in Service: " + ex.ToString());
                return new ErrorDetails("Error in Platform processing", ex.Message);
            }
        }

    }
}

