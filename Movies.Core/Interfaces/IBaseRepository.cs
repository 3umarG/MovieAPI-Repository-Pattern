﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync();

		Task<T> GetByIdAsync(int id);

		Task AddAsync(T entity);

		Task DeleteByIdAsync(int id);

		Task UpdateAsync(T entity);



		
	}
}