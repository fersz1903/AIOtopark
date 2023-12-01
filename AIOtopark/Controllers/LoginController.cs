using Firebase.Auth;
using FirebaseAdmin;
using FirebaseService;
using FirebaseService.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AIOtopark.Controllers
{
    public class LoginController : Controller
    {
        //private readonly FirebaseAuthProvider authProvider;

        //public LoginController()
        //{
        //    // FirebaseApp.Create() ile oluşturulan app nesnesini kullanın
        //    //string apiKey = "AIzaSyBnTce8nLA0KRY-_YSX6UUqdfWWamdPLpM";

        //    //if (string.IsNullOrEmpty(apiKey))
        //    //{
        //    //    Console.WriteLine("API key not found in environment variables.");
        //    //    return;
        //    //}

        //    //authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
        //}



        public IActionResult SignIn()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
                return RedirectToAction("Index", "Home");
            else return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                LoginControl loginControl = new LoginControl();
                string message = await loginControl.login(model);

                if (message.Equals("success")) {
                    HttpContext.Session.SetString("UserSession", model.email);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", message);

                //try
                //{
                //    // Kullanıcıyı email ve şifre ile giriş yaptırın
                //    var auth = await authProvider.SignInWithEmailAndPasswordAsync(model.email, model.password);
                //    // Giriş başarılı ise, kullanıcıyı ana sayfaya yönlendirin
                //    return RedirectToAction("Index", "Home");
                //}
                //catch (FirebaseAuthException ex)
                //{
                //    var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                //    // Giriş başarısız ise, hata mesajını gösterin
                //    ModelState.AddModelError("", firebaseEx.error.message);
                //}
            }
            return View("SignIn", model);
        }


        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // forma bir token ekler ve geri dönüşünü kontrol eder. CSRF atağını engellemek için kullanılır.
        public async Task<IActionResult> Register(RegisterModel register)
        {

            if (ModelState.IsValid)
            {
                //FirebaseService.Program obj = new FirebaseService.Program();
                var result = await FirebaseService.Program.createUser(register);

                if (!result.Equals("success"))
                {
                    ModelState.AddModelError(string.Empty, result);
                    return View("Registration", register); // Hatayı içeren view'i göster
                }

                ModelState.Clear(); // ModelState'i temizle
                return RedirectToAction("SignIn", "Login");
            }
            else
            {
                // Model geçerli değilse, hatayı içeren view'i göster
                return View("Registration", register);
            }
        }
    }
}
