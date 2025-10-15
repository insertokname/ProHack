using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Discord.Announcments
{
    public interface IFileAnnouncement : IAnnouncement
    {
        public async Task SendFile(
            DiscordSocketClient client,
            IEnumerable<FileAttachment> fileAttachments,
            string? text = null)
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

            await channel.SendFilesAsync(fileAttachments, text);
        }
    }
}
