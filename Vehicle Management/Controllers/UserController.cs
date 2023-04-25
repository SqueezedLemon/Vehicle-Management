using Microsoft.AspNetCore.Mvc;

namespace Vehicle_Management.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }
    }
}
