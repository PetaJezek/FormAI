using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace FormAI
{
    public class BaseWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the BaseWindow class with custom styling.
        /// Sets the window to be borderless, transparent, and centered on the screen.
        /// </summary>
        public BaseWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = System.Windows.Media.Brushes.Transparent;
            ResizeMode = ResizeMode.CanResizeWithGrip;
            UseLayoutRounding = true;
            SnapsToDevicePixels = true;

            Loaded += (_, __) => UpdateMaximizeButton();
        }

        /// <summary>
        /// Handles mouse-down events on the title bar to enable window dragging and double-click to maximize/restore.
        /// </summary>
        protected virtual void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    ToggleMaximize();
                    return;
                }

                SafeDragMove();
            }
        }

        /// <summary>
        /// Handles the click event for the minimize button, minimizing the window.
        /// </summary>
        protected virtual void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the click event for the maximize button, toggling the window between maximized and normal states.
        /// </summary>
        protected virtual void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        /// <summary>
        /// Handles the click event for the close button, closing the window.
        /// </summary>
        protected virtual void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Overrides the base method to update the maximize button's icon whenever the window state changes.
        /// </summary>
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            UpdateMaximizeButton();
        }

        /// <summary>
        /// Updates the content of the "MaximizeButton" to reflect the current window state (Maximize or Restore icon).
        /// </summary>
        protected void UpdateMaximizeButton()
        {
            if (FindName("MaximizeButton") is Button maximizeButton)
            {
                maximizeButton.Content = WindowState == WindowState.Maximized ? "\uE923" : "\uE922";
            }
        }

        /// <summary>
        /// A wrapper for the DragMove method that safely handles potential InvalidOperationException.
        /// </summary>
        protected void SafeDragMove()
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
            catch (InvalidOperationException)
            {
                
            }
        }

        /// <summary>
        /// Toggles the window's state between Maximized and Normal.
        /// </summary>
        protected void ToggleMaximize()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}