using Discord.Commands;

namespace Infrastructure.Discord.Modules
{
    internal class GetScreenshotCommand : ModuleBase<SocketCommandContext>
    {
        [Command("get-screenshot")]
        [Summary("get a screenshot of your desktop")]
        public async Task GetScreenshotAsync()
        {
            if (Context.User.Id != Database.Database.Tables.AuthorizedUserId)
            {
                await ReplyAsync("User is not authorized!");
                return;
            }

            await Context.Channel.SendFileAsync(VideoManager.ScreenshotAttachment());
        }
    }
}
