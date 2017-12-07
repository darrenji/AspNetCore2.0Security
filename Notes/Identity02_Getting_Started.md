> ASP.NET Identity的组件

Microsoft.AspNet.Identity.Core: 管理用户和角色等

Microsoft.AspNet.Identity.EntityFramework:持久化

Microsoft.AspNet.Identity.Owin:使用OWIN中间件，来进行Cookie Authentication,使用第三方登录。

```
install-package Microsoft.AspNet.Identity.Core
intstall-package Microsoft.AspNet.Identity.EntityFramework
instal-package Micofosoft.AspNet.Identity.Owin

当然装以上的时候附带了其它组件，包括：EntityFramework, Newtonsoft.JSON, Owin, Microsoft.OWin, Microsoft.Owin.Security, Microsoft.Own.Security.Cookie, Microsoft.Own.Security.OAuth
```

> 准备数据库

```
<connectionString>
	<add name="DefaultConnection" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ASPNetIdentity.mdf;Initial Catalog=ASPNetIdentity;Integrated Security=True" providerName="System.Data.SqlClient" />
</connectionString>
```

> IdentityUser

```
public virtual string Email{get;set;}
public virutal bool EmailConfirmed{get;set;}
public virtual TKey Id{get;set;}
public virtual bool LockoutEnabled{get;set;}
public virtual DateTime? LockoutEndDateUtc{get;set;}
public vrtual ICollection<TLogin> Logins{get;set;}
public vrtual string PasswordHash{get;set;}
public virtual string PhoneNumber{get;set;}
public virtual bool bool PhoneNumberConfirmed{get;set;}
public virutal ICollection<TRole> Roles{get;}
public vritual string SecurityStamp{get;set;}
pbulic virtual bool TwoFatorEnabled{get;set;}
publci vrtual string UserName{get;set;}
```

通常这样使用：

```
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
public class ApplicaitonUser : IdentityUser
{}
```

> Database Context

```
public class ApplicaitonDbContext : IdentityDbContext<ApplicaitonUser>
{
	public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema:false){}
}
```

> Configuring ASP.NET Identity

IUserStore:

```
public interface IUserStore<TUser, in TKey> : IDisposable where TUser : class, Microsoft.AspNet.Identity.IUser<TKey> 
{
	Task CreateAsync(TUser user);
	Task DeletAsync(TUser user);
	Task<TUser> FindByIdAsync(TKey userId);
	Task<TUser> FindByNameAsync(string userNmae);
	Task UpdateAsync(TUser user);
}
```

ApplicationUserStore

```
public class ApplicaitonUserStore : UserStore<ApplicaitonUser>
{
	public ApplicaitonUserStore(ApplicationDbContext context):base(context){}
}
```

> User Manager

UserManager<TUser>使用IUserStore来操作， UserStore在UserManager中传值给UserManager中

```
public class ApplicaitonUserManager : UserManager<ApplicaitonUser>
{
	public ApplicaitonUserManager(IUserStore<ApplicaitonUser> store) : base(store){}
}
```

> HomeController

视图中
```
@Html.ActionLink("Add User", "AddUser", "Home")
```

控制器中
```
public async Task<string> AddUser()
{
	ApplicationUser user;
	ApplicaitonUserStore Store = new ApplicationUserStore(new ApplicationDbContext());
	ApplicationUserManager userManager = new ApplicationUserManager(Store);

	user = new ApplicaitonUser{
		UserName = "",
		Email = ""
	};

	var result = await userManager.CreateAsync(user);
	if(!result.Succeeded)
	{
		return result.Errors.First();
	}

	return "User Added";
}
```

> The ASP.NET Identity Architecture

```
UserManager, RoleManager
UserStore, RoleStore
Database
```

> 注册用户模型

视图模型
```
public class RegisterViewModel
{
	[Required]
	[EmailAddress]
	[Display(Name = "")]
	public string Email{get;set;}

	[Required]
	[StringLength(100, ErrorMessage="",MinimumLength=0)]
	[DataType(DataType.Password)]
	[Dsiplay(Name = "")]
	public string Password{get;set;}

	[DataType(DataType.Password)]
	[Display(Name="")]
	[Compare("Password",ErrorMessage="")]
	public string ConfirmPassword{get;set;}
}
```

> AccountController

```
public class AccountController : Controller
{
	[AllowAnonymous]
	public ActionResult Register()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Register(RegisterViewModel model)
	{
		if(ModelState.IsValid)
		{
			var user = new ApplicationUser {
				UserName = model.Email,
				Email = mdoel.Email
			};

			UserStore<ApplicatioinUser> Store = new UserStore<ApplicaitonUser>(new ApplicationDbContext());
			ApplicationUserManager userManager = new ApplicaitonUserManager(Store);

			var result = await userManager.CreateAsync(user, model.Password);
			if(result.Succeeded)
			{
				return RedirecToAction("Index", "Home");
			}	

			AddErrors(result);
			return View(model);
		}
	}

	private void AddErrors(IdentityResult result)
	{
		foreach(var error in resutl.Errors)
		{
			ModelState.AddModelError("",error);
		}
	}
}
```

> 注册视图

```
@model RegisterViewModel

@Html.AntiForgeryToken()
@Html.ValidationSummary()

```

> _Layout.cshtml

```
@Htm.Partial("_LoginPartial")
@RenderBody
```

> 登录视图
> 
_LoginPartial.cshtml
```
@if(Request.IsAuthenticated)
{
	user.Identity.GetUserName
	Logout
}
else 
{
	Register
	log in
}
```
