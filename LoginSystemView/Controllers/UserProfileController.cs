using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using LoginSystemView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;

namespace LoginSystemView.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotyfService _notyfService;

        public UserProfileController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, INotyfService notyfService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _notyfService = notyfService;
        }
        public async  Task<IActionResult> Index()
        {
            if (ModelState.IsValid)
            {

            }
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            ViewData["userData"] = new User()
            {
                UserName = username,


            };

           
            if (username != null)
            {
                var baseUrl = _configuration.GetSection("Url")["baseUrl"];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
                var responseProfile = httpClient.GetAsync(baseUrl + $"/api/Profile/GetUserProfile?UserId={username}").Result;
                if (responseProfile.IsSuccessStatusCode)
                {
                    var content = await responseProfile.Content.ReadAsStringAsync();


                    var data = JsonConvert.DeserializeObject<userProfileDto>(content);
                    ViewData["profileData"] = new UserProfile()
                    {
                        imgurl = data.imgurl
                    };
                }
            }


            //check the user is admin or user

            if (User.IsInRole("SupperAdmin"))
            {
                ViewBag.UserRole = "SupperAdmin";
            }
            else if (User.IsInRole("User"))
            {
                ViewBag.UserRole = "User";
            }

            return View();
        }

        public async Task<IActionResult> AddProfile([FromForm] UserProfile Profile)
        {
            if (!ModelState.IsValid)
                return View("Index");

            


                var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            var url = baseUrl + "/api/Profile/AddProfile";
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (username != null)
            {

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
                var responseProfile = httpClient.GetAsync(baseUrl + $"/api/Profile/GetUserProfile?UserId={username}").Result;
                if (responseProfile.IsSuccessStatusCode)
                {
                    var content = await responseProfile.Content.ReadAsStringAsync();


                    var data = JsonConvert.DeserializeObject<userProfileDto>(content);
                    ViewData["profileData"] = new UserProfile()
                    {
                        imgurl = data.imgurl
                    };
                }
            }


            using (var httpClient = new HttpClient())
            {

                using (var form = new MultipartFormDataContent())
                {
                    var fileContent = new StreamContent(Profile.Image.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Image",
                        FileName = Profile.Image.FileName
                    };
                    form.Add(fileContent);

                    // Add other form fields as needed
                    form.Add(new StringContent(Profile.City), "City");
                    form.Add(new StringContent(Profile.UserId), "UserId");
                    form.Add(new StringContent(Profile.Address), "Address");
                    form.Add(new StringContent(Profile.Gender), "Gender");
                    form.Add(new StringContent(Convert.ToString(Profile.Age)), "Age");
                    form.Add(new StringContent(Profile.Education), "Education");

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
                    var response = httpClient.PostAsync(url, form).Result;
                    // handle response as needed



                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _notyfService.Success("User Profile created");
                        return RedirectToAction("Index");

                    }
                    _notyfService.Error("User Profile not creat");
                    return RedirectToAction("Index");
                  

                }
            }
        

     }

        public async Task<IActionResult> Edit(string userName)
        {

            

            var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            HttpClient client = new HttpClient();
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (username != null)
            {

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
                var responseProfile = httpClient.GetAsync(baseUrl + $"/api/Profile/GetUserProfile?UserId={username}").Result;
                if (responseProfile.IsSuccessStatusCode)
                {
                    var content = await responseProfile.Content.ReadAsStringAsync();


                    var data = JsonConvert.DeserializeObject<userProfileDto>(content);
                    ViewData["profileData"] = new UserProfile()
                    {
                        imgurl = data.imgurl
                    };
                }
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
              var response = client.GetAsync(baseUrl + $"/api/Profile/GetUserProfile?UserId={userName}").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserProfile>(content.Result);
                return View(data);
                
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditData([FromForm] UserProfile Profile)
        {
            if (!ModelState.IsValid)
                return View("Edit");

            var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            var url = baseUrl + "/api/Profile/EditUser";

            using (var httpClient = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {

                    var fileContent = new StreamContent(Profile.Image.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Image",
                        FileName = Profile.Image.FileName
                    };
                    form.Add(fileContent);

                    // Add other form fields as needed
                    form.Add(new StringContent(Profile.City), "City");
                    form.Add(new StringContent(Profile.UserId), "UserId");
                    form.Add(new StringContent(Profile.Address), "Address");
                    form.Add(new StringContent(Profile.Gender), "Gender");
                    form.Add(new StringContent(Convert.ToString(Profile.Age)), "Age");
                    form.Add(new StringContent(Profile.Education), "Education");

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
                    var response = httpClient.PutAsync(url, form).Result;
                    // handle response as needed



                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
;
                        _notyfService.Success($"Successfully edit the UserId {Profile.UserId}");
                        return RedirectToAction("Index");

                    }
                    _notyfService.Error($"failed edit the UserId {Profile.UserId}");
                    return RedirectToAction("Index");

                }
            }
        }
         public ActionResult passwordview()
        {
            return View("ChangePassword");
        }
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
      
        {
            if (!ModelState.IsValid)
                return View();
           
                var baseUrl = _configuration.GetSection("Url")["baseUrl"];
                HttpClient httpClient = new HttpClient();
                ChangePasswordModel obj = new ChangePasswordModel();
                obj.CurrentPassword = model.CurrentPassword;
                obj.NewPassword = model.NewPassword;
                obj.UserId = model.UserId;
                obj.ConfirmNewPassword = model.ConfirmNewPassword;

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
                HttpContent body = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                var response = httpClient .PostAsync(baseUrl + "/api/UserRegister/ChangePassword", body).Result;
            if (!response.IsSuccessStatusCode)
            {
                _notyfService.Error("Failed to change password");
                return View();
                
            }
            _notyfService.Success("Successfully change password");
            return  RedirectToAction("Index", "Dashboard");


        }


    }
}
