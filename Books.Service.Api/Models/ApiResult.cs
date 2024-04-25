using System;
using Microsoft.AspNetCore.Http;

namespace Books.Service.Internal.Api.Models
{
    [Serializable]
	public class Result<TData, TError>
	{
		private readonly bool _isSuccess;
        public TData Data { get; }
        public TError Error { get; }

        public Result(TData data)
		{
			_isSuccess = true;
			Data = data;
            Error = default;
		}


        public Result(TError error)
        {
            Data = default;
            Error = error;
        }

        public bool IsSuccess() => _isSuccess;

        public static implicit operator Result<TData, TError>(TData success) => new (success);
        public static implicit operator Result<TData, TError>(TError error) => new (error);
    }
}

