
using System.ComponentModel.DataAnnotations;

namespace Movies.Core.DTOs
{
	public class GenreRequestDto
	{
		[MaxLength(100)]
        public string Name { get; set; }
    }
}
