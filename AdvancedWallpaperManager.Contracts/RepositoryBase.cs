﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedWallpaperManager.DAL.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected DbSet<T> _dbSet;

        public (DbContext Context, DbSet<T> DbSet) DatabaseInfo { get { return (_context, _dbSet); } }
        public int Count => GetAllQuery().Count();

        public RepositoryBase(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual void Add(T item)
        {
            _dbSet.Add(item);
        }
        public virtual void AddAndCommit(T item)
        {
            Add(item);
            Commit();
        }
        public virtual void AddRange(IEnumerable<T> items)
        {
            _dbSet.AddRange(items);
            _context.SaveChanges();
        }

        public virtual void Clear()
        {
            var allItems = GetAll();
            _dbSet.RemoveRange(allItems);
            _context.SaveChanges();
        }

        public virtual bool Exists(int key)
        {
            return Find(key) != null;
        }

        public virtual T Find(int key)
        {
            return _dbSet.Find(key);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet;
        }
        public virtual IQueryable<T> GetAllQuery()
        {
            return _dbSet;
        }

        public virtual void Remove(int key)
        {
            var entity = Find(key);

            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
        }
        public virtual void RemoveAndCommit(int key)
        {
            Remove(key);
            Commit();
        }
        public virtual void RemoveRange(IEnumerable<T> items)
        {
            _dbSet.AttachRange(items);
            _dbSet.RemoveRange(items);
            _context.SaveChanges();
        }

        public virtual void Update(T item)
        {
            _dbSet.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
        }
        public virtual void UpdateAndCommit(T item)
        {
            Update(item);
            Commit();
        }

        public virtual void Commit()
        {
            _context.SaveChanges();
        }
        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}
