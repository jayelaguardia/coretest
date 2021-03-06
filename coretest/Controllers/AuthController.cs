﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using coretest.Domain.Models;
using coretest.Domain.Services;
using AutoMapper;
using coretest.Resources;
using coretest.Extensions;
using coretest.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;

namespace coretest.Controllers
{

    [EnableCors("AllowOrigin")]
    [ApiKeyAuth]
    [Route("/api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        //log in endpoint
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] LoginResource resource)
        {
            //if request body does not contain required fields return error
            if (!ModelState.IsValid)
            {
                var errorString = JsonSerializer.Serialize(ModelState.GetErrorMessages());
                return BadRequest(errorString);
            }
                

            var TestUsername = _mapper.Map<LoginResource, Auth>(resource);

            //test if user exists
            var result = await _authService.FindAsync(TestUsername.username);
            if (!result.Success)
            {
                var errorString = JsonSerializer.Serialize(result.Message);
                return BadRequest(errorString);
            }

            //if user does exists, check pass
            result = _authService.CheckPass(TestUsername, result.User);
            if (!result.Success)
            {
                var errorString = JsonSerializer.Serialize(result.Message);
                return BadRequest(errorString);
            }

            //create auth token
            var token = _authService.CreateToken(result.User);
            var tokenString = JsonSerializer.Serialize(token);

            //send auth token
            return Ok(tokenString); 
        }       

        //refresh token
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutAsync()
        {
            //jwt is part of request header
            //get username from jwt
            var username = HttpContext.User.FindFirst("username").Value;
            //check user exists
            var result = await _authService.FindAsync(username);
            if (!result.Success)
            {
                var errorString = JsonSerializer.Serialize(result.Message);
                return BadRequest(errorString);
            }
            //create new jwt using username
            var token = _authService.CreateToken(result.User);
            var tokenString = JsonSerializer.Serialize(token);

            //return new jwt
            return Ok(tokenString);
        }
    }
}
