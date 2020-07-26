using System.Threading.Tasks;
using coretest.Domain.Models;
using coretest.Domain.Services;
using coretest.Domain.Repositories;
using coretest.Domain.Services.Communication;
using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace coretest.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<CreateUserResponse> FindAsync(string username)
        {
            var existingUser = await _userRepository.FindByNameAsync(username);

            if (existingUser == null)
                return new CreateUserResponse("Incorrect username or password.");

            return new CreateUserResponse(existingUser);
        }

        public CreateUserResponse CheckPass(Auth attemptUser, User realUser)
        {
            byte[] savedPasswordBytes = Convert.FromBase64String(realUser.password);
            byte[] salt = new byte[16];
            Array.Copy(savedPasswordBytes, 0, salt, 0, 16);
            var inputPass = new Rfc2898DeriveBytes(attemptUser.password, salt, 10000);
            byte[] inputHash = inputPass.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (savedPasswordBytes[i + 16] != inputHash[i])
                    return new CreateUserResponse("Incorrect username or password");
            }

            return new CreateUserResponse(realUser);
        }

        public string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["SecurityKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("username", user.username)
            };

            var stoken = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: now.AddDays(1),
                signingCredentials: credentials
            );

            var token = tokenHandler.WriteToken(stoken);

            return token;
        }
    }
}
