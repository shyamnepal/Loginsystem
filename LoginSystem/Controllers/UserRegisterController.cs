using Antlr.Runtime;
using Azure.Core;
using Azure.Messaging;
using Castle.Core.Smtp;
using LoginSystem.Data;
using LoginSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        public readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public UserRegisterController(UserManager<User> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //User obj = new User();

            //obj.PhoneNumber = user.PhoneNumber;
            //obj.UserName = user.UserName;
            //obj.Email = user.Email;
            //obj.FirstName = user.FirstName;
            //obj.LastName = user.LastName;


            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (result.Succeeded)
            {
               var res= await _userManager.AddToRoleAsync(user, "User");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "UserRegister", new { token, email = user.Email }, Request.Scheme);
               var message = user.Email + "Confirmation email link" + confirmationLink;
                 _emailSender.Send(from: "shyamnepal595@gmail.com",to:user.Email,subject:"Confirm your email",messageText: "<html><body> " + message + " </body></html>");
               
                return Ok("User is created");
            }
            return BadRequest(result);

        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return Ok(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserLogin user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }

                var newUser = await _userManager.FindByNameAsync(user.UserName);

                //get role of the user
                var userRoles = await _userManager.GetRolesAsync(newUser);


                if (newUser != null && await _userManager.CheckPasswordAsync(newUser, user.Password))
                {

                    List<Claim> claims = new List<Claim>()
                                 {
                                new Claim(ClaimTypes.Name, user.UserName),

                                };

                    var rls = userRoles.ToList();
                    if (userRoles != null)
                    {
                        foreach (var r in rls)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, r));
                        }

                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

                    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: cred
                        );

                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(new response
                    {
                        statusCode = 200,
                        token = jwt,
                        userEmail= newUser.Email,
                        userId=newUser.Id,
                    }) ;

                }

                else
                {
                    return BadRequest("Password does not match");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       // [Authorize(Roles = "SupperAdmin")]
        [HttpGet("GetData")]
        public async Task<ActionResult<User>> getUserInfo(string userName)
        {

            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return Ok(user);
                
            }
            return BadRequest("User not found");

        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangeUserPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                User userById = _userManager.FindByIdAsync(model.UserId).Result;
                if (userById != null)
                {
                    IdentityResult result = _userManager.ChangePasswordAsync(user: userById, model.CurrentPassword, model.NewPassword).Result;
                    if (result.Succeeded)
                    {
                        return Ok("Successfully change the password");
                   
                    }
                }
                return BadRequest("User can not found");
            }
            return BadRequest("Model is not valid");
        }



    }
}
