在VS2017中创建，会自动加载很多组件。比如：

```
Microsoft.AspNetCore.Identity
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Authenticaiton
Microsoft.AspNetCore.Abstractions
Microsoft.AspNetCOre.Cookies
Microsoft.AspNetCore.Facebook
....
```

> 数据库

appsettings.json

```
"ConnectionStrings": {
	"DefaultConnection": "Server=(Localdb)\\mssqllocaldb;Database=ASPNetIdentity;Trusted_Connection=True;MultipleAcitveResultSets=true"
}
```

> IdentityUser

ApplicationUser
```
public class ApplicaitonUser : IdentitUser
{}
```

与IdentityUser类似的还有：

```
IdentitRole
IdentityUserRole
IdentityUserClaim
IdentityUserLogin
IdentityUserToken
IdentityRoleClaim
```

> Configure Entity Framework

ApplicationDbContext.cs

```
public class ApplicationDbContext : IdentityDbContext<Applicatioinuser>
{
	public ApplicaitonDbContext(DbContextOptions<ApplicationDbContext> options) : base(opitons){}

	public override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
	}
}
```

> Startup.cs

```
services.AddDbContext<ApplicaitonDbContext>(options => options.UserSqlServer(Configuration.GetConnectionString("DefaultConnection")));

services.AddIdentity<ApplicationUser, IdentitRole>()
	.AddEntityFrameworkStores<ApplicaitonDbContext>()
	.AddDefaultTokenProviders();

app.UseAuthentiaiton();
```

> 生成数据库

安装一些组件

```
install-package Microsoft.EntityFrameworkCore.Tools
install-package Microsoft.EntityFrameworkCore.Tools.DotNet
```

ASPNETCoreIdentity.csproj

```
<ItemGroup>
	<DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools" Version="2.0.0" />
	<DotNetCliToolReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Tools" Version="2.0.0" />
	<DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0">
</ItemGroup>
```

实现迁移
```
dotnet ef migrations add Initial
dotnet ef database update
```

> 注册

```
public class RegisterViewModel
{
	public string Email{get;set;}

	public string Password{get;set;}

	public string ConfirmPassword{get;set;}
}


public class AccountController
{
	private readonly UserManager<ApplicaitonUser> _userManager;
	private redonly SignInManager<ApplicationUser> _signInManger;
	private readonly ILogger _logger;

	public AccountController(
		UserManager<ApplicatonUser> userManager,
		SignInMAANGER<ApplicaitonUser> signInManger,
		Ilogger<AccountController> looger
	)
	{
		this._userManger = userManger;
		this._signInManager = signInManger;
		this._logger = logger;
	}

	private void AddErrors(IdentityResult result)
	{
		foreach(var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}
	}

	privarte IActionResult RedirecToLocal(string returnUrl)
	{
		if(Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}
		return RedirectToAction(nameof(HomeController.Index),"Home");
	}

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Register(string returnUrl = null)
	{
		ViewData["ReturnUrl"] = returnUrl;
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
	{
		ViewData["ReturnUrl"] = returnUrl;
		if(ModelState.IsValid)
		{
			var user = new ApplicaitonUser{UserName = model.Email, Email = model.Email};

			var result = await _userManager.CreateAsync(user, model.Pasword);

			if(result.Suceeded)
			{
				_logger.LogInformation("User created a new account with password.");

				await _signInManager.SignInAsync(user, isPersistent:false);
			
				_logger.LogInformation("user logged");
				return RedirectToLocal(returnUrl);
			}
			AddErrors(result);
		}
		return View(model);
	}
}
```

> _ViewImports.cshtml

```
@using Microsoft.AspNetCore.Identity
@using ASPNetCoreIdentity
@using ASPNetCoreIdentity.Models
@using ASPNetCoreIdentity.Models.AccountViewModles
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

> 注册视图

```
@model ASPNetCoreIdentity.Models.AccountViewModels.RegisterViewModel

<form asp-route-returnUrl ="@ViewData["ReturnUrl"]" mehtod="post">
	<div asp-validation-summary="All"></div>
	
	<div class="form-group">
		<label asp-for="Email"></label>
		<input asp-for="Email"></label>
		<input asp-for="Email" class="form-control"/>
		<span asp-validation-for="Email" class="text-danger"></span>
	</div>

	...

	<button type="submit" class="btn btn-default">Register</button>
</form>

@section Scripts {
	@await Html.PartialAsync("_ValidationScriptsPartial");
}

