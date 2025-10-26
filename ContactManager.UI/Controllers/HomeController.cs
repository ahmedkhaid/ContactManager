using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("~/Error")]
        public IActionResult Index()
        {
            IExceptionHandlerPathFeature? exceptionPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if(exceptionPathFeature!=null&& exceptionPathFeature.Error!=null)
            {
                ViewBag.Error = exceptionPathFeature.Error.Message;
            }
            return View();
        }
    }
}
