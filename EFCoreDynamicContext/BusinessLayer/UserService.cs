using DynamicContext;
using EFCoreDynamicContext.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreDynamicContext.BusinessLayer
{
    public interface IUserService
    {
        List<UserModel> GetUsers();
    }

    [UsesDbModel(typeof(UserModel), typeof(UserRoleModel))]
    public class UserService : IUserService
    {
        public UserService(DynamicDbContext<UserService> dbContext)
        {
            DbContext = dbContext;
        }

        public DynamicDbContext<UserService> DbContext { get; }

        public List<UserModel> GetUsers()
        {
            var roleCount = DbContext.Table<UserRoleModel>().Count();

            return DbContext.Table<UserModel>().ToList();
        }

        
    }
}
