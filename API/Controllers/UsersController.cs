using API.Data;
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

}
