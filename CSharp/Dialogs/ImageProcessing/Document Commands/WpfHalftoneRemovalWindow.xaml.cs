using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfImagingDemo
{
    public partial class WpfHalftoneRemovalWindow : Window
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
        /// Initial value of the fourth parameter.
        /// </summary>
        WpfImageProcessingParameter _initialParameter4;

        /// <summary>
        /// Initial value of the fifth parameter.
        /// </summary>
        WpfImageProcessingParameter _initialParameter5;

        /// <summary>
        /// Initial value of the sixth parameter.
        /// </summary>
        WpfImageProcessingParameter _initialParameter6;

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        bool _isShown = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfHalftoneRemovalWindow"/> class.
        /// </summary>
        public WpfHalftoneRemovalWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            WpfImageProcessingParameter parameter1 = new WpfImageProcessingParameter("Max segment size", 2, 50, 10);
            WpfImageProcessingParameter parameter2 = new WpfImageProcessingParameter("Cell size", 10, 500, 20);
            WpfImageProcessingParameter parameter3 = new WpfImageProcessingParameter("Min halftone width", 10, 500, 25);
            WpfImageProcessingParameter parameter4 = new WpfImageProcessingParameter("Min halftone height", 10, 500, 25);
            WpfImageProcessingParameter parameter5 = new WpfImageProcessingParameter("Black pixel removal threshold", 0, 100, 33);
            WpfImageProcessingParameter parameter6 = new WpfImageProcessingParameter("White pixel removal threshold", 0, 100, 33);

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);
            Title = "Halftone removal";

            _initialParameter1 = parameter1;
            _initialParameter2 = parameter2;
            _initialParameter3 = parameter3;
            _initialParameter4 = parameter4;
            _initialParameter5 = parameter5;
            _initialParameter6 = parameter6;

            groupBox1.Header = parameter1.Name;
            slider1.Minimum = parameter1.MinValue;
            slider1.Maximum = parameter1.MaxValue;
            slider1.Value = parameter1.DefaultValue;
            numericUpDown1.Minimum = parameter1.MinValue;
            numericUpDown1.Maximum = parameter1.MaxValue;
            numericUpDown1.Value = parameter1.DefaultValue;
            minValueLabel1.Content = parameter1.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel1.Content = parameter1.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox2.Header = parameter2.Name;
            slider2.Minimum = parameter2.MinValue;
            slider2.Maximum = parameter2.MaxValue;
            slider2.Value = parameter2.DefaultValue;
            numericUpDown2.Minimum = parameter2.MinValue;
            numericUpDown2.Maximum = parameter2.MaxValue;
            numericUpDown2.Value = parameter2.DefaultValue;
            minValueLabel2.Content = parameter2.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel2.Content = parameter2.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox3.Header = parameter3.Name;
            slider3.Minimum = parameter3.MinValue;
            slider3.Maximum = parameter3.MaxValue;
            slider3.Value = parameter3.DefaultValue;
            numericUpDown3.Minimum = parameter3.MinValue;
            numericUpDown3.Maximum = parameter3.MaxValue;
            numericUpDown3.Value = parameter3.DefaultValue;
            minValueLabel3.Content = parameter3.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel3.Content = parameter3.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox4.Header = parameter4.Name;
            slider4.Minimum = parameter4.MinValue;
            slider4.Maximum = parameter4.MaxValue;
            slider4.Value = parameter4.DefaultValue;
            numericUpDown4.Minimum = parameter4.MinValue;
            numericUpDown4.Maximum = parameter4.MaxValue;
            numericUpDown4.Value = parameter4.DefaultValue;
            minValueLabel4.Content = parameter4.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel4.Content = parameter4.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox5.Header = parameter5.Name;
            slider5.Minimum = parameter5.MinValue;
            slider5.Maximum = parameter5.MaxValue;
            slider5.Value = parameter5.DefaultValue;
            numericUpDown5.Minimum = parameter5.MinValue;
            numericUpDown5.Maximum = parameter5.MaxValue;
            numericUpDown5.Value = parameter5.DefaultValue;
            minValueLabel5.Content = parameter5.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel5.Content = parameter5.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

            groupBox6.Header = parameter6.Name;
            slider6.Minimum = parameter6.MinValue;
            slider6.Maximum = parameter6.MaxValue;
            slider6.Value = parameter6.DefaultValue;
            numericUpDown6.Minimum = parameter6.MinValue;
            numericUpDown6.Maximum = parameter6.MaxValue;
            numericUpDown6.Value = parameter6.DefaultValue;
            minValueLabel6.Content = parameter6.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            maxValueLabel6.Content = parameter6.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

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

        int _parameter1;
        /// <summary>
        /// Value of the first parameter.
        /// </summary>
        public int Parameter1
        {
            get
            {
                return _parameter1;
            }
        }

        int _parameter2;
        /// <summary>
        /// Value of the second parameter.
        /// </summary>
        public int Parameter2
        {
            get
            {
                return _parameter2;
            }
        }

        int _parameter3;
        /// <summary>
        /// Value of the third parameter.
        /// </summary>
        public int Parameter3
        {
            get
            {
                return _parameter3;
            }
        }

        int _parameter4;
        /// <summary>
        /// Value of the fourth parameter.
        /// </summary>
        public int Parameter4
        {
            get
            {
                return _parameter4;
            }
        }

        int _parameter5;
        /// <summary>
        /// Value of the fifth parameter.
        /// </summary>
        public int Parameter5
        {
            get
            {
                return _parameter5;
            }
        }

        int _parameter6;
        /// <summary>
        /// Value of the sixth parameter.
        /// </summary>
        public int Parameter6
        {
            get
            {
                return _parameter6;
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
#if !REMOVE_DOCCLEANUP_PLUGIN
        /// <summary>
        /// Returns the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public virtual ProcessingCommandBase GetProcessingCommand()
        {
            HalftoneRemovalCommand command = new HalftoneRemovalCommand();
            command.MaxSegmentSize = this.Parameter1;
            command.CellSize = this.Parameter2;
            command.MinHalftoneWidth = this.Parameter3;
            command.MinHalftoneHeight = this.Parameter4;
            if (autoThresholdCheckBox.IsChecked.Value)
            {
                command.AutoThreshold = true;
            }
            else
            {
                command.AutoThreshold = false;
                command.BlackPixelRemovalThreshold = this.Parameter5;
                command.WhitePixelRemovalThreshold = this.Parameter6;
            }
            return command;
        }
#endif

        #endregion


        #region PROTECTED

        /// <summary>
        /// Executes the processing command.
        /// </summary>
        protected void ExecuteProcessing()
        {
            _parameter1 = (int)Math.Round(slider1.Value);
            _parameter2 = (int)Math.Round(slider2.Value);
            _parameter3 = (int)Math.Round(slider3.Value);
            _parameter4 = (int)Math.Round(slider4.Value);
            _parameter5 = (int)Math.Round(slider5.Value);
            _parameter6 = (int)Math.Round(slider6.Value);
#if !REMOVE_DOCCLEANUP_PLUGIN
            ProcessingCommandBase command = GetProcessingCommand();
            if (command != null)
                _imageProcessingPreviewInViewer.SetCommand(command);
#endif
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Handles the Click event of OkButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of CancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the ValueChanged event of Slider1 object.
        /// </summary>
        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider1.Value);
            if (numericUpDown1.Value != sliderValue)
                numericUpDown1.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of Slider2 object.
        /// </summary>
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider2.Value);
            if (numericUpDown2.Value != sliderValue)
                numericUpDown2.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of Slider3 object.
        /// </summary>
        private void slider3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider3.Value);
            if (numericUpDown3.Value != sliderValue)
                numericUpDown3.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of Slider4 object.
        /// </summary>
        private void slider4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider4.Value);
            if (numericUpDown4.Value != sliderValue)
                numericUpDown4.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of Slider5 object.
        /// </summary>
        private void slider5_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider5.Value);
            if (numericUpDown5.Value != sliderValue)
                numericUpDown5.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of Slider6 object.
        /// </summary>
        private void slider6_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)Math.Round(slider6.Value);
            if (numericUpDown6.Value != sliderValue)
                numericUpDown6.Value = sliderValue;
        }

        /// <summary>
        /// Handles the ValueChanged event of NumericUpDown1 object.
        /// </summary>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            slider1.Value = numericUpDown1.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of NumericUpDown2 object.
        /// </summary>
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            slider2.Value = numericUpDown2.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of NumericUpDown3 object.
        /// </summary>
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            slider3.Value = numericUpDown3.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of NumericUpDown4 object.
        /// </summary>
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            slider4.Value = numericUpDown4.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of NumericUpDown5 object.
        /// </summary>
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            slider5.Value = numericUpDown5.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the ValueChanged event of NumericUpDown6 object.
        /// </summary>
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            slider6.Value = numericUpDown6.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the Click event of ResetButton1 object.
        /// </summary>
        private void resetButton1_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown1.Value = _initialParameter1.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of ResetButton2 object.
        /// </summary>
        private void resetButton2_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown2.Value = _initialParameter2.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of ResetButton3 object.
        /// </summary>
        private void resetButton3_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown3.Value = _initialParameter3.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of ResetButton4 object.
        /// </summary>
        private void resetButton4_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown4.Value = _initialParameter4.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of ResetButton5 object.
        /// </summary>
        private void resetButton5_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown5.Value = _initialParameter5.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of ResetButton6 object.
        /// </summary>
        private void resetButton6_Click(object sender, RoutedEventArgs e)
        {
            numericUpDown6.Value = _initialParameter6.DefaultValue;
        }

        /// <summary>
        /// Handles the Click event of AutoThresholdCheckBox object.
        /// </summary>
        private void autoThresholdCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (autoThresholdCheckBox.IsChecked.Value == true)
            {
                groupBox5.IsEnabled = false;
                groupBox6.IsEnabled = false;
                previewCheckBox.IsEnabled = false;
                IsPreviewEnabled = false;
            }
            else
            {
                groupBox5.IsEnabled = true;
                groupBox6.IsEnabled = true;
                previewCheckBox.IsEnabled = true;
                IsPreviewEnabled = previewCheckBox.IsChecked.Value;
            }

            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the Click event of PreviewCheckBox object.
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
