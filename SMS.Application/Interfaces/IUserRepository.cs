using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SMS.Core.Entities;

namespace SMS.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(CancellationToken token, User user);
        Task<int> SetUserPasswordHashAsync(CancellationToken token, int userId, string passwordHash);
        Task<User?> GetUserByIdAsync(CancellationToken token, int userId);
        Task<User?> GetUserByEmailAsync(CancellationToken token, string email);
        Task<User?> GetUserByUserNameAsync(CancellationToken token, string userName);

        Task<int> AssignRoleToUserAsync(CancellationToken token, int userId, string roleName);
        Task<int> RemoveRoleFromUserAsync(CancellationToken token, int userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(CancellationToken token, int userId);

        Task<int> LinkStudentToUserAsync(CancellationToken token, int studentId, int userId);
        Task<int> UpdateUserLastLoginAsync(CancellationToken token, int userId);

    }
}