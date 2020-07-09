using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Services.Communication;

namespace coretest.Domain.Services
{
    public interface IAuthService
    {
        Task<(CreateUserResponse, Auth)> FindAsync(Auth auth);
    }
}
