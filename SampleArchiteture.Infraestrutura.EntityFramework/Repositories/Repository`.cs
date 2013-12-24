﻿using System;
using System.Data.Entity;
using System.Linq;
using SampleArchiteture.Dominio.Repositories;

namespace SampleArchiteture.Infraestrutura.EntityFramework.Repositories
{
    /// <summary>
    /// Repositório genérico para EntityFramework.
    /// </summary>
    /// <typeparam name="TEntity">Entidade</typeparam>
    /// <typeparam name="TKey">DataType da chave primária</typeparam>
    public abstract class Repository<TEntity, TKey> : Repository, IRepository<TEntity,  TKey>
        where TEntity : class
        where TKey : IComparable
    {

        private DbSet<TEntity> _entitySet;

        protected Repository(DbContext context) : base(context)
        {
        }

        protected DbSet<TEntity> EntitySet
        {
            get
            {
                return _entitySet ?? (_entitySet = Context.Set<TEntity>());
            }
        }

        public TEntity Get(TKey id)
        {
            return EntitySet.Find(id);
        }

        public IQueryable<TEntity> GetAll()
        {
            return EntitySet;
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            EntitySet.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            EntitySet.Remove(entity);
        }

        public void Remove(TKey id)
        {
            var entity = EntitySet.Find(id);

            Remove(entity);
        }
    }
}