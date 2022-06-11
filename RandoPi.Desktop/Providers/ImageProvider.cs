using System.Collections.Generic;
using luxick.Result;
using RandoPi.Desktop.Providers.Sources;
using RandoPi.Shared;

namespace RandoPi.Desktop.Providers;

public class ImageProvider
{
    public ImageMode CurrentMode { get; set; } = ImageMode.None;

    private Dictionary<ImageMode, IImageSource> _sources = new();

    public ImageProvider()
    {
        
    }

    public void LoadImageSources()
    {
        var fileSource = new FileSource();
        _sources.Add(ImageMode.File, fileSource);
    }

    public Result<byte[]> LoadNextImage()
    {
        if (!_sources.ContainsKey(CurrentMode))
            return new Error<byte[]>($"Mode '{CurrentMode}' is not implemened!");
        
        var source = _sources[CurrentMode];
        var data = source.Get();
        return new Ok<byte[]>(data);
    }
}