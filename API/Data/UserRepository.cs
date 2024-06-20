using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data
{
    /// <summary>
    /// Repository for managing operations related to <see cref="AppUser"/>.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of members based on the specified user parameters.
        /// </summary>
        /// <param name="userParams">An object containing the filtering and pagination parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of MemberDTOs.</returns>
        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            // Create an IQueryable from the Users table in the database context.
            var query = _context.Users.AsQueryable();

            // Filter the users to exclude the one with the current username.
            query = query.Where(u => u.UserName != userParams.CurrentUsername);

            // Further filter the users based on the specified gender.
            query = query.Where(u => u.Gender == userParams.Gender);

            // Calculate the minimum date of birth based on the maximum age provided in user parameters.
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));

            // Calculate the maximum date of birth based on the minimum age provided in user parameters.
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            // Filter the users based on the date of birth range calculated.
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch{
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            // Project the filtered query to MemberDTOs, disable change tracking for better performance,
            // and paginate the results.
            return await PagedList<MemberDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize);
        }


        /*
        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await _context.Users
                                    .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
        }
        */

        /// <summary>
        /// Retrieves a user by their identifier asynchronously.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The <see cref="AppUser"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The <see cref="AppUser"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        /// <summary>
        /// Retrieves all users including their associated photos asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="AppUser"/>.</returns>
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Photos).ToListAsync();
        }

        /// <summary>
        /// Saves all pending changes to the database asynchronously.
        /// </summary>
        /// <returns><c>true</c> if one or more entities were changed; otherwise, <c>false</c>.</returns>
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Updates an existing user entity.
        /// </summary>
        /// <param name="user">The user entity to update.</param>
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