```

> _Layout.cshtml

```
@await Html.PartialAsync("_LoginPartial")
```

> _LoginPartial.cshtml

```
@using Microsoft.AspNetCore.Identity
@using ASPNetCoreIdentity.Models
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
@if(SignInManager.IsIignedIn(User))
{
	<form asp-area="" asp-controller="Account" asp-action="Logout" method="post" id="logoutForm" class="navbar-right">
		<ul class="nav navbar-nav navbar-right">
			<li>
				<a asp-area="" asp-controller="Manage" asp-action="Index" title="Manage"> Hello @UserManage.GetUserName(User)</a>
			</li>
			<li>
				<button type="submit" class="btn btn-link navbar-btn navbar-link">Logout</button>
			</li>
		</ul>
	</form>
}
else 
{
	<ul class="nav nav-bar navbar-right">
		<li>
			<a asp-area="" asp-controller="Account" asp-action="Register">Register</a>
		</li>
		<li>
			<a asp-area="" asp-controller="Account" asp-action="Login">Log in</a>
		</li>
	</ul>
}
```

> 登录视图

```
public class LoginViewModel
{
	public string Email{get;set;}
	public string Password{get;set;}
	public bool RemberMe{get;set;}
}
```

> 登录action

```
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> Login(string returUrl = null)
{
	await HttpContext.SignOutAsync(IdentityConstancts.ExternalScheme);
	ViewData["ReturnUrl"] = returnUrl;
	return View();
}

[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
publi async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
{
	ViewData["ReturnUrl"] = returnUrl;
	if(ModelState.IsValid)
	{
		var result = await _signInManger.PasswordSignInAsync(model.Email, model.Password, model.RemberMe, LockoutOnFailure:false);
		
		if(result.Suceeded)
		{
			_logger.LogInformation();
			return RedirecToLocal(returnUrl);
		}
		if(result.IsLockedOut)
		{
			_logger.LogWarning();
			return RedirectToAction(nameof(Lockout));
		}
		else 
		{
			ModelState.AddModelError(string.Empty, "");
			return View(model);
		}
	}
	return View(model);
}

[HttpGet]
[AllowAnonymous]
publi IActionResult Lockout()
{
	return View();
}
```

> 登录视图

```
@using System.Collections.Generic
@using System.Linq
@uisng Microsoft.AspNectCore.Http
@using Microsoft.AspNetCore.Http.Authenticaiton
@model LoginVIwModel
@inject SignInManger<ApplicaiotnUser> SignInManager


<div class="row">
	<div class="col-md-4">
		<seciton>
			<form asp-route-returnUrl = "@ViewData["ReturUrl"]" method="post">
				<div asp-validation-summary="All" class="text-danger"></div>

				<div class="form-group">
					<label asp-for="Email"></label>
					<input asp-for="Email" class="form-control" />
					<span class="text-danger" asp-validation-for="Eamil"></span>
				</div>
				...

				<div class="form-group">
					<div class="checkbox">	
						<label asp-for="RemberMe">
							<input asp-for="RemberMe" />
							@Html.DisplayNameFor(m => m.RemberMe)
						</label>
					</div>
				</div>

				<div class="form-group">
					<button type="submit" clas="btn btn-default">Log in</button>
				</div>
				<div class="form-group">
					<p>
						<a asp-action="ForgotPassword">Forgot your password></a>
					</p>
					<p>
						<a asp-action="Register" asp-route-returnUrl="@ViewData["ReturnUrl"]">Register as a new user?</a>
					</p>
				</div>
			</form>
		</section>
	</div>
	<div class="col-md-6 col-md-offset-2">
		<section>
			@{
				var loginPrividers = (await signInManager.GetExternalAuthenticationSchemeAsync()).ToList();

				if(loginProviders.COunt == 0){
					<div>
						<p>
							
						</p>
					</div>
				}
				else 
				{
					<form asp-action="ExternalLogin" asp-route-returnUrl="@ViewData["ReturnUrl"]" method="post" class="form-horizontal">
						<div>
							<p>
								@foreach(var provider in loginProviders){
									<button type="submit" class="btn btn-default" name="provider" value="@provider.Name" title="">@provider.Name</button>
								}
							</p>
						</div>
					</form>
				}
			}
		</section>
	</div>
</div>
@section Scripts {
	@await Html.PartialAsync("_ValidationScriptsPartial")
}
```

> 登出

```
[HttpPost]
[ValidateAntiFogeryToken]
public async Task<IActionResult> Logout()
{
	await _signInManager.SignOutAsync();
	_logger.LogInformation();
	return RedirecToAction(nameof(HomeCOntroler.Index), "Home");
}
```
