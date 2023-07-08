using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositories
{
	public class CharactersRepository : BaseRepository<Character>, ICharactersRepository
	{
		private readonly ApplicationDbContext _context;
		public CharactersRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<CharacterInMovie?> AddCharacterToMovieWithSalary(int chId, int movieID, double salary)
		{
			var isValidCharacter = _context.Characters.Any(c => c.ID == chId);
			var isValidMovie = _context.Movies.Any(m => m.ID == movieID);

			if (!isValidCharacter || !isValidMovie)
			{
				return null;
			}


			var chWithMovie = new CharacterInMovie { CharacterID = chId, MovieID = movieID, Salary = salary };
			var result = await _context.CharactersInMovies.AddAsync(chWithMovie);
			await _context.SaveChangesAsync();
			return result.Entity;
		}
	
	
		public async Task<Character?> GetCharacterWithAllMoviesAsync(int characterId)
		{
			var character = await _context
								.Characters
								.Include(c => c.CharacterActInMovies)
								.ThenInclude(cm => cm.Movie)
								.ThenInclude(m => m.Genre)
								.FirstOrDefaultAsync(c => c.ID == characterId);
			return character;

			#region Return only Movies without the character data
			/*
			var isValidId = await _context.Characters.AnyAsync(c => c.ID == characterId);
			if (!isValidId)
				return null;

			var movies =  _context
										.CharactersInMovies
										.Where(cm => cm.CharacterID == characterId)
										.Include(cm => cm.Movie)
										.ThenInclude(m => m.Genre)
										.Select(cm => cm.Movie);

			return await movies.ToListAsync();
			*/
			#endregion
		}

		public async Task<Movie?> GetMovieWithAllCharactersAsync(int movieId)
		{
			var movie = await _context
								.Movies
								.Include(M => M.CharacterActInMovies)
								.ThenInclude(CM => CM.Character)
								.Include(M => M.Genre)
								.FirstOrDefaultAsync(m => m.ID == movieId);

			return movie;

								
		}
	}
}
