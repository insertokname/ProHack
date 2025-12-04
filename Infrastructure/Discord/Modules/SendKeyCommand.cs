using Discord.Commands;
using Infrastructure.Memory;

namespace Infrastructure.Discord.Modules
{

    internal class SendKeyCommand(PROMemoryManager _proMemoryManager) : ModuleBase<SocketCommandContext>
    {
        [Command("send-key")]
        [Summary("Send a key press to the game. use !help send-key for a list of keys")]
        public async Task SendKeysync([Summary("Key names can be found here https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=windowsdesktop-9.0")] Keys key)
        {
            if (Context.User.Id != Database.Database.Tables.AuthorizedUserId)
            {
                await ReplyAsync("User is not authorized!");
                return;
            }

            if (_proMemoryManager.Process == null)
            {
                await ReplyAsync("PRO isn't running or wasn't found!");
                return;
            }

            Controller.SendKeyPress(_proMemoryManager.Process, (ushort)key);
            await ReplyAsync("Sent the key");
        }
    }
}
