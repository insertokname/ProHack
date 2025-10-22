using Discord;
using Discord.WebSocket;

namespace Infrastructure.Discord.Announcments
{
    public interface ITextAnnouncement : IAnnouncement
    {
        public async Task SendText(
            DiscordSocketClient client,
            string text)
        {
            if (Database.Database.Tables.AnnouncementChannelId == null)
            {
                throw new ArgumentException("Announcement channel is null");
            }

            var channel = await client.GetChannelAsync(Database.Database.Tables.AnnouncementChannelId!.Value) as IMessageChannel;

            if (channel == null)
            {
                throw new ArgumentException("Announcement channel may not be accesible");
            }

            await channel.SendMessageAsync(text);
        }
    }
}
