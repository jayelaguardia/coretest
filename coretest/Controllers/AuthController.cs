using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using coretest.Domain.Models;
using coretest.Domain.Services;
using AutoMapper;
using coretest.Resources;
using coretest.Extensions;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using coretest.Filters;

namespace coretest.Controllers
{
    [ApiKeyAuth]
    [Route("/api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IMapper mapper, IConfiguration configuration)
        {
            _authService = authService;
            _mapper = mapper;
            _configuration = configuration;
        }

        //log in endpoint
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] LoginResource resource)
        {
            //if request body does not contain required fields return error
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var TestUsername = _mapper.Map<LoginResource, Auth>(resource);
            //test if user exists
            var result = await _authService.FindAsync(TestUsername);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            
            //if user does exists, check pass
            byte[] savedPasswordBytes = Convert.FromBase64String(result.User.password);
            byte[] salt = new byte[16];
            Array.Copy(savedPasswordBytes, 0, salt, 0, 16);
            var inputPass = new Rfc2898DeriveBytes(resource.password, salt, 10000);
            byte[] inputHash = inputPass.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (savedPasswordBytes[i + 16] != inputHash[i])
                    return BadRequest("Incorrect username or password");
            }

            //create auth token
            var symmetricKey = Convert.FromBase64String(_configuration["SecurityKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, resource.username)
                }),

                Expires = now.AddDays(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            //send auth token
            return Ok(token); 
        }       

        //refresh token
        [HttpPut]
        public async Task<IActionResult> PutAsync()
        {
            return Ok();
        }
    }
}
