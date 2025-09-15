﻿using System.Linq.Expressions;
using CinemaBookingApplication.Domain.DomainModels;
using Microsoft.EntityFrameworkCore.Query;

namespace CinemaBookingApplication.Repository.Interface;

public interface IRepository<T> where T : BaseEntity

{
    T Insert(T entity);
    ICollection<T> InsertMany(ICollection<T> entities);
    T Update(T entity);
    T Delete(T entity);

    E? Get<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    IEnumerable<E> GetAll<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
}
