namespace Movies.Core.Models
{
	public class Character
	{
		public int ID { get; set; }
		public Name CharacterName { get; set; }

        public DateTime BirthDate { get; set; }

		public virtual ICollection<CharacterInMovie> CharacterActInMovies { get; set; } = new HashSet<CharacterInMovie>();

    }

	public class Name
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
