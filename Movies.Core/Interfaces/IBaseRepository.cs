﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync();

		Task<T?> GetByIdAsync(int id);
		Task<T?> GetByExpressionAsync(Expression<Func<T , bool>> expression);

		Task<int?> AddAsync(T entity);

		Task<int?> DeleteByIdAsync(int id);

		void Delete(T entity);

		T UpdateAsync(T entity);




	}
}
