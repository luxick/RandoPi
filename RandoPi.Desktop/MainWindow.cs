using System;
using System.Diagnostics;
using System.IO;
using Gtk;
using Gdk;
using RandoPi.Desktop.Providers;
using RandoPi.Shared;
using Key = Gdk.Key;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace RandoPi.Desktop;

class MainWindow : Window
{
    private bool _isFullScreen;
    private Pixbuf? _currentFrame;
    private readonly ImageProvider _imageProvider;
    private DrawingArea _drawingArea;
        
    public MainWindow() : base(WindowType.Toplevel)
    {
        _imageProvider = new ImageProvider();
        _imageProvider.LoadImageSources();
        _imageProvider.CurrentMode = ImageMode.Foxes;
        
        KeyReleaseEvent += OnKeyReleaseEvent;
        DeleteEvent += Window_DeleteEvent;
        
        _drawingArea = new DrawingArea();
        _drawingArea.Drawn += AreaDrawn;
        Add(_drawingArea);

        ShowAll();
        
        if (!Debugger.IsAttached)
            ToggleFullscreen();
    }

    private void PrepareNextFrame()
    {
        var result = _imageProvider.LoadNextImage();
        if (!result.Success) return;
        _currentFrame = new Pixbuf(result.Value);
        QueueDraw();
    }

    private void AreaDrawn(object o, DrawnArgs args)
    {
        if(o is not DrawingArea area) return;
        
        var cr = args.Cr;
        cr.SetSourceRGB(0, 0, 0);
        cr.Paint();
        
        if (_currentFrame != null)
        {
            var (pixbuf, x, y) = ImageHelper.ScaleFrameToArea(area, _currentFrame);
            Gdk.CairoHelper.SetSourcePixbuf(cr, pixbuf, x, y);
            cr.Paint();
        }
        
        ((IDisposable)cr.GetTarget()).Dispose();
        ((IDisposable)cr).Dispose();
    }

    private void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
    {
        if (args.Event.Key is Key.q or Key.Escape)
            Application.Quit();

        if (args.Event.Key is Key.f)
            ToggleFullscreen();

        if (args.Event.Key is Key.space)
            PrepareNextFrame();

    }

    private void ToggleFullscreen()
    {
        if (_isFullScreen)
        {
            Unfullscreen();
            _isFullScreen = false;
        }
        else
        {
            Fullscreen();
            _isFullScreen = true;
        }
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }
}