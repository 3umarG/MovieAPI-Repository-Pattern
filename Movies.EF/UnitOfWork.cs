using FirstWebApi.Models;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using Movies.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Movies = new MoviesRepository(context);
			Genres = new BaseRepository<Genre>(context);
		}

		public IMoviesRepository Movies { get; private set; }

		public IBaseRepository<Genre> Genres { get; private set; }

		public int Complete()
		{
			return _context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
