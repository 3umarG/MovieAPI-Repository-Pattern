namespace MoviesApi.Models
{
	public class CharacterInMovie
	{
        public int CharacterID { get; set; }

        public int MovieID { get; set; }

        public double Salary { get; set; }

        public virtual Character Character { get; set; }
        public virtual Movie Movie { get; set; }

    }
}
