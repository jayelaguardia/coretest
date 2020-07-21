using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Services.Communication;

namespace coretest.Domain.Services
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateUserAsync(User user);
        Task<CreateUserResponse> FindNameAsync(User user);
        Task<CreateUserResponse> FindEmailAsync(User user);
        Task<CreateUserResponse> PasswordValidation(User user);
        Task<User> HashPassword(User user);
    }
}
