using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;

        }


        [HttpPost]

        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            //useraname from the token
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower())
            {

                return BadRequest("Cannot send messages to itself.");
            }
            //get senders userdata
            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (sender == null) return NotFound("Couldnt find sender");

            //get recipients userdata
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null) return NotFound();
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                SenderUsername = sender.UserName,
                Content = createMessageDto.Content
            };
            _unitOfWork.MessageRepository.AddMessage(message);
            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("failed to send message");
        }


        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            //get messages for current user
            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            //add pagination for the response
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        /// <summary>
        /// delete message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {

            //we arebnt really deleting message until both sides delte message

            var username = User.GetUserName();

            //find getmessage id
            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();
            //checker  for if both deleted the message then we delete on client side we only show them filtered if deleted is false
            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            //we delete message when this is both true
            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting message");
        }


    }

}
