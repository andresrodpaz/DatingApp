using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController:ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;    
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _repository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public  async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        var users = await _repository.GetMembersAsync();
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
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Asynchronously fetch the user entity from the repository using the username.
        var user = await _repository.GetUserByUsernameAsync(username);

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

}
