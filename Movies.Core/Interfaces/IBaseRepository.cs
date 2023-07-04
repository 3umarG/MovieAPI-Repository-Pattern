using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync();

		Task<T?> GetByIdAsync(int id);

		Task<int?> AddAsync(T entity);

		Task<int?> DeleteByIdAsync(int id);

		Task<int?> UpdateAsync(T entity);



		
	}
}
