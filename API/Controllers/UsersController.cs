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

namespace API.Controllers;
[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController:ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;    
    private readonly IPhotoService _photoService;
    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        _repository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
    }


    [HttpGet]
    public  async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams){
        
        var currentUser = await _repository.GetUserByUsernameAsync(User.GetUsername());
        userParams.CurrentUsername = currentUser.UserName;
        
        if(string.IsNullOrEmpty(userParams.Gender)){
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
        }

        var users = await _repository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
        users.TotalCount, users.TotalPages));
        return Ok(users);
        
    }

    [HttpGet("{username}")] //api/users/username
    public async Task< ActionResult<MemberDTO>> GetUser(string username){
        return await _repository.GetMemberAsync(username);
    }


    /// <summary>
    /// Updates the user information.
    /// </summary>
    /// <param name="memberUpdateDto">DTO containing updated member information.</param>
    /// <returns>ActionResult indicating the result of the update operation.</returns>
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){

        // Retrieve the username from the current user's claims.
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Asynchronously fetch the user entity from the repository using the username.
        var user = await _repository.GetUserByIdAsync(int.Parse(userId));

        // If the user is not found, return a NotFound result.
        if(user == null) return NotFound();

        // Map the fields from the memberUpdateDto to the user entity.
        _mapper.Map(memberUpdateDto,user);

        // Asynchronously save all changes to the repository.
        // If the save operation is successful, return a NoContent result.
        if(await _repository.SaveAllAsync()) return NoContent();

        // If the save operation fails, return a BadRequest result with an error message.
        return BadRequest("Failed to update user");

    }

    [HttpPost("add-photo")]
    public async Task <ActionResult<PhotoDTO>> AddPhoto(IFormFile file){
        var user = await _repository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if(await _repository.SaveAllAsync()) {
            return CreatedAtAction(nameof(GetUser), 
                                    new{username = user.UserName},  
                                    _mapper.Map<PhotoDTO>(photo)
                                    );
        }

        return BadRequest("Problems adding photo");

    }

    [HttpPut("set-main-photo/{photoId}")]
public async Task<ActionResult> SetMainPhoto(int photoId)
{
    // Get the user by user namen
    var user = await _repository.GetUserByUsernameAsync(User.GetUsername());
    if (user == null) 
    {
        return NotFound();
    }

    // Find the photo specified by its ID
    var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
    if (photo == null) 
    {
        return NotFound();
    }

    // Check if the photo is already the main photo
    if (photo.IsMain) 
    {
        return BadRequest("This is already your main photo");
    }

    // Find the current main photo and change its status
    var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
    if (currentMain != null) 
    {
        currentMain.IsMain = false;
    }

    // Set the new photo as the main photo
    photo.IsMain = true;

    //Save changes in the repository
    if (await _repository.SaveAllAsync()) 
    {
        return NoContent();
    }

    // Handle any problems that occur when saving changes
    return BadRequest("Problem setting the main photo");
}

[HttpDelete("delete-photo/{photoId}")]
public async Task<ActionResult> DeletePhoto(int photoId){
    var user = await _repository.GetUserByUsernameAsync(User.GetUsername());
    var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
    if(photo == null) return NotFound();
    if(photo.IsMain) return BadRequest("You cannot delete your main photo");
    if(photo.PublicId != null){
        var result = await _photoService.DeletePhotoAsync(photo.PublicId);  
        if (result.Error != null) return BadRequest(result.Error.Message);
    }
    user.Photos.Remove(photo);

    if(await _repository.SaveAllAsync()){
        return Ok();
    }

    return BadRequest("Problem deleting photo");
}


}
