using AspNetCore;
using LoginSystemView.Model;
using LoginSystemView.Model.ApyTypeEnum;
using LoginSystemView.Models;
using LoginSystemView.Services.IServices;
using System.Data;

namespace LoginSystemView.Services
{
    public class UserRoles : BaseServices, IUserRoles
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string baseurl;
        public UserRoles(IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(clientFactory, httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            baseurl = configuration.GetValue<string>("Url:baseUrl");

        }

        public Task<T> AssignRoles<T>(RoleAssignModel roleAssign)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.POST,
                Data = roleAssign,
                ApiUrl = baseurl + "/api/RoleAssign"
            });
        }

       

        public Task<T> CreateAsync<T>(AddRoles role)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.POST,
                Data = role,
                ApiUrl = baseurl + "/UserRole/CreateRole"
            });
        }

        public Task<T> DeleteUserRoles<T>(string userId, string roleName)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.DELETE,
                ApiUrl = baseurl + $"/UserRole/DeleteUserRole?userId={userId}&roleName={roleName}"
            });
        }

        public Task<T> EditUserRoles<T>(UserRolesModel role)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.POST,
                Data = role,
                ApiUrl = baseurl + "/UserRole/EditRoles"
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                ApiUrl = baseurl + "/api/UserInfo",
            });
        }

        public Task<T> GetAllUser<T>()
        {

            return SendAsync<T>(new ApiRequest
            {
                ApiUrl = baseurl + "/api/RoleAssign/GetUserInfo",
                ApiType=ApiType.GET,

            });
        }

        public Task<T> GetAsync<T>(string userName)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                ApiUrl = baseurl + $"/api/UserInfo/{userName}",
            });

        }

        public Task<T> GetOnlyRoles<T>()
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                ApiUrl = baseurl + "/UserRole/GetRoles",
            });
        }
    }

}
