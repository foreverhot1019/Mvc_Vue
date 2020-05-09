﻿using System;
using System.Data;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;

namespace Repository.Pattern.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        System.Data.Entity.DbContext getDbContext();
        int SaveChanges();
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IObjectState;
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
        void SetAutoDetectChangesEnabled(bool enabled);
    }
}