using Microsoft.AspNetCore.SignalR;

namespace WuXingGameBackend.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendBattleResult(string result)
        {
            await Clients.All.SendAsync("ReceiveBattleResult", result);
        }

        public async Task JoinGame(string playerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, playerId);
            await Clients.Group(playerId).SendAsync("PlayerJoined", playerId);
        }

        public async Task SelectElement(string playerId, string element)
        {
            await Clients.Group(playerId).SendAsync("ElementSelected", element);
        }
    }
}