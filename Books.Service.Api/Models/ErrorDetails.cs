using System;
using Microsoft.AspNetCore.Http;

namespace Books.Service.Internal.Api.Models
{
	public class ErrorDetails
	{
        public string Message { get; }
        public string Detail { get; }

        public ErrorDetails(string message, string detail)
		{
			Message = message;
            Detail = detail;
		}
    }
}

