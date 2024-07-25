using API.Entities;
using API.Extensions;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    /// <summary>
    /// Provides methods to manage user likes in the database.
    /// </summary>
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LikesRepository"/> class.
        /// </summary>
        /// <param name="dataContext">The data context for interacting with the database.</param>
        public LikesRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Retrieves a specific user like relationship by source and target user IDs.
        /// </summary>
        /// <param name="sourceId">The ID of the user who liked another user.</param>
        /// <param name="targetId">The ID of the user who is liked.</param>
        /// <returns>The <see cref="UserLike"/> entity if found, otherwise null.</returns>
        public async Task<UserLike> GetUserLike(int sourceId, int targetId)
        {
            return await _context.Likes.FindAsync(sourceId, targetId);
        }

        /// <summary>
        /// Retrieves a paginated list of users who are liked or who liked the specified user.
        /// </summary>
        /// <param name="likesParams">Parameters for filtering and pagination.</param>
        /// <returns>A <see cref="PagedList{LikeDto}"/> containing the paginated list of user likes.</returns>
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            // Filter likes based on the predicate
            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserID == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }
            else if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            // Project the result into LikeDto objects
            var likedUsers = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            // Create a paginated list of LikeDto
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        /// <summary>
        /// Retrieves a user along with their liked users.
        /// </summary>
        /// <param name="userId">The ID of the user whose likes are to be retrieved.</param>
        /// <returns>The <see cref="AppUser"/> entity including the liked users if found, otherwise null.</returns>
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedByUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
