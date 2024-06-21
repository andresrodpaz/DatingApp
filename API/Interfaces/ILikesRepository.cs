using API.Entities;
using API.Helpers;

namespace API;
public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceId, int targetId);
    Task<AppUser> GetUserWithLikes(int userId);
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    
}
