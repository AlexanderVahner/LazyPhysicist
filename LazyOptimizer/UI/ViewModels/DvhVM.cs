using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class DvhVM
    {
        private double width = 600;
        private double height = 400;

        private double gridDose = 100;
        private const double gridVolume = 100;

        private const double borderTop = 20;
        private const double borderLeft = 30;
        private const double borderBottom = 30;
        private const double borderRight = 25;


        private const double gridStrokeThickness = 1;
        private const double gridLinesStrokeThickness = .5;
        private const double minGridWidth = 50;
        private const double minGridHeight = 50;
        private Color gridColor = Color.FromRgb(180, 180, 180);
        private readonly DoubleCollection dashArray = new DoubleCollection() { 1, 3 };

        private const double volumeGridStep = 10;
        private const double doseGridStep = 10;

        private const double objectiveImageZoom = 15;
        private const double objectiveStrokeThickness = .7;
        private const double objectiveOpacity = .7;
        private Color objectiveStrokeColor = Color.FromRgb(0, 0, 0);
        private Color objectiveNoneStructureFill = Color.FromRgb(200, 200, 200);

        private const double volumeCaptionAlignment = 8;
        private const double volumeCaptionMarginFromGrid = 18;
        private const double doseCaptionAlignment = 6;
        private const double doseCaptionMarginFromGrid = 18;


        private readonly Canvas canvas = new Canvas();
        public Canvas Canvas { get => canvas; }
        public double GridTop => borderTop;
        public double GridLeft => borderLeft;
        public double GridBottom => height - borderBottom > borderTop ? height - borderBottom : borderTop;
        public double GridRight => width - borderLeft - borderRight > 0 ? width - borderRight : borderLeft;
        public double GridWidth => GridRight - GridLeft;
        public double GridHeight => GridBottom - GridTop;
        public double GridXMultiplier => GridWidth / gridDose;
        public double GridYMultiplier => GridHeight / gridVolume;

        public bool Frozen { get; set; } = false;


        private IEnumerable<StructureVM> _structures;
        

        public DvhVM()
        {
            canvas.SizeChanged += Canvas_SizeChanged;
        }

        public void Paint(IEnumerable<StructureVM> structures)
        {
            if (Frozen)
            {
                return;
            }

            if (structures == null || structures.Count() == 0)
            {
                gridDose = 100;
                canvas.Children.Clear();
                PaintGrid();
                return;
            }

            if (_structures != null && _structures.Count() > 0)
            {
                foreach (var s in _structures)
                {
                    s.PropertyChanged -= Structure_PropertyChanged;
                }
            }


            double maxDose = 0;
            foreach (var structure in structures)
            {
                if (structure == null || structure.Objectives.Count == 0)
                {
                    continue;
                }

                structure.PropertyChanged += Structure_PropertyChanged;

                foreach (var objective in structure.Objectives)
                {
                    maxDose = (objective.Dose ?? 0) > maxDose ? objective.Dose ?? 0 : maxDose;
                }
            }

            _structures = structures.OrderBy(s => s.SourceModel.OrderByDoseDescProperty);

            maxDose = maxDose == 0 ? 100 : maxDose;
            gridDose = (int)(maxDose / doseGridStep) * doseGridStep + doseGridStep;

            canvas.Children.Clear();
            PaintGrid();

            if (_structures != null)
            {
                PaintStructures(_structures);
            }
        }

        private void Structure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is StructureVM structure)
            {
                if (e.PropertyName == nameof(structure.PlanStructure))
                {
                    Paint(_structures);
                }
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
            Paint(_structures);
        }

        private void PaintStructures(IEnumerable<StructureVM> structures)
        {
            foreach (var structure in structures)
            {
                foreach (var objective in structure.Objectives)
                {
                    PlaceObjective(structure, objective);
                }
            }
        }

        private void PlaceObjective(StructureVM structure, ObjectiveVM objective)
        {
            var image = GetObjectiveImage(objective);
            image.Fill = new SolidColorBrush(structure.PlanStructure?.StructureInfo?.Color ?? objectiveNoneStructureFill);
            canvas.Children.Add(image);

            if (objective.ObjectiveType == ESAPIInfo.Plan.ObjectiveType.Point && (objective.Volume == 0 || objective.Volume == 100))
            {
                double x = (objective.Dose ?? 0) * GridXMultiplier + GridLeft;
                AddGridLine(x, GridTop, x, GridBottom, dashed: true);
            }
        }

        public Polygon GetObjectiveImage(ObjectiveVM objective)
        {
            double offsetX;
            double offsetY;
            Polygon objImage = new Polygon();
            var points = new PointCollection();

            if (objective != null)
            {
                offsetX = (objective.Dose * GridXMultiplier ?? 0) + GridLeft;
                offsetY = GridBottom;

                if (objective.ObjectiveType == ESAPIInfo.Plan.ObjectiveType.Point)
                {
                    offsetY = GridBottom - (objective.Volume * GridYMultiplier ?? 0);
                }

                switch (objective.ObjectiveType)
                {
                    case ESAPIInfo.Plan.ObjectiveType.Point:
                        if (objective.ObjectiveOperator == ESAPIInfo.Plan.Operator.Upper)
                        {
                            points.Add(CreatePoint(0, 0));
                            points.Add(CreatePoint(.5, -1));
                            points.Add(CreatePoint(.6, -.6));
                            points.Add(CreatePoint(1, -.5));
                        }
                        else
                        {
                            points.Add(CreatePoint(-1, .5));
                            points.Add(CreatePoint(0, 0));
                            points.Add(CreatePoint(-.5, 1));
                            points.Add(CreatePoint(-.6, .6));
                        }

                        break;

                    case ESAPIInfo.Plan.ObjectiveType.Mean:
                        points.Add(CreatePoint(0, 0));
                        points.Add(CreatePoint(.4, .4));
                        points.Add(CreatePoint(0, .8));
                        points.Add(CreatePoint(-.4, .4));
                        break;

                    case ESAPIInfo.Plan.ObjectiveType.EUD:
                        if (objective.ObjectiveOperator == ESAPIInfo.Plan.Operator.Upper)
                        {
                            points.Add(CreatePoint(0, 0));
                            points.Add(CreatePoint(1, -.5));
                            points.Add(CreatePoint(.5, 0));
                            points.Add(CreatePoint(1, .5));
                        }
                        else if (objective.ObjectiveOperator == ESAPIInfo.Plan.Operator.Exact)
                        {
                            points.Add(CreatePoint(-.5, -.5));
                            points.Add(CreatePoint(.5, .5));
                            points.Add(CreatePoint(.25, 0));
                            points.Add(CreatePoint(.5, -.5));
                            points.Add(CreatePoint(-.5, .5));
                            points.Add(CreatePoint(-.25, 0));
                        }
                        else
                        {
                            points.Add(CreatePoint(-1, -.5));
                            points.Add(CreatePoint(0, 0));
                            points.Add(CreatePoint(-1, .5));
                            points.Add(CreatePoint(-.5, 0));
                        }
                        break;

                    case ESAPIInfo.Plan.ObjectiveType.Unknown:
                        points.Add(CreatePoint(-.5, -.5));
                        points.Add(CreatePoint(.5, -.5));
                        points.Add(CreatePoint(.5, .5));
                        points.Add(CreatePoint(-.5, .5));
                        break;
                }
            }
            objImage.Points = points;
            objImage.Stroke = new SolidColorBrush(objectiveStrokeColor);
            objImage.StrokeThickness = objectiveStrokeThickness;
            objImage.Opacity = objectiveOpacity;
            return objImage;

            Point CreatePoint(double x, double y)
            {
                return new Point(x * objectiveImageZoom + offsetX, y * objectiveImageZoom + offsetY);
            }
        }

        private void PaintGrid()
        {
            var rect = new Rectangle
            {
                Width = GridWidth,
                Height = GridHeight,
                Stroke = new SolidColorBrush(gridColor),
                StrokeThickness = gridStrokeThickness
            };
            Canvas.SetTop(rect, GridTop);
            Canvas.SetLeft(rect, GridLeft);

            canvas.Children.Add(rect);

            PaintGridLines();
            PaintCaptions();

        }

        private void PaintCaptions()
        {
            for (double y = volumeGridStep; y < gridVolume; y += volumeGridStep)
            {
               AddCaption(y.ToString("F0"), borderLeft - volumeCaptionMarginFromGrid, GridBottom - y * GridYMultiplier - volumeCaptionAlignment);
            }

            for (double x = doseGridStep; x <= gridDose; x += doseGridStep)
            {
                AddCaption(x.ToString("F0"), borderLeft + x * GridXMultiplier - doseCaptionAlignment, borderTop - doseCaptionMarginFromGrid);
            }
        }

        private void AddCaption(string text, double x, double y)
        {
            var caption = new TextBlock() { Text = text };

            Canvas.SetTop(caption, y);
            Canvas.SetLeft(caption, x);

            canvas.Children.Add(caption);
        }

        private void PaintGridLines()
        {
            if (GridHeight < minGridHeight || GridWidth < minGridWidth || gridDose <= 0)
            {
                return;
            }
                        
            for (double y = volumeGridStep; y < gridVolume; y += volumeGridStep)
            {
                AddGridLine(GridLeft, GridTop + y * GridYMultiplier, GridRight, GridTop + y * GridYMultiplier);
            }

            for (double x = doseGridStep; x < gridDose; x += doseGridStep)
            {
                AddGridLine(GridLeft + x * GridXMultiplier, GridTop, GridLeft + x * GridXMultiplier, GridBottom);
            }
        }

        private void AddGridLine(double x1, double y1, double x2, double y2, bool dashed = false)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(gridColor),
                StrokeThickness = gridLinesStrokeThickness
            };

            if (dashed)
            {
                line.StrokeDashArray = dashArray;
            }

            canvas.Children.Add(line);
        }
    }
}
