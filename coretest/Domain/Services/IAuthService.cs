using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Services.Communication;

namespace coretest.Domain.Services
{
    public interface IAuthService
    {
        Task<CreateUserResponse> FindAsync(string username);
        CreateUserResponse CheckPass(Auth auth, User user);
        string CreateToken(User user);
    }
}
