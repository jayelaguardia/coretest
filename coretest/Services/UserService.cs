using System;
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

        public async Task<CreateUserResponse> FindNameAsync(User user)
        {
            var existingUser = await _userRepository.FindByNameAsync(user.username);

            if (existingUser != null)
                return new CreateUserResponse("This username already exists.");

            return new CreateUserResponse(user);
        }

        public async Task<CreateUserResponse> FindEmailAsync(User user)
        {
            var existingUser = await _userRepository.FindByEmailAsync(user.email);

            if (existingUser != null)
                return new CreateUserResponse("This email already exists.");

            return new CreateUserResponse(user);
        }
    }
}
