using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using coretest.Domain.Models;
using coretest.Domain.Services;
using AutoMapper;
using coretest.Resources;
using coretest.Extensions;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using coretest.Filters;

namespace coretest.Controllers
{
    [ApiKeyAuth]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        //sign up endpoint
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateUserResource resource)
        {
            //if request body does not contain required fields return error
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var TestUser = _mapper.Map<CreateUserResource, User>(resource);
            //test if email is taken
            var result = await _userService.FindEmailAsync(TestUser);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            //test if username is taken
            result = await _userService.FindNameAsync(TestUser);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            //test password validation
            if (resource.password.StartsWith(" ") || resource.password.EndsWith(" "))
            {
                return BadRequest("Password must not start or end with empty spaces");

            }
            var rx = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&])[\S]+");
            if (rx.IsMatch(resource.password) == false)
            {
                return BadRequest("password must contain one upper case, lower case, number, and special character");
            }

            //hash the password
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(resource.password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            resource.password = Convert.ToBase64String(hashBytes);

            var user = _mapper.Map<CreateUserResource, User>(resource);

            //save user to database
            var response = await _userService.CreateUserAsync(user);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            //return success with user email/name
            var userResource = _mapper.Map<User, UserResource>(response.User);
            return Ok(userResource);
        }
    }
}
