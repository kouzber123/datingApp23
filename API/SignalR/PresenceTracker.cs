using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        //username is the key
        private static readonly Dictionary<string, List<string>> OnlineUsers = new();

        public Task<bool> UserConnected(string username, string connectionId)
        {

            bool isOnline = false;

            //ensures that one thread is executing a piece of code at one time.
            lock (OnlineUsers)
            {

                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        /// <summary>
        /// return bool instead of object to optimise, we dont need to send actual data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {

            string[] onlineUsers;

            lock (OnlineUsers)
            {
                //key = username
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
        /// <summary>
        /// not scalable solution
        /// DATABSE OR REDIS BETTER
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns> <summary>
        ///
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Task<List<string>> GetConnectionsForUser(string username)
        {

            List<string> ConnectionIds;
            lock (OnlineUsers)
            {
                ConnectionIds = OnlineUsers.GetValueOrDefault(username);

            }

            return Task.FromResult(ConnectionIds);
        }
    }
}
