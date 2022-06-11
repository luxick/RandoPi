using System;

namespace RandoPi.Desktop;

[Serializable]
public class AppSettings
{
    /// <summary>
    /// Should the API server be started with the application?
    /// </summary>
    public bool StartApiServer { get; set; } = false;

    /// <summary>
    /// Base path for image files
    /// </summary>
    public string FilesPath { get; set; } = "";
}