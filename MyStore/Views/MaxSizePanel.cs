using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace MyStore.Views
{
    public class MaxSizePanel : Panel
    {
        private double _maxWidth = 0;
        private double _maxHeight = 0;

        protected override Size MeasureOverride(Size availableSize)
        {
            _maxWidth = _maxHeight = 0;

            if (Children.Count == 0)
                return new Size();

            foreach (var child in Children)
            {
                child.Measure(availableSize);

                if (child.DesiredSize.Width > _maxWidth)
                    _maxWidth = child.DesiredSize.Width;

                if (child.DesiredSize.Height > _maxHeight)
                    _maxHeight = child.DesiredSize.Height;
            }

            double itemsPerRow = _maxWidth == 0 ? 0 : Math.Floor(availableSize.Width / _maxWidth);

            var rowsCount = _maxHeight == 0 ? 0 : Math.Ceiling(Children.Count / itemsPerRow);

            return new Size(itemsPerRow * _maxWidth, rowsCount * _maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return new Size();

            double x = 0;
            double y = 0;

            foreach (var child in Children)
            {
                // If item exceeds available width => move it to the next row
                if ((x + _maxWidth) > finalSize.Width)
                {
                    x = 0;
                    y += _maxHeight;
                }

                var rect = new Rect(x, y, _maxWidth, _maxHeight);

                child.Arrange(rect);

                x += _maxWidth;
            }

            return finalSize;
        }
    }
}
