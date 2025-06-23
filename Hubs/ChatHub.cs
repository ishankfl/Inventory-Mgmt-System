/*using Microsoft.AspNetCore.SignalR;

public class RealTimeCommunicationHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        if (!string.IsNullOrEmpty(groupName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }

    public async Task LeaveGroup(string groupName)
    {
        if (!string.IsNullOrEmpty(groupName))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }

    public async Task SendMessageToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
    }

    public async Task SendMessageToAll(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}*/