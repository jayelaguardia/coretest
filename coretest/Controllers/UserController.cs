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

namespace coretest.Controllers
{
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

        [HttpGet]
        public async Task<IEnumerable<UserResource>> GetAllAsync()
        {
            var users = await _userService.ListAsync();
            var resources = _mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(users);

            return resources;
        }
        //public IActionResult Get()
        //{
        //    return Ok("this is the get request");
        //}

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CreateUserResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var TestUsername = _mapper.Map<CreateUserResource, User>(resource);

            var result = await _userService.FindAsync(TestUsername);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            if (resource.password.StartsWith(" ") || resource.password.EndsWith(" "))
            {
                return BadRequest("Password must not start or end with empty spaces");

            }

            var rx = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&])[\S]+");
            if (rx.IsMatch(resource.password) == false)
            {
                return BadRequest("password must contain one upper case, lower case, number, and special character");
            }

            //hash password
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(resource.password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            resource.password = Convert.ToBase64String(hashBytes);

            var user = _mapper.Map<CreateUserResource, User>(resource);

            var response = await _userService.CreateUserAsync(user);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            var userResource = _mapper.Map<User, UserResource>(response.User);
            return Ok(userResource);
        }
    }
}
