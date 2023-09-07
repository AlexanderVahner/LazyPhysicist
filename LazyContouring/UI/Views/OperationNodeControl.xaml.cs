using LazyContouring.Models;
using LazyContouring.Operations;
using LazyContouring.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LazyContouring.UI.Views
{
    /// <summary>
    /// Interaction logic for OperationNodeControl.xaml
    /// </summary>
    public partial class OperationNodeControl : UserControl
    {
        private const int arrowsMarginY = 20;
        private const double insertPlaceSize = 30;
        private const double insertPlaceFromRightPosition = 30;
        private readonly Brush arrowsBrush = new SolidColorBrush(Colors.Black);
        private const double arrowsThickness = 1.0;

        private OperationNodeVM vM;

        public OperationNodeControl()
        {
            InitializeComponent();
        }

        public OperationNodeVM VM
        {
            get => vM;
            set
            {
                vM = value;
                vM.PropertyChanged += VM_PropertyChanged;
                DataContext = vM;
                MainBorder.Child = vM.OperationVM.UIElement;
                LeftBorder.Child = CreateNodeUIElement(vM.NodeLeftVM);
                RightBorder.Child = CreateNodeUIElement(vM.NodeRightVM);
                DrawGrid();
            }
        }


        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(vM.OperationVM):
                    MainBorder.Child = vM.OperationVM.UIElement;
                    DrawGrid();
                    break;
                case nameof(vM.NodeLeftVM):
                    LeftBorder.Child = CreateNodeUIElement(vM.NodeLeftVM);
                    break;
                case nameof(vM.NodeRightVM):
                    RightBorder.Child = CreateNodeUIElement(vM.NodeRightVM);
                    break;
            }
        }

        public UIElement CreateNodeUIElement(OperationNodeVM vm)
        {
            if (vm == null)
            {
                return null;
            }
            return new OperationNodeControl { VM = vm };
        }

        private void DrawGrid()
        {
            LeftDropCanvas.Visibility = vM.LeftNodeNedded ? Visibility.Visible : Visibility.Hidden;
            RightDropCanvas.Visibility = vM.RightNodeNedded ? Visibility.Visible : Visibility.Hidden;

            MainGrid.ColumnDefinitions[1].Width = vM.LeftNodeNedded || vM.RightNodeNedded ? new GridLength(insertPlaceSize, GridUnitType.Pixel) : new GridLength(0); 
            MainGrid.ColumnDefinitions[2].Width = vM.LeftNodeNedded || vM.RightNodeNedded ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);

            MainGrid.RowDefinitions[1].Height = vM.RightNodeNedded ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
        }

        private void InsertionDrop(object sender, DragEventArgs e)
        {
            NodeDirection direction = (NodeDirection)((FrameworkElement)sender).Tag;
            VM.InsertDrop(e.Data, direction);
        }

        private void ReplaceDrop(object sender, DragEventArgs e)
        {
            VM.ReplaceDrop(e.Data);
        }

        private void NodeArrows_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Equals(e.NewSize.Width, double.NaN) || Equals(e.NewSize.Height, double.NaN))
            {
                return;
            }

            NodeDirection direction = (NodeDirection)(((FrameworkElement)sender).Tag/* ?? NodeDirection.Left*/);
            var canvas = direction == NodeDirection.Left ? LeftArrowsCanvas : RightArrowsCanvas;

            canvas.Children.Clear();

            if (direction == NodeDirection.Left && !VM.LeftNodeNedded)
            {
                return;
            }

            if (direction == NodeDirection.Right && !VM.RightNodeNedded)
            {
                return;
            }

            var directNode = direction == NodeDirection.Left ? VM.Node.NodeLeft : VM.Node.NodeRight;

            double x1 = direction == NodeDirection.Left ? 0 : (e.NewSize.Width - insertPlaceFromRightPosition) / 2;
            double x2 = directNode != null ? e.NewSize.Width : e.NewSize.Width - insertPlaceFromRightPosition / 2;

            canvas.Children.Add(new Line
            {
                X1 = x1,
                Y1 = arrowsMarginY,
                X2 = x2,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            });

            if (VM.RightNodeNedded)
            {
                x1 = (e.NewSize.Width  - insertPlaceFromRightPosition) / 2;
                double y1 = direction == NodeDirection.Left ? arrowsMarginY : 0;
                double y2 = direction == NodeDirection.Left ? e.NewSize.Height : arrowsMarginY;

                canvas.Children.Add(new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x1,
                    Y2 = y2,
                    Stroke = arrowsBrush,
                    StrokeThickness = arrowsThickness
                });
            }
        }
    }
}
