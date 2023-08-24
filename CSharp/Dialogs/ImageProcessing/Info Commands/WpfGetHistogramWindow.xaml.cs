using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Info;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view an image histogram.
    /// </summary>
    public partial class WpfGetHistogramWindow : Window
    {

        #region Constants

        const int HISTOGRAM_IMAGE_WIDTH = 600;
        const int HISTOGRAM_IMAGE_HEIGHT = 300;
        const int HISTOGRAM_IMAGE_QUIET_ZONE_SIZE = 10;

        const int HISTOGRAM_DATA_SIZE = 255;

        #endregion



        #region Fields

        VintasoftImage _image;
        System.Drawing.Rectangle _imageRegion;

        ImageSource _histogramImageSource;
        HistogramType _histogramType = HistogramType.Luminosity;
        int[] _histogramData;

        int _pixelCount;

        Color _histogramColor = Colors.Red;
        Color _histogramBackground = Colors.White;

        /// <summary>
        /// The value indicating whether the processing command need to
        /// convert the processing image to the nearest pixel format without color loss
        /// if processing command does not support pixel format
        /// of the processing image.
        /// </summary>
        bool _needConvertToSupportedPixelFormat = false;

        #endregion



        #region Constructor

        public WpfGetHistogramWindow(
            VintasoftImage image,
            System.Drawing.Rectangle imageRegion,
            bool needConvertToSupportedPixelFormat)
        {
            InitializeComponent();

            levelLabel.Content = "";
            countLabel.Content = "";
            percentageLabel.Content = "";

            _needConvertToSupportedPixelFormat = needConvertToSupportedPixelFormat;
            _image = image;
            _imageRegion = imageRegion;
            if (imageRegion.IsEmpty)
                _pixelCount = image.Width * image.Height;
            else
                _pixelCount = imageRegion.Width * imageRegion.Height;

            _histogramImageSource = GetHistogramImage(_image, imageRegion, _histogramType);
            histogramImage.Source = _histogramImageSource;
            histogramImageBorder.Background = new SolidColorBrush(_histogramBackground);

            histogramTypeComboBox.SelectedIndex = 0;
            pixelCountLabel.Content = string.Format("Pixels: {0}", _pixelCount);
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of CloseButton object.
        /// </summary>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Returns an image with histogram.
        /// </summary>
        /// <param name="image">Source image.</param>
        /// <param name="imageRegion">The image region, which should be processed.</param>
        /// <param name="histogramType">Histogram type.</param>
        /// <returns>Image with histogram.</returns>
        private ImageSource GetHistogramImage(
            VintasoftImage image,
            System.Drawing.Rectangle imageRegion,
            HistogramType histogramType)
        {
            // get histogram data

            GetHistogramCommand command = new GetHistogramCommand(histogramType);
            command.RegionOfInterest = new RegionOfInterest(imageRegion);
            command.ExpandSupportedPixelFormats = _needConvertToSupportedPixelFormat;
            command.Results = new ProcessingCommandResults();
            command.ExecuteInPlace(image);

            GetHistogramCommandResult getHistogramCommandResult = (GetHistogramCommandResult)command.Results[0];
            _histogramData = getHistogramCommandResult.HistogramData;


            // get the maximum value of histogram
            int histogramMaxValue = 0;
            for (int i = 0; i < 256; i++)
            {
                if (histogramMaxValue < _histogramData[i])
                    histogramMaxValue = _histogramData[i];
            }


            // create an image with histogram

            GeometryDrawing histogramImage = new GeometryDrawing();

            float histogramSizeX = HISTOGRAM_IMAGE_WIDTH - 2 * HISTOGRAM_IMAGE_QUIET_ZONE_SIZE;
            float histogramSizeY = HISTOGRAM_IMAGE_HEIGHT - 2 * HISTOGRAM_IMAGE_QUIET_ZONE_SIZE;

            float histogramPenWidth = histogramSizeX / HISTOGRAM_DATA_SIZE;
            Pen histogramPen = new Pen(new SolidColorBrush(_histogramColor), histogramPenWidth);
            Brush backgroundBrush = new SolidColorBrush(_histogramBackground);

            GeometryGroup histogramLines = new GeometryGroup();

            float x1, y1, x2, y2;
            for (int i = 0; i < 256; i++)
            {
                int v = _histogramData[i];
                x1 = HISTOGRAM_IMAGE_QUIET_ZONE_SIZE + histogramPenWidth * i - histogramPenWidth / 2;
                y1 = HISTOGRAM_IMAGE_HEIGHT - HISTOGRAM_IMAGE_QUIET_ZONE_SIZE;
                x2 = x1;
                y2 = y1 - v * (histogramSizeY / histogramMaxValue);
                if (y2 == y1)
                    y2 = y1 - 1.0f;
                histogramLines.Children.Add(new LineGeometry(new Point(x1, y1), new Point(x2, y2)));
            }

            histogramImage.Geometry = histogramLines;
            histogramImage.Brush = backgroundBrush;
            histogramImage.Pen = histogramPen;

            // return the image with histogram
            return new DrawingImage(histogramImage);
        }

        /// <summary>
        /// Handles the MouseMove event of HistogramImage object.
        /// </summary>
        private void histogramImage_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(histogramImage).X;
            float oneElementWidth = (float)HISTOGRAM_IMAGE_WIDTH / HISTOGRAM_DATA_SIZE;
            int histogramIndex = (int)Math.Round(x / oneElementWidth);
            if (histogramIndex >= 0 && histogramIndex <= 255)
            {
                levelLabel.Content = histogramIndex;
                countLabel.Content = _histogramData[histogramIndex];
                percentageLabel.Content = string.Format("{0:F2}%", (float)_histogramData[histogramIndex] / _pixelCount * 100);
            }
            else
            {
                levelLabel.Content = "";
                countLabel.Content = "";
                percentageLabel.Content = "";
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of HistogramImage object.
        /// </summary>
        private void histogramImage_MouseLeave(object sender, MouseEventArgs e)
        {
            levelLabel.Content = "";
            countLabel.Content = "";
            percentageLabel.Content = "";
        }

        /// <summary>
        /// Handles the SelectionChanged event of HistogramTypeComboBox object.
        /// </summary>
        private void histogramTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HistogramType histogramType = HistogramType.Luminosity;
            switch (histogramTypeComboBox.SelectedIndex)
            {
                case 1:
                    histogramType = HistogramType.Red;
                    break;

                case 2:
                    histogramType = HistogramType.Green;
                    break;

                case 3:
                    histogramType = HistogramType.Blue;
                    break;
            }

            if (_histogramType != histogramType)
            {
                _histogramType = histogramType;

                _histogramImageSource = GetHistogramImage(_image, _imageRegion, _histogramType);
                histogramImage.Source = _histogramImageSource;
            }
        }

        #endregion

    }
}
