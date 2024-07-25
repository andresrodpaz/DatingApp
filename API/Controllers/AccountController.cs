using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Handles user registration and login functionality.
    /// </summary>
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager service for user operations.</param>
        /// <param name="tokenService">The token service for generating JWT tokens.</param>
        /// <param name="mapper">The automapper service for mapping objects.</param>
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a new user with the provided registration data.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object containing user details.</param>
        /// <returns>A <see cref="UserDto"/> containing the registered user's details and JWT token if successful; otherwise, an error message.</returns>
        [HttpPost("register")] //POST api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDto)
        {
            // Check if the username already exists in the database.
            if (await UserExists(registerDto.Username)) return BadRequest("Username is already taken!");

            // Map the RegisterDTO to the AppUser entity.
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();

            // Create a new user with the provided password.
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // Return bad request with errors if user creation fails.
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Assign a default role to the user (e.g., "Member").
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            // Return user details along with a JWT token.
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        /// <summary>
        /// Authenticates a user and provides a JWT token upon successful login.
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing username and password.</param>
        /// <returns>A <see cref="UserDto"/> containing the authenticated user's details and JWT token if successful; otherwise, an unauthorized response.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Find the user by username.
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            // Return unauthorized if the user does not exist.
            if (user == null) return Unauthorized("Invalid user");

            // Check if the provided password matches the stored password.
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return Unauthorized("Invalid Password");

            // Return user details along with a JWT token.
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        /// <summary>
        /// Checks if a user with the given username already exists.
        /// </summary>
        /// <param name="username">The username to check for existence.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
