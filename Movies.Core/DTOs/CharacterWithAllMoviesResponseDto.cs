using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.DTOs
{
	public record CharacterWithAllMoviesResponseDto
	{
		public int Id { get; set; }
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTime BirthDate { get; set; }

		public List<MovieResponseDto> Movies { get; set; }
	}
}
