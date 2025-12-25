using ContactManager.Core.Domain.IdentityEntites;
using ContactManager.Core.DTO;
using ContactManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration.UserSecrets;
using NuGet.Packaging.Signing;
using System.Threading.Tasks;
namespace ContactManager.UI.Controllers
{
    //applying Conventional routing
    //[Route("[controller]/[action]")]
    //[AllowAnonymous] allow unconditional access

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAntiforgery _antiforgery;
        private readonly IEmailSender<ApplicationUser> _emailSender;
        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IAntiforgery antiforgery)

        {
            _roleManager = roleManager;
            _UserManager = manager;
            _signInManager=signInManager;
            _antiforgery=antiforgery;
        }
        [HttpGet]
        [Authorize("NotAuthorized")]

        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        [Authorize("NotAuthorized")]
        [AutoValidateAntiforgeryToken]//adding the forgery token to the Post request form
        public async Task<IActionResult> Register(RegisterDTO dTORegister)
        {
            if (ModelState.IsValid==false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(dTORegister);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = dTORegister.Email,
                PhoneNumber=dTORegister.Phone,
                UserName=dTORegister.Email,
                PersonName = dTORegister.PersonName,

            };

            IdentityResult result = await _UserManager.CreateAsync(user, dTORegister.Password);//create the application USer as User in the identuty data base
            if (result.Succeeded==false)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);

                }
                return View(dTORegister);
            }
            if (dTORegister.UserType == Core.Enums.UserOptionType.Admin)
            {
                if (await _roleManager.FindByNameAsync(Core.Enums.UserOptionType.Admin.ToString()) is null)
                {
                    ApplicationRole role = new ApplicationRole() { Name=Core.Enums.UserOptionType.Admin.ToString() };
                    await _roleManager.CreateAsync(role);//creating the role in to the application role

                }
                await _UserManager.AddToRoleAsync(user, Core.Enums.UserOptionType.Admin.ToString());//adding the user to the rule
            }
            else
            {
                ApplicationRole role = new ApplicationRole() { Name=UserOptionType.User.ToString() };
                await _roleManager.CreateAsync(role);
                await _UserManager.AddToRoleAsync(user, Core.Enums.UserOptionType.User.ToString());
            }
            await _signInManager.SignInAsync(user, isPersistent: false);


            return RedirectToAction(nameof(PersonsController.Index), "Persons");

            //since we are getting the value from controlelr to the view  we can use
        }
        [Authorize("NotAuthorized")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Erros=ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                ApplicationUser? user = await _UserManager.FindByEmailAsync(loginDTO.Email);
                if (user != null)
                {
                    if (await _UserManager.IsInRoleAsync(user, UserOptionType.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }
                if (!string.IsNullOrEmpty(ReturnUrl)&&Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            ModelState.AddModelError("Login", "User name or password incorrect");

            return View(loginDTO);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailRegistered(string email)
        {
            ApplicationUser? user = await _UserManager.FindByEmailAsync(email);
            if (user==null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public IActionResult GetForgeryToken()
        {
            //create the token to send to the browser
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return new JsonResult(new
            {
                tokenName = tokens.HeaderName,
                tokenValue = tokens.RequestToken
            });

        }
        [HttpGet]
        [Authorize("NotAuthorized")]
        public IActionResult ResetEmail()
        {
            return View();
        }
        [Authorize("NotAuthorized")]
        [HttpPost]
        public async Task<IActionResult> ResetEmail(ResetEmail? resetEmail)
        {
            if (resetEmail!=null)
            {


                if (resetEmail.Email!=null)
                {
                    ApplicationUser? user = await _UserManager.FindByEmailAsync(resetEmail.Email);
                    if (user!=null)
                    {
                        var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
                        var CallbackURL = Url.Page(pageName: "/Account/ResetPassword", pageHandler: null
                            , values: new { userId = user.Id, token = token, email = resetEmail.Email },protocol:Request.Scheme)
                        ;
                        if(CallbackURL!=null)
                        await _emailSender.SendPasswordResetLinkAsync(user, resetEmail.Email, CallbackURL);


                    }
                    else
                    {
                        ModelState.AddModelError("Email", "the email does not  exist");
                        return View(resetEmail);
                    }
                    return View(resetEmail);
                }
            }
            return View();

        }
        [Authorize("NotAuthorized")]
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO.Code!=null) { 
            ApplicationUser user = await _UserManager.FindByIdAsync(resetPasswordDTO.Code);
                if (user!=null)
                {
                  await _UserManager.ResetPasswordAsync(user, resetPasswordDTO.Code, resetPasswordDTO.ConfirmPassword);
                    LocalRedirect("Account/Login");
                }
            }
            ViewBag.Erros=ModelState.Values.SelectMany(v => v.Errors).Select(error => error.ErrorMessage);
            return View(resetPasswordDTO);

        }
    }
}
