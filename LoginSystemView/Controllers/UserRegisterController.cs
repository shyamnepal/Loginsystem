using AspNetCoreHero.ToastNotification.Abstractions;
using LoginSystemView.Models;
using LoginSystemView.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NToastNotify;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web.Http;

namespace LoginSystemView.Controllers
{
    [Authorize]
    public class UserRegisterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _toastNotification;
        private readonly IUserRoles _userRoles;


        public UserRegisterController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            INotyfService toastNotification,
            IUserRoles userRoles


            )
        {
            _httpClientFactory = httpClientFactory;
            _options = new JsonSerializerOptions();
            _configuration = configuration;
            _toastNotification = toastNotification;
            _userRoles = userRoles;


        }

        public async Task<IActionResult> Index()
        {
            var baseUrl = _configuration.GetSection("Url")["baseUrl"];

            using (var httpClient = new HttpClient())
            {
                return View();
            }

        }
        //Navigate to the CreateUser page 
        public ActionResult CreateUser()
        {
            User model = new User();
            return View();
        }



        public async Task<IActionResult> Create(User UserData)
        {
            if (!ModelState.IsValid)
                return View("CreateUser");
            var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
           
             HttpContent body = new StringContent(JsonConvert.SerializeObject(UserData), Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/UserRegister", body).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _toastNotification.Success("User Register Successfully");
                return RedirectToAction("Index");

            }
            _toastNotification.Error("Failed to register please try again");
            return RedirectToAction("CreateUser");




        }

        //Navigate to the login page 
        public ActionResult LoginPage()
        {
            return View("UserLogin");
        }


        //for Login post request 

        public async Task<IActionResult> UserLogin(UserLogin UserCredential)
        {
            if (!ModelState.IsValid)
                return View("Index");
            var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
        
            HttpContent body = new StringContent(JsonConvert.SerializeObject(UserCredential), Encoding.UTF8, "application/json");
            var response = client.PostAsync("/api/UserRegister/Login", body).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ApiResponse data = JsonConvert.DeserializeObject<ApiResponse>(content);

                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, UserCredential.UserName),
                     new Claim(type: "id", value: data.userId),
                     new Claim(ClaimTypes.Email, data.userEmail),
                     new Claim(ClaimTypes.NameIdentifier, data.userId)

                };
                List<UserRolesModel> list = new();
                //var response = await _userRoles.GetAllAsync<Response>();
                var anotherResponse = await _userRoles.GetAsync<Response>(UserCredential.UserName);
               
                if (anotherResponse!=null)
                {
                    
                    var role = JsonConvert.DeserializeObject<UserRolesModel>(Convert.ToString(anotherResponse.Result));

                   claims.Add(new Claim(ClaimTypes.Role, role.Role));
                    


                }




                //add rolesmif any 

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                   );
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                    IsPersistent = true,


                };
                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                if (!string.IsNullOrEmpty(content))
                {
                    var objDeserializeObject = JsonConvert.DeserializeObject<ApiResponse>(content);

                    if (objDeserializeObject != null)
                    {
                        Console.WriteLine(objDeserializeObject.token.ToString());

                    }
                }
                _toastNotification.Success("Login Successfully");

                //Add HttpContext
                HttpContext.Session.SetString("token", data.token);

                return RedirectToAction("Index", "Dashboard");

            }



            _toastNotification.Error("Login faield");
            return RedirectToAction("Index");
        }
    }
}
