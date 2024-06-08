namespace Atmoos.World.InMemory.IO;

internal sealed class InputOutputException : IOException
{
    public InputOutputException(String message) : base(message) { }
    public InputOutputException(String message, Exception innerException) : base(message, innerException) { }
}
