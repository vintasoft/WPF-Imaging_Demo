using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    /// <summary>
    /// Config form for image processing function with three parameters.
    /// </summary>
    public partial class WpfCannyEdgeDetectorWindow : Window
    {

        #region Fields

        /// <summary>
        /// Initial value of the first parameter.
        /// </summary>
        WpfImageProcessingParameter _initialParameter1;

        /// <summary>
        /// Initial value of the second parameter.
        /// </summary>
        WpfImageProcessingParameter _initialParameter2;

        /// <summary>
        /// Initial value of the third parameter.
        /// </summary>
        WpfImageProcessingParameter _initialParameter3;

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        bool _isShown = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfThreeParamsConfigWindow"/> class.
        /// </summary>
        public WpfCannyEdgeDetectorWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);

            _initialParameter1 = new WpfImageProcessingParameter("Blur radius", 1, 15, 3);
            _initialParameter2 = new WpfImageProcessingParameter("High threshold", 0, 255, 80);
            _initialParameter3 = new WpfImageProcessingParameter("Low threshold", 0, 255, 20);

            groupBox1.Header = _initialParameter1.Name;
            slider1.Minimum = _initialParameter1.MinValue;
            slider1.Maximum = _initialParameter1.MaxValue;
            slider1.Value = _initialParameter1.DefaultValue;
            numericUpDown1.Minimum = _initialParameter1.MinValue;
            numericUpDown1.Maximum = _initialParameter1.MaxValue;
            numericUpDown1.Value = _initialParameter1.DefaultValue;
            minValueLabel1.Content = _initialParameter1.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel1.Content = _initialParameter1.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox2.Header = _initialParameter2.Name;
            slider2.Minimum = _initialParameter2.MinValue;
            slider2.Maximum = _initialParameter2.MaxValue;
            slider2.Value = _initialParameter2.DefaultValue;
            numericUpDown2.Minimum = _initialParameter2.MinValue;
            numericUpDown2.Maximum = _initialParameter2.MaxValue;
            numericUpDown2.Value = _initialParameter2.DefaultValue;
            minValueLabel2.Content = _initialParameter2.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel2.Content = _initialParameter2.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox3.Header = _initialParameter3.Name;
            slider3.Minimum = _initialParameter3.MinValue;
            slider3.Maximum = _initialParameter3.MaxValue;
            slider3.Value = _initialParameter3.DefaultValue;
            numericUpDown3.Minimum = _initialParameter3.MinValue;
            numericUpDown3.Maximum = _initialParameter3.MaxValue;
            numericUpDown3.Value = _initialParameter3.DefaultValue;
            minValueLabel3.Content = _initialParameter3.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel3.Content = _initialParameter3.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            slider2_ValueChanged(slider2, null);
            previewCheckBox.IsChecked = IsPreviewEnabled;
        }

        #endregion



        #region Properties

        bool _isPreviewEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether the image preview in image viewer is enabled.
        /// </summary>
        [Browsable(false)]
        public bool IsPreviewEnabled
        {
            get
            {
                return _isPreviewEnabled;
            }
            set
            {
                if (IsPreviewEnabled != value)
                {
                    if (_isPreviewEnabled != value)
                    {
                        _isPreviewEnabled = value;
                        if (_isShown)
                        {
                            if (_isPreviewEnabled)
                            {
                                _imageProcessingPreviewInViewer.StartPreview();
                                ExecuteProcessing();
                            }
                            else
                            {
                                _imageProcessingPreviewInViewer.StopPreview();
                            }
                        }
                    }

                    previewCheckBox.IsChecked = value;
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whether the image processing must be previewed
        /// with zoom from image viewer.
        /// </summary>
        [Browsable(false)]
        public bool UseCurrentViewerZoomWhenPreviewProcessing
        {
            get
            {
                return _imageProcessingPreviewInViewer.UseCurrentViewerZoomWhenPreviewProcessing;
            }
            set
            {
                _imageProcessingPreviewInViewer.UseCurrentViewerZoomWhenPreviewProcessing = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the processing command need to
        /// convert the processing image to the nearest pixel format without color loss
        /// if processing command does not support pixel format
        /// of the processing image.
        /// </summary>
        [Browsable(false)]
        public bool ExpandSupportedPixelFormats
        {
            get
            {
                return _imageProcessingPreviewInViewer.ExpandSupportedPixelFormats;
            }
            set
            {
                _imageProcessingPreviewInViewer.ExpandSupportedPixelFormats = value;
            }
        }

        int _blurRadius;
        /// <summary>
        /// Gets the blur radius.
        /// </summary>
        public int BlurRadius
        {
            get
            {
                return _blurRadius;
            }
        }

        byte _highThreshold;
        /// <summary>
        /// Gets the high threshold.
        /// </summary>
        public byte HighThreshold
        {
            get
            {
                return _highThreshold;
            }
        }

        byte _lowThreshold;
        /// <summary>
        /// Gets the low threshold.
        /// </summary>
        public byte LowThreshold
        {
            get
            {
                return _lowThreshold;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Shows the processing dialog.
        /// </summary>
        /// <returns>
        /// <b>true</b> if form is closed and OK button is pressed;
        /// <b>false</b> if form is closed and not OK button is pressed.</returns>
        public bool ShowProcessingDialog()
        {
            try
            {
                if (IsPreviewEnabled)
                {
                    _imageProcessingPreviewInViewer.StartPreview();
                    ExecuteProcessing();
                }
                _isShown = true;
                if (ShowDialog() == true)
                    return true;
                else
                    return false;
            }
            catch (ImageProcessingException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            finally
            {
                if (IsPreviewEnabled)
                {
                    if (IsPreviewEnabled)
                        _imageProcessingPreviewInViewer.StopPreview();
                    _isShown = false;
                }
            }
        }

        /// <summary>
        /// Returns the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public ProcessingCommandBase GetProcessingCommand()
        {
            if (HighThreshold < LowThreshold)
                return new CannyEdgeDetectorCommand(BlurRadius, HighThreshold, HighThreshold);
            else
                return new CannyEdgeDetectorCommand(BlurRadius, HighThreshold, LowThreshold);
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Executes the processing command.
        /// </summary>
        protected void ExecuteProcessing()
        {
            _blurRadius = (int)Math.Round(slider1.Value);
            _highThreshold = (byte)Math.Round(slider2.Value);
            _lowThreshold = (byte)Math.Round(slider3.Value);
            ProcessingCommandBase command = GetProcessingCommand();
            if (command != null)
                _imageProcessingPreviewInViewer.SetCommand(command);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the ValueChanged event of slider1 object.
        /// </summary>
        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider1.Value);
            if (numericUpDown1.Value != sliderValue)
                numericUpDown1.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of slider2 object.
        /// </summary>
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider2.Value);
            if (numericUpDown2.Value != sliderValue)
                numericUpDown2.Value = sliderValue;

            slider3.Maximum = sliderValue;
            numericUpDown3.Maximum = sliderValue;
            maxValueLabel3.Content = sliderValue.ToString();
        }

        /// <summary>
        /// Handles the ValueChanged event of slider3 object.
        /// </summary>
        private void slider3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider3.Value);
            if (numericUpDown3.Value != sliderValue)
                numericUpDown3.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of numericUpDown1 object.
        /// </summary>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            slider1.Value = numericUpDown1.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of numericUpDown2 object.
        /// </summary>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            slider2.Value = numericUpDown2.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of numericUpDown3 object.
        /// </summary>
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            slider3.Value = numericUpDown3.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the Click event of resetButton1 object.
        /// </summary>
        private void resetButton1_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown1.Value = _initialParameter1.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of resetButton2 object.
        /// </summary>
        private void resetButton2_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown2.Value = _initialParameter2.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of resetButton3 object.
        /// </summary>
        private void resetButton3_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown3.Value = _initialParameter3.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of previewCheckBox object.
        /// </summary>
        private void previewCheckBox_Click(object sender, RoutedEventArgs e)
        {
            IsPreviewEnabled = previewCheckBox.IsChecked.Value == true;
            if (IsPreviewEnabled)
                previewCheckBox.Foreground = new SolidColorBrush(Colors.Black);
            else
                previewCheckBox.Foreground = new SolidColorBrush(Colors.Green);
        }

        #endregion

        #endregion

    }
}
