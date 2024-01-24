using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AIOtopark.Controllers
{
    public class BaseAdmin : Controller
    {
        public bool IsSessionAlive()
        {
            var session = HttpContext.Session.GetString("UserSession");
            if (session != null)
            {
                if (session.Equals("admin"))
                    return true;
                return false;
            }
            return false;
            
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsSessionAlive())
            {
                TempData["error"] = "Sayfayı görüntülemek için giriş yapmalısınız!";
                context.Result = RedirectToAction("adminSignIn", "Login");
            }
        }   
    }
}
