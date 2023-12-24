﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AIOtopark.Controllers
{
    public class AdminController : Controller
    {
        public async Task<IActionResult> Index()
        {
			//try
			//{
			//	List<String> list = await FirebaseService.Program.getAllParkingLots();
			//	ViewData["plCount"] = list.Count().ToString();
			//	ViewData["userCount"] = await FirebaseService.Program.getUserCount();
			//}
			//catch (Exception e)
			//{
			//	Debug.WriteLine(e.Message);
			//}
            return View();
        }

		public IActionResult Picker()
		{
			return View();
		}
		
		public IActionResult ParkingLotAddPanel()
		{
			//SpotsCheckService.Program.checkParkingLotStatuses();
			return View();
		}

        [HttpPost]
        public IActionResult saveParkingLot(string plName,
                                            string adress, 
                                            IFormFile mainPhoto, 
                                            IFormFile firstFrame, 
                                            IFormFile poses)
        {

            string mainPhotoPath = "../parkingLotImages/" + uploadImage(mainPhoto);
            string firstFramePath = "/parkingLotImages/" + uploadImage(firstFrame);

            //FirebaseService.Program.uploadParkPoses(readPoses(poses),plName);
            var result = FirebaseService.Program.addParkingLot(plName,adress,mainPhotoPath,firstFramePath,readPoses(poses));

            if (result.Result == true)
            {
                SpotsCheckService.Program.databaseInit(plName);
            }
            else
            {
                TempData["error"] = "Otopark Eklenemedi. Result:" + result.Result;
            }

            return RedirectToAction("ParkingLotAddPanel");
        }


        public static string readPoses(IFormFile jsonFile)
        {
            if (jsonFile != null && jsonFile.Length > 0)
            {
                // JSON dosyasının içeriğini oku
                using (var streamReader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    var jsonContent = streamReader.ReadToEnd();
                    return jsonContent;
                }

            }
            else
                return null;
        }

        public static string uploadImage(IFormFile file)
        {
            var localImageDir = $"wwwroot/parkingLotImages";
            var localImagePath = $"{localImageDir}/{file.FileName}";

            if (!Directory.Exists(Path.Combine(localImageDir)))
            {
                Directory.CreateDirectory(Path.Combine(localImageDir));
            }

            using (Stream fileStream = new FileStream(localImagePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            //string modelsImagePath = Path.GetFileNameWithoutExtension(file.FileName).ToString();
            string modelsImagePath = Path.GetFileName(file.FileName).ToString();
            return modelsImagePath;
        }
    }
}
