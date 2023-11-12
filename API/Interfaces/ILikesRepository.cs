using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int sargetUserId);

        Task<AppUser> GetUsersWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams); //liked or being liked
    }
}
