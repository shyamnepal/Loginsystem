namespace LoginSystemView.Services.IServices
{
    public interface IUsersPofiles
    {
        Task<T> GetUserProfileAsync<T>(string username);
        Task<T> AddProfileAsync<T>(MultipartFormDataContent content);
        Task<T> EditUserProfileAsync<T>(MultipartFormDataContent content);
    
    }
}

