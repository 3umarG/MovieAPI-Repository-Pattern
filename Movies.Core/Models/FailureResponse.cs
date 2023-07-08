using Movies.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models
{
	public class FailureResponse : IResponse
	{
		public bool Status { get; private set; }

		public int StatusCode { get; private set; }

		public string? Message { get; private set; }

		public FailureResponse(int statusCode, string? message = null)
		{
			Status = false;
			StatusCode = statusCode;
			Message = message;
		}
	}
}
