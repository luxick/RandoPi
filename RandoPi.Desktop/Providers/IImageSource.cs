using RandoPi.Shared;

namespace RandoPi.Desktop.Providers;

/// <summary>
/// Describes a source to pull random images from
/// </summary>
public interface IImageSource
{
    /// <summary>
    /// Image mode for which this source can provide images
    /// </summary>
    ImageMode Mode { get; }

    /// <summary>
    /// Gets the next image from the source
    /// </summary>
    /// <returns>raw image data</returns>
    byte[] Get();
}