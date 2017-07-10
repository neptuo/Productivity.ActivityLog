using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Activity.Core
{
    public class IconHelper
    {
        public static ImageSource GetIcon(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            Icon extractedIcon = Icon.ExtractAssociatedIcon(fileName);
            ImageSource imageSource;

            using (Icon i = Icon.FromHandle(extractedIcon.ToBitmap().GetHicon()))
                imageSource = Imaging.CreateBitmapSourceFromHIcon(i.Handle, new Int32Rect(0, 0, 32, 32), BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        public static Icon GetCurrentIcon(Window window)
        {
            return Icon.ExtractAssociatedIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Activity.UI.exe"));
        }
    }
}
