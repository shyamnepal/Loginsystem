using LoginSystem.Data;
using LoginSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LoginSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        public UserInfoController(UserManager<User> userManager, AppDbContext context)
        {
            this._response = new ApiResponse();
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> getUserInfo()
        {
            try
            {
                var usersWithRoles = (from user in _context.Users
                                      join userRole in _context.UserRoles
                                       on user.Id equals
                                       userRole.UserId
                                      join roles in _context.Roles
                                      on userRole.RoleId equals roles.Id

                                      select new UserRoles()
                                      {
                                          Username = user.UserName,
                                          Email = user.Email,
                                          Role = roles.Name,
                                          Id = user.Id,
                                          RoleId = roles.Id



                                      }).ToList();

                _response.Result = usersWithRoles;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<String>() { ex.ToString() };
            }
            return _response;
            
        }

        [HttpGet("{UserName}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> GetUserById(string UserName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(UserName);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { "User Not Found" };
                    return NotFound(_response);
                }

                var roles = await _userManager.GetRolesAsync(user);

                var UserRoles = new UserRoles()
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault()

                };
                _response.Result = UserRoles;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<String>() { ex.ToString() };
            }
            return _response;


        }

    }
}
