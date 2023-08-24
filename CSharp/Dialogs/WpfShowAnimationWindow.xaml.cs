using System;
using System.Windows;

using Vintasoft.Imaging;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to display an animation.
    /// </summary>
    public partial class WpfShowAnimationWindow : Window
    {

        #region Constructor

        public WpfShowAnimationWindow(ImageCollection images)
        {
            InitializeComponent();

            defaultDelayNumericUpDown.Value = 2000;
            animatedImageViewer1.Images.AddRange(images.ToArray());
            animatedImageViewer1.FocusedIndex = 0;
            animatedImageViewer1.Animation = true;
            animatedImageViewer1.DefaultDelay = (int)defaultDelayNumericUpDown.Value;
            animatedImageViewer1.DisableAutoScrollToFocusedImage();


            stopButton.IsEnabled = true;
            startButton.IsEnabled = false;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the ValueChanged event of DefaultDelayNumericUpDown object.
        /// </summary>
        private void defaultDelayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            animatedImageViewer1.DefaultDelay = (int)defaultDelayNumericUpDown.Value;
        }

        /// <summary>
        /// Handles the Click event of StartButton object.
        /// </summary>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            animatedImageViewer1.Animation = true;

            stopButton.IsEnabled = true;
            startButton.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Click event of StopButton object.
        /// </summary>
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            animatedImageViewer1.Animation = false;

            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Closed event of Window object.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            animatedImageViewer1.Animation = false;
        }

        #endregion

    }
}
