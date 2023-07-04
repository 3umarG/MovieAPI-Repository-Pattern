using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		private readonly ApplicationDbContext _context;

		public BaseRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<int?> AddAsync(T entity)
		{
			_context.Set<T>().Add(entity);
			return await _context.SaveChangesAsync();
		}

		public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
		{
			return await _context.Set<T>().AnyAsync(expression);
		}

		public void Delete(T entity)
		{
			_context.Set<T>().Remove(entity);
		}

		public async Task<int?> DeleteByIdAsync(int id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			if (entity is null)
				return -1;

			_context.Set<T>().Remove(entity);
			return _context.SaveChanges();
		}

		public async Task<List<T>> GetAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public  List<S> GetAllAsync<S>(Func<T , S> selector)
		{
			return  _context.Set<T>().Select(selector).ToList();
		}

		public List<S> GetAllAsync<S>(Func<T, S> selector , string[] includes)
		{
			IQueryable<T> query = _context.Set<T>();

            foreach (var icnlude in includes)
            {
				query = query.Include(icnlude);
            }

			var result = query.Select(selector).ToList();
			return result;
        }

		public async Task<T?> GetByExpressionAsync(Expression<Func<T, bool>> expression)
		{
			return await _context.Set<T>().FirstOrDefaultAsync(expression);
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			return entity;
		}

		

		public T Update(T entity)
		{
			_context.Update(entity);
			return entity;
		}
	}
}
