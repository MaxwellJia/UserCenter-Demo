using UserCenter.Core.Entities;
using UserCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


// This file is part of the UserCenter project to generate example user data for testing purposes. This will be started when there is no users in the database
public class DataSeeder
{
    private readonly UserCenterDbContext _context;

    public DataSeeder(UserCenterDbContext context)
    {
        _context = context;
    }

    public async Task SeedUsersAsync(int count = 500)
    {
        if (await _context.Users.AnyAsync()) return;

        var users = new List<ApplicationUser>();
        for (int i = 0; i < count; i++)
        {
            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = $"testuser{i}",
                Email = $"testuser{i}@example.com",
                PhoneNumber = $"04000000{i:D3}",
                NickName = $"User{i}",
                AvatarUrl = "",
                Gender = (short?)(i % 2),
                UserRole = i % 2,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                IsDelete = 0,
                CreateTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow,
                UserStatus = 1,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                NormalizedEmail = $"TESTUSER{i}@EXAMPLE.COM",
                NormalizedUserName = $"TESTUSER{i}"
            });
        }

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }
}
