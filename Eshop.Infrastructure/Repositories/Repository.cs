using Eshop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eshop.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
    private readonly Eshop.Infrastructure.EshopDbContext _context;
        private DbSet<T> _entities;

        public Repository(Eshop.Infrastructure.EshopDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.ToList();
        }

        public T? GetById(Guid id)
        {
            return _entities.Find(id);
        }

        public void Insert(T entity)
        {
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            T? entity = _entities.Find(id);
            if (entity is null)
                return;

            _entities.Remove(entity);
            _context.SaveChanges();
        }
    }
}
