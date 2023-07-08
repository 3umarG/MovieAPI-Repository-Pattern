using Movies.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models
{
	public class SuccessResponse<T> : IResponse where T : class
	{
		public bool Status { get; private set; }

		public int StatusCode { get; private set; }

		public string? Message { get; private set; }

		public T Data { get; set; }

		public SuccessResponse(int statusCode, T data, string? message = null)
		{
			Status = true;
			Message = message;
			StatusCode = statusCode;
			Data = data;
		}


	}
}
