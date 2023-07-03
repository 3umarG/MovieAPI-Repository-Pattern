namespace MoviesApi.Models
{
	public class Character
	{
		public int ID { get; set; }
		public Name CharacterName { get; set; }

        public DateTime BirthDate { get; set; }

		public virtual ICollection<CharacterInMovie> CharacterActInMovies { get; set; }

    }

	public class Name
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
