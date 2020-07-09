using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using coretest.Domain.Models;
using coretest.Domain.Services;
using AutoMapper;
using coretest.Resources;
using coretest.Extensions;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace coretest.Controllers
{
    [Route("/api/[controller]")]
    public class AuthController : Controller
    {
        //remember to move this secret to secure config. don't just leave it here
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] LoginResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var TestUsername = _mapper.Map<LoginResource, Auth>(resource);

            var result = await _authService.FindAsync(TestUsername);
            if (!result.Item1.Success)
            {
                return BadRequest(result.Item1.Message);
            }

            //if user does exists, check pass
            byte[] savedPasswordBytes = Encoding.ASCII.GetBytes(result.Item2.password);
            byte[] salt = new byte[16];
            Array.Copy(savedPasswordBytes, 0, salt, 0, 16);
            var inputPass = new Rfc2898DeriveBytes(resource.password, salt, 10000);
            byte[] inputHash = inputPass.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (savedPasswordBytes[i + 16] != inputHash[i])
                    return BadRequest("Incorrect username or password");
            }

            //create auth
            var symmetricKey = Encoding.ASCII.GetBytes(Secret);
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

            //send auth
            return Ok(token);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync()
        {
            return Ok();
        }
    }
}
