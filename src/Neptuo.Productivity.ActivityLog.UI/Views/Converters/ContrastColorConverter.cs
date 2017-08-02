using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.Views.Converters
{
    public class ContrastColorConverter : IValueConverter
    {
        public bool IsTargetBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TryGetValue(value, out Color color))
            {
                int grayScale = (color.R + color.G + color.B) / 3;

                if (grayScale >= (Byte.MaxValue * 2 / 3))
                    return Return(Colors.Black);

                return Return(Colors.White);
            }

            return Return(Colors.White);
        }

        private bool TryGetValue(object value, out Color color)
        {
            Color? c = value as Color?;
            if (c == null)
            {
                SolidColorBrush brush = value as SolidColorBrush;
                if (brush == null)
                {
                    color = Colors.White;
                    return false;
                }

                c = brush.Color;
            }

            color = c.Value;
            return true;
        }

        private object Return(Color color)
        {
            if (IsTargetBrush)
                return new SolidColorBrush(color);

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
