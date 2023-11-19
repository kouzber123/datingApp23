using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        //we are not user to add messages
        void AddMessage(Message message);
        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);

        
        // to update our online entity
        void AddGroup(Group group);

        void RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);

        Task<Group> GetGroupForConnection(string connectionId);

    }
}
