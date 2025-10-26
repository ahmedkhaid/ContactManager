using ContactManager.Core.Domain.IdentityEntites;
using ContactManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace ContactManager.UI.Controllers
{
    //applying Conventional routing
    //[Route("[controller]/[action]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager)
        {
            _manager = manager;
            _signInManager=signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
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
           
          IdentityResult result= await _manager.CreateAsync(user,dTORegister.Password);
            if(result.Succeeded==false)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);

                }
                return View(dTORegister);
            }
           await _signInManager.SignInAsync(user,isPersistent:false);
             
          
            return RedirectToAction(nameof(PersonsController.Index), "Persons");

            //since we are getting the value from controlelr to the view  we can use
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
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
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }
      
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
