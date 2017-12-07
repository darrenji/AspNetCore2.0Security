OWIN,是Open Web Interface for .NET的缩写，是用来验证用户的一个框架。也会给出authetnicaiton cookie也允许第三方登录。

```
install-package Microsoft.AspNet.Identity.Owin
```

> Configuring OWIN Middleware

```
public parital class Startup
{
	public void Configuration(IAppBuilder app)
	{
		ConfigureAuth(app);
	}

	
}

public partial class Startup
	{
		public void ConfigureAuth(IAppBuilder app)
		{

		}
	}
```

IdentityModels.cs

```
public class ApplicaitonDbContext : IdentityDbContext<Applicaitonuser>
{
	public ApplicaiotnDbContext():base()
	{
		
	}

	public astatic ApplicaitonDbContext Create()
	{
		return enw ApplicaitonDbCotext();
	}
}
```

Startup.Auth.cs

```
app.CreatePerOwnConext(AppplicaitonDbContext.Create);
```

> UserManager

IdentityConfig.cs

```
public class ApplicationUserManager : UserManger<ApplicaitonUser>
{
	public ApplicationUserManager(IUserStore<ApplicationUser> store):base(store){}

	public static ApplicationUseManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
	{
		//在创建store的时候用到了OWin中间件，或者用Owin中间件创建了一个上下文实例
		var store = new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>);
		
		var manager = new ApplicaitonUserManager(store);
		return manager;
	}
}
```

> Sign in Manager

```
public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
{
	public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticatioManager) : base(userManager, authenticationManager)
	{

	}

	public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicaiotnSignInManager> options, IOwinContext context)
	{
		return enw ApplicaitonSignInManager(context.GetUserManager<ApplicatonUserManager>(), context.Authentication);
	}
}
```

> Confiute Cookie Authenticaiton

Startup.AUth.cs
```
app.UseCookieAuthenticaiton(new CookieAuthenticaitonOptions{
	AuhtenticationType = DefaultAuthenticaitonTypoes.ApplicaoitnCookie,
	LoginPath = new PathString("/Account/Login");
});
```

> 登录的视图模型

```
public class LoginViewModel
{
	public string Email
	publis tring Password
	public bool RememberMe
}
```
> Account Controller

```
public class AccountControlle : Controller
{
	private ApplicatonSignInManager _signInManager;
	private ApplicatioinUserManager _userManger;

	public ApplicationSignInManager SignInMaanger
	{
		get
		{
			return _signInManager ?? HttpContext.GetWinCOntext().Get<ApplicaitonSignInManager>();
		}
		private set
		{
			_signInManger = value;
		}
	}

	public ApplicaitonUserManager UserManager
	{
		get
		{
			return _userManager?? HttpContext.GetOwinContext().GetUserManager<ApplicaiotnUserManager>();
		}
		private set
		{
			_userManager = value;
		}
	}

	private IAuthenticaitonManager AuthenticationManager
	{
		get {
			return HttpContext.GetOwinCOntext().Authenticaiton;
		}
	}

	[AllowAnonymous]
	public ActionResult Login(string returnUrl)
	{
		ViewBag.ReturnUrl = returnUrl;
		return View();
	}

	[HttpPost]
	[AllowAnonmous]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Login(LoginViewModel model, string retunUrl)
	{
		if(!ModelState.IsValid)
			return View(model);

		var result = awiat SignInManager.PasswordSignInAsync(model.Email, model.passwrod, model.RemeberMe, shoudlLockout:false);

		switch(result)
		{
			case SignInStatus.Success:
				return ReirectTolOCAL(returnUrl);
			case SignInStatus.LockdOut:
				return View("Lockout");
			case SignInStatus.RequrieVerification:
				return RedirectToAction("SendCOde", new {ReturnUrl = returnUlr, RemeberMe = mdoel.RemberMe});
			case SignInStatus.Failure:
			default:
				ModelState.AddModelError("", "");
				return View(model);
		}
	}

	private ActionResult RedirecToLocal(string returnUrl)
	{
		if(Url.IsLocalUrl(returUrl))
		{
			return Redirect(retunRul);
		}
		return RedirecToAction("Index", "Home");
	}

	[ttpPost]
	[ValidateAntiForgeryToken]
	[AllowAnonymous]
	public async Task<ActionResult> Register(ReegisterViewMoel model)
	{
		if(ModelState.IsValid)
		{
			var user = new ApplicationUser{UserName=model.Email, Email = model.Email};
			
			var result = await UserManger.CreateAsync(user, user.Password);
			if(result.Succeeded)
			{
				awit SignInManager.SingInAsync(user, isPersistnt:false, remenberBRWOSERF:FALSE);
			}
		}
		AddErrors(result);
	}


	[HttpPost]
	[ValdiateAntiForgeryToken]
	public ActionResult LogOff()
	{
		AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicaitonCookie);
		return RedirecToAction("Index", "Home");
	}
}
```




