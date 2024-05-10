using API.Entities;
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

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await _context.Users
                                    .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
        }

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
