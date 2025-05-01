using System;
using System.Threading.Tasks;
using UserCenter.Core.Entities;
using UserCenter.Infrastructure.Data;

namespace UserCenter.API
{
    public class InsertTest
    {
        private readonly UserCenterDbContext _dbContext;

        public InsertTest(UserCenterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertUserAsync()
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "hyf050416@gmail.com",
                NickName = "Yufei",
                CreateTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow,
                Gender = 1,
                UserStatus = 1,
                IsDelete = 0,
                UserRole = 1
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}