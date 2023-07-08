using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
	public interface IResponse
	{
		public bool Status { get; }

		public int StatusCode { get; }

        public string? Message { get; }

    }
}
