Identity team给出了一个Sample Project.

```
install-package Microsoft.AspNet.Identity.Samples -Pre
```

ApplicationUser
```
public class ApplicationUser : IdentityUser
{
	public async Task<ClaimsIdentity> GenereateUserIdentityAsync(UserManager<ApplicationUser> manager)
	{
		var userIdetnity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicaitonCookie);
		return userIdentity;
	}
}
```

IdentityUser
```
public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUser, IUser<string>
{
	public Identityuser()
	{
		this.Id = Guid.NewGuid().ToString();
	}

	public Identityuser(string userName) : this()
	{
		this.UserName = userName;
	}
}
```

IdentityUser看上去像一个类，实际上是泛型的，继承实现了一些泛型接口。其它类似的还有：

```
public class IdentityUserRole : IdentityUserRole<string>{}

public class IdentityRole : IdentityRole<string, IdentityUserRole>{}

public class IdentityUserClaim : IdentityUserClaim<string>{}

public class IdentityUserLogin : IdentityUserLogin<string>{}

public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityUserRole,, IdentityUserClaim>, IUser, IUser<string>{}

public class IdentityDbContext : IdentityDbContext<IdentityUser,, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>{}

public class UserStore<TUser> : UserStore<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUserStore<TUser>, IUserSotre<TUser,, string>, IDisposable where TUser: IdentityUser{}


public class RoleStore<TRole> : RoleStore<TRole, string, IdentityUserRole>,, IQueryableRoleStore<TRole>, IQueryableRoleStore<TRole, string>,IRoleStore<TRole, string>,, IDisposable where TRole : IdentityRole, new(){}
```

> 自定义IdentityUser

```
public class ApplicationUser : IdentityUser
{
	public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
	{
		var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTyeps.ApplicationCookie);
		return userIdentity;
	}

	public string Address{get;set;}
	public string City{get;set;}
	public string State{get;set;}
	[Displa(Name="Postal Code")]
	public string PostalCode{get;set;}

	public string DisplayAddress
	{
		get {
			string dspAddress = string.IsNullOrWhiteSpace(this.Address) ? "" this.Address;

			string dspCity = string.IsNullOrWhiteSpace(this.City) ? "" : this.City;

			string dspState = string.IsNullOrWhiteSpace(this.State) ? "" : this.State;

			string dspPostalCode = string.IsNullOrWhiteSpace(this.PostalCode) ? "" : this.PostalCode;

			return string.Format("{0} {1} {2} {3}", dspAddress, dspCity, dspState, dspPostalCode);
		}
	}
}
```

> 注册模型

```
public class RegisterViewModel
{
	public string Email{get;set;}

	public string Password{get;set;}

	public string ConfirmPassword{get;set;}

	//自定义属性
	public string Address{get;set;}
	public string City{get;set;}
	public string State{get;set;}

	[Display(Name = "Postal Code")]
	public string PostalCode{get;set;}
}
```

> 注册视图

Accounts/Register.cshtml

```
@model IdentitySample.Models.RegisterViewModel
@{
	ViewBag.Title = "Register";
}
<h2>@ViewBag.Title</h2>
@using(Html.BebinForm("Register", "Account", FormMethod.Post, new {@class="form-horizontal",role="form"})){
	@Html.AntiForgeryToken()
	<h4>Create a new account.</h4>
	<hr/>

	@Html.ValidationSummary("",new {@class="text-danger"})

	<div class="form-group">
		@Html.LabelFor(m => m.Email, new {@class="col-md-2 control-label"})
		<div class="col-md-10">
			@Html.TextBoxFor(m => m.Email, new {@class="form-control"})
		</div>
	</div>

	...
	
	<div class="form-group">
		<div class="col-md-offset-2 col-md-10">
			<input type="submit" class="btn btn-default" value="Register" />
		</div>
	</div>
}
@section Scripts {
	@Scripts.Render("~/bundles/jqueryval")
}
```

