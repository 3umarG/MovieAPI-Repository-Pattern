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
		public Task<List<T>> GetAllAsync();
		public Task<List<T>> GetAllAsync(string[] includes);

		Task<T?> GetByIdAsync(int id);

		public S? GetByExpressionWithInclude<S>(Func<T, S> selector, Func<S, bool> expression, string[] includes);
		public T? GetByExpressionWithInclude(Func<T, bool> expression, string[] includes);

		public List<S> GetAllByExpressionWithInclude<S>(Func<T, S> selector, Func<S, bool> expression, string[] includes);

		Task<T?> GetByExpressionAsync(Expression<Func<T , bool>> expression);

		Task<int?> AddAsync(T entity);

		Task<int?> DeleteByIdAsync(int id);

		void Delete(T entity);

		T Update(T entity);

		Task<bool> AnyAsync(Expression<Func<T , bool>> expression);

		List<S> GetAllAsync<S>(Func<T, S> selector);
		
		List<S> GetAllAsync<S>(Func<T, S> selector, string[] includes);

		IQueryable<T> QueryableOf();

	}
}
