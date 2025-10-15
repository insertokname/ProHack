using Application;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