> 注册的控制器部分

```
[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model)
{
	if(ModelState.IsValid)
	{
		var user = new Applicaitonuser{
			UserName = model.Email,
			Email = model.Email
		};

		//add the custom properties
		user.Address = model.Address;
		user.City = model.City;
		user.State = model.State;
		user.PostalCode = model.PostalCode;

		var result = await UserManager.CreateAsync(user, model.Password);
		if(result.Succedded)
		{
			var code = await UserManager.GenerateEmailCONfirmationTokenAsync(user.Id);

			var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code = code}, protocol: Request.Url.Scheme);

			await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your acount by clicking this link:<a href=\""+callbackUrl + "\">link</a>");

			ViewBag.Link = callbackUrl;
			return View("DisplayEmail");
		}
		AddErrors(result);
	}
	return View(model);
}
```

> 管理用户注册的视图

UsersAdin/Create.cshtml

```
@model IdentitySampe.Models.RegisterViewModel
@{
	ViwBag.Title = "Create";
}
<h2>@ViewBag.Title</h2>
@using(Html.BeginForm("Create", "UsersAdmin", FormMethod.Post, new{@class="form-control", role="form"})) {
	@Html.AntiForgeryToken()
	<h4>Create a new account</h4>
	<hr/>
	@Html.ValidationSummary("", new {@class="text-error"})

	<div class="form-group">
		@Html.LabelFor(m => m.Email, new {@class="col-md-2 control-label"})
		<div class="col-md-10">
			@Html.TextBoxFor(m => m.Email, new {@class="form-control"})
		</div>
	</div>

	<div class="form-group">
		<label class="col-md-2 controler-label">
			Select User Role
		</label>
		<div class="col-md-10">
			@foreach(var item in (SelectList)ViewBag.RoleId)
			{
				<input type="checkbox" name="SelectedRoles" value="@item.Value" class="checkbox-inline" />
		
				@Html.Label(item.Value, new {@class="control-label"})
			}
		</div>		
	</div>

	<div class="form-group">
		<div class="col-md-offset-2 col-md-10">
			<input type="submit" class="btn btn-default" value="Create" />
		</div>
	</div>
}
@section Scripts {
	@Scripts.Rneder("~/bundles/jqueryval")
}
```

> 管理用户视图模型

```
public class EditUserViewModel
{
	public string Id{get;set;}

	public string Email{get;set;}

	public string Address{get;set;}
	public string CIty{get;set;}
	public string State{get;set;}

	[Display(Name="")]
	public string PostalCode{get;set;}

	public IEnumerable<SelectListItem> RolesList{get;set;}
}
```

> 管理用户更新视图

```
@using(Html.BeginForm(){
	@Html.AntiForgeryToken()

	<div class="form-horizontal">
		<h4>Edit user</h4>
		<hr/>
		@Html.ValidationSummary(true)
		@Html.HiddenFor(model => model.Id)

		<div class="form-group">
			@Html.LabelFor(model => model.Email, new {@class="control-label  col-md-2"})
			<div class="col-md-10">
				@Html.TextBoxFor(m => m.Email, new {@class="form-control"})
				@Html.ValidationMessageFor(model => model.Email)
			</div>
		</div>
		...
		<div class="form-group">
			@Html.Label("Roles", new {@class="control-label col-md-2"})
			<span class="col-md-10">
				@foreach(var item in Model.RoleList)
				{
					<input type="checkbox" name="SelectedRole" value="@item.Value" />

					@Html.Label(item.Value, new {@class="control-label"})
				}
			</span>
		</div>
	</div>
})

<div>
	@Html.ActionLink("Back to List", "Index")
</div>

@section Scripts {
	@Scripts.Render("~/bundles/jqueryval")
}
```

> 管理用户查看

UsersAdmin/Index.cshtml

