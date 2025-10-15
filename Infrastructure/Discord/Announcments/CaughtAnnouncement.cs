using Application;
using Discord;
using Discord.WebSocket;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Discord.Announcments
{
    public class CaughtAnnouncement(
        bool isSpecial,
        int id) : IFileAnnouncement
    {
        public async Task Send(DiscordSocketClient client)
        {

            await (this as IFileAnnouncement).SendFile(
                client,
                [VideoManager.ScreenshotAttachment()],
                $"Found {(isSpecial ? "a special form of" : "")} id: {id}. I attached a screenshot of the desktop.");
        }
    }
}
