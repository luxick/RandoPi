using System.Linq;
using Gtk;

namespace RandoPi.Desktop.Helpers;

public static class Extensions
{
    public static void TryRemove(this Container container, Widget widget)
    {
        if (container.HasChild(widget))
            container.Remove(widget);
    }
    
    public static void TryAdd(this Container container, Widget widget)
    {
        if (container.HasChild(widget))
            return;
        container.Add(widget);
    }
    public static bool HasChild(this Container container, Widget widget)
        => container.Children.FirstOrDefault(x => ReferenceEquals(x, widget)) != null;
}