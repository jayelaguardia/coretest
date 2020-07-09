using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Services;
using coretest.Domain.Repositories;
using coretest.Domain.Services.Communication;

namespace coretest.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<(CreateUserResponse, Auth)> FindAsync(Auth auth)
        {
            var existingUser = await _userRepository.FindByNameAsync(auth.username);

            if (existingUser == null)
                return (new CreateUserResponse("Incorrect username or password."), auth);

            return (new CreateUserResponse(auth), auth);
        }
    }
}
