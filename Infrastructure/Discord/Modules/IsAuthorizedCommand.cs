using Discord.Commands;

namespace Infrastructure.Discord.Modules
{

    internal class IsAuthorizedCommand : ModuleBase<SocketCommandContext>
    {
        [Command("is-authorized")]
        [Summary("check if the current user is authorized")]
        public async Task MessageAsync()
        {
            if (Context.User.Id != Database.Database.Tables.AuthorizedUserId)
            {
                await ReplyAsync("User is not authorized!");
                return;
            }
            await ReplyAsync("User is authorized");
        }
    }
}
