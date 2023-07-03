namespace MoviesApi.Models
{
	public class Movie
	{
		public int ID { get; set; }

		public string Title { get; set; }

		public int Year { get; set; }

		public double Rate { get; set; }

		public string StoryLine { get; set; }

		public byte[] Poster { get; set; }

		public byte GenreID { get; set; }

		public Genre Genre { get; set; }

		public virtual ICollection<CharacterInMovie> CharacterActInMovies { get; set; }
	}
}
