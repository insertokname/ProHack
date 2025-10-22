using Discord.WebSocket;

namespace Infrastructure.Discord.Announcments
{
    public interface IAnnouncement
    {
        public Task Send(DiscordSocketClient client);
    }
}
