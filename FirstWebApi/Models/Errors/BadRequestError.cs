using System.Net;

namespace MoviesApi.Models.Errors
{
    public class BadRequestError : ApiError
    {
        public BadRequestError() : base(400, "Bad Request !!")
        {
        }

        public BadRequestError(string message) : base(400, "Bad Request !!", message) { }
    }
}
