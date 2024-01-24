using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FirebaseService;
using FirebaseService.Models;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using SpotsCheckService.Models;

namespace SpotsCheckService
{
    public class Program
    {
        public static bool IsServiceRunning = false;
        static void Main(string[] args)
        {
            //FirebaseService.Program srv = new FirebaseService.Program();
            //deneme();
            //FirebaseService.Program.setEnviroment();
            //ThreadPool.QueueUserWorkItem(BackgroundService);
            //sendPostRequest("otopark1");
            //Thread.Sleep(50000);
        }

        public static async void startService()
        {
            if (!IsServiceRunning)
            {
				ThreadPool.QueueUserWorkItem(BackgroundService);
                IsServiceRunning = true;
			}
        }


        public static void BackgroundService(Object? stateInfo)
        {
            while (IsServiceRunning)
            {
                Debug.WriteLine("Service working");
                // otopark alanı kontrolünü yap
                ThreadPool.QueueUserWorkItem(checkParkingLotStatuses);

                ThreadPool.QueueUserWorkItem(checkReservationsStatusesService);

                //ThreadPool.QueueUserWorkItem(fileCheck);
                Debug.WriteLine("Service continuing");
                
                Thread.Sleep(TimeSpan.FromMinutes(1)); // x dakika aralıklarla kontrol et
            }
        }

        public static async void deneme()
        {
            //Debug.WriteLine("SpotsCheckService working...");
            //var cts = new CancellationTokenSource();
            //ThreadPool.QueueUserWorkItem(state => ExecuteAsync(cts));
            //Thread.Sleep(10000);
            //cts.Cancel();

            //ThreadPool.QueueUserWorkItem(sendPostRequest);
            //await sendPostRequest();
            FirebaseService.Program.setEnviroment();
            await sendPostRequest("otopark1");
            //checkParkingLotStatuses(); // web uygulamasından başlatma gerekli fotoğraf yolunu bulamaz
            //getPoses("otopark1");
            Thread.Sleep(5000);
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


        public static async Task sendPostRequest(string plName)
        {
            var jsonContentFile = getPoses(plName);
            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string apiUrl = "http://127.0.0.1:5000/check_parking_status"; // Endpoint URL'si

                string path = @"C:\Users\Furkan\source\repos\AIOtopark\ParkingLotCameraImages\";
                Random rand = new Random();
                string imagePath = path + rand.Next(1, 29) +".png" ; // 1 den 29a kadar sayı üret ve rastgele foto seç
                // Resim dosyasını içeriğe ekle
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

                HttpResponseMessage response = new HttpResponseMessage();
                string responseContent = null;
                try
                {
                    // POST isteği gönderme
                    response = await client.PostAsync(apiUrl, content);

                    // Sunucudan gelen yanıtı okuma
                    responseContent = await response.Content.ReadAsStringAsync();

                }
                catch (Exception e)
                {
                    
                    Debug.WriteLine(e.Message);
                }

                if (responseContent != null)
                    writeParkStatesFirestore(responseContent, plName);
            }
        }

        public static string getPoses(string plName)
        {
            string path = @"C:\Users\Furkan\source\repos\AIOtopark\Temp\output.json";

            string poses = FirebaseService.Program.getParkPosesFromFirestore(plName);

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
        public static void writeParkStatesFirestore(string response, string plName)
        {
            List<ParkingSpotModel> parkingSpots = JsonConvert.DeserializeObject<List<ParkingSpotModel>>(response);

            //Dictionary<string, bool> data = new Dictionary<string, bool>();
           
            Dictionary<string,object> state = new Dictionary<string,object>();

            foreach (var spot in parkingSpots)
            {
                Dictionary<string, object> stateDetail = new Dictionary<string, object>()
                {
                    {"spotIndex", spot.SpotIndex },
                    {"status", spot.Status }
                    //{"isReserved", false}
                };

                state.Add(spot.SpotIndex.ToString(),stateDetail);
                //data.Add(spot.SpotIndex.ToString(), spot.Status);
            }

            //FirebaseService.Program obj = new FirebaseService.Program();
            FirebaseService.Program.writeStates(state, plName);

            Thread.Sleep(10000);

        }

        public static void writeParkStatesFirestoreInit(string response, string plName)
        {
            List<ParkingSpotModel> parkingSpots = JsonConvert.DeserializeObject<List<ParkingSpotModel>>(response);

            //Dictionary<string, bool> data = new Dictionary<string, bool>();

            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (var spot in parkingSpots)
            {
                Dictionary<string, object> stateDetail = new Dictionary<string, object>()
                {
                    {"spotIndex", spot.SpotIndex },
                    {"status", spot.Status },
                    {"isReserved", false}
                };

                state.Add(spot.SpotIndex.ToString(), stateDetail);
                //data.Add(spot.SpotIndex.ToString(), spot.Status);
            }

            //FirebaseService.Program obj = new FirebaseService.Program();
            FirebaseService.Program.writeStates(state, plName);

            Thread.Sleep(10000);

        }


        public static async void checkParkingLotStatuses(Object? stateInfo)
        {
            List<String> parkingLotList = await FirebaseService.Program.getAllParkingLots();

            foreach(var lot in parkingLotList)
            {
                int reservationCount = 0;
                await sendPostRequest(lot);
            }
        }

        public static async void checkReservationsStatusesService(Object? stateInfo)
        {
            await FirebaseService.Program.deleteExpiredReservations();
        }


        public static async Task databaseInit(string plName)
        {
            var jsonContentFile = getPoses(plName);
            using (HttpClient client = new HttpClient())
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                string apiUrl = "http://127.0.0.1:5000/check_parking_status"; // Endpoint URL'si

                // Resim dosyasını içeriğe ekle
                string imagePath = "./wwwroot" + FirebaseService.Program.getFirstFramePath(plName); // fotoğrafın yolu
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
                Debug.WriteLine("Response: " + responseContent);

                writeParkStatesFirestoreInit(responseContent, plName);
            }
        }

        /// rezervasyonları say ve otopark durumlarını firebase' e yaz
        /// firebase' e yazma kodu hazır zaten sadece reservasyonları da dikkate alarak otopark durumlarını güncelle
        /// Rezervasyonları kontrol eden servisi yaz. Süresi biten rezervasyonları silmeli.
    }
}