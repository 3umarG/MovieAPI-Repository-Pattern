using MoviesApi.Models;

namespace FirstWebApi.Models
{
	public class Genre
	{
        public byte ID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}
