using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{

    //private msg for users GROUP
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            this._presenceHub = presenceHub;
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._messageRepository = messageRepository;

        }

        public override async Task OnConnectedAsync()
        {
            //we want to get access to the user

            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];

            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);

            //create connection id and group name from users
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            //cleint who sees this can update existing list, 1. client creates the chat room
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            //sender and recipient names
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            //when user connects instead of api call to messages, user will receive messages from signalR

            //we should send indiviaully the message thread not to whole group
            //user joints the room then will receive messages : before we just send all here to one person ie
            await Clients.Caller.SendAsync("ReceivedMessageThread", messages);


        }
        /// <summary>
        /// when user disconnects signalR removes them from any group they belong to
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns> <summary>
        ///
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            //who is in group, sends only one user as 2 people can be in group
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {

            var username = Context.User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower())
            {

                throw new HubException("You cannot send messages to yourself");
            }
            var sender = await _userRepository.GetUserByUsernameAsync(username) ?? throw new HubException("Sender Not Found");
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername) ?? throw new HubException("Recipient Not Found");
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                SenderUsername = sender.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);
            //if user is in the chat we set read now else we will track the user that is online but not inchat we send message notification to them, if not connected to message group
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync())
            {
                //url new message and return mapped message
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            };
            throw new HubException("Couldnt send the message");
        }
        private static string GetGroupName(string caller, string other)
        {
            //return bool less than 0 ,  that indicate between two values
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());
            if (group is null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }
            //we add connetion to our connection list
            group.Connections.Add(connection);
            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to add to group");
        }
        private async Task<Group> RemoveFromMessageGroup()
        {
            //no need for new connection when we are removing
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);

            if (await _messageRepository.SaveAllAsync()) return group;


            throw new HubException("Failed to remove from group");
        }
    }
}
