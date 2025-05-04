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