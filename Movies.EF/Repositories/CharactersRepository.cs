﻿using FirstWebApi.Models;
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
	}
}
