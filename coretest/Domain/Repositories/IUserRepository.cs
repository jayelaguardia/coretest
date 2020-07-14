using System.Threading.Tasks;
using coretest.Domain.Models;

namespace coretest.Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User> FindByNameAsync(string name);
        Task<User> FindByEmailAsync(string name);
    }
}
