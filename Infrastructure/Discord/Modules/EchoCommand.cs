using Discord.Commands;

namespace Infrastructure.Discord.Modules
{
    internal class EchoCommand : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes back what was said")]
        public async Task ExecuteAsync([Summary("A phrase")] string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
            {
                await ReplyAsync($"Usage: !echo <phrase>");
                return;
            }

            await ReplyAsync(phrase + "2");
        }
    }
}