```
@model IEnumerable<IdentitySample.Models.ApplicaitonUser>

@{
	ViewBag.Title = "Index";
}

<h2>Index</h2>

<p>
	@Html.ActionLink("Create New", "Create")
</p>


<table class="table">
	<tr>
		<th>
			@Html.DisplayNameFor(model => model.UserName)
		</th>
		<th>
			@Html.DisplayNameFor(model => model.DisplayAddress)
		</th>
		<th>
		</th>
	</tr>
	@foreach(var item in Model)
	{
		<tr>
			<td>
				@Html.DisplayFor(modelItem => item.UserName)
			</td>
			<td>
				@Html.DisplayFor(modelItem => item.DisplayAddress)
			</td>
			<td>
				@Html.ActionLink("Edit", "Edit", new {id =item.Id}) |	
				@Html.ActionLink("Details", "Details", new {id=item.Id}) |
				@Html.ActionLink("Delete", "Delete", new {id = item.Id})
			</td>
		</tr>
	}
</table>
```

> 用户管理控制器部分

```
//创建
public async Task<IActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles) {
	if(ModelState.IsValid)
	{
		var user = new ApplicaitonUser
		{
			UserName = userViewodel.Email,
			Email = userViewMOdel.Email,
			Address = userViewModel.Address,
			City = userViewModel.City,
			State = userViewModel.State,
			PostalCode = userViewModel.PostalCode
		};

		user.Address = userVIewModel.Address;
		user.City = userViewModel.City;
		user.State = userViewModel.State;
		user.PostalCode = userViewmodle.PostalCode;

		var adminResult = await UserManager.CreateAsync(user, userViewmodel.Password);

		if(adminResult.Succeeded)
		{
			if(selectedRoles!=null)
			{
				var result = await UserManager.AddToRoleAsync(user.Id, selectedRoles);
				if(!result.Succeeded)
				{
					ModelState.AddModelError("", result.Errors.First());
					ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(),"Name", "Name");

					return View();
				}
			}
		}
		else 
		{
			ModelState.AddModelError("", adminresult.Errors.First());
			ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
			return View();
		}
		return RedirectToAction("Index");
	}
	ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
	return View();
}

//更新展示
public async Task<IActionResult> Edit(string id)
{
	if(id == null)
	{
		return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
	}
	var user = await UserManager.FindByIdAsync(id);
	if(user == null)
	{
		return HttpNotFound();
	}

	var userRoles = await UserManager.GetRolesAsync(user.Id);

	return View(new EidtUserViewModel(){
		Id = user.id,
		Email = user.Email,
		Addres = user.Address,	
		City = user.CIty,
		State = user.State,
		PostalCode = user.PostalCode,
		RolesList = RoleManager.Roels.ToList().Select(x => new SelectListItem(){
			Selected  = userRoles.Contains(x.Name),
			Text = x.Name,
			Value = x.Name
		})
	});
}

//更新
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit([Bind(Include="Email, Id, Address, CIty, State, PostalCode")]EditUserViewModel editUser, params string[] selectedRole)
{
	if(ModelState.IsValid)
	{
		var user = await UserManager.FindByIdAsync(editUser.Id);
		if(user==null)
		{
			return HttpNotFound();
		}

		user.UserName = editUser.Email;
		user.Email = edtUser.Email;
		user.Address = editUser.Email;
		user.City = editUser.CIty;
		user.State = editUser.State;
		user.PostalCode = editUser.PostalCode;

		var userRoles = await UserManager.GetRolesAsync(user.Id);
		selectedRole = selectedRole ?? new string[]{};

		var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

		if(!result.Succeeded)
		{
			ModelState.AddModelError("", result.Errors.First());
			return View();
		}

		result = await UserManager.RemoveFromRolesAsync(user.Id,userRoles.Except(selectedRole).ToArray<string>());

		if(!result.Succeeded)
		{
			ModelState.AddModelError("", result.Errors.First());
			return View();
		}
		return RedirecToAction("Index");
	}
	ModelState.AddModelError("", "sth failed");
	return View();
}
```

> ApplicaitonDbInitializer class in IdentityConfig.cs

```
public class ApplicaitonDbInitializer : DropCreateDtabaseIfModelChanges<ApplicationDbContext>
{
	protected override void Seed (ApplicationDbContext context)
	{
		InitializeIdentityForEF(context);
		base.Seed(context);
	}

	public static void InitializeIdentityForEF(ApplicationDbContext db)
	{
		var userManager = HttpContext.Current.GetOwinContext()
			.GetUserManager<ApplicaitonuserManager>();
		var roleManager = HttpContext.Current.GetOwinContext()
			.Get<ApplicationRoleManager>();

		const string name = "admin@example.com";
		const string password = "Admin@123456";
		const strig roleName = "Admin";

		var role = roleManager.FindyName(roleName);
		if(role==null){
			role = new IdentityRole(roleName);
			var roleresult = roleManager.Create(role);
		}

		var user = userMaanger.FindByName(name);		
		if(user == null)
		{
			user = new ApplicationUser{UserName = name, Email = name};
			var resul  = userManager.Create(user, password);
			result = userManager.SetLockoutEnabled(ussed.ID, false);
		}

		//add user role to roles
		var rolesForUser = userManager.GetRoles(user.id);
		if(!rolesForUser.Contains(role.Name))
		{
			var result = userManager.AddToRole(user.Id, role.Name);
		}
	}
}
```

> 准备扩展IdentityRole

```
public class IdentityRole  : IdentityRole<string, IdentityUserRole>
{
	publci IdentityRole()
	{
		base.Id = Guid.NewGuid.ToString();
	}

	public IdentityRole(string roleName) : this()
	{
		base.Name = roleName;
	}
}
```

> IdentityRole<TKey, TUserRole>

```
public class IdentityRole<TKey, TUserRole> : IRole<TKEY> where TUserRole : IdentityUserRole<TKey>
{
	public TKey Id
	{
		get {
			return JstDecompileGenereated_get_Id();
		}
		set {
			JustDecompliGenerated_set_Id(value);
		}
	}

	public string Name
	{
		get;
		set;
	}

	public ICollectioin<TUserRole> Users
	{
		get
		{
			return JustDecompileGenerated_get_Users();
		}
		set
		{
			JstDecompileGenerated_set_Users(value);
		}
	}
	public IdentityRole()
	{
		this.Users = new List<TUserRole>();
	}
}
```

> IdentityUserRole<TKey>

```
public class IdentityUserRole<TKey>
{
	public vrtual TKey RoleId
	{
		get;
		set;
	}

	public virtual TKey UserId
	{
		get;
		set;
	}
	public IdentityUserRole()
	{}
}
```

> 自定义角色模型

```
public class ApplicaitonRole : IdentityRole
{
	public ApplicaitonRole():base(){}
	publci ApplicaitonRole(string name) : base(name){}
	public string Description{get;set;}
}
```

> 由于自定义了角色模型，也要改ApplicaitonRoleManager

没改之前是这样的

```
public class ApplicationRoleManager : RoleManager<IdentityRole>
{
	public ApplicaitonRoleManager(IRoleStore<IdentityRole, string> roleStore):base(roleStore){}

	publicstaticApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
	{
		return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicaitonDbContext>()));
	}
}
```

我们需要修改成让依赖ApplicaitonRole而不是IdentityRole

```
public class ApplicaitonRoleManager : RoleManager<ApplicaionRole>
{
	public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore) : base(roleStore){}

	public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
	{
		return new ApplicaitonRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext()>));
	}
}
```

> 由于自定义了角色模型，初始化数据的时候也需要改

