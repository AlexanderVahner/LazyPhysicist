using System.Windows.Media;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Graphics
{
    public class StructureStorage
    {
        private readonly ContourStorage contourStorage;
        private readonly Structure structure;
        private Path path;
        private PathGeometry geometry;
        private Color color;
        private Brush strokeBrush;
        private double strokeThickness = 1;

        public StructureStorage(ContourStorage contourStorage)
        {
            this.contourStorage = contourStorage;
            structure = contourStorage.Structure;
            Color = structure.Color;
        }

        public PathFigure[] GetSegments(int planeIndex)
        {
            return Visible ? contourStorage.GetSegments(planeIndex) : null;
        }

        public void RepaintPath(int planeIndex)
        {
            Geometry.Figures.Clear();

            if (!Visible || structure.IsEmpty)
            {
                return;
            }

            Path.Stroke = StrokeBrush;
            Path.StrokeThickness = StrokeThickness;
            //Path.Fill = StrokeBrush;

            var segments = GetSegments(planeIndex);

            if (segments == null)
            {
                return;
            }

            foreach (var segment in segments)
            {
                geometry.Figures.Add(segment);
            }
        }

        public bool Visible { get; set; } = true;
        public Structure Structure => structure;
        public Path Path => path ?? (path = new Path() { Data = Geometry });
        public PathGeometry Geometry => geometry ?? (geometry = new PathGeometry());
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                StrokeBrush = new SolidColorBrush(color);
            }
        }
        public Brush StrokeBrush
        {
            get => strokeBrush;
            set
            {
                strokeBrush = value;
            }
        }
        public double StrokeThickness
        {
            get => strokeThickness;
            set
            {
                strokeThickness = value;
            }
        }

    }
}
