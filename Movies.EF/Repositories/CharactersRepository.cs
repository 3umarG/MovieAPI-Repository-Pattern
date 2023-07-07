using FirstWebApi.Models;
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
	}
}
