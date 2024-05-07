using Microsoft.EntityFrameworkCore;
using RideMe.Core.Interfaces;
using RideMe.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RideMe.EF
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly ApplicationDbContext _dbContext;

		public GenericRepository(ApplicationDbContext dbContext)
        {
			_dbContext = dbContext;
		}


        public async Task AddAsync(T entity)
		{
			await _dbContext.Set<T>().AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}


		public async Task<T> GetByIdAsync(int id)
			=> await _dbContext.Set<T>().FindAsync(id);


		public async Task DeleteAsync(int id)
		{
			var entity = await _dbContext.Set<T>().FindAsync(id);

			if (entity != null)
			{
				_dbContext.Set<T>().Remove(entity);
				await _dbContext.SaveChangesAsync();
			}
			else
			{
				// Handle the case where the entity with the given ID does not exist
				throw new InvalidOperationException($"Entity with ID {id} does not exist.");
			}
		}


		public async Task UpdateAsync(T entity)
		{
			_dbContext.Set<T>().Update(entity);
			await _dbContext.SaveChangesAsync();
		}


		
		//  returns one object ( T ) according to specific condition ( Where ) and Include another thing to the returned object
		public async Task<T?> FindWithIncludesAsync(Expression<Func<T, bool>> criteria, Expression<Func<T, object>>[] includes)
		{

			if (criteria == null)
			{
				throw new ArgumentNullException(nameof(criteria));
			}

			if (includes == null)
			{
				throw new ArgumentNullException(nameof(includes));
			}

			IQueryable<T> query = _dbContext.Set<T>().Where(criteria);

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			return await query.FirstOrDefaultAsync();
		}


		//  returns <T> according to specific condition ( Where )
		public async Task<T?> FindAsync(Expression<Func<T, bool>> criteria)
			=> await _dbContext.Set<T>().Where(criteria).FirstOrDefaultAsync();


		public async Task<IEnumerable<T>> GetAllAsync()
			=> await _dbContext.Set<T>().ToListAsync();


		//  returns IEnumerable<T> according to specific condition ( Where )
		public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria)
			=> await _dbContext.Set<T>().Where(criteria).ToListAsync();


		//  returns IEnumerable<T> and Include another thing to the returned json
		public async Task<IEnumerable<T>> FindAllWithIncludesAsyncc(params Expression<Func<T, object>>[] includes)
		{

			if (includes == null)
			{
				throw new ArgumentNullException(nameof(includes));
			}

			IQueryable<T> query = _dbContext.Set<T>();

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			return await query.ToListAsync();
		}


		//  returns IEnumerable<T> according to specific condition ( Where ) and Include another thing to the returned json
		public async Task<IEnumerable<T>> FindAllWithIncludesAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
		{

			IQueryable<T> query = _dbContext.Set<T>().Where(criteria);

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			return await query.ToListAsync();
		}

	}
}
