using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;

        private readonly IUserRepository _user;
        public UsersController(IUserRepository user, IMapper mapper)
        {
            _mapper = mapper;
            _user = user;

        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {

            var users = await _user.GetMembersAsync();

            return Ok(users);

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _user.GetMemberAsync(username);

        }

        // [HttpGet("{id}")]
        // public async Task<ActionResult<MemberDto>> GetUserById(int id)
        // {
        //     var user = await _user.GetUserById(id);

        //     var userToReturn = _mapper.Map<MemberDto>(user);

        //     return userToReturn;
        // }
    }
}
