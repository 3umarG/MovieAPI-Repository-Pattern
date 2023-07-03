using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MoviesApi.Models.Errors
{
	public class NotFoundError : ApiError 
	{
		public NotFoundError() : base(404, "Not Found Error !!")
		{
		}

		public NotFoundError(string message) : base(404, "Not Found Error !!" , message)
		{
		}
	}
}
