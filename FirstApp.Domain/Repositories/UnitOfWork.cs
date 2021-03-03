﻿using FirstApp.Domain.EntityFramework;
using FirstApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopDbContext _context;

        private bool disposed = false;

        public IProductRepository Products { get; }

        public UnitOfWork(ShopDbContext shopDbContext, IProductRepository productRepository)
        {
            _context = shopDbContext;
            Products = productRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing) 
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
