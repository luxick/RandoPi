using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using Gtk;
using Gdk;
using RandoPi.Desktop.Helpers;
using RandoPi.Desktop.Providers;
using RandoPi.Shared;
using Key = Gdk.Key;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace RandoPi.Desktop;

class MainWindow : Window
{
    private const double OSDTimeout = 3000;
    
    private bool _isFullScreen;
    private Pixbuf? _currentFrame;
    private readonly ImageProvider _imageProvider;
    private readonly DrawingArea _drawingArea;
    private readonly Box _overlayBox;
    private readonly Box _osdInfo;
    private readonly Label _osdText;
    private readonly Overlay _overlay;
    private readonly Spinner _spinner;

    private readonly Timer _osdTimer;
        
    public MainWindow() : base(WindowType.Toplevel)
    {
        _imageProvider = new ImageProvider();
        _imageProvider.LoadImageSources();
        _imageProvider.CurrentMode = ImageMode.File;
        
        KeyReleaseEvent += OnKeyReleaseEvent;
        DeleteEvent += Window_DeleteEvent;
        
        _drawingArea = new DrawingArea();
        _drawingArea.Drawn += AreaDrawn;

        _overlayBox = new Box(Orientation.Vertical, 0);
        _overlayBox.Expand = true;

        _spinner = new Spinner { Active = true };
        
        _osdText= new Label();
        
        _osdInfo = new Box(Orientation.Horizontal, 10);
        _osdInfo.Add(_osdText);
        _osdInfo.Add(_spinner);
        
        var box = new Box(Orientation.Horizontal, 0);
        box.PackEnd(_osdInfo, false, true, 0);

        _overlayBox.Add(new Label("This is the OSD"));
        _overlayBox.PackEnd(box, false, true, 0);

        _overlay = new Overlay();
        _overlay.Child = _drawingArea;
        Add(_overlay);
        ShowAll();

        _osdTimer = new Timer();
        _osdTimer.Interval = OSDTimeout;
        _osdTimer.Elapsed += (_, _) =>
        {
            HideOSD();
            _osdTimer.Enabled = false;
        };
        
        if (!Debugger.IsAttached)
            ToggleFullscreen();
    }

    private void PrepareNextFrame()
    {
        var result = _imageProvider.LoadNextImage();
        if (!result.Success) return;
        _currentFrame = new Pixbuf(result.Value);
        DisplayOnOSD("Next Image Is Loaded!");
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
        switch (args.Event.Key)
        {
            case Key.q or Key.Escape:
                Application.Quit();
                break;
            case Key.f:
                ToggleFullscreen();
                break;
            case Key.space:
                PrepareNextFrame();
                break;
            case Key.Key_1:
                ShowOSD();
                break;
            case Key.Key_2:
                HideOSD();
                break;
            case Key.Key_3:
                DisplayOnOSD("Test Message", true);
                break;
        }
    }

    private void DisplayOnOSD(string message, bool spin = false)
    {
        
        _osdText.Text = message;
        ShowOSD();

        // if (spin)
        //     _osdInfo.TryAdd(_spinner);
        // else
        //     _osdInfo.TryRemove(_spinner);
    }

    private void ShowOSD()
    {
        if (_overlay.Children.Length == 1)
        {
            _overlay.AddOverlay(_overlayBox);
            _overlay.ShowAll();
            _osdTimer.Enabled = true;
        }
    }

    private void HideOSD()
    {
        if (_overlay.Children.Length == 2) _overlay.TryRemove(_overlayBox);
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