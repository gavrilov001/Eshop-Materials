using System;
using System.Collections.Generic;

namespace Eshop.Domain.Interfaces
{
    // Генерички интерфејс за да работи за било кој модел (Product, Order...)
    public interface IRepository<T> where T : class
    {
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }
}
