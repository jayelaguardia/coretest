using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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

        public async Task<CreateUserResponse> PasswordValidation(User user)
        {
            if (user.password.StartsWith(" ") || user.password.EndsWith(" "))
            {
                return new CreateUserResponse("Password must not start or end with empty spaces");

            }

            var rx = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&])[\S]+");
            if (rx.IsMatch(user.password) == false)
            {
                return new CreateUserResponse("password must contain one upper case, lower case, number, and special character");
            }

            return new CreateUserResponse(user);
        }

        public async Task<User> HashPassword(User user)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(user.password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            user.password = Convert.ToBase64String(hashBytes);

            return user;
        }
    }
}
