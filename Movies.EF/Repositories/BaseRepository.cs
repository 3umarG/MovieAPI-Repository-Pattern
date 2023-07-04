using Movies.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		public Task AddAsync(T entity)
		{
			throw new NotImplementedException();
		}

		public Task DeleteByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<List<T>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<T> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(T entity)
		{
			throw new NotImplementedException();
		}
	}
}
