using System;
using Gdk;
using Gtk;

namespace RandoPi.Desktop;

public static class ImageHelper
{
    /// <summary>
    /// Scales the given image to best fit into the drawing area while keeping its original aspect ratio 
    /// </summary>
    /// <param name="area">The area for the maximium size</param>
    /// <param name="frame">The image to scale</param>
    /// <returns>Scaled image, x-offset and y-offset for centering</returns>
    public static Tuple<Pixbuf, int, int> ScaleFrameToArea(DrawingArea area, Pixbuf frame)
    {
        try
        {
            var maxWidth = Convert.ToDouble(area.AllocatedWidth);
            var maxHeight = Convert.ToDouble(area.AllocatedHeight);

            var imgWidth = Convert.ToDouble(frame.Width);
            var imgHeight = Convert.ToDouble(frame.Height);

            var maxRatio = maxWidth / maxHeight;
            var ratio = imgWidth / imgHeight;

            var offsetX = 0;
            var offsetY = 0;

            int width;
            int height;
            if (maxRatio > ratio)
            {
                width = Convert.ToInt32(imgWidth * (maxHeight / imgHeight));
                height = Convert.ToInt32(maxHeight);
                offsetX = Convert.ToInt32((maxWidth - width) / 2);
            }
            else
            {
                width = Convert.ToInt32(maxWidth);
                height = Convert.ToInt32(imgHeight * (maxWidth / imgWidth));
                offsetY = Convert.ToInt32((maxHeight - height) / 2);
            }
            
            var scaledFrame = frame.ScaleSimple(width, height, InterpType.Hyper);
            return new Tuple<Pixbuf, int, int>(scaledFrame, offsetX, offsetY);
        }
        catch (Exception)
        {
            return new Tuple<Pixbuf, int, int>(frame, 0, 0);
        }
    }
}