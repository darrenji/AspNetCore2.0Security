ASP.NET里有一个forms authentication系统来认证用户。而forms authenticaiton用到了cookie,当用户登录的时候，用户信息就放到了cookie中，以后每次登录就会用到这个cookie,如果cookie过期了，用户需要重新登录。

现在，我们可以增加用户信息，使用entity framework code first的方式处理用户信息。

two-factory authenticaiton: 发送password和一个security code到用户注册的手机或邮箱。

account confirmation email:发一份账户认证邮件给用户注册邮箱。

account lockout:当用户密码错误达到一定次数，就lockout用户账户。

roles management:管理用户角色。

claims management:以键值对存储。

extenal identity providers:第三方登录，可以写自己的identity provider.

owin integration:asp.net identity使用owin来创建cookie和验证。我们也可以使用forms authentication来进行验证。



