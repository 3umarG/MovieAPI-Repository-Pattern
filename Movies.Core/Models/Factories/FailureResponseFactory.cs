using Movies.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models.Factories
{
    public class FailureResponseFactory : IResponseFactory
    {
        public int StatusCode { get; private set; }

        public string? Message { get; private set; }

        public FailureResponseFactory(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public IResponse Create()
        {
            return new FailureResponse(StatusCode, Message);
        }
    }
}
