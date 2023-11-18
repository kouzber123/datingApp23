using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            this._tracker = tracker;

        }
        //when this user connects we send to  others this info
        /// <summary>
        /// We add user to our private list
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            if (isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

            var currentUsers = await _tracker.GetOnlineUsers();
            //only calling client get this
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        /// <summary>
        /// tracker > disconnet we pass the user and connection id and remove them and keep updating users online
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns> <summary>
        ///
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var isOffline = await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            if (isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            await base.OnDisconnectedAsync(exception);
        }
    }
}
