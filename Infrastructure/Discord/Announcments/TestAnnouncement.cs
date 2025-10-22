using Discord.WebSocket;

namespace Infrastructure.Discord.Announcments
{
    public class TestAnnouncement : IFileAnnouncement
    {
        public async Task Send(DiscordSocketClient client)
        {
            await (this as IFileAnnouncement).SendFile(
                client,
                [VideoManager.ScreenshotAttachment()],
                "If you recieved this message then everything is setup and working!");
        }
    }
}
