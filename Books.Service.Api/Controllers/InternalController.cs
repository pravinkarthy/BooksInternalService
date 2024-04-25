using Books.Service.Internal.Api.Infrastructure;
using Books.Service.Internal.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using NSwag.Annotations;
using System.Net.Mime;

namespace Books.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternalController : ControllerBase
{

    private readonly ILogger<InternalController> _logger;
    private readonly IBookDataService _bookService;

    public InternalController(ILogger<InternalController> logger, IBookDataService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    [HttpGet]
    [Authorize]
    [Route("books/{searchTerm}")]
    [ProducesResponseType(typeof(PaginatedList<BookInfoModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<ActionResult> Books(string searchTerm, int? pageIndex, int? pageSize)
    {
        var resultList = await _bookService.SearchBooks(searchTerm, pageIndex, pageSize);

        if (!resultList.IsSuccess())
        {
            return new ObjectResult(resultList.Error)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        return Ok(resultList.Data);
    }
}

