using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
        private readonly IPhotoService _photoService;

        private readonly IUserRepository _user;
        public UsersController(IUserRepository user, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _user = user;

        }

        [HttpGet]
        //we have to give hint where to look for params
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            //context of current user using this service
            var currentUser = await _user.GetUserByUsernameAsync(User.GetUserName());
            userParams.CurrentUsername = currentUser.UserName;

            if (string.IsNullOrEmpty(currentUser.UserName))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }

            //Response = we intercept the http return with our addition, method works without this but we want to use this for navigation
            var users = await _user.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users); //and we send our paginated list

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _user.GetMemberAsync(username);

        }


        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //claimTypes come directly  from JWT

            var user = await _user.GetUserByUsernameAsync(User.GetUserName());

            if (user is null) return NotFound();

            _mapper.Map(memberUpdateDto, user);
            if (await _user.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            //update user photo list

            var user = await _user.GetUserByUsernameAsync(User.GetUserName());
            if (user is null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId

            };
            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo); //entity is tracking now this in memory

            if (await _user.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }; //<to> and (from)

            return BadRequest("Problem adding photo");

        }


        [HttpPut("set-main-photo/{photoId}")]

        public async Task<ActionResult> SetMainPhoto(int photoId)
        {

            var user = await _user.GetUserByUsernameAsync(User.GetUserName());
            if (user is null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo is null) return NotFound();

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _user.SaveAllAsync()) return NoContent();

            return BadRequest("Problem Setting the main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _user.GetUserByUsernameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("Cannot remove main photo, change main photo first");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);

            if (await _user.SaveAllAsync()) return Ok();

            return BadRequest("Problem Deleting photo");
        }
    }
}
