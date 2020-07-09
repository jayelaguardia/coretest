using coretest.Persistence.Contexts;

namespace coretest.Persistence.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly CoreTestDbContext _context;

        public BaseRepository(CoreTestDbContext context)
        {
            _context = context;
        }
    }
}