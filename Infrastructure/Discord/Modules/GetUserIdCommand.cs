using Discord;
using Discord.Commands;

namespace Infrastructure.Discord.Modules
{

    internal class GetUserIdCommand : ModuleBase<SocketCommandContext>
    {
        [Command("get-user-id")]
        [Summary("dm you your user id")]
        public async Task MessageAsync()
        {
            await Context.User.SendMessageAsync($"your user id is: ```\n{Context.User.Id}\n```");
        }
    }
}
