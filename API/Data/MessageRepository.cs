using System.IO.Compression;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API
{
    /// <summary>
    /// Handles data operations related to messages and real-time communication groups.
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepository"/> class.
        /// </summary>
        /// <param name="context">The data context for interacting with the database.</param>
        /// <param name="mapper">The AutoMapper instance for mapping entities to DTOs.</param>
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new message to the database.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> entity to add.</param>
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        /// <summary>
        /// Retrieves a message by its ID.
        /// </summary>
        /// <param name="id">The ID of the message to retrieve.</param>
        /// <returns>The <see cref="Message"/> entity if found, otherwise null.</returns>
        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a paginated list of messages for a user based on the specified parameters.
        /// </summary>
        /// <param name="messageParams">The parameters for filtering and pagination.</param>
        /// <returns>A <see cref="PagedList{MessageDto}"/> containing the paginated list of messages.</returns>
        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSend)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null),
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        /// <summary>
        /// Retrieves a paginated list of messages exchanged between the current user and a specified recipient.
        /// </summary>
        /// <param name="currentUserName">The username of the current user.</param>
        /// <param name="recipientUserName">The username of the recipient.</param>
        /// <returns>A <see cref="PagedList{MessageDto}"/> containing the messages thread.</returns>
        public async Task<PagedList<MessageDto>> GetMessagesThread(string currentUserName, string recipientUserName)
        {
            var messagesQuery = _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                .Where(m =>
                    (m.Recipient.UserName == currentUserName && m.Sender.UserName == recipientUserName && m.RecipientDeleted == false) ||
                    (m.Recipient.UserName == recipientUserName && m.Sender.UserName == currentUserName && m.SenderDeleted == false))
                .OrderByDescending(m => m.MessageSend)
                .AsQueryable();

            var unreadMessages = await messagesQuery
                .Where(m => m.DateRead == null && m.Recipient.UserName == currentUserName)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            var mappedMessages = _mapper.Map<IEnumerable<MessageDto>>(messagesQuery);

            return new PagedList<MessageDto>(mappedMessages, messagesQuery.Count(), 1, messagesQuery.Count());
        }

        /// <summary>
        /// Deletes a message from the database.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> entity to delete.</param>
        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        #region Groups

        /// <summary>
        /// Adds a new group to the database.
        /// </summary>
        /// <param name="group">The <see cref="Group"/> entity to add.</param>
        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        /// <summary>
        /// Removes a connection from the database.
        /// </summary>
        /// <param name="connection">The <see cref="Connection"/> entity to remove.</param>
        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        /// <summary>
        /// Retrieves a connection by its ID.
        /// </summary>
        /// <param name="connectionId">The ID of the connection to retrieve.</param>
        /// <returns>The <see cref="Connection"/> entity if found, otherwise null.</returns>
        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        /// <summary>
        /// Retrieves a group by its name.
        /// </summary>
        /// <param name="groupName">The name of the group to retrieve.</param>
        /// <returns>The <see cref="Group"/> entity if found, otherwise null.</returns>
        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        /// <summary>
        /// Retrieves a group that contains the specified connection.
        /// </summary>
        /// <param name="connectionId">The ID of the connection to find the group for.</param>
        /// <returns>The <see cref="Group"/> entity if found, otherwise null.</returns>
        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        #endregion
    }
}
