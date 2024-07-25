using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Handles operations related to messages, including creating, retrieving, and deleting messages.
    /// </summary>
    [ApiController]
    [Route("api/messages")]
    public class MessageController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper service for mapping between entities and DTOs.</param>
        /// <param name="uow">The unit of work service for handling database operations.</param>
        public MessageController(IMapper mapper, IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Creates a new message from the current user to the specified recipient.
        /// </summary>
        /// <param name="createMessageDto">The data transfer object containing message details.</param>
        /// <returns>A response with the created message or an error message if the operation fails.</returns>
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            // Check if the sender is trying to send a message to themselves
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send message to yourself");
            }

            // Retrieve sender and recipient from the repository
            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            // Check if the recipient exists
            if (recipient == null)
            {
                return NotFound();
            }

            // Create a new message entity
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            // Add message to the repository and save changes
            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete())
            {
                var messageDto = _mapper.Map<MessageDto>(message);
                return Ok(messageDto);
            }

            return BadRequest("Failed to send message");
        }

        /// <summary>
        /// Retrieves a paginated list of messages for the current user.
        /// </summary>
        /// <param name="messageParams">The parameters for pagination and filtering of messages.</param>
        /// <returns>A paginated list of messages.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            // Set the current user's username in the message parameters
            messageParams.Username = User.GetUsername();

            // Retrieve the list of messages for the user
            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            // Add pagination headers to the response
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        /// <summary>
        /// Deletes a message. Only the sender or recipient of the message can delete it.
        /// </summary>
        /// <param name="id">The ID of the message to be deleted.</param>
        /// <returns>A response indicating the success or failure of the delete operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await _uow.MessageRepository.GetMessage(id);

            // Check if the current user is either the sender or recipient of the message
            if (message.SenderUsername != username && message.RecipientUsername != username)
            {
                return Unauthorized();
            }

            // Mark the message as deleted by the sender or recipient
            if (message.SenderUsername == username)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientUsername == username)
            {
                message.RecipientDeleted = true;
            }

            // If both sender and recipient have deleted the message, remove it from the repository
            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if (await _uow.Complete())
            {
                return Ok();
            }

            return BadRequest("Problem deleting the message");
        }
    }
}
