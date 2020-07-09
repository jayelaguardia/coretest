using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Services.Communication;

namespace coretest.Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ListAsync();
        Task<CreateUserResponse> CreateUserAsync(User user);
        Task<CreateUserResponse> FindAsync(User user);
    }
}
