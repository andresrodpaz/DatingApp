using API.Controllers;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    /// <summary>
    /// Handles operations related to user likes, including adding likes and retrieving liked users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Initializes a new instance of the <see cref="LikesController"/> class.
        /// </summary>
        /// <param name="uow">The unit of work service for handling database operations.</param>
        public LikesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Adds a like from the current user to another user specified by username.
        /// </summary>
        /// <param name="username">The username of the user to be liked.</param>
        /// <returns>An action result indicating the success or failure of the operation.</returns>
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            try
            {
                // Retrieve the ID of the current user from the claims
                var userIdStr = User.GetUserId();
                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized("User ID is invalid.");
                }

                // Convert user ID from string to integer
                if (!int.TryParse(userIdStr, out var sourceUserId))
                {
                    return BadRequest("User ID is invalid.");
                }

                // Fetch the user to be liked from the repository
                var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
                if (likedUser == null)
                {
                    return NotFound("Liked user not found.");
                }

                // Fetch the source user and their likes
                var sourceUser = await _uow.LikesRepository.GetUserWithLikes(sourceUserId);
                if (sourceUser == null)
                {
                    return NotFound("Source user not found.");
                }

                // Check if the source user is trying to like themselves
                if (sourceUser.UserName == username)
                {
                    return BadRequest("You can't like yourself!");
                }

                // Check if the user has already liked this user
                var userLike = await _uow.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
                if (userLike != null)
                {
                    return BadRequest("You already liked this user.");
                }

                // Create and add the new like
                userLike = new UserLike
                {
                    SourceUserID = sourceUserId,
                    TargetUserId = likedUser.Id
                };

                sourceUser.LikedUsers.Add(userLike);

                // Save changes and check for success
                if (await _uow.Complete())
                {
                    return Ok();
                }

                return BadRequest("Failed to like user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves a paginated list of users that the current user has liked.
        /// </summary>
        /// <param name="likesParams">Parameters for pagination and filtering of likes.</param>
        /// <returns>A paginated list of liked users.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            // Set the user ID from the claims to the likes parameters
            likesParams.UserId = int.Parse(User.GetUserId());

            // Retrieve the list of liked users
            var users = await _uow.LikesRepository.GetUserLikes(likesParams);

            // Add pagination headers to the response
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }
}
