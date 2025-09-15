// CinemaBookingApplication.Repository/Implementation/Repository.cs
using System.Linq.Expressions;
using CinemaBookingApplication.Domain.DomainModels;
using CinemaBookingApplication.Repository.Data;
using CinemaBookingApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CinemaBookingApplication.Repository.Implementation;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _entities;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public T Insert(T entity)
    {
        _entities.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<T> InsertMany(ICollection<T> entities)
    {
        _entities.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public T Update(T entity)
    {
        _entities.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public T Delete(T entity)
    {
        _entities.Remove(entity);
        _context.SaveChanges();
        return entity;
    }

    public E? Get<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> q = _entities;

        if (include != null) q = include(q);
        if (predicate != null) q = q.Where(predicate);

        if (orderBy != null)
            return orderBy(q).Select(selector).FirstOrDefault();

        return q.Select(selector).FirstOrDefault();
    }

    public IEnumerable<E> GetAll<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> q = _entities;

        if (include != null) q = include(q);
        if (predicate != null) q = q.Where(predicate);

        if (orderBy != null)
            return orderBy(q).Select(selector).AsEnumerable();

        return q.Select(selector).AsEnumerable();
    }
}
