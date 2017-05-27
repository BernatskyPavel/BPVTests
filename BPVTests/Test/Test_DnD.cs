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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BPVTests
{
    partial class Test
    {
        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("UIElement"))
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception)
            { }
        }
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("UIElement"))
                {
                    UIElement droptarget = e.Source as UIElement;
                    int droptargetIndex = -1, i = 0;
                    foreach (UIElement element in this.TestAnswers.Children)
                    {
                        if (element.Equals(droptarget))
                        {
                            droptargetIndex = i;
                            break;
                        }
                        i++;
                    }
                    if (droptargetIndex != -1)
                    {
                        this.TestAnswers.Children.Remove(_realDragSource);
                        this.TestAnswers.Children.Insert(droptargetIndex, _realDragSource);
                    }
                    _isDown = false;
                    _isDragging = false;
                    _realDragSource.ReleaseMouseCapture();
                }
            }
            catch (Exception)
            { }
        }
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.Source == this.TestAnswers)
                {
                }
                else
                {
                    _isDown = true;
                    _startPoint = e.GetPosition(this.TestAnswers);
                }
            }
            catch (Exception)
            { }
        }
        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
            }
            catch (Exception)
            { }
        }
        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_isDown)
                {
                    if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.TestAnswers).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.TestAnswers).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    {
                        _isDragging = true;
                        _realDragSource = e.Source as UIElement;
                        _realDragSource.CaptureMouse();
                        DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}

