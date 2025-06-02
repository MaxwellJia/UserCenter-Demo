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
   - Bug: 返回给前端的信息需要进行优化，例如不返回密码(不能让他看到，可以给接口修改), userId是否有必要返回等等 需要详细讨论
   - 整体规划:
   - 在list接口的基础上进行修改
   - 前端采用ant design pro table进行处理
   - 前端传递params参数给后端，params参数会写到TypeScript里，包含的属性有currentPage，pageSize，以及各个属性是否被加入筛选及筛选的值，用object表示key:value
   - 后端接受params参数并且抽象为一个DTO,DTO传递给service层进行查询和返回相关数据，service可能会用到LINQ
   - 2025.05.26查询存在bug，待修复

-  前端如何写和规划才合理之类的
- 后端生成接口去完成前端的请求filter, search, sort，分页还有相关功能

## 2025/05/29
- 已知list的bug：
  - 1. 输入User ID后仍返回整个数据 数据传输的名字要保持一致，params的名字就算用TypeScript名字传输时还会保持原样(模糊查询待处理)
	2. 输入UserName后只有MaxwellJia的返回正确 Service层有一个Bug，已解决
	3. 输入Famle时查询结果不返回 生成的数据默认gender是null，现注册分配默认值为1，就是男性
	4. UserRole查询结果同样不返回正确的normal user 已修复

- 前端的工作list
   1. 明暗交替user list太亮 Pro Table嵌套太多，优化需要用ant design pro本身的按钮(考虑后来换table，留以后修复)
   2. 前端需要优化为英文
   3. 前端edit、delete等按钮需要进行优化，后端需要写相关接口

## 2025/06/02
- 项目计划
  - 1. 更换Table list组件在完成当前ant design pro后，用cursor进行
	2. 当前需完成前端edit、delete等按钮需要进行优化，后端需要写相关接口

### 删除功能的实现

#### 前端实现

- 前端按钮处调用delete api
- 写delete api
- isDelete问题，如果用户的isDelete属性为1，则数据不显示在前端表格中，因为其已经被删除

#### 后端实现

- 后端先写interface
- 再写service，service修改数据库只是让用户的isDelete属性改为1
- 最后写controller，其接受id参数并将其转换为Gui类型

----
### 更新用户功能的实现

#### 前端实现
- 前端修改相关按钮
- 修改api传数据给后端
- 点击edit后的save和delete问题

#### 后端实现
- 后端先写interface
- 再写service，service中去查找该用户并去修改该用户信息
- 最后写controller，其接受相关参数并进行处理

#### 其他问题
- edit点击后的delete如何生效--在editable中加onSave进行修改

### 日后相关工作(等待Bill完成前端优化后做)
- 前端Protable英文化
- 前端代码注释优化为英文标准格式
- 前端优化为英文
- Toast弹窗增加到合适位置
- 校验用户身份，没有管理权限的用户看不到用户修改的table