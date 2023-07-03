namespace MoviesApi.DTOs
{
	public record MovieResponseDto
	{
		public int ID { get; set; }

		public string Title { get; set; }

		public int Year { get; set; }

		public double Rate { get; set; }

		public string StoryLine { get; set; }


		public GenreResponseDto Genre { get; set; }
	}
}
