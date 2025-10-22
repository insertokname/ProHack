using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure.Discord.Announcments;
using Infrastructure.Discord.Exceptions;
using Infrastructure.Discord.Modules;

namespace Infrastructure.Discord
{
    public class DiscordBot
    {
        private IServiceProvider? _serviceProvider;

        public bool IsConnected { get { return _client.ConnectionState == ConnectionState.Connected; } }
        public bool IsConnecting { get { return _client.ConnectionState == ConnectionState.Connecting; } }

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public DiscordBot(IServiceProvider serviceProvider)
        {
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);
            _client.MessageReceived += HandleCommandAsync;
            _commands = new CommandService();
            _serviceProvider = serviceProvider;
        }

        public async Task InitCommandService()
        {
            await _commands.AddModuleAsync<EchoCommand>(_serviceProvider);
            await _commands.AddModuleAsync<HelpCommand>(_serviceProvider);
            await _commands.AddModuleAsync<IsAuthorizedCommand>(_serviceProvider);
            await _commands.AddModuleAsync<GetUserIdCommand>(_serviceProvider);
            await _commands.AddModuleAsync<SetAnnounceChannelCommand>(_serviceProvider);
            await _commands.AddModuleAsync<SendKeyCommand>(_serviceProvider);
            await _commands.AddModuleAsync<GetScreenshotCommand>(_serviceProvider);
        }

        public async Task StartAsync()
        {
            var token = Database.Database.Tables.DiscordBotToken?.Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new NoDiscordBotTokenException("Discord token was not set when initializing!");
            }

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message || message.Author.IsBot)
            {
                return;
            }

            int position = 0;
            bool messageIsCommand = message.HasCharPrefix('!', ref position);

            if (messageIsCommand)
            {
                await _commands.ExecuteAsync(
                    new SocketCommandContext(_client, message),
                    position,
                    _serviceProvider);
                return;
            }
        }

        public async Task SendAnnouncement(IAnnouncement announcement)
        {
            await announcement.Send(_client);
        }
    }
}
