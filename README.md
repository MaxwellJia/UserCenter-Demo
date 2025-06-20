# UserCenter Backend API

This is a user center backend API built with ASP.NET Core for the Cam Fall demo, which supports User Registration, Login, Viewing and Modifying Personal Information, User Management, JSON Web Token, and Role-Based Access Control. It adopts a layered architecture design and uses JWT for identity authentication.

## Access the website already deployment

[UserCenter](https://thankful-smoke-011c73b00.1.azurestaticapps.net)

## Technology stack

- **Backend framework**: ASP.NET Core Web API (.NET 8)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (Code First)
- **Authentication**: JWT (JSON Web Token)
- **Architecture design**:
- `UserCenter.API`: Web API interface layer
- `UserCenter.Core`: core business logic, interface and entity definition
- `UserCenter.Infrastructure`: specific service implementation, database context, storage, etc.

## Quick Start

### 1. Clone the repository

```bash
git clone https://github.com/MaxwellJia/UserCenter-Demo.git
cd UserCenter.API
```

### 2. Configure database connection and JWT settings (environment variables)

You need to set the following environment variables to run the service correctly:

#### ✅ Required environment variables (local `appsettings.Development.json` and `Program.cs` example)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
"AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "JwtSettings": {
    "Issuer": "Please replace with your Issuer",
    "Audience": "Please replace with your Audience",
    "SecretKey": "Please replace with your SecretKey",
    "ExpirationMinutes": 30
  }
}
```

Change your own database driver (SQL driver for example)

```
builder.Services.AddDbContext<UserCenterDbContext>(options => options.UseSqlServer(connectionString));
```

#### 🌐 Front-end API address and CROS ('Program.cs' example)

Add the front-end address in the Program.cs

```env
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("Your front end address 1", "Your front end address 2")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

> Note: JWT's `SecretKey` needs to be kept confidential. It is recommended to use User Secrets or CI/CD environment variables for management.

---

### 3. Database migration & update

Already created an entity ApplicationUser and a DB context; you can change it accordingly.

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

When you first run the server and there is no data in your database, some data will be automatically generated. You can change it(change the number for example) in the Program.cs:

```
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<UserCenterDbContext>();
            var seeder = new DataSeeder(dbContext);
            Console.WriteLine("[Seeder] ");
            await seeder.SeedUsersAsync(500); 
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Seeder] Data seed failed: " + ex.Message);

        }
    }
}
```

### 4. Start the project

```bash
dotnet run --project UserCenter.API
```

The API runs at `https://localhost:5001` by default.


## API route description

| Method | Route | Description |
|------|-------------------------|------------------------|
| POST | `/api/Auth/register` | User registration |
| POST | `/api/Auth/login` | User login |
| POST | `/api/Auth/signOut` | User signOut |
| GET | `/api/User/saveChanges` | Modify current user information (need to log in) |
| POST | `/api/User/list` | Show all users' information (need to log in and be an admin) |
| DELETE | `/api/User/delete` | Delete the specific user (need to log in and be an admin) |
| POST | `/api/User/update` | Update users' information (need to log in and be an admin) |

## Sample user account

> Can be used for front-end Demo login test:

- Administrator account: `admin / Pw123456!`
- Ordinary user account: `user1 / Pw123456!`

## Identity authentication description

After successful login, the backend will return JWT as a cookie:

- Cookies are configured as `HttpOnly`, `Secure`, `SameSite=None`
- The front-end request needs to carry authentication information:

```ts
axios.get('/api/User/me', {
withCredentials: true
});
```

## Local development requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

- [PostgreSQL](https://www.postgresql.org/)

- Optional tool: [pgAdmin](https://www.pgadmin.org/)

## Azure deployment recommendations

- Use Azure App Service to deploy the backend

- Configure environment variables (such as JWT keys, database connection strings)

- Configure CORS policies to support front-end access

- Deploy on the same domain name as the front-end or use a gateway proxy to achieve cross-domain communication

## Author information

Built and maintained by [Yufei Huang (Bill)](https://www.linkedin.com/in/yufei-huang-18582426a) and [Wangtao Jia (Maxwell)](https://www.linkedin.com/in/maxwelljia/).
