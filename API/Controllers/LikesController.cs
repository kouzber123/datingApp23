
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;

        }

        [HttpPost("{username}")]

        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            //get user by name target
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            //get user with likes source
            var sourceUser = await _unitOfWork.LikesRepository.GetUsersWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            //get user Like source and likeuser id
            var userlike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userlike != null) return BadRequest("You already like this user");

            userlike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userlike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]

        public async Task<ActionResult<PagedList<LikeDto>>> GetUserlikes([FromQuery] LikesParams likesParams)
        {

            likesParams.UserId = User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }
    }
}
