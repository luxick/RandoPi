using System;
using Gtk;
using Key = Gdk.Key;
using UI = Gtk.Builder.ObjectAttribute;

namespace RandoPi.Desktop
{
    class MainWindow : Window
    {
        private bool _isFullScreen;
        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade"))
        {
        }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            KeyReleaseEvent += OnKeyReleaseEvent;
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;
        }

        private void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key is Key.q or Key.Escape)
                Application.Quit();

            if (args.Event.Key is Key.f)
                ToogleFullscreen();

        }

        private void ToogleFullscreen()
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

        private void Button1_Clicked(object sender, EventArgs a)
        {
            _counter++;
            _label1.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
        }
    }
}