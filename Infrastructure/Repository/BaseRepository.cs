﻿using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Core.Entities;
using Infrastructure.Utility;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repository
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        internal readonly DbSet<T> _dbSet;
        internal readonly AppDbContext _context;

        protected BaseRepository(AppDbContext context)
        {
            _dbSet = context.Set<T>();
            _context = context;
        }


        public IQueryable<T> All => _dbSet.AsNoTracking();

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public Task<List<T>> GetAllAsync(
            List<Expression<Func<T, bool>>>? filters = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            IPagingParams? pagingParams = null
        )
        {
            IQueryable<T> query = _dbSet;

            if (filters is not null)
            {
                filters.ForEach(filter => { query = query.Where(filter); });
            }

            if (include is not null)
            {
                query = include(query);
            }

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            if (pagingParams is not null)
            {
                query = query
                    .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
                    .Take(pagingParams.PageSize);
            }

            return query.AsNoTracking().ToListAsync();
        }

        public Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            var query = _dbSet.Where(filter);
            
            if (include is not null)
            {
                query = include(query);
            }

            return query.FirstOrDefaultAsync();
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> range)
        {
            _dbSet.RemoveRange(range);
        }

        public int Count()
        {
            return _dbSet.Count();
        }
    }
}