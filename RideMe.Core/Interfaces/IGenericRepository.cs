using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RideMe.Core.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		Task AddAsync(T entity);

		Task UpdateAsync(T entity);

		Task DeleteAsync(int id);

		Task<T> GetByIdAsync(int id);

		Task<IEnumerable<T>> GetAllAsync();

		Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria);

		Task<T?> FindAsync(Expression<Func<T, bool>> criteria);

		Task<IEnumerable<T>> FindAllWithIncludesAsyncc(params Expression<Func<T, object>>[] includes);

		Task<IEnumerable<T>> FindAllWithIncludesAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
		
		Task<T?> FindWithIncludesAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

	}
}
