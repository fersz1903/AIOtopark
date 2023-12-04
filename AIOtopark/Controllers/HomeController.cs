using AIOtopark.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SpotsCheckService;
using FirebaseService.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FirebaseAdmin.Auth;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
//using Firebase.Auth.Providers;

namespace AIOtopark.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<ParkingLotPreviewModel> list = await FirebaseService.Program.getParkingLotPreviews();


            //getAvailableSpots();
            //SpotsCheckService.Program.deneme();
            //getAvailableSpotsWithPhoto();
            //SpotsCheckService.Program.sendPostRequest();
            return View(list);
        }

        //public async void getAvailableSpots()
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        // API'den veri almak istediğiniz URL
        //        string apiUrl = "http://127.0.0.1:5000/process_image";

        //        // GET isteği oluştur
        //        HttpResponseMessage response = await client.GetAsync(apiUrl);

        //        // Yanıtı kontrol et
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string result = await response.Content.ReadAsStringAsync();
        //            Debug.WriteLine("Başarılı GET isteği. Yanıt: " + result);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("GET isteği başarısız! HTTP durum kodu: " + response.StatusCode);
        //        }
        //    }
        //}

        //public async void getAvailableSpotsWithPhoto()
        //{
        //    // API endpoint URL'si
        //    string apiUrl = "http://127.0.0.1:5000/check_parking_status"; // Endpoint URL'si

        //    // Fotoğraf dosyasının yolu
        //    string imagePath = @"C:\Users\Furkan\Desktop\first_frame.png"; // fotoğrafın yolu

        //    // HTTP Client oluşturma
        //    using (HttpClient client = new HttpClient())
        //    using (MultipartFormDataContent content = new MultipartFormDataContent())
        //    {
        //        // Fotoğraf dosyasını içeriğe ekleyin
        //        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        //        ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
        //        content.Add(imageContent, "image", "image.png"); // "image" parametresi, API'de beklenen dosya adıdır

        //        // POST isteği gönderme
        //        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        //        // Sunucudan gelen yanıtı okuma
        //        string responseContent = await response.Content.ReadAsStringAsync();

        //        // Yanıtı konsola yazdırma
        //        Debug.WriteLine("Response: " + responseContent);
        //    }

        //}


        public IActionResult Privacy()
        {
            return View();
        }


        


        public IActionResult SignIn() 
        {
            return View();
        }

        //public async Task<IActionResult> Login(LoginModel login)
        //{



        //}


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}