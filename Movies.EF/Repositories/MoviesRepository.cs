using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositories
{
	public class MoviesRepository : BaseRepository<Movie>, IMoviesRepository
	{
		private readonly ApplicationDbContext _context;
		public MoviesRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<Movie>> GetMoviesByGenreIdAsync(int id)
		{
			return await _context.Movies.Where(M => M.GenreID == id).ToListAsync();
		}
	}
}
