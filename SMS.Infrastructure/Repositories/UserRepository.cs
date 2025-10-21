using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepository _db;
        private readonly ILog _logger;

        public UserRepository(IRepository db)
        {
            _db = db;
            _logger = new LogService();
        }

        public async Task<int> CreateUserAsync(CancellationToken token, User user)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@UserName", ParameterValue = user.UserName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@Email", ParameterValue = user.Email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PhoneNumber", ParameterValue = user.PhoneNumber, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PasswordHash", ParameterValue = user.PasswordHash, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@IsActive", ParameterValue = user.IsActive, ParameterType = DbType.Boolean, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "CreateUser", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CreateUser failed");
                throw;
            }
        }

        public async Task<int> SetUserPasswordHashAsync(CancellationToken token, int userId, string passwordHash)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@PasswordHash", ParameterValue = passwordHash, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "SetUserPasswordHash", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SetUserPasswordHash failed");
                throw;
            }
        }

        public Task<User?> GetUserByIdAsync(CancellationToken token, int userId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<User>(token, "GetUserById", p);
        }

        public Task<User?> GetUserByEmailAsync(CancellationToken token, string email)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@Email", ParameterValue = email, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<User>(token, "GetUserByEmail", p);
        }

        public Task<User?> GetUserByUserNameAsync(CancellationToken token, string userName)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserName", ParameterValue = userName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpSingleAsync<User>(token, "GetUserByUserName", p);
        }

        public async Task<int> AssignRoleToUserAsync(CancellationToken token, int userId, string roleName)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@RoleName", ParameterValue = roleName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "AssignRoleToUser", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AssignRoleToUser failed");
                throw;
            }
        }

        public async Task<int> RemoveRoleFromUserAsync(CancellationToken token, int userId, string roleName)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@RoleName", ParameterValue = roleName, ParameterType = DbType.String, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "RemoveRoleFromUser", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RemoveRoleFromUser failed");
                throw;
            }
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(CancellationToken token, int userId)
        {
            var p = new List<ParametersCollection>
            {
                new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
            };
            return _db.ExecuteSpListAsync<string>(token, "GetUserRoles", p);
        }

        public async Task<int> LinkStudentToUserAsync(CancellationToken token, int studentId, int userId)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@StudentId", ParameterValue = studentId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input },
                    new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "LinkStudentToUser", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "LinkStudentToUser failed");
                throw;
            }
        }

        public async Task<int> UpdateUserLastLoginAsync(CancellationToken token, int userId)
        {
            try
            {
                var p = new List<ParametersCollection>
                {
                    new() { ParameterName = "@UserId", ParameterValue = userId, ParameterType = DbType.Int32, ParameterDirection = ParameterDirection.Input }
                };
                return (int)await _db.ExecuteSpReturnValueAsync(token, "UpdateUserLastLogin", p);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UpdateUserLastLogin failed");
                throw;
            }
        }
    }
}