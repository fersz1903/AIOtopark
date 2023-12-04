using Microsoft.AspNetCore.Mvc;

namespace AIOtopark.Controllers
{
    public class UserController : Controller // base controller ile değiştirilecek authentication için.
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ParkingLots()
        {
            return View();
        }
    }
}
