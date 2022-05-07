using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TerminalsManagerUI.Behaviors
{
    public class ScrollViewMousePan : Behavior<ScrollViewer>
    {
        private Point elementStartPosition;
        private Point mouseStartPosition;
        private TranslateTransform transform = new TranslateTransform();


        private UIElement content;
        private Point scrollMousePoint;
        private double scrollHorizontalOffset;
        private double scrollVerticalOffset;


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            content = (UIElement)AssociatedObject.Content;
            content.MouseLeftButtonDown += OnMouseLeftButtonDown;
            content.MouseMove += OnMouseMove;
            content.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            content.CaptureMouse();
            AssociatedObject.Cursor = Cursors.Hand;
            scrollMousePoint = e.GetPosition(AssociatedObject);
            scrollHorizontalOffset = AssociatedObject.HorizontalOffset;
            scrollVerticalOffset = AssociatedObject.VerticalOffset;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (content.IsMouseCaptured)
            {
                var newVerticalOffset = scrollVerticalOffset + (scrollMousePoint.Y - e.GetPosition(AssociatedObject).Y);
                var newHorizontalOffset = scrollHorizontalOffset + (scrollMousePoint.X - e.GetPosition(AssociatedObject).X);

                AssociatedObject.ScrollToVerticalOffset(newVerticalOffset);
                AssociatedObject.ScrollToHorizontalOffset(newHorizontalOffset);
            }
        }

        private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            content.ReleaseMouseCapture();
            AssociatedObject.Cursor = Cursors.Arrow;
        }

        /*
        protected override void OnAttached()
        {
            
            Window parent = Application.Current.MainWindow;
            AssociatedObject.RenderTransform = transform;
            AssociatedObject.MouseLeftButtonDown += (sender, e) =>
            {

                elementStartPosition = AssociatedObject.TranslatePoint(new Point(), parent);
                mouseStartPosition = e.GetPosition(AssociatedObject);
                AssociatedObject.CaptureMouse();
            };

            AssociatedObject.MouseLeftButtonUp += (sender, e) =>
            {
                AssociatedObject.ReleaseMouseCapture();
            };

            AssociatedObject.MouseMove += (sender, e) =>
            {
                Vector diff = e.GetPosition(parent) - mouseStartPosition;
                if (AssociatedObject.IsMouseCaptured)
                {
                    transform.X = diff.X - parent.Left;
                    transform.Y = diff.Y - parent.Top;
                }    
            };
        }        
        */
    }
}
