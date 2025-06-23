namespace Inventory_Mgmt_System.Hubs
{
    public interface IRealTimeClient
    {
        Task ReceiveMessage(string message);
        Task UserJoined(string userId);
        Task UserLeft(string userId);
    }
}
