namespace Infrastructure.Discord.Exceptions
{

    [Serializable]
    public class NoDiscordBotTokenException : Exception
    {
        public NoDiscordBotTokenException() { }
        public NoDiscordBotTokenException(string message) : base(message) { }
        public NoDiscordBotTokenException(string message, Exception inner) : base(message, inner) { }
    }
}
