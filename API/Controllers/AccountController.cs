using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext dataContext, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user is null) return Unauthorized("Username does not exist");

            //reverse has with hmac key
            using var hmac = new HMACSHA512(user.PasswordSalt);
            //we can compare 2 pws
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            var userDto = new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            return userDto;
        }

        [HttpPost("register")] //api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            //hash the password, using = we want to auto dispose this hmac after complete, class is removed from memory
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            var userDto = new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            return userDto;

        }
        private async Task<bool> UserExists(string username)
        {

            return await _dataContext.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
