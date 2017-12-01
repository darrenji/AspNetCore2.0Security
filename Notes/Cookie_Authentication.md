> 当打开Home/Index的时候来到了Security/Login,为什么？

因为在HomeController上设置了[Authorize]特性，并且在Startup中设置了当验证不通过的时候，就来到Security/Login

> Security/Login展示是怎么回事？

Security/Login方法会把returnUrl放到视图的隐藏域中，以便在登录成功后跳转到returnUrl。

> Security/Login接受数据是怎么回事？

也就是在Security/Login方法中发生的事。首先是判断参数，然后List<Claim>,再把它交给ClaimsPrincipal,ClaimsPrincipal再交给ClaimsPrincipal,最后HttpContext.SignInAsync进行登录。

> Security/Logout是怎么回事？

调用HtttpContext.SignOutAsync方法，然后RedirectToAction跳转到某个Action。

> Startup.cs如何设置。

```
services.AddAuthentication("名称")
	.AddCookie("名称", optioins => {
		options.AccessDeniedPath
		options.Cookie
		options.Events
		options.ReturnUrlParameter
		options.SlidingExpiration
	});
```