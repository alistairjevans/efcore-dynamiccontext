using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicContext
{
  public static class RegistrationExtensions
  {
    public static void AddDynamicDbContexts(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> optionsCallback)
    {
      InternalAddDynamicDbContexts(services, optionsCallback);
    }

    private static void InternalAddDynamicDbContexts(IServiceCollection services,                                                     
                                                     Action<IServiceProvider, DbContextOptionsBuilder> optionCallback, 
                                                     ServiceLifetime contextLifeTime = ServiceLifetime.Scoped,
                                                     ServiceLifetime optionsLifeTime = ServiceLifetime.Singleton)
    {
      // Register our generic context.
      services.TryAdd(new ServiceDescriptor(typeof(DynamicDbContext<>), typeof(DynamicDbContext<>), contextLifeTime));
      
      // Force the options lifetime to be a singleton if the context lifetime is a singleton.
      if(contextLifeTime == ServiceLifetime.Singleton)
      {
        optionsLifeTime = ServiceLifetime.Singleton;
      }

      services.TryAdd(
          new ServiceDescriptor(
             typeof(DynamicDbContextOptions),
             p =>
             {
               return new DynamicDbContextOptions
               {
                 ValidateSetAccess = true
               };
             }, optionsLifeTime
          ));
                                    
      
      services.TryAdd(
          new ServiceDescriptor(
              typeof(DbContextOptions),
              p =>
              {
                var builder = new DbContextOptionsBuilder();

                builder.UseApplicationServiceProvider(p);

                // Call configuration options.
                optionCallback(p, builder);

                return builder.Options;
              },
              optionsLifeTime));
    }
  }
}
