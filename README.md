# UserCenter Backend API

这是一个使用 ASP.NET Core 构建的用户中心后端 API，支持用户注册、登录、查看和修改个人信息。采用分层架构设计，并使用 JWT 进行身份认证。

## 技术栈

- **后端框架**：ASP.NET Core Web API (.NET 8)
- **数据库**：PostgreSQL
- **ORM**：Entity Framework Core (Code First)
- **身份认证**：JWT（JSON Web Token）
- **架构设计**：
  - `UserCenter.API`：Web API 接口层
  - `UserCenter.Core`：核心业务逻辑、接口和实体定义
  - `UserCenter.Infrastructure`：具体服务实现、数据库上下文、仓储等

## 快速开始

### 1. 克隆仓库

```bash
git clone https://github.com/yourname/UserCenter.API.git
cd UserCenter.API
```

### 2. 配置数据库连接

修改 `appsettings.json` 或 `appsettings.Development.json`：

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=UserCenterDb;Username=youruser;Password=yourpassword"
}
```

### 3. 数据库迁移 & 更新

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. 运行项目

```bash
dotnet run --project UserCenter.API
```

API 默认运行在 `https://localhost:5001`。

## API 路由说明

| 方法 | 路由                    | 描述                   |
|------|-------------------------|------------------------|
| POST | `/api/Auth/register`    | 用户注册               |
| POST | `/api/Auth/login`       | 用户登录               |
| GET  | `/api/User/me`          | 获取当前用户信息（需登录） |
| POST | `/api/User/saveChanges` | 修改个人资料（需登录） |

## 示例用户账号

> 可用于前端 Demo 登录测试：

- 管理员账号：`admin / Pw123456!`
- 普通用户账号：`user1 / Pw123456!`

## 身份认证说明

登录成功后，后端会将 JWT 作为 Cookie 返回：

- Cookie 配置为 `HttpOnly`、`Secure`、`SameSite=None`
- 前端请求时需要携带认证信息：

```ts
axios.get('/api/User/me', {
  withCredentials: true
});
```

## 本地开发要求

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- 可选工具：[pgAdmin](https://www.pgadmin.org/)

## Azure 部署建议

- 使用 Azure App Service 部署后端
- 配置环境变量（如 JWT 密钥、数据库连接字符串）
- 配置 CORS 策略以支持前端访问
- 与前端部署于同一域名或使用网关代理实现跨域通信

## 作者信息

由 [Yufei Huang (Bill)](https://www.linkedin.com/in/yufei-huang-18582426a) 构建与维护。

欢迎 Star 🌟、Issue、或 PR！
