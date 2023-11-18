namespace API.Entities
{
    public class Connection
    {
        //initial state
        public Connection()
        {

        }
        //state when called
        public Connection(string connectionId, string username)
        {
            this.ConnectionId = connectionId;
            Username = username;
        }
        public string ConnectionId { get; set; }
        public string Username { get; set; }


    }
}
