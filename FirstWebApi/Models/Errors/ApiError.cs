using Newtonsoft.Json;

namespace MoviesApi.Models.Errors
{
    public class ApiError
    {
        public bool Status { get; } = false;
        public int StatusCode { get; private set; }

        /// NotFound , BadRequest , Created ...
        public string StatusDescription { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; private set; }

        public ApiError(int statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public ApiError(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            Message = message;
        }
    }
}
