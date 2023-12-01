using Firebase.Auth;
using FirebaseService.Models;
using Google.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService
{
    public class LoginControl
    {
        private readonly FirebaseAuthProvider authProvider;
        public LoginControl()
        {
            // FirebaseApp.Create() ile oluşturulan app nesnesini kullanın
            string apiKey = "AIzaSyBnTce8nLA0KRY-_YSX6UUqdfWWamdPLpM";

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found in environment variables.");
                return;
            }

            authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
        }

        public async Task<string> login(LoginModel model)
        {
            try
            {
                // Kullanıcı email ve şifre ile giriş
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(model.email, model.password);
                return "success";
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                // Giriş başarısız ise, hata mesajını göster
                return firebaseEx.error.message;
            }
        }

    }
}
