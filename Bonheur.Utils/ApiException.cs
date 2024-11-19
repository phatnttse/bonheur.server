using Microsoft.AspNetCore.Http;  
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bonheur.Utils
{

    public class ApiException : Exception
    {
        private HttpStatusCode _httpStatusCode;
        public HttpStatusCode StatusCode { get => _httpStatusCode; }

        public ApiException() : base() { }

        public ApiException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message)
        {
            _httpStatusCode = statusCode;
        }
    }

}