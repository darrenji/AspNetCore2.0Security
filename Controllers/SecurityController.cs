using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Darren.Security.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Darren.Security.Services.Identity;
using Darren.Security.Services.Email;

namespace Darren.Security.Controllers
{
    public class SecurityController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly SignInManager<AppIdentityUser> signInManager;
        private readonly IEmailSender emailSender;

        public SecurityController(
            UserManager<AppIdentityUser> userManager,
            SignInManager<AppIdentityUser> signInManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        #region login, authentication and authorization
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //先看看有没有这个用户
            var user = await this.userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                //再看看是否邮箱验证过了
                if (!await this.userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "confirm your email please");
                    return View(model);
                }
            }

            //好了，登录吧
            var result = await this.signInManager.PasswordSignInAsync(model.Username,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "login failied");
            return View(model);

        }

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        } 
        #endregion

        #region register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppIdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Age = model.Age
            };

            var result = await this.userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //登录
                //await this.signInManager.SignInAsync(user, isPersistent:false);

                //产生验证码
                var confirmationCode = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                //回调地址
                var callbackurl = Url.Action(
                    controller: "Security",
                    action: "ConfirmEmail",
                    values: new { userId = user.Id, code = confirmationCode },
                    protocol: Request.Scheme);
                //发邮件
                await this.emailSender.SendEmailAsync(email: user.Email, subject: "confirm email", message: callbackurl);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        //当在邮件中点击的时候，执行这里
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToAction("Index", "Home");

            //先确认是否有用户
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApplicationException($"unable to load user with id {userId}");

            var result = await this.userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            return RedirectToAction("Index", "Home");
        } 
        #endregion

        #region forgot password
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            //先判断参数
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }

            //根据email找用户
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToAction("ForgotPasswordEmailSent");

            //用户邮件确认过了吗
            if (!await this.userManager.IsEmailConfirmedAsync(user))
                return RedirectToAction("ForgotPasswordEmailSent");

            //发邮件
            var confirmationCode = await this.userManager.GeneratePasswordResetTokenAsync(user);
            var callbackurl = Url.Action(controller: "Security",
                action: "ResetPassword",
                values: new { userId = user.Id, code = confirmationCode },
                protocol: Request.Scheme);
            await this.emailSender.SendEmailAsync(email: user.Email, subject: "reset password", message: callbackurl);
            return RedirectToAction("ForgotPasswordEmailSent");
        }

        public IActionResult ForgotPasswordEmailSent()
        {
            return View();
        } 
        #endregion

        #region reset password

        public IActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                throw new ApplicationException("code must be supplied for pasword reset");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //找找用户
            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirm");

            var result = await this.userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirm");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);

        }

        public IActionResult ResetPasswordConfirm()
        {
            return View();
        } 
        #endregion
    }
}