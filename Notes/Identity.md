> 当Home/Index的时候

由于在HomeController上打上了Authorize特性，根据Startup中的设置，来到了Security/Login，并且还带着一个ReturnUrl的参数。

> 注册：由于没有注册过，来到Security/Register注册

首先判断ModelState的状态，让后组装AppIdentityUser,再调用userManager.CreateAsync方法创建用户账户。如果创建成功，登录，发邮件，跳转到指定页面。

> 登录：Security/Login

判断ModelState状态，根据userManager.FindByNameAsync(model.Username)判断用户是否存在，如果用户存在再判断是否邮件验证过。再通过signInManager进行登录，登录成功就跳转到指定页，不成功把错误信息写入到ModelState中。

> _Laout.cshtml中如何显示登录或不登录状态？

通过Context.User.Identity.IsAuthenticated来判断。

> 忘记密码

输入邮件，来到Security/ForgotPassword中，判断email是否为null,判断是否有该用户，判断该用户是否邮件确认过了，再发邮件，跳转到提醒已发邮件的视图。

> 重置密码

调用userManager.ResetPasswordAsync方法。

