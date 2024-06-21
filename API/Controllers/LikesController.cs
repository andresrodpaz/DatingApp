using API.Controllers;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API
{
    [ApiController]
[Route("api/[controller]")]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        private readonly ILogger<LikesController> _logger;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository, ILogger<LikesController> logger)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
            _logger = logger;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            try
            {
                var userIdStr = User.GetUserId();
                if (string.IsNullOrEmpty(userIdStr))
                {
                    _logger.LogError("User.GetUserId() returned null or empty.");
                    return Unauthorized("User ID is invalid.");
                }

                if (!int.TryParse(userIdStr, out var sourceUserId))
                {
                    _logger.LogError("Failed to parse User ID: {UserIdStr}", userIdStr);
                    return BadRequest("User ID is invalid.");
                }

                var likedUser = await _userRepository.GetUserByUsernameAsync(username);
                if (likedUser == null)
                {
                    _logger.LogWarning("Liked user not found: {Username}", username);
                    return NotFound("Liked user not found.");
                }

                var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
                if (sourceUser == null)
                {
                    _logger.LogWarning("Source user not found: {SourceUserId}", sourceUserId);
                    return NotFound("Source user not found.");
                }

                if (sourceUser.UserName == username)
                {
                    _logger.LogWarning("User {Username} tried to like themselves.", username);
                    return BadRequest("You can't like yourself!");
                }

                var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
                if (userLike != null)
                {
                    _logger.LogWarning("User {SourceUserId} already liked user {LikedUserId}", sourceUserId, likedUser.Id);
                    return BadRequest("You already liked this user.");
                }

                userLike = new UserLike
                {
                    SourceUserID = sourceUserId,
                    TargetUserId = likedUser.Id
                };

                sourceUser.LikedUsers.Add(userLike);

                if (await _userRepository.SaveAllAsync())
                {
                    _logger.LogInformation("User {SourceUserId} successfully liked user {LikedUserId}", sourceUserId, likedUser.Id);
                    return Ok();
                }

                _logger.LogError("Failed to like user {LikedUserId} by user {SourceUserId}", likedUser.Id, sourceUserId);
                return BadRequest("Failed to like user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a like for user {Username}", username);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = int.Parse(User.GetUserId());
            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }
}
