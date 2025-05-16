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
- 1. 修改前端页面，添加cam fall内容和相关样式
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
1. 相关网页去访问local stroage去拿用户信息展示，用户可以通过相关的功能修改信息
   新写一个useeffect的context的tsx文件，将这个context文件套用在全局context，使其能够全局使用user变量
   在想要调用的页面中初始化useAuthContext()，然后{user?.nickName}之类去调用用户信息
   1. 修改前端信息 √
   2. 头像需要改为上传相关照片，存后端还是哪 （常规逻辑为存到云端或者服务器，后端记录图片URLs，但我没云端）
   3. 前端提交表格后端写接口(User Profile)√
   4. gender改下拉框 √

2. 访问自己信息时或者重要信息时去验证一下token，用Cookie HTTP only去带token，后端通过 Response.Cookies.Append
   向前端发送cookie并带有token，验证时前端通过请求中设置withCredentials: true,带token给后端 √
1. 后端通过JwtBearer读取token(后端验证并未完成)

3. 对用户修改的操作验证user role
4. 解决登录态的问题
5. 解决标签图标问题