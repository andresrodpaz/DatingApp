using System.IO.Compression;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Handles operations related to user accounts, including retrieving, updating, and managing user photos.
    /// </summary>
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="uow">The unit of work service for handling database operations.</param>
        /// <param name="mapper">The mapper service for mapping between entities and DTOs.</param>
        /// <param name="photoService">The photo service for handling photo uploads and deletions.</param>
        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            _uow = uow;
            _mapper = mapper;
            _photoService = photoService;
        }

        /// <summary>
        /// Retrieves a paginated list of users based on the specified parameters.
        /// </summary>
        /// <param name="userParams">The parameters for filtering and pagination of users.</param>
        /// <returns>A paginated list of user DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _uow.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        /// <summary>
        /// Retrieves the user profile for a specific username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The user profile DTO.</returns>
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await _uow.UserRepository.GetMemberAsync(username);
        }

        /// <summary>
        /// Updates the current user's information.
        /// </summary>
        /// <param name="memberUpdateDto">DTO containing updated member information.</param>
        /// <returns>ActionResult indicating the result of the update operation.</returns>
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // Retrieve the user ID from the current user's claims.
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fetch the user entity from the repository using the user ID.
            var user = await _uow.UserRepository.GetUserByIdAsync(int.Parse(userId));

            // Return NotFound if the user does not exist.
            if (user == null) return NotFound();

            // Map the updated fields from the DTO to the user entity.
            _mapper.Map(memberUpdateDto, user);

            // Save changes to the repository and return NoContent if successful.
            if (await _uow.Complete()) return NoContent();

            // Return BadRequest if the save operation fails.
            return BadRequest("Failed to update user");
        }

        /// <summary>
        /// Adds a new photo for the current user.
        /// </summary>
        /// <param name="file">The photo file to upload.</param>
        /// <returns>The uploaded photo DTO if successful.</returns>
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _uow.Complete())
            {
                return CreatedAtAction(nameof(GetUser), 
                                        new { username = user.UserName },  
                                        _mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problems adding photo");
        }

        /// <summary>
        /// Sets a specific photo as the main photo for the current user.
        /// </summary>
        /// <param name="photoId">The ID of the photo to set as main.</param>
        /// <returns>ActionResult indicating the result of the update operation.</returns>
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            // Get the user by their username
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            // Find the photo specified by its ID
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();

            // Return BadRequest if the photo is already the main photo
            if (photo.IsMain) return BadRequest("This is already your main photo");

            // Find and update the current main photo
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;

            // Set the new photo as the main photo
            photo.IsMain = true;

            // Save changes and return NoContent if successful
            if (await _uow.Complete())
            {
                return NoContent();
            }

            // Return BadRequest if there is a problem saving changes
            return BadRequest("Problem setting the main photo");
        }

        /// <summary>
        /// Deletes a specific photo of the current user.
        /// </summary>
        /// <param name="photoId">The ID of the photo to delete.</param>
        /// <returns>ActionResult indicating the result of the delete operation.</returns>
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();

            // Return BadRequest if attempting to delete the main photo
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            // Delete the photo from the photo service
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);  
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            // Save changes and return Ok if successful
            if (await _uow.Complete())
            {
                return Ok();
            }

            // Return BadRequest if there is a problem deleting the photo
            return BadRequest("Problem deleting photo");
        }
    }
}
