using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Repositories;
using coretest.Domain.Services;
using coretest.Domain.Services.Communication;

namespace coretest.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _userRepository.ListAsync();
        }

        public async Task<CreateUserResponse> CreateUserAsync(User user)
        {
            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return new CreateUserResponse(user);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CreateUserResponse($"An error occurred when saving the user: {ex.Message}");
            }
        }

        public async Task<CreateUserResponse> FindAsync(User user)
        {
            var existingUser = await _userRepository.FindByNameAsync(user.username);

            if (existingUser != null)
                return new CreateUserResponse("This username already exists.");

            return new CreateUserResponse(user);
        }
    }
}
