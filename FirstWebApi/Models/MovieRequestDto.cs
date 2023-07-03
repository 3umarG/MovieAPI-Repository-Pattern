namespace MoviesApi.Models
{
	public record MovieRequestDto
	{
		[Required]
		[MaxLength(250)]
		public string Title { get; set; }

		public int? Year { get; set; }

		[Precision(2,1)]
		public double? Rate { get; set; }

		[MaxLength(2500)]
		public string? StoryLine { get; set; }

		public IFormFile? Poster { get; set; }

		public byte? GenreID { get; set; }
	}
}
