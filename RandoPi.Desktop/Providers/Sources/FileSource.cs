using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandoPi.Shared;

namespace RandoPi.Desktop.Providers.Sources;

public class FileSource : IImageSource
{
    public ImageMode Mode => ImageMode.File;
    
    private readonly string _baseDir;
    private readonly Queue<string> _images = new();

    public FileSource()
    {
        _baseDir = StaticData.AppSettings.FilesPath;
    }
    
    public byte[] Get()
    {
        if (_images.Count == 0)
            FillImageQueue();
        var next = _images.Dequeue();
        var data = File.ReadAllBytes(next);
        return data;
    }

    private void FillImageQueue()
    {
        var allFiles = Directory.EnumerateFiles(_baseDir, "*.jpg", SearchOption.AllDirectories)
            .ToList()
            .Shuffle();
        allFiles.ForEach(_images.Enqueue);
    }
    
}