using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.DTOs
{
	public class CharacterWithMovieResponseDto
	{
        public int MovieId { get; set; }

        public int CharacterId { get; set; }

        public double Salary { get; set; }
    }
}
