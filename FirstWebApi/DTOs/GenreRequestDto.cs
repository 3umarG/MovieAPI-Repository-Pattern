
namespace MoviesApi.DTOs
{
	public class GenreRequestDto
	{
		[MaxLength(100)]
        public string Name { get; set; }
    }
}
