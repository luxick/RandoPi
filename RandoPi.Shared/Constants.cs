namespace RandoPi.Shared;

/// <summary>
/// Shared constants
/// </summary>
public static class Constants
{
    /// <summary>
    /// Name for the Message Pipe
    /// </summary>
    public const string PipeName = "randopipipe";
    
    /// <summary>
    /// Name of the main process. Used by the API server watchdog
    /// </summary>
    // TODO Check process names for linux
    public const string MainProcessName = "RandoPi.Desktop";
}