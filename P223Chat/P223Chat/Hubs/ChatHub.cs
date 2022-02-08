using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using P223Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P223Chat.Hubs
{
    public class ChatHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task SendMessage(string receiverUserId,string message)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                if (string.IsNullOrWhiteSpace(receiverUserId))
                {
                    await Clients.All.SendAsync("ReceiveMessage", user.FullName, message, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                }
                else
                {
                    AppUser receiverUser = _userManager.FindByIdAsync(receiverUserId).Result;

                    if(receiverUser.ConnectionId!=null)
                        await Clients.Client(receiverUser.ConnectionId).SendAsync("ReceiveMessage", user.FullName, message, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                }
            }

        }
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                user.ConnectionId = Context.ConnectionId;

                var result = _userManager.UpdateAsync(user).Result;

                Clients.All.SendAsync("UserConnected", user.Id);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                user.ConnectionId = null;

                var result = _userManager.UpdateAsync(user).Result;

                Clients.All.SendAsync("UserDisConnected", user.Id);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
