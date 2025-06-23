using Inventory_Mgmt_System.Hubs;
using Microsoft.AspNetCore.SignalR;

public class RealTimeHub : Hub<IRealTimeClient>
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).UserJoined(Context.ConnectionId);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).UserLeft(Context.ConnectionId);
    }

    public async Task SendToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).ReceiveMessage(message);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}