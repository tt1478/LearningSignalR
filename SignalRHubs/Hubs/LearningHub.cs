using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.IHubs;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace SignalRServer.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LearningHub: Hub<ILearningHubClient>
    {
        public async Task BroadcastMessage(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception();
                }
                await Clients.All.ReceiveMessage(message);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
        }
        public async Task SendToOthers(string message)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception();
                }
                await Clients.Others.ReceiveMessage(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        public async Task SendToCallers(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception();
                }
                await Clients.Caller.ReceiveMessage(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task SendToIndivisual(string connectionId, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception();
                }
                else if(string.IsNullOrWhiteSpace(connectionId))
                {
                    throw new Exception();
                }
                await Clients.Client(connectionId).ReceiveMessage(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task SendToGroup(string groupName, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception();
                }
                else if (string.IsNullOrWhiteSpace(groupName))
                {
                    throw new Exception();
                }
                await Clients.Group(groupName).ReceiveMessage(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task AddUserToGroup(string groupName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    throw new Exception();
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.All.ReceiveMessage($"User with connctionId {Context.ConnectionId} has added to group with name {groupName}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
        public async Task RemoveUserFromGroup(string groupName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    throw new Exception();
                }
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Clients.All.ReceiveMessage($"User with connctionId {Context.ConnectionId} has removed from group with name {groupName}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        public async Task BroadcastStream(IAsyncEnumerable<string> stream)
        {
            await foreach (var item in stream)
            {
                await Clients.Caller.ReceiveMessage($"Server received {item}");
            }
        }
        public async IAsyncEnumerable<string> TriggerStream(int jobsCount, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            for(int i = 0; i <= jobsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return $"Job {i} executed successfully";
                await Task.Delay(1000, cancellationToken);
            }
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        //private string GetMessageToSend(string originalMessage)
        //{
        //    return $"User connection id: {Context.ConnectionId}. Message: {originalMessage}";
        //}
    }
}
