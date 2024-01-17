using Microsoft.AspNetCore.Mvc;

namespace AutomatedEducationProgram.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
