using System;
using System.Diagnostics;
using System.IO;
using Gtk;
using Cairo;
using Gdk;
using Key = Gdk.Key;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace RandoPi.Desktop;

class MainWindow : Window
{
    private bool _isFullScreen;

    private Pixbuf _currentFrame;
        
    public MainWindow() : base(WindowType.Toplevel)
    {
        KeyReleaseEvent += OnKeyReleaseEvent;
        DeleteEvent += Window_DeleteEvent;
        
        var drawingArea = new DrawingArea();
        drawingArea.Drawn += AreaDrawn;
        Add(drawingArea);
        
        LoadNextFrame();

        ShowAll();
        
        if (!Debugger.IsAttached)
            ToggleFullscreen();
    }

    private void LoadNextFrame()
    {
        var bytes = File.ReadAllBytes(@"image.png");
        _currentFrame = new Pixbuf(bytes);
    }

    private void AreaDrawn(object o, DrawnArgs args)
    {
        var cr = args.Cr;
        cr.SetSourceRGB(0, 0, 0);
        cr.Paint();
        
        Gdk.CairoHelper.SetSourcePixbuf(cr, _currentFrame, 0, 0);
        cr.Paint();
        
        ((IDisposable)cr.GetTarget()).Dispose();
        ((IDisposable)cr).Dispose();
    }

    private void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
    {
        if (args.Event.Key is Key.q or Key.Escape)
            Application.Quit();

        if (args.Event.Key is Key.f)
            ToggleFullscreen();

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