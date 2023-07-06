
using System.ComponentModel.DataAnnotations;

namespace Movies.Core.DTOs
{
	public class GenreRequestDto
	{
		[MaxLength(100)]
        public virtual string? Name { get; set; }
    }
}
