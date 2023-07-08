using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.DTOs
{
	public class MovieWithAllCharacterResponseDto
	{
		public int ID { get; set; }

		public string Title { get; set; }

		public int Year { get; set; }

		public double Rate { get; set; }

		public string StoryLine { get; set; }


		public GenreResponseDto Genre { get; set; }

		public List<CharacterResponseDto> Characters { get; set; }
	}
}
