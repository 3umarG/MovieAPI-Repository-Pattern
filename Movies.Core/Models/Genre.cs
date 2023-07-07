
namespace Movies.Core.Models
{
	public class Genre
	{
        public byte ID { get; set; }

        public string Name { get; set; }

        public virtual List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
