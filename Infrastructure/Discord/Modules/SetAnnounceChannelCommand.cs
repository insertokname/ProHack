using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Discord.Modules
{

    internal class SetAnnounceChannelCommand : ModuleBase<SocketCommandContext>
    {
        [Command("set-announce-channel")]
        [Summary("set the current channel as the announcement channel")]
        public async Task MessageAsync()
        {
            if (Context.User.Id != Database.Database.Tables.AuthorizedUserId)
            {
                await ReplyAsync("User is not authorized!");
                return;
            }

            Database.Database.Tables.AnnouncementChannelId = Context.Channel.Id;
            Database.Database.Save();
            await ReplyAsync("Saved the current channel as the anouncement channel");
        }
    }
}
