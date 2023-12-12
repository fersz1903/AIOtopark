using FirebaseAdmin.Auth;
using FirebaseService.Models;
using Google.Protobuf.Collections;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AIOtopark.Controllers
{
    public class UserController : Controller // base controller ile değiştirilecek authentication için.
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ParkingLots()
        {
            List<ParkingLotPreviewModel> list = await FirebaseService.Program.getParkingLotPreviews();

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ParkingLotDetail(string plName) 
        {
            ParkingLotModel parklot = await FirebaseService.Program.GetParkingLot(plName);
            return View(parklot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> makePayment(string cardHolderName, 
                                         string cardNumber, 
                                         string expireMonth, 
                                         string expireYear, 
                                         string cvc,
                                         string plName,
                                         string priceKey,
                                         string plate,
                                         string date,
                                         string startHour)
        {

            string price = FirebaseService.Program.getPrice(plName,priceKey);


            try
            {
                var session = HttpContext.Session.GetString("UserSession");

                if (session == null)
                {
                    RedirectToAction("SignIn", "User");
                }

                //UserModel user = new UserModel();
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(session);
                if (userRecord == null)
                {
                    RedirectToAction("SignIn", "User");
                }

                Options options = new Options();
                options.ApiKey = "sandbox-rdAkch8ZVX5E09F5GAiIsFOMEqpTWVME";
                options.SecretKey = "sandbox-gHUruAebot5XrEgbCAG0XyMSdI2fiFLC";
                options.BaseUrl = "https://sandbox-api.iyzipay.com";


                CreatePaymentRequest request = new CreatePaymentRequest();
                request.Locale = Locale.TR.ToString();
                request.ConversationId = "123456789";
                //request.Price = "1";
                //request.PaidPrice = "1.2";
                request.Price = price;
                request.PaidPrice = price;
                //request.PaidPrice = (double.Parse(price) + double.Parse(price) * 0.03).ToString();
                request.Currency = Currency.TRY.ToString();
                request.Installment = 1;
                request.BasketId = "B67832";
                request.PaymentChannel = PaymentChannel.WEB.ToString();
                request.PaymentGroup = PaymentGroup.PRODUCT.ToString();



                PaymentCard paymentCard = new PaymentCard();
                paymentCard.CardHolderName = cardHolderName;
                paymentCard.CardNumber = cardNumber;
                paymentCard.ExpireMonth = expireMonth;
                paymentCard.ExpireYear = expireYear;
                paymentCard.Cvc = cvc;
                paymentCard.RegisterCard = 0;
                request.PaymentCard = paymentCard;

                Buyer buyer = new Buyer();

                string fname = userRecord.DisplayName;
                int boslukIndex = fname.IndexOf(' ');
                if (boslukIndex != -1)
                {
                    buyer.Name = fname.Substring(0, boslukIndex);
                    buyer.Surname = fname.Substring(boslukIndex + 1);
                }
                else
                {
                    buyer.Name = fname;
                    buyer.Surname = "null";
                }


                buyer.Id = "BY" + userRecord.Uid;
                //buyer.Name = ;
                //buyer.Surname = surname;
                buyer.GsmNumber = userRecord.PhoneNumber;
                buyer.Email = userRecord.Email;
                buyer.IdentityNumber = "11111111111";
                buyer.LastLoginDate = "2015-10-05 12:43:35";
                //buyer.RegistrationDate = "2013-04-21 15:12:09";
                buyer.RegistrationAddress = userRecord.Email;
                //buyer.Ip = "85.34.78.112";
                buyer.City = "Ankara";
                buyer.Country = "Turkey";
                //buyer.ZipCode = "34732";
                request.Buyer = buyer;

                Address shippingAddress = new Address();
                shippingAddress.ContactName = fname;
                shippingAddress.City = "Istanbul";
                shippingAddress.Country = "Turkey";
                shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
                shippingAddress.ZipCode = "34742";
                request.ShippingAddress = shippingAddress;

                Address billingAddress = new Address();
                billingAddress.ContactName = fname;
                billingAddress.City = "Istanbul";
                billingAddress.Country = "Turkey";
                billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
                billingAddress.ZipCode = "34742";
                request.BillingAddress = billingAddress;

                List<BasketItem> basketItems = new List<BasketItem>();
                BasketItem firstBasketItem = new BasketItem();
                firstBasketItem.Id = plName;
                firstBasketItem.Name = plName;
                firstBasketItem.Category1 = "Collectibles";
                //firstBasketItem.Category2 = "Accessories";
                firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                firstBasketItem.Price = price;
                basketItems.Add(firstBasketItem);
                request.BasketItems = basketItems;

                Payment payment = Payment.Create(request, options);
                if (payment.Status == "success")
                {
                    
                    var status = await FirebaseService.Program.setReservation(userRecord, plName, plate, priceKey, date, startHour);
                    if (status == "success")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", status);
                        return RedirectToAction("ParkingLotDetail", "User");
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            
            return RedirectToAction("ParkingLots", "User");

        }




    }
}
