using ContactManager.Core.Domain.IdentityEntites;
using ContactManager.Core.DTO;
using ContactManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace ContactManager.UI.Controllers
{
    //applying Conventional routing
    //[Route("[controller]/[action]")]
    //[AllowAnonymous] allow unconditional access
 
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager,RoleManager<ApplicationRole>roleManager)

        {
            _roleManager = roleManager;  
            _manager = manager;
            _signInManager=signInManager;
        }
        [HttpGet]
        [Authorize("NotAuthorized")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthorized")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterDTO dTORegister)
        {
            if (ModelState.IsValid==false) {
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
           
          IdentityResult result= await _manager.CreateAsync(user,dTORegister.Password);//create the application USer as User in the identuty data base
            if(result.Succeeded==false)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);

                }
                return View(dTORegister);
            }
            if(dTORegister.UserType == Core.Enums.UserOptionType.Admin)
            {
                if(await _roleManager.FindByNameAsync(Core.Enums.UserOptionType.Admin.ToString()) is null)
                {
                    ApplicationRole role= new ApplicationRole() { Name=Core.Enums.UserOptionType.Admin.ToString()};
                   await _roleManager.CreateAsync(role);//creating the role in to the application role
                    
                }
               await _manager.AddToRoleAsync(user, Core.Enums.UserOptionType.Admin.ToString());//adding the user to the rule
            }
            else
            {
                ApplicationRole role = new ApplicationRole() { Name=UserOptionType.User.ToString() };
               await _roleManager.CreateAsync(role);
                await _manager.AddToRoleAsync(user, Core.Enums.UserOptionType.User.ToString());
            }
           await _signInManager.SignInAsync(user,isPersistent:false);
             
          
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
        public async Task<IActionResult> Login(LoginDTO loginDTO,string?ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Erros=ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                ApplicationUser ?user = await _manager.FindByEmailAsync(loginDTO.Email);
                if (user != null) {
                    if(await _manager.IsInRoleAsync(user,UserOptionType.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }
                if(!string.IsNullOrEmpty(ReturnUrl)&&Url.IsLocalUrl(ReturnUrl))
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
        public async Task<IActionResult>IsEmailRegistered(string email)
        {
            ApplicationUser? user =await _manager.FindByEmailAsync(email);
            if(user==null)
            {
                return Json(true);
            }   
            else
            {
                return Json(false);
            }
        }
    }
}
