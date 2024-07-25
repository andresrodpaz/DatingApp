using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    /// <summary>
    /// Implements the Unit of Work pattern to manage repositories and save changes to the database.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> used to interact with the database.</param>
        /// <param name="mapper">The <see cref="IMapper"/> used for object mapping.</param>
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the <see cref="IUserRepository"/> for managing user data.
        /// </summary>
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        /// <summary>
        /// Gets the <see cref="IMessageRepository"/> for managing message data.
        /// </summary>
        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        /// <summary>
        /// Gets the <see cref="ILikesRepository"/> for managing likes data.
        /// </summary>
        public ILikesRepository LikesRepository => new LikesRepository(_context);

        /// <summary>
        /// Saves all changes made in the unit of work to the database.
        /// </summary>
        /// <returns>True if the changes were saved successfully; otherwise, false.</returns>
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Checks if there are any changes tracked by the context.
        /// </summary>
        /// <returns>True if there are changes; otherwise, false.</returns>
        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
