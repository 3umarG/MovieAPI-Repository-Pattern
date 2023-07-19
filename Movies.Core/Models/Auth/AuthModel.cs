using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models.Auth
{
	public class AuthModel
	{
		public string UserName { get; set; }
		public string Email { get; set; }

		public string Message { get; set; }

		public bool IsAuthed { get; set; }

		public string Token { get; set; }

		[JsonIgnore]
		public string? RefreshToken { get; set; }

		public DateTime AccessTokenExpiration { get; set; }

		public DateTime RefreshTokenExpiration { get; set; }


	}
}
