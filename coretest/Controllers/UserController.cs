using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using coretest.Domain.Models;
using coretest.Domain.Services;
using AutoMapper;
using coretest.Resources;
using coretest.Extensions;
using coretest.Filters;
using Microsoft.AspNetCore.Cors;

namespace coretest.Controllers
{
    [EnableCors("AllowOrigin")]
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
            result = await _userService.PasswordValidation(TestUser);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            //hash the password
            var user = await _userService.HashPassword(TestUser);

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
