using Microsoft.AspNetCore.Mvc;
using ContactManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Identity;
using ContactManager.Core.Domain.IdentityEntites;
using System.Threading.Tasks;
namespace ContactManager.UI.Controllers
{
    [Route("[controller]/[action]")]
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
                UserName=dTORegister.PersonName,
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
    }
}
