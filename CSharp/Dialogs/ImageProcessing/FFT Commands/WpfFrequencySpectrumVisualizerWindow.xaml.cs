using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Fft;
using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;

namespace WpfImagingDemo
{
    /// <summary>
    /// Interaction logic for FrequencySpectrumVisualizerWindow.xaml
    /// </summary>
    public partial class WpfFrequencySpectrumVisualizerWindow : Window
    {

        #region Fields

        /// <summary>
        /// Visualization type.
        /// </summary>
        FrequencySpectrumVisualizationType _visualizationType;

        /// <summary>
        /// Use grayscale visualization.
        /// </summary>
        bool _grayscaleVisualiation;

        /// <summary>
        /// Use normalization.
        /// </summary>
        bool _normalization;

        /// <summary>
        /// Use absolute values.
        /// </summary>
        bool _absoluteValues;

        /// <summary>
        /// Size of the image spectrum.
        /// </summary>
        int _spectrumSize;

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        /// <summary>
        /// Indicates that the window is shown.
        /// </summary>
        bool _isShown = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfFrequencySpectrumVisualizerWindow"/> class.
        /// </summary>
        public WpfFrequencySpectrumVisualizerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrequencySpectrumVisualizerWindow"/> class.
        /// </summary>
        /// <param name="viewer">Image viewer.</param>
        public WpfFrequencySpectrumVisualizerWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);

            // preview check box
            previewCheckBox.IsChecked = false;

            visualizationTypeComboBox.Items.Add(FrequencySpectrumVisualizationType.Real);
            visualizationTypeComboBox.Items.Add(FrequencySpectrumVisualizationType.Imaginary);
            visualizationTypeComboBox.Items.Add(FrequencySpectrumVisualizationType.Magnitude);
            visualizationTypeComboBox.Items.Add(FrequencySpectrumVisualizationType.SquareRootMagnitude);
            visualizationTypeComboBox.Items.Add(FrequencySpectrumVisualizationType.LogarithmMagnitude);

            InitializeDefaultValues();
        }

        #endregion



        #region Properties

        bool _isPreviewEnabled = false;
        /// <summary>
        /// Gets or sets a value indicating whether the preview in ImageViewer is enabled.
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
        /// Gets or sets a value indicating whether the image with current viewer zoom must be used for previewing of image processing.
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

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Show processing dialog.
        /// </summary>
        /// <returns>
        /// <b>true</b> if form is closed and OK button is pressed;
        /// <b>false</b> if form is closed and not OK button is pressed.
        /// </returns>
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
                return ShowDialog().Value;
            }
            catch (ImageProcessingException ex)
            {
                DemosTools.ShowErrorMessage(ex);
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
        public Vintasoft.Imaging.ImageProcessing.ProcessingCommandBase GetProcessingCommand()
        {
            FrequencySpectrumVisualizerCommand command = new FrequencySpectrumVisualizerCommand();
            command.VisualizationType = _visualizationType;
            command.GrayscaleVisualization = _grayscaleVisualiation;
            command.UseNormalization = _normalization;
            command.UseAbsoluteValues = _absoluteValues;
            if (fixedSpectrumSizeCheckBox.IsChecked.Value)
                command.SpectrumImageSize = _spectrumSize;
            else
                command.SpectrumImageSize = 0;
            return command;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Initializes default values.
        /// </summary>
        private void InitializeDefaultValues()
        {
            FrequencySpectrumVisualizerCommand command = new FrequencySpectrumVisualizerCommand();

            _absoluteValues = command.UseAbsoluteValues;
            _grayscaleVisualiation = command.GrayscaleVisualization;
            _normalization = command.UseNormalization;
            _visualizationType = command.VisualizationType;
            _spectrumSize = command.SpectrumImageSize;

            if (_spectrumSize == 0)
                fixedSpectrumSizeCheckBox.IsChecked = false;
            else
                fixedSpectrumSizeCheckBox.IsChecked = true;

            spectrumSizeNumericUpDown.Value = _spectrumSize;
            visualizationTypeComboBox.SelectedItem = _visualizationType;
            grayscaleVisualizationCheckBox.IsChecked = _grayscaleVisualiation;
            normalizationCheckBox.IsChecked = _normalization;
            absoluteValuesCheckBox.IsChecked = _absoluteValues;
        }

        /// <summary>
        /// Executes the processing command.
        /// </summary>
        private void ExecuteProcessing()
        {
            if (!IsInitialized)
                return;

            ProcessingCommandBase command = GetProcessingCommand();
            _imageProcessingPreviewInViewer.SetCommand(command);
        }

        /// <summary>
        /// "Fixed Spectrum Size" checkbox checked state is changed.
        /// </summary>
        private void fixedSpectrumSizeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {

            if (fixedSpectrumSizeCheckBox.IsChecked.Value)
            {
                if (spectrumSizeNumericUpDown != null)
                    spectrumSizeNumericUpDown.IsEnabled = true;
            }
            else
                spectrumSizeNumericUpDown.IsEnabled = false;
            ExecuteProcessing();
        }

        /// <summary>
        /// The checked state of "Grayscale Visualization" check box is changed.
        /// </summary>
        private void grayscaleVisualizationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _grayscaleVisualiation = grayscaleVisualizationCheckBox.IsChecked.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// The checked state of "Use Normalization" check box is changed.
        /// </summary>
        private void normalizationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _normalization = normalizationCheckBox.IsChecked.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// The checked state of "Use Absolute Values" check box is changed.
        /// </summary>
        private void absoluteValuesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _absoluteValues = absoluteValuesCheckBox.IsChecked.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// The selected index of "Visualization Type" combo box is changed.
        /// </summary>
        private void visualizationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _visualizationType = (FrequencySpectrumVisualizationType)visualizationTypeComboBox.SelectedItem;
            ExecuteProcessing();
        }

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// The checked state in the preview check box is changed.
        /// </summary>
        private void previewCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            IsPreviewEnabled = previewCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// The value of "Spectrum Size" numeric up-down is changed.
        /// </summary>
        private void spectrumSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if ((int)spectrumSizeNumericUpDown.Value < 0)
                spectrumSizeNumericUpDown.Value = _spectrumSize;
        }

        /// <summary>
        /// "Spectrum Size" numeric up-down lost focus.
        /// </summary>
        private void spectrumSizeNumericUpDown_LostFocus(object sender, RoutedEventArgs e)
        {

            FrequencySpectrumVisualizerCommand command = new FrequencySpectrumVisualizerCommand();
            try
            {
                command.SpectrumImageSize = (int)spectrumSizeNumericUpDown.Value;
            }
            catch
            {
                spectrumSizeNumericUpDown.Value = _spectrumSize;
                return;
            }
            _spectrumSize = (int)spectrumSizeNumericUpDown.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// "Enter" is pressed on "Spectrum Size" numeric up-down.
        /// </summary>
        private void spectrumSizeNumericUpDown_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                FrequencySpectrumVisualizerCommand command = new FrequencySpectrumVisualizerCommand();
                try
                {
                    command.SpectrumImageSize = (int)spectrumSizeNumericUpDown.Value;

                }
                catch
                {
                    spectrumSizeNumericUpDown.Value = _spectrumSize;
                    return;
                }
                _spectrumSize = (int)spectrumSizeNumericUpDown.Value;
                ExecuteProcessing();
            }
        }

        #endregion

        #endregion

    }
}
