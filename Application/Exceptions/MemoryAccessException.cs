namespace Application.Exceptions
{
    [System.Serializable]
    public class MemoryAccessException : Exception
    {
        public MemoryAccessException() { }
        public MemoryAccessException(string message) : base(message) { }
        public MemoryAccessException(string message, Exception inner) : base(message, inner) { }
    }
}