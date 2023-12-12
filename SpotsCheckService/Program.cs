using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FirebaseService;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using SpotsCheckService.Models;

namespace SpotsCheckService
{
    public class Program
    {
        static void Main(string[] args)
        {
            FirebaseService.Program srv = new FirebaseService.Program();
            deneme();

            Thread.Sleep(10000);
        }

        public static async void deneme()
        {
            //Debug.WriteLine("SpotsCheckService working...");
            //var cts = new CancellationTokenSource();
            //ThreadPool.QueueUserWorkItem(state => ExecuteAsync(cts));
            //Thread.Sleep(10000);
            //cts.Cancel();

            //ThreadPool.QueueUserWorkItem(sendPostRequest);
            await sendPostRequest();

            Thread.Sleep(10000);
        }
        public static async Task ExecuteAsync(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                // API endpoint URL'si
                string apiUrl = "http://127.0.0.1:5000/check_parking_status"; // Endpoint URL'si

                // Fotoğraf dosyasının yolu
                string imagePath = @"C:\Users\Furkan\Desktop\first_frame.png"; // fotoğrafın yolu

                // HTTP Client oluşturma
                using (HttpClient client = new HttpClient())
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    // Fotoğraf dosyasını içeriğe ekleyin
                    byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                    ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
                    content.Add(imageContent, "image", "image.png"); // "image" parametresi, API'da beklenen dosya adıdır

                    // POST isteği gönderme
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Sunucudan gelen yanıtı okuma
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Yanıtı konsola yazdırma
                    Debug.WriteLine("Response: " + responseContent);
                }
                await Task.Delay(TimeSpan.FromSeconds(4), cts.Token);
            }
        }


        public static async Task sendPostRequest()
        {
            var jsonContentFile = getPoses();
            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string apiUrl = "http://127.0.0.1:5000/check_parking_status"; // Endpoint URL'si

                // Resim dosyasını içeriğe ekle
                string imagePath = @"C:\Users\Furkan\Desktop\first_frame.png"; // fotoğrafın yolu
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                content.Add(imageContent, "image", "image.png");

                // JSON verisini içeriğe ekle
                var jsonBytes = File.ReadAllBytes(jsonContentFile);
                var jsonContent = new ByteArrayContent(jsonBytes);
                jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                content.Add(jsonContent, "json", "CarParkPoses.json");

                //StringContent jsonContentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                //content.Add(jsonContentString, "json");


                // POST isteği gönderme
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // Sunucudan gelen yanıtı okuma
                string responseContent = await response.Content.ReadAsStringAsync();

                // Yanıtı konsola yazdırma
                Console.WriteLine("Response: " + responseContent);

                writeParkStatesFirestore(responseContent);
            }
        }

        public static string getPoses()
        {
            string path = @"C:\Users\Furkan\source\repos\AIOtopark\Temp\output.json";

            string poses = FirebaseService.Program.getParkPosesFromFirestore();

            try
            {
                // JSON dosyasına yazma işlemi
                File.WriteAllText(path, poses);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
            }

            return path;
        }

        // park alanlarının durumunu firebase' e yaz
        public static void writeParkStatesFirestore(string response)
        {
            List<ParkingSpotModel> parkingSpots = JsonConvert.DeserializeObject<List<ParkingSpotModel>>(response);

            Dictionary<string, bool> data = new Dictionary<string, bool>();

            foreach (var spot in parkingSpots)
            {
                data.Add(spot.SpotIndex.ToString(), spot.Status);
            }

            //FirebaseService.Program obj = new FirebaseService.Program();
            FirebaseService.Program.writeStates(data);

            Thread.Sleep(10000);

        }



        /// rezervasyonları say ve otopark durumlarını firebase' e yaz
        /// firebase' e yazma kodu hazır zaten sadece reservasyonları da dikkate alarak otopark durumlarını güncelle
        /// Rezervasyonları kontrol eden servisi yaz. Süresi biten rezervasyonları silmeli.
    }
}