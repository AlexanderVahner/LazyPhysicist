using System.Linq;
using System.Windows;
using System.Windows.Media;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Graphics
{
    public sealed class ContourSlice
    {
        private readonly PathFigure[] segments;

        public ContourSlice(Structure structure, Image image, int planeIndex)
        {
            var contour = structure.GetContoursOnImagePlane(planeIndex);

            if (contour.Count() == 0)
            {
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

        public PathFigure[] Segments { get => segments; }
    }
}

