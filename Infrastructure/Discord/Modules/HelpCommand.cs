using Discord.Commands;

namespace Infrastructure.Discord.Modules
{


    public class HelpCommand(CommandService _commands) : ModuleBase<SocketCommandContext>
    {

        [Command("help")]
        public async Task HelpAsync(string? commandName = null)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                var summaries = _commands.Commands
                    .Select(cmd => $"{GetPrimaryAlias(cmd)} - {cmd.Summary ?? "No summary provided."}");

                await ReplyAsync(string.Join(Environment.NewLine, summaries));
                return;
            }

            var search = _commands.Search(Context, commandName);
            if (!search.IsSuccess)
            {
                await ReplyAsync($"No command named `{commandName}` found.");
                return;
            }

            foreach (var match in search.Commands)
            {
                var cmd = match.Command;
                var summary = cmd.Summary ?? "No summary available.";
                var parameters = string.Join(", ",
                    cmd.Parameters.Select(p => $"{p.Name}: {p.Summary ?? "No description."}"));

                await ReplyAsync($"**{GetPrimaryAlias(cmd)}**\nSummary: {summary}\nParameters: {parameters}");
            }
        }

        private static string GetPrimaryAlias(CommandInfo command) =>
            command.Aliases.FirstOrDefault() ?? command.Name;
    }
}