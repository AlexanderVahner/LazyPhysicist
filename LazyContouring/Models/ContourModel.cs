using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LazyContouring.Models
{
    public sealed class ContourModel
    {
        private readonly StructureVariable structure;
        private readonly ImageModel image;

        private PathFigure[] segments;
        private readonly Path path;
        private readonly PathGeometry geometry = new PathGeometry();
        private const double defaultThickness = 1;
        private const double selectedThickness = 2;
        private int currentPlaneIndex;

        public ContourModel(StructureVariable structure, ImageModel image)
        {
            this.structure = structure;
            this.image = image;

            path = new Path
            {
                Data = geometry,
                Stroke = new SolidColorBrush(structure.Color),
                StrokeThickness = defaultThickness
            };

            structure.PropertyChanged += Structure_PropertyChanged;
        }

        private void Structure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(structure.SegmentVolume):
                    Repaint();
                    break;
                case nameof(structure.Color):
                    path.Stroke = new SolidColorBrush(structure.Color);
                    break;
                case nameof(structure.IsSelected):
                    path.StrokeThickness = structure.IsSelected ? selectedThickness : defaultThickness;
                    break;
                case nameof(structure.IsVisible):
                    Repaint();
                    break;
            }
        }

        public void Repaint()
        {
            geometry.Figures.Clear();

            if (!structure.IsVisible || structure.IsEmpty)
            {
                return;
            }

            CheckSegments(currentPlaneIndex);

            if (segments == null)
            {
                return;
            }

            foreach (var segment in segments)
            {
                geometry.Figures.Add(segment);
            }
        }

        private void CheckSegments(int planeIndex)
        {
            var contour = structure.GetContoursOnImagePlane(planeIndex);

            if (contour.Count() == 0)
            {
                segments = null;
                return;
            }

            segments = new PathFigure[contour.Count()];

            for (int s = 0; s < contour.Count(); s++)
            {
                segments[s] = new PathFigure() { IsClosed = true };

                bool start = true;
                foreach (var vector in contour[s])
                {
                    var x = (vector.x - image.Origin.x) / image.XRes;
                    var y = (vector.y - image.Origin.y) / image.YRes;
                    var newPoint = new Point(x, y);

                    if (start)
                    {
                        segments[s].StartPoint = newPoint;
                        start = false;
                    }
                    else
                    {
                        segments[s].Segments.Add(new LineSegment { Point = newPoint });
                    }
                }
            }
        }

        private void SetPlane(int index)
        {
            currentPlaneIndex = index;
            Repaint();
        }

        public StructureVariable Structure => structure;
        public Path Path => path;
        public int CurrentPlaneIndex { get => currentPlaneIndex; set => SetPlane(value); }
    }
}
