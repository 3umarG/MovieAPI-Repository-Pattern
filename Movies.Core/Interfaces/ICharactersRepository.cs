using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
	public interface ICharactersRepository : IBaseRepository<Character>
	{
		Task<CharacterInMovie?> AddCharacterToMovieWithSalary(int chId, int movieID, double salary);

		Task<Character?> GetCharacterWithAllMoviesAsync(int characterId);

		Task<Movie?> GetMovieWithAllCharactersAsync(int movieId);
	}
}
