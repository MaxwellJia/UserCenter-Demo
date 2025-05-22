# 02/05/2025
## 当前进度
- 1. 数据库已经建立， 正向工程已经完成， 数据migration已经完成
- 2. 明确了初步的后端框架，建立了controller-interface-service的分层结构，对应为API-Core-Infrastructure
- 3. Service端继承IAuthService来进行权限验证等功能，例如注册、登录等，用其中的_usermapper来操作数据库
- 4. 建立DFOs来帮助内部进行数据传播

## 今日计划
- 1. 完善DFOs
- 2. 后端交流设计，打通DFOs
- 3. 时间基本的前端，主要是注册功能，链接后端，跑通单个注册功能
  
    ## 完善DTOs
  - 1. 前端注册传递的DTO
	2. 后端给前端返回的DTO
	
    ## 后端交流的设计，注册逻辑
  - 1. 前端注册需要的内容（后续再实现邮箱注册和验证逻辑）: 
	- 用户名
	- 密码
	- 确认密码
  - 2. API端接受到前端请求后，将其直接转换为相应的DTO进行传递，一个请求一般来讲只需要一个DTO进行传递即可
  - 3. DTOs传递到service层后将用户数据新型保存后，生成一个JWT Token, 返回并传递一个ResponseDTO给core层再到API层，API层将这个DTO给前端 √
	
## 学到内容
   ## JWT Token的作用和使用方法
   JWT Token 相当于一个身份证，保存在local session或者local storage里
   放在前端的请求头中，后端通过验证这个Token来判断用户是否登录和权限等问题

   注册登录等相关业务JWT Token的使用流程：
   1. 前端注册，后端显示注册成功与否，成功跳转相应页面，失败则提示错误信息
   2. 登录时，前端将用户名和密码传递给后端，后端验证成功后返回一个JWT Token给前端
   3. 前端将Token

# 15/05/2025
## 今日计划
- 1.修改前端页面，添加cam fall内容和相关样式 
- 2. 数据前端传输展示
	1. 头像 如果没有显示默认头像
	2. Nick Name
	3. 邮箱
	4. 电话
	5. 性别
	6. UserRole
- 3. 如何验证用户的登录态
	1. 登录态和userrole问题
	提到转到相关页面就去看他的local stroage中的token是否过期，过期则跳转到登录页面， 没过期则提取user信息登录
	不希望userrole为0的用户看到相关页面，可以在sidebar里写，对相关页面进行隐藏(判断userrole是否为0)

	2.token整体流程
	登录时生成token，token每一段时间过期 √
	用户拿到token后存储在local stroage中 √
	相关网页去访问local stroage去拿用户信息展示，用户可以通过相关的功能修改信息(user profile已完成) √
	访问自己信息时或者重要信息时去验证一下token
	对用户修改的操作验证user role

# 16/05/2025
## 今日计划
### 1. 用户信息展示与修改

- 相关网页去访问 local storage 去拿用户信息展示，用户可以通过相关的功能修改信息  ✅
- 新写一个 `useEffect` 的 context 的 `.tsx` 文件，将这个 context 文件套用在全局 context，使其能够全局使用 `user` 变量  ✅
- 在想要调用的页面中初始化 `useAuthContext()`，然后使用 `{user?.nickName}` 之类去调用用户信息  ✅
- 修改前端信息 ✅
- 头像需要改为上传相关照片，存后端还是哪（常规逻辑为存到云端或者服务器，后端记录图片 URLs，但我没云端）  
- 前端提交表格后端写接口（User Profile) ✅
- gender 改下拉框 ✅

---

### 2. 登录状态与 Token 验证

- 访问自己信息时或者重要信息时去验证一下 token，用 Cookie HTTP only 去带 token，后端通过 `Response.Cookies.Append` 向前端发送 cookie 并带有 token，验证时前端通过请求中设置 `withCredentials: true`，带 token 给后端 ✅  
- 后端通过 JwtBearer 读取 token（后端验证并未完成，现在不需要这个步骤）
- **整体流程说明：**  ✅
  - `middleware` 对整体页面进行控制  
  - 如果要访问 `/dashboard/...` 之后的页面，`middleware` 会去验证 token 是否存在和过期（其前端会去验证 SecretKey）  
  - 如果过期会跳转到登录页面，token 有效则放行  
  - 在登出时会向后端发送一个请求，后端会将 cookie 清除  

---

### 3. 权限与角色控制

- 对用户修改的操作验证 `user role`

---

### 4. 登录状态问题修复

- 解决登录态的问题  

---

### 5. 标签图标配置

- 解决标签图标问题 直接加在 `public/favicon.ico` ✅

---

### 6. 后端架构调整

- 后端的生成 Token 可以被修改到更高层级，因为 `IAuthService` 和 `IUserService` 都需要用到  
- `IUserService` 需要新生成一个 controller 来管理，不能在 `AuthController` 中 

---

### 7. Welcome 页面优化

- 修改一下 Welcome 页面，使其更加美观并加入 User 元素

# 18/05/2025
## 相关计划

### 后端优化
- 后端的生成 Token 可以被修改到更高层级，因为 `IAuthService` 和 `IUserService` 都需要用到 (在Helpers里) ✅
- `IUserService` 需要新生成一个 controller 来管理，不能在 `AuthController` 中（更新到了UserController, 前端也做出了访问地址的改变） ✅
- saveChanegs优化一下，去验证一下token，而不是直接返回新token✅ 现在为验证token，token过期重新登陆，不过期可以修改
- 优化cookie的生成验证，使其成为一个文件，controller里边可以随时调用✅（将cookie写成一个service来调用操作cookie）

### 前端优化
- 优化一下welcome页面，使其更加专业美观
- 对用户修改的操作验证 `user role`，如果其为1则显示用户管理form，0则不显示
- 修复前端的一些bug，例如nextjs报的错，还有一个bug
- 用户在登陆状态下，还能跳到login页面
- 前端首次跳转到welcome页面时不加载local stroage数据
- saveChanges第一次不会修改任何内容，第二次之后才会修改
- useContext一些相关的钩子
- 直接关闭网页的话local strotage的数据不会被清除，是否需要清除

### 推进项目
- 生成用户管理的form，去管理用户
- 后端生成接口去完成前端的请求filter, search, sort，分页还有相关功能