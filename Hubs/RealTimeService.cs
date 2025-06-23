using Inventory_Mgmt_System.Hubs;
using Microsoft.AspNetCore.SignalR;

public class RealTimeService
{
    private readonly IHubContext<RealTimeHub, IRealTimeClient> _hubContext;

    public RealTimeService(IHubContext<RealTimeHub, IRealTimeClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToGroupAsync(string groupName, string message)
    {
        await _hubContext.Clients.Group(groupName).ReceiveMessage(message);
    }

    public async Task SendToUserAsync(string userId, string message)
    {
        await _hubContext.Clients.User(userId).ReceiveMessage(message);
    }

    public async Task SendToAllAsync(string message)
    {
        await _hubContext.Clients.All.ReceiveMessage(message);
    }
}