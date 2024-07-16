using System.IO.Compression;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;
public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

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

   public async Task<PagedList<MessageDto>> GetMessagesThread(string currentUserName, string recipientUserName)
{
    var messagesQuery = _context.Messages
        .Include(m => m.Sender).ThenInclude(u => u.Photos)
        .Include(m => m.Recipient).ThenInclude(u => u.Photos)
        .Where(m =>
            (m.Recipient.UserName == currentUserName && m.Sender.UserName == recipientUserName && m.RecipientDeleted == false) ||
            (m.Recipient.UserName == recipientUserName && m.Sender.UserName == currentUserName && m.SenderDeleted == false ))
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
        await _context.SaveChangesAsync();
    }

    var mappedMessages = _mapper.Map<IEnumerable<MessageDto>>(messagesQuery);

    return new PagedList<MessageDto>(mappedMessages, messagesQuery.Count(), 1, messagesQuery.Count());
}


    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    #region Groups
    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
        .Include(x => x.Connections)
        .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }
    #endregion
}
