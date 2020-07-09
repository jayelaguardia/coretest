using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coretest.Domain.Repositories;
using coretest.Persistence.Contexts;

namespace coretest.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoreTestDbContext _context;

        public UnitOfWork(CoreTestDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
