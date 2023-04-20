using LoginSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Net;

namespace LoginSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        protected ApiResponse _response;
        public UserRoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this._response = new ApiResponse();
        }
        //list all the roles created by users
        //[HttpGet]
        //public ActionResult AllRoles()
        //{
        //    var roles = _roleManager.Roles;
        //    return Ok(roles);
        //}

        // Create The Role 
        //Role is only Create by supper Admin

        [Authorize(Roles = "SupperAdmin")]
        [HttpPost("CreateRole")]
        public async Task<ActionResult<ApiResponse>> CreateRole(IdentityRole model)
        {
            try
            {
                //avoid duplicate role
                if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
                    
                    
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages
                        = new List<string>() { "Role is already exist" };
                    return _response;
                }
               
             
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };

                return _response;
            }
         

        }

        [HttpGet("GetRoles")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> getRoles()
        {
            try
            {
                List<IdentityRole> roles = _roleManager.Roles.ToList();

                _response.Result = roles;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
               
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString()  };
                return _response;
            }


        }

        [HttpPost("EditRoles")]
        [Authorize(Roles = "SupperAdmin")]
        public async Task<ActionResult<ApiResponse>> EditRoleUser([Bind(include: "Email,Id, Username, RoleId")]UserRoles singleUserRole)
        {
            try
            {
                ////if (!ModelState.IsValid)
                ////    return BadRequest("Model is invalid");
                //var user = await _userManager.FindByIdAsync(singleUserRole.Id);
                //IEnumerable<string> oldrole = await _userManager.GetRolesAsync(user);
                //foreach (string item in oldrole)
                //{
                //    var remove = await _userManager.RemoveFromRoleAsync(user, item);
                //}

                //var newrole = await _roleManager.FindByIdAsync(singleUserRole.RoleId);
                //if (newrole != null && user != null)
                //{




                //    if (!_roleManager.RoleExistsAsync(newrole.ToString()).GetAwaiter().GetResult())
                //    {
                //        _response.StatusCode = HttpStatusCode.BadRequest;
                //        _response.ErrorMessages = new List<string> { "Role does not exist" };
                //        _response.IsSuccess = false;
                //        return BadRequest(_response);
                //    }
                //    else
                //    {

                //        var result = await _userManager.AddToRoleAsync(user, newrole.ToString());

                //        _response.Result = result;
                //        _response.IsSuccess = true;
                //        _response.StatusCode = HttpStatusCode.OK;
                //        return Ok(_response);

                //    }
                //}
                //_response.StatusCode = HttpStatusCode.BadRequest;
                //_response.ErrorMessages = new List<string> { "role or user is not found" };
                //_response.IsSuccess = false;
                //return BadRequest(_response);

                var user = await _userManager.FindByIdAsync(singleUserRole.Id);
                var newrole = await _roleManager.FindByIdAsync(singleUserRole.RoleId);

                if (user == null && newrole==null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.ErrorMessages = new List<string> { "User not found." };
                            _response.IsSuccess = false;
                            return BadRequest(_response);
                   
                }

                // Get the current roles of the user
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove the user from current roles
               var remove= await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add the user to the new role
              var result=  await _userManager.AddToRoleAsync(user, newrole.ToString());

                _response.Result = result;
                        _response.IsSuccess = true;
                        _response.StatusCode = HttpStatusCode.OK;
                        return Ok(_response);


            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return _response;
            }



        }

        [HttpDelete("DeleteUserRole")]
        [Authorize(Roles = "SupperAdmin")]
        public async Task<ActionResult<ApiResponse>> DeleteRoleOfUser(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    _response.Result = result;
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_response);

                }
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { $"Failed to delete user {user.UserName}" };
                _response.IsSuccess = false;
                return BadRequest(_response);

            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return _response;
            }
           

        }




    }
}
