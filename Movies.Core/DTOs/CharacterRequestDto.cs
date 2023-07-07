using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.DTOs
{
	public record CharacterRequestDto
	{

		//public Name CharacterName { get; set; }
		public string FirstName { get; set; }

		public string? LastName { get; set; }

		public DateTime BirthDate { get; set; }


	}
}
