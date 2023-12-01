using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseService.Model;
using FirebaseService.Models;
using Google.Cloud.Firestore;
//using Google.Cloud.Firestore.V1;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;


namespace FirebaseService
{
    public class Program
    {
        private readonly FirebaseAuthProvider authProvider;


        public Program()
        {
            setEnviroment();
        }
        static void Main(string[] args)
        {
            setEnviroment();
            //deneme();
            //createUser();
            Thread.Sleep(5000);
            //uploadParkPoses();
            //getParkPosesFromFirestore();
        }

        //public static async void deneme()
        //{
        //    //FirebaseApp.Create();
        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync("asdasdasdasdad@example.com");

        //    // See the UserRecord reference doc for the contents of userRecord.
        //    Console.WriteLine($"Successfully fetched user data: {userRecord.PhoneNumber}");
        //}

        public static void setEnviroment()
        {
            string path = @"C:\Users\Furkan\source\repos\AIOtopark\";
            string credentialsPath = path + "\\Credentials\\";
            string cloudFirePath = credentialsPath + "cloudfire.json";

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", cloudFirePath);
            FirebaseApp.Create();
        }


        public static async Task<string> createUser(RegisterModel register)
        {
            //FirebaseApp.Create();

            try
            {
                UserRecordArgs args = new UserRecordArgs()
                {
                    Email = register.email,
                    EmailVerified = false,
                    PhoneNumber ="+90" + register.phone,
                    Password = register.password,
                    DisplayName = register.fname,
                    Disabled = false,
                };
                UserRecord userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateUserAsync(args);

                Console.WriteLine($"Successfully created new user: {userRecord.Uid}");

                return "success";

            }
            catch (Exception ex)
            {
                // Auth işlemi başarısız oldu, hatayı kontrol et
                Console.WriteLine($"Error creating new user: {ex.Message}");

                // Hata mesajını belirten bir değer döndür
                return ex.Message;
            }
        }

        public static FirestoreDb ConnectionConfig()
        {
            FirestoreDb db = FirestoreDb.Create("aiotopark"); // projeId'si 

            Console.WriteLine("Database Baglantisi yapildi");
            return db;
        }

        public static async void uploadParkPoses()
        {
            FirestoreDb db = ConnectionConfig();
            string path = AppDomain.CurrentDomain.BaseDirectory + "ParkPoses\\" + "CarParkPos.json";

            // JSON dosyasından verileri oku
            List<List<int>> data = GetDataFromJson(path);

            // Firestore'a verileri yükle
            int i = 0;
            foreach (var coordinates in data)
            {
                var doc = new Dictionary<string, object>
                {
                    { "x", coordinates[0] },
                    { "y", coordinates[1] }
                };
                var result = db.Collection("otopark1").Document("ParkPoseCoordinates").Collection("Section1").Document(""+i.ToString()).SetAsync(doc).GetAwaiter().GetResult();
                i++;
            }
            //var docRef = db.Collection("otopark1").Document("CarParkPoses");
            //var result = docRef.SetAsync(doc).GetAwaiter().GetResult();
            Console.WriteLine("Veriler başarıyla Firestore'a yüklendi.");
        }

        public static string getParkPosesFromFirestore()
        {
            FirestoreDb db = ConnectionConfig();

            // Firestore'dan veriyi çekme
            QuerySnapshot snapshot = db.Collection("otopark1")
                                        .Document("ParkPoseCoordinates")
                                        .Collection("Section1")
                                        .GetSnapshotAsync()
                                        .GetAwaiter()
                                        .GetResult();

            var colRef = db.Collection("otopark1")
                                        .Document("ParkPoseCoordinates")
                                        .Collection("Section1");

                                        

            string poses = "[";

            for (int i = 0; i < snapshot.Documents.Count(); i++)
            {
                DocumentSnapshot docSnap = colRef.Document(i.ToString()).GetSnapshotAsync().GetAwaiter().GetResult();
                //var document = snapshot.Documents[i];
                Dictionary<string, object> coordinate = docSnap.ToDictionary();
                string data = coordinate["x"].ToString() + ", " + coordinate["y"].ToString();
                poses += "[" + data + "]";

                // eğer son koordinata geldiyse sona virgül ekleme
                if(i != snapshot.Documents.Count()-1)
                {
                    poses += ", ";
                }
            }

            //foreach (DocumentSnapshot document in snapshot.Documents)
            //{
            //    //ParkPoseModel parkPose = document.ConvertTo<ParkPoseModel>();


            //    Dictionary<string, object> coordinate = document.ToDictionary();
            //    string data = coordinate["x"].ToString() + ", " + coordinate["y"].ToString();
            //    poses += "[" + data + "], ";


            //}
            poses += "]";

            try
            {
                // JSON dosyasına yazma işlemi
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "output.json", poses);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
            }


                return poses;
        }

        static List<List<int>> GetDataFromJson(string jsonPath)
        {
            // JSON dosyasını okuyarak bir liste oluştur
            List<List<int>> data = new List<List<int>>();

            using (StreamReader file = File.OpenText(jsonPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = (List<List<int>>)serializer.Deserialize(file, typeof(List<List<int>>));
            }
            return data;
        }

        public static async void writeStates(Dictionary<string,bool> data)
        {
            FirestoreDb db = ConnectionConfig();
            var docRef = db.Collection("otopark1").Document("SpotsStatus");

            var result = docRef.SetAsync(data).GetAwaiter().GetResult();

            //var docRef = await coll.AddAsync(new {SpotsStatusList= spots});
        }




    }
}