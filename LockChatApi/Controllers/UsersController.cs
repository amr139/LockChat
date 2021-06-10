using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LockChatLibrary.Entities;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LockChatLibrary.Services;

namespace LockChatApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserEntity authRequest)
        {
            UserEntity user = _userService.Authenticate(authRequest.Email, authRequest.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(new AuthToken { 
                Token = TokenJWTGenerate(user.Id, "JWT1C"), 
                idUser = user.Id 
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public int Register(UserEntity user)
        {

            try
            {
                // save 
                var newUser = _userService.Create(user);
                return newUser.Id;
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return -1;
            }
        }

        [HttpPost("list")]
        public IEnumerable<UserEntity> GetAll(int aux)
        {
            var users = _userService.GetAll();
            return users;
        }


        [HttpPost("getprofile")]
        public UserEntity GetProfile(int aux)
        {
            var user = _userService.GetById(int.Parse(HttpContext.User.Claims.First(_ => _.Type == ClaimTypes.Name).Value));
            return user;
        }


        [HttpPost("delete")]
        public bool Delete(UserEntity user)
        {
            _userService.Delete(user.Id);
            return true;
        }

        [HttpPost("generatekeys")]
        public bool GenerateKeys(UserKeys key)
        {
            var user = _userService.GetById(int.Parse(HttpContext.User.Claims.First(_ => _.Type == ClaimTypes.Name).Value));
            _userService.GenerateKeys(user.Id,key);
            return true;
        }

        [HttpPost("getkeys")]
        public UserKeys GetKeys()
        {
            var user = int.Parse(HttpContext.User.Claims.First(_ => _.Type == ClaimTypes.Name).Value);
            return _userService.GetKeys(user);
        }
        
        [HttpPost("getkeybundle")]
        public PreKeyBundleEntity GetKeys(UserEntity user)
        {
            return _userService.GetKeyBundle(user.Id);
        }

        private string TokenJWTGenerate(int idUser, string authMode)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, idUser.ToString()),
                     new Claim(ClaimTypes.Authentication, authMode)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
