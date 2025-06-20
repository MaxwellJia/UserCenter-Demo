# UserCenter Backend API

This is a user center backend API built with ASP.NET Core, which supports user registration, login, viewing and modifying personal information. It adopts a layered architecture design and uses JWT for identity authentication.

## Technology stack

- **Backend framework**: ASP.NET Core Web API (.NET 8)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (Code First)
- **Authentication**: JWT (JSON Web Token)
- **Architecture design**:
- `UserCenter.API`: Web API interface layer
- `UserCenter.Core`: core business logic, interface and entity definition
- `UserCenter.Infrastructure`: specific service implementation, database context, storage, etc.

## Quick start

### 1. Clone the repository

```bash
git clone https://github.com/yourname/UserCenter.API.git
cd UserCenter.API
```

### 2. Configure database connection

Modify `appsettings.json` or `appsettings.Development.json`:

```json
"ConnectionStrings": {
"DefaultConnection": "Host=localhost;Port=5432;Database=UserCenterDb;Username=youruser;Password=yourpassword"
}
```

### 3. Database migration & update

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the project

```bash
dotnet run --project UserCenter.API
```

The API runs at `https://localhost:5001` by default.

## API route description

| Method | Route | Description |
|------|-------------------------|------------------------|
| POST | `/api/Auth/register` | User registration |
| POST | `/api/Auth/login` | User login |
| GET | `/api/User/me` | Get current user information (need to log in) |
| POST | `/api/User/saveChanges` | Modify personal information (need to log in) |

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

Built and maintained by [Yufei Huang (Bill)](https://www.linkedin.com/in/yufei-huang-18582426a).