```
public static void InitializeIdentityForEF(ApplicationDbContext db)
{
	var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

	var roleManager = HttpContext.CUrrent.GetOwnContext().Get<ApplicaitonRoleManager>();

	const string name = "admin@example.com";
	const string password = "Adim@123456";
	const string roleName = "Admin";

	var role = roleManager.FindByName(roleName);
	if(role == null)
	{
		role = new ApplicatonRole(roleName);
		var roleResult = roleManger.Create(role);
	}

	var user = userManger.FIndByName(name);
	if(user==null)
	{
		user = new Applicationuser{UserNmae = name, Email = name};
		var result = userManager.Create(user, password);
		result = userManager.SetLockoutEnabled(user.Id, false);
	}

	var rolesForUser = userManager.GetRoles(user.Id);
	if(!rolesForUser.Contains(role.Name))
		var result = userManager.AddToRole(user.id, role.Name);
}
```

> 有关role的控制器

```
public async Task<IActionResult> Create(RoleViewModel roleViewModel)
{
	if(ModelState.IsValid)
	{
		var role = new ApplicaitonRole(roleViewModel.Name);
		var roleResult = await RoleManager.CreateAsync(role);
		if(!roleResult.Suceeded)
		{
			ModelState.AddModelEror("", roleresult.Errors.First());
			return View();
		}
		return RedirectToAction("Index");
	}
	return View();
}
```

> 有关role的视图模型

```
public class RoleViewModel
{
	public string Id{get;set;}
	public string Name{get;set;}
	public strig Description{get;set;}
}
```

> role的创建视图

```
@model IdentitySamoe.Models.RoleViewModel

@{
	ViewBag.Title = "Create";
}

<h2>Create</h2>

@using(Html.BeginForm())
{
	@Html.AntiForgeryToken()

	<div class="form-horizontal">
		@Html.ValidationSummary(true)

		<div class="form-group">
			@Html.LabelFor(model => model.name, new {@class = "control-label col-md-2"})
			<div class="col-md-10">
				@Html.TextBoxFor(model => model.Name, new {@class="form-control"})
				@Html.ValidationMessageFor(model => model.Name)
			</div>
		</div>
	</div>
}
```

> role的更新视图

```
@model IdentitySample.Models.RoleViewModel

@{
	ViewBag.Title = "";
}

<h2>Edit</h2>

@using(Html.BeginForm())
{
	@Html.AntiForgeryToken()

	<div class="form-horizontal">
		@Html.ValidationSummary(true)
		@Html.HiddenFor(model => model.Id)
	</div>
}
```

> 管理员的查看视图

```
@model IEnuemrable<IdentitySample.Models.ApplicationRole>

@foreach(var item in Model)
{
	
}
```

> role有关的管理员控制器

```
[HttpPost]
public async Task<IActionResult> Create(RoleViewModel roleViewModel)
{
	if(ModelState.IsValid)
	{
		var role = new ApplicationRole(roleViewModel.Name);

		role.Description = roleViewModel.Description;
		var roleResut = await RoleManager.CreateAsync(role);
		if(!rileresult.Succeeded)
		{
			ModelState.AddModelError("", reolseResult.Errrors.First());
			return View();
		}
		return RedirectToAction("Index");
	}
	return View();
}

//更新查看
public async Task<IActionResulut> Edit(string id)
{
	if(id == null)
		return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

	var role = await RoleManager.FindByIdAsyncf(id);
	if(role==null)
		return HttpNotFound();

	RoleViewModel roleModel = new RoleViewMOdel
	{
		Id = role.Id,
		Name = role.Name
	};

	roleModel.Description = role.Description;
	return View(roleModel);
}

//更新
[HttpPost]
[ValidateAntiForgeryToken]
public asycn Task<IActionResult> Edit([Bind(Include="Name, Id, Description")]RoleViewModel roleModel)
{
	if(ModelState.IsValid)
	{
		var role = await RoleManager.FindByIdAsync(roleModel.Id);
		role.Name = romeModel.Name;

		role.Descripiton = roleModel.Description;
		awit RoleManager.UpdateAsync(role);
		return RedirecToAction("Index");
	}
	return View();
}
```
