using AspNetCoreHero.ToastNotification.Abstractions;
using LoginSystemView.Models;
using LoginSystemView.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace LoginSystemView.Controllers
{
    [Authorize]
    public class RoleAssignController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRoles _userRole;
        private readonly INotyfService _notyfService;
     

        public RoleAssignController(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IUserRoles userRole,
            INotyfService notyfService
            
            )
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userRole = userRole;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index()
        {
            
                var username = _httpContextAccessor.HttpContext.User.Identity.Name;
                ViewData["userData"] = new User()
                {
                    UserName = username,


                };

                //check the user is admin or user

                if (User.IsInRole("SupperAdmin"))
                {
                    ViewBag.UserRole = "SupperAdmin";
                }
                else if (User.IsInRole("User"))
                {
                    ViewBag.UserRole = "User";
                }

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


                var response = await _userRole.GetOnlyRoles<Response>();
                if (response != null)
                {
                    var data = JsonConvert.DeserializeObject<List<IdentityRole>>(Convert.ToString(response.Result));
                    ViewBag.roles = data.ToList();

                  
                }

            var getUserResponse = await _userRole.GetAllUser<Response>();
            if(getUserResponse!=null)
            {
                var userdata = JsonConvert.DeserializeObject<List<User>>(Convert.ToString(getUserResponse.Result));
                var dropdown = userdata.Select(x => new SelectListItem()
                {
                    Value = x.UserName,
                    Text = x.UserName
                }).ToList();
                ViewData["dropDown"] = dropdown;
                return View();
            }
            
           

            return View();
               
            


          
        }

        public async Task<IActionResult> RoleAssignToUser(RoleAssignModel model)

        {
            //check the user is admin or user

            if (User.IsInRole("SupperAdmin"))
            {
                ViewBag.UserRole = "SupperAdmin";
            }
            else if (User.IsInRole("User"))
            {
                ViewBag.UserRole = "User";
            }

            if (!ModelState.IsValid)
            {

                var roleDropDown = await _userRole.GetOnlyRoles<Response>();
                if (roleDropDown != null)
                {
                    var data = JsonConvert.DeserializeObject<List<IdentityRole>>(Convert.ToString(roleDropDown.Result));
                    ViewBag.roles = data.ToList();

                }
                var getUserResponse = await _userRole.GetAllUser<Response>();
                if (getUserResponse != null)
                {
                    var userdata = JsonConvert.DeserializeObject<List<User>>(Convert.ToString(getUserResponse.Result));
                    var dropdown = userdata.Select(x => new SelectListItem()
                    {
                        Value = x.UserName,
                        Text = x.UserName
                    }).ToList();
                    ViewData["dropDown"] = dropdown;
                    return View("Index");
                }




            }




            RoleAssignModel userRole = new RoleAssignModel();
            userRole.UserName = model.UserName;
            userRole.Name = model.Name;


            //HttpContent body = new StringContent(JsonConvert.SerializeObject(userRole), Encoding.UTF8, "application/json");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token").ToString());
            //var response = client.PostAsync("/api/RoleAssign", body).Result;
            var response = await _userRole.AssignRoles<Response>(model);


            if (response!=null && response.IsSuccess)
            {
                //var content = await response.Content.ReadAsStringAsync();
                _notyfService.Success($"Successfully Assign the role to the user {model.UserName}");
                return RedirectToAction("Index");
            }
            _notyfService.Success($" {response.ErrorMessages}  {model.UserName}");
            return RedirectToAction("Index");
        }
        
    }
}
