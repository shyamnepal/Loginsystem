
using LoginSystemView.Models;

namespace LoginSystemView.Services.IServices
{
    public interface IUserRoles
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(string userName);
        Task<T> CreateAsync<T>(AddRoles role);
        Task<T> GetOnlyRoles<T>();
        Task<T> EditUserRoles<T>(UserRolesModel role);
        Task<T> DeleteUserRoles<T>(string userId, string roleName);
        Task<T> AssignRoles<T>(RoleAssignModel roleAssign);
        Task<T> GetAllUser<T>();
    }
}
