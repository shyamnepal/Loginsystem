using AspNetCoreHero.ToastNotification.Abstractions;
using LoginSystemView.Models;
using LoginSystemView.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace LoginSystemView.Controllers
{
    [Authorize]
    public class GetUserRolesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRoles _userRoles;
        private readonly INotyfService _notyService;
        public GetUserRolesController(IConfiguration configuration, IHttpContextAccessor httpContext, IUserRoles userRoles, INotyfService notyfService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContext;
            _userRoles = userRoles;
            _notyService = notyfService;
        }
        
        public async Task<IActionResult> Index()
        {

            var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            //check the user is admin or user

            if (User.IsInRole("SupperAdmin"))
            {
                ViewBag.UserRole = "SupperAdmin";
            }
            else if (User.IsInRole("User"))
            {
                ViewBag.UserRole = "User";
            }
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

            List<UserRolesModel> list = new();
            var response = await _userRoles.GetAllAsync<Response>();
            if(response!=null)
            {
                list = JsonConvert.DeserializeObject<List<UserRolesModel>>(Convert.ToString(response.Result));
            }
            return View(list);

           
        }
        public async Task<IActionResult> GetEditData(string Userid)
        {
            try
            {
                
                if (User.IsInRole("SupperAdmin"))
                {
                    ViewBag.UserRole = "SupperAdmin";
                }
                else if (User.IsInRole("User"))
                {
                    ViewBag.UserRole = "User";
                }
                var username = _httpContextAccessor.HttpContext.User.Identity.Name;
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
                

                List<UserRolesModel> list = new();
                var response = await _userRoles.GetAllAsync<Response>();
                if (response != null)
                {
                    //Get all the user from the database
                    list = JsonConvert.DeserializeObject<List<UserRolesModel>>(Convert.ToString(response.Result));

                    //get the particular user that is going to be edit
                   UserRolesModel user = list.FirstOrDefault(x => x.Id == Userid);
                    List<IdentityRole> role = new List<IdentityRole>();
                    var roleResponse = await _userRoles.GetOnlyRoles<Response>();
                    if (response != null)
                    {
                        //get all the for select the roles 
                        role= JsonConvert.DeserializeObject<List<IdentityRole>>(Convert.ToString(roleResponse.Result));
                        var dropdown = role.Select(x => new SelectListItem()
                        {
                            Value = x.Id,
                            Text = x.Name
                        }).ToList();
                        ViewData["dropDown"] = dropdown;
                        return View("EditRoles", user);

                    }
                  
                }
                return View("EditRoles");
              
            } catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<IActionResult> EditUserRoles([Bind(include:"Email,Id,RoleId,Username")] UserRolesModel role)
        {
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
            {
                List<IdentityRole> userRole = new List<IdentityRole>();
                var roleResponse = await _userRoles.GetOnlyRoles<Response>();
                if (roleResponse != null)
                {
                    //get all the for select the roles 
                    userRole = JsonConvert.DeserializeObject<List<IdentityRole>>(Convert.ToString(roleResponse.Result));
                    var dropdown = userRole.Select(x => new SelectListItem()
                    {
                        Value = x.Id,
                        Text = x.Name
                    }).ToList();
                    ViewData["dropDown"] = dropdown;
                    return View("EditRoles");

                }
            }


                var baseUrl = _configuration.GetSection("Url")["baseUrl"];
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            UserRolesModel urole = new UserRolesModel();
            urole.RoleId = role.RoleId;
            urole.Id = role.Id; 

            urole.Email = role.Email;
            urole.Username = role.Username;           
            var response = await _userRoles.EditUserRoles<Response>(role);
            if (response!=null && response.IsSuccess)
            {
                _notyService.Success("Successfully edit the roles");
                return RedirectToAction("Index");
            }


            _notyService.Error(response.ErrorMessages.ToString());
            return View();

        }

        public async Task<IActionResult> DeleteUserRoles(string userId, string roleName)
        {

            var response = await _userRoles.DeleteUserRoles<Response>(userId, roleName);

            if (response!=null)
                return RedirectToAction("Index");
            
            return RedirectToAction("Index");

        

           
        }
    }
}
