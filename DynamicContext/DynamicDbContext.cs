using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicContext
{
    public class DynamicDbContextOptions
    {
        public bool ValidateSetAccess { get; set; }
    }

    public class DynamicDbContext<TConsumer> : DbContext
    {
        protected DynamicDbContextOptions DynamicOptions { get; }

        public DynamicDbContext(DbContextOptions ctxtOptions, DynamicDbContextOptions dynamicOptions) : base(ctxtOptions)
        {
            DynamicOptions = dynamicOptions;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Get all the models
            var models = GetDeclaredModels();

            foreach(var dataModel in models)
            {
                // Registers the entity in the set.
                modelBuilder.Entity(dataModel);
            }
        }

        public IQueryable<TModel> Table<TModel>() where TModel : class
        {
            return InternalSetAccess<TModel>().AsNoTracking();
        }

        public DbSet<TModel> TableForModify<TModel>() where TModel : class
        {
            return InternalSetAccess<TModel>();
        }

        protected IEnumerable<Type> GetDeclaredModels()
        {
            // Get the set of UsesDbModel attributes on the consuming class.
            var modelConsumer = typeof(TConsumer).GetCustomAttributes(typeof(UsesDbModelAttribute), true)
                                                 .Cast<UsesDbModelAttribute>();

            return modelConsumer.SelectMany(x => x.ModelTypes);
        }

        private DbSet<TModel> InternalSetAccess<TModel>() where TModel : class
        {
            if (DynamicOptions.ValidateSetAccess)
            {
                if(!GetDeclaredModels().Contains(typeof(TModel)))
                {
                    throw new InvalidOperationException($"The model {typeof(TModel).Name} was not declared on " +
                                                        $"consumer {typeof(TConsumer).Name}, so cannot be used.\n" +
                                                        $"Add a [UsesModel(typeof({typeof(TModel).Name}))] definition on the consumer.");
                }
            }

            return Set<TModel>();
        }

    }
}
