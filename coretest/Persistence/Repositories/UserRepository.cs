using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using coretest.Domain.Models;
using coretest.Domain.Repositories;
using coretest.Persistence.Contexts;

namespace coretest.Persistence.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(CoreTestDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
        }

        public async Task<User> FindByNameAsync(string name)
        {
            return await _context.User.FirstOrDefaultAsync(s => s.username == name);     
        }
    }
}
