using LoginSystemView.Model;
using LoginSystemView.Model.ApyTypeEnum;
using LoginSystemView.Services.IServices;

namespace LoginSystemView.Services
{
    public class UsersProfiles : BaseServices, IUsersPofiles
    {

        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string baseurl;
        public UsersProfiles(IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(clientFactory, httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            baseurl = configuration.GetValue<string>("Url:baseUrl");
        }

        public Task<T> AddProfileAsync<T>(MultipartFormDataContent content)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiUrl = baseurl + "/api/Profile/AddProfile",
                ApiType = ApiType.POST,
                Data=content

            });
        }

        public Task<T> EditUserProfileAsync<T>(MultipartFormDataContent content)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiUrl = baseurl + "/api/Profile/EditUser",
                ApiType = ApiType.PUT,
                Data = content

            });
        }

      

        public Task<T> GetUserProfileAsync<T>(string username)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiUrl = baseurl + $"/api/Profile/GetUserProfile?UserId={username}",
                ApiType=ApiType.GET,

            });
        }
    }
}
