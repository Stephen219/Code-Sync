namespace FileSync.Web.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    

    public class SpaceHub : Hub
    {
        // Join a space group
        public async Task JoinSpace(string spaceCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, spaceCode);
        }

        // Leave a space group
        public async Task LeaveSpace(string spaceCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, spaceCode);
        }

        // Broadcast content update to everyone in the space
        public async Task UpdateContent(string spaceCode, string content)
        {
            await Clients.OthersInGroup(spaceCode).SendAsync("ReceiveContent", content);
        }

        // Broadcast file added to everyone in the space
        public async Task FileAdded(string spaceCode, string fileName)
        {
            await Clients.OthersInGroup(spaceCode).SendAsync("ReceiveFileAdded", fileName);
        }
    }
}
