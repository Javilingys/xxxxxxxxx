﻿using FirstApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace FirstApp.Domain.EntityFramework
{
    public class ShopDbContext : DbContext
    {
        // Products Table
        public DbSet<Product> Products { get; set; }

        public ShopDbContext()
        {

        }

        public ShopDbContext(DbContextOptions options) : base(options)
        {
        }

        // Configure Enteties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // FirstApp.Domain/EntityFramework/Config - Directory for configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // конвертирует все в дабл. Как работает не очень понял.
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal));

                    foreach (var property in properties)
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion<double>();
                    }
                }
            }
        }
    }
}
