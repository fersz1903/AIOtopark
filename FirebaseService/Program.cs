﻿using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseService.Model;
using FirebaseService.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

//using Google.Cloud.Firestore.V1;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Net;
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
            deneme();
            //createUser();

            //setParkingLotMainPhoto("otopark2", "parkinglot2.jpg");
            //getAllParkingLotsMainPhoto();

            //getParkingLotPreviews();
            //uploadParkPoses();
            //getParkPosesFromFirestore();
            //GetParkingLot("otopark1");


            Thread.Sleep(10000);
        }

        public static async void deneme()
        {
            //FirebaseApp.Create();
            //UserRecord userRecord = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync("asdasaad@example.com");

            await GetParkingLot("otopark1");
            Thread.Sleep(5000);
            //setReservation(userRecord, "otopark1","06 AA 06","3");
            // See the UserRecord reference doc for the contents of userRecord.
            //Console.WriteLine($"Successfully fetched user data: {userRecord.PhoneNumber}");
        }

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

        //public static async void uploadParkPoses()
        //{
        //    FirestoreDb db = ConnectionConfig();
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "ParkPoses\\" + "CarParkPos.json";

        //    // JSON dosyasından verileri oku
        //    List<List<int>> data = GetDataFromJson(path);

        //    // Firestore'a verileri yükle
        //    int i = 0;
        //    foreach (var coordinates in data)
        //    {
        //        var doc = new Dictionary<string, object>
        //        {
        //            { "x", coordinates[0] },
        //            { "y", coordinates[1] }
        //        };
        //        var result = db.Collection("otopark5").Document("ParkPoseCoordinates").Collection("Section1").Document(""+i.ToString()).SetAsync(doc).GetAwaiter().GetResult();
        //        i++;
        //    }
        //    //var docRef = db.Collection("otopark1").Document("CarParkPoses");
        //    //var result = docRef.SetAsync(doc).GetAwaiter().GetResult();
        //    Console.WriteLine("Veriler başarıyla Firestore'a yüklendi.");
        //}

        public static async Task uploadParkPoses(string jsonContent, string plName)
        {
            List<List<int>> data = JsonConvert.DeserializeObject<List<List<int>>>(jsonContent);

            FirestoreDb db = ConnectionConfig();

            int i = 0;
            foreach (var coordinates in data)
            {
                var doc = new Dictionary<string, object>
                {
                    { "x", coordinates[0] },
                    { "y", coordinates[1] }
                };
                var result = db.Collection(plName).Document("ParkPoseCoordinates").Collection("Section1").Document(i.ToString()).SetAsync(doc).GetAwaiter().GetResult();
                i++;
            }

            Console.WriteLine("Veriler başarıyla Firestore'a yüklendi.");
        }



        // park alanının coordinatlarını firestoredan alır
        public static string getParkPosesFromFirestore(string plName)
        {
            FirestoreDb db = ConnectionConfig();

            // Firestore'dan veriyi çekme
            QuerySnapshot snapshot = db.Collection(plName)
                                        .Document("ParkPoseCoordinates")
                                        .Collection("Section1")
                                        .GetSnapshotAsync()
                                        .GetAwaiter()
                                        .GetResult();

            var colRef = db.Collection(plName)
                                        .Document("ParkPoseCoordinates")
                                        .Collection("Section1");

                                        

            string poses = "[";

            for (int i = 0; i < snapshot.Documents.Count(); i++)
            {
                // manuel döngü kullanarak düzgün bir şekilde indexlerin sıralanması sağlandı
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

        public static async void writeStates(Dictionary<string,object> data,string plName)
        {
            FirestoreDb db = ConnectionConfig();
            var docRef = db.Collection(plName).Document("SpotsStatus").Collection("Section1").Document("Statuses");

            var result = docRef.SetAsync(data, SetOptions.MergeAll ).GetAwaiter().GetResult();

            //var docRef = await coll.AddAsync(new {SpotsStatusList= spots});
        }

        //düzenlenecek
        public static void updateParkingLotMainPhoto(string pLotName, string photoName)
        {
            FirestoreDb db = ConnectionConfig();
            CollectionReference collRef = db.Collection(pLotName);
            DocumentReference docRef = collRef.Document("images");
            string mainPhotoPath = "../parkingLotImages/" + photoName;


            //Dictionary<string, object> docData = new Dictionary<string, object>
            //{
            //    { "MainPhoto2", mainPhotoPath }
            //};

            Dictionary<string, object> docData = new Dictionary<string, object>
            {
                { "MainPhoto", mainPhotoPath }
            };
            docRef.UpdateAsync(docData).GetAwaiter().GetResult();
            Debug.WriteLine("writed: "+docData);
        }

        public static async Task<List<string>> getAllParkingLotsMainPhoto()
        {
            List<string> list = new List<string>();
            FirestoreDb db = ConnectionConfig();
            try
            {
                IAsyncEnumerable<CollectionReference> rootCollRef = db.ListRootCollectionsAsync();
                IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = rootCollRef.GetAsyncEnumerator(default);

                while (await subcollectionsEnumerator.MoveNextAsync())
                {
                    CollectionReference subcollectionRef = subcollectionsEnumerator.Current;
                    DocumentSnapshot snapshot = await subcollectionRef.Document("images").GetSnapshotAsync();

                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();
                        foreach (KeyValuePair<string, object> pair in data)
                        {
                            if (pair.Key == "MainPhoto")
                            {
                                list.Add(pair.Value.ToString());
                            }
                        }
                    }

                }
                return list;
            }
            catch (Exception e)
            {              
                Debug.WriteLine(e.Message);
                return null;
            }
        }


        //public static async Task<List<ParkingLotModel>> getParkingLots()
        //{
        //    List<ParkingLotModel> list = new List<ParkingLotModel>();
        //    ParkingLotModel parkingLotModel = new ParkingLotModel();
        //    FirestoreDb db = ConnectionConfig();

        //    try
        //    {
        //        IAsyncEnumerable<CollectionReference> rootCollRef = db.ListRootCollectionsAsync();
        //        IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = rootCollRef.GetAsyncEnumerator(default);

        //        while (await subcollectionsEnumerator.MoveNextAsync())
        //        {
        //            CollectionReference subcollectionRef = subcollectionsEnumerator.Current;
        //            parkingLotModel.name = subcollectionRef.Id.ToString();

        //            DocumentReference docRef = subcollectionRef.Document("SpotsStatus");

        //            // buralar düzenlenecek
                    
                    
                    
        //            DocumentSnapshot snapshot = await subcollectionRef.Document("images").GetSnapshotAsync();

                    

        //        }
        //        return list;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(e.Message);
        //        return null;
        //    }

        //}

        public static int countTrueStatements(Dictionary<string,object> data)
        {
            int count = 0;
            foreach (KeyValuePair<string, object> pair in data)
            {
                if (((Dictionary<string,object>)pair.Value).GetValueOrDefault("status").Equals(true))
                    count++;
            }
            return count;
        }

        public static async Task<List<ParkingLotPreviewModel>> getParkingLotPreviews()
        {
            List<ParkingLotPreviewModel> list = new List<ParkingLotPreviewModel>();
            FirestoreDb db = ConnectionConfig();

            try
            {
                IAsyncEnumerable<CollectionReference> rootCollRef = db.ListRootCollectionsAsync();
                IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = rootCollRef.GetAsyncEnumerator(default);

                while (await subcollectionsEnumerator.MoveNextAsync())
                {
                    ParkingLotPreviewModel parkingLotPreviewModel = new ParkingLotPreviewModel();
                    CollectionReference subcollectionRef = subcollectionsEnumerator.Current;
                    parkingLotPreviewModel.name = subcollectionRef.Id.ToString(); //otopark1-2-3

                    DocumentReference docRef = subcollectionRef.Document("SpotsStatus").Collection("Section1").Document("Statuses"); // farklı section durumları düzenlenecek
                    DocumentSnapshot snap = await docRef.GetSnapshotAsync();
                    if (snap.Exists)
                    {
                        Console.WriteLine("Document data for {0} document:", snap.Id);
                        Dictionary<string, object> data = snap.ToDictionary();
                        parkingLotPreviewModel.totalParkCount = data.Keys.Count();
                        parkingLotPreviewModel.freeParkCount = countTrueStatements(data);
                    }
                    else
                    {
                        Console.WriteLine("Document {0} does not exist!", snap.Id);
                    }

                    docRef = subcollectionRef.Document("images");
                    snap = await docRef.GetSnapshotAsync();
                    if (snap.Exists)
                    {
                        parkingLotPreviewModel.mainPhoto = snap.GetValue<string>("MainPhoto");
                    }
                    list.Add(parkingLotPreviewModel);

                }
                return list;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }


        public static async Task<ParkingLotModel> GetParkingLot(string plName)
        {
            ParkingLotModel parkingLotModel = new ParkingLotModel();
            FirestoreDb db = ConnectionConfig();
            CollectionReference collRef = db.Collection(plName);
            parkingLotModel.name = collRef.Id;

            DocumentReference docRef = collRef.Document("SpotsStatus").Collection("Section1").Document("Statuses");
            DocumentSnapshot snap = await docRef.GetSnapshotAsync();
            Dictionary<string, object> data = new Dictionary<string, object>();

            List<SpotsStatusesModel> spotsList = new List<SpotsStatusesModel>();

            if (snap.Exists)
            {
                //data = snap.ConvertTo<Dictionary<string, bool>>();
                data = snap.ToDictionary();

                foreach (var spotData in data)
                {
                    // Her haritayı SpotsStatusesModel sınıfına dönüştürün ve listeye ekleyin
                    SpotsStatusesModel spot = new SpotsStatusesModel();
                    spot.spotIndex = ((Dictionary<string, object>)spotData.Value).GetValueOrDefault("spotIndex").ToString();
                    spot.status = (bool)((Dictionary<string, object>)spotData.Value).GetValueOrDefault("status");
                    spot.isReserved = (bool)((Dictionary<string, object>)spotData.Value).GetValueOrDefault("isReserved");
                    spotsList.Add(spot);
                }

                parkingLotModel.spotsStatusDetail = spotsList;
                parkingLotModel.freeParkCount = countTrueStatements(data);
                parkingLotModel.totalParkCount = data.Count();
            }
            docRef = collRef.Document("Prices");
            snap = await docRef.GetSnapshotAsync();

            if (snap.Exists)
            {
                data = snap.ToDictionary();
                parkingLotModel.prices = data;
            }
            docRef = collRef.Document("Adress");
            snap = await docRef.GetSnapshotAsync();
            if (snap.Exists)
            {
                data = snap.ToDictionary();
                parkingLotModel.adress = data.GetValueOrDefault("adress").ToString();
            }
            docRef = collRef.Document("images");
            snap = await docRef.GetSnapshotAsync();
            if(snap.Exists)
            {
                data = snap.ToDictionary();
                parkingLotModel.mainPhoto = data.GetValueOrDefault("MainPhoto").ToString();
                parkingLotModel.firstFrame = data.GetValueOrDefault("FirstFrame").ToString();
            }
            return parkingLotModel;
        }

        public static async Task<string> setReservation(UserRecord user, string plName, string plate, string range,string date, string startHour, string spotIndex)
        {
            try
            {
                FirestoreDb db = ConnectionConfig();
                var docRef = db.Collection(plName).Document("Reservations");

                Dictionary<string, object> reservationDetail = new Dictionary<string, object>()
                {
                    { "startDate", date },
                    { "startHour", startHour },
                    { "range", range },
                    { "plate",  plate },
                    { "reservatedSpot", spotIndex }
                };

                Dictionary<string, object> reservation = new Dictionary<string, object>()
                {
                    { user.Uid , reservationDetail },
                };

                if (!checkReservation(plName,user.Uid))
                {
                    var result = docRef.SetAsync(reservation, SetOptions.MergeAll).GetAwaiter().GetResult();
                    if (result != null)
                    {
                        docRef = db.Collection(plName).Document("SpotsStatus").Collection("Section1").Document("Statuses");

                        Dictionary<string, bool> update = new Dictionary<string, bool>()
                        {
                            { "isReserved", true }
                        };
                        Dictionary<string, object> data = new Dictionary<string, object>()
                        {
                            { spotIndex.ToString(), update }
                        };
                                               
                        docRef.SetAsync(data,SetOptions.MergeAll);

                    }
                    return "success";
                }
                else
                    return "failed";

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return "failed";
            }
        }

        public static string getPrice(string plName, string key)
        {
            FirestoreDb db = ConnectionConfig();
            var docRef = db.Collection(plName).Document("Prices");
            DocumentSnapshot snap = docRef.GetSnapshotAsync().GetAwaiter().GetResult();
            return snap.GetValue<string>(key);
        }

        public static bool checkReservation(string plName,string uid)
        {
            FirestoreDb db = ConnectionConfig();
            var docRef = db.Collection(plName).Document("Reservations");
            DocumentSnapshot snap = docRef.GetSnapshotAsync().GetAwaiter().GetResult();
            foreach (var item in snap.ToDictionary())
            {
                if (item.Key == uid) { return true; }
            }
            return false;
        }

        public static async Task<List<String>> getAllParkingLots()
        {
            List<String> list = new List<String>();
            FirestoreDb db = ConnectionConfig();
            try
            {
                IAsyncEnumerable<CollectionReference> rootCollRef = db.ListRootCollectionsAsync();
                IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = rootCollRef.GetAsyncEnumerator(default);
                while (await subcollectionsEnumerator.MoveNextAsync())
                {
                    CollectionReference subcollectionRef = subcollectionsEnumerator.Current;
                    list.Add(subcollectionRef.Id.ToString());
                }
                return list;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public static string getFirstFramePath(string plName)
        {
            FirestoreDb db = ConnectionConfig();
            DocumentReference docRef = db.Collection(plName).Document("images");

            DocumentSnapshot snap = docRef.GetSnapshotAsync().GetAwaiter().GetResult();
            Dictionary<string,object> dict = new Dictionary<string,object>();
            dict = snap.ToDictionary();
            return dict.ContainsKey("FirstFrame") ? dict.GetValueOrDefault("FirstFrame").ToString() : null;
        }

        public static async Task<int> getUserCount()
        {
            int count = 0;
            try
            {
                var enumerator = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.ListUsersAsync(null).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync())
                {
                    ExportedUserRecord user = enumerator.Current;
                    count++;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return -1;
            }
            return count;
		}

        public static async Task<bool> addParkingLot(string plName, string adress,string mainPhotoPath, string firstFramePath, string poses)
        {
            FirestoreDb db = ConnectionConfig();
            DocumentReference docRef = db.Collection(plName).Document("Adress");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "adress", adress }
            };
            await docRef.SetAsync(data);

            data = new Dictionary<string, object>()
            {
                { "FirstFrame", firstFramePath},
                { "MainPhoto", mainPhotoPath}
            };
            docRef = db.Collection(plName).Document("images");
            await docRef.SetAsync(data);

            await uploadParkPoses(poses, plName);

            return true;
            // SpotsStatus hemen eklenmeli!!

        }

    }
}