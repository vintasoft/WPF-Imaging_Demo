using System;
using System.ComponentModel;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Fft.Filtering;
using Vintasoft.Imaging.ImageProcessing.Fft.Filters;
using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and change settings for the image sharpening command.
    /// </summary>
    public partial class WpfImageSharpeningWindow : Window
    {

        #region Fields

        /// <summary>
        /// Blending mode.
        /// </summary>
        BlendingMode _blendingMode = BlendingMode.SoftLight;

        /// <summary>
        /// Type of frequency filter.
        /// </summary>
        FrequencyFilterType _filter = FrequencyFilterType.Gaussian;

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        /// <summary>
        /// Indicates that the window is shown.
        /// </summary>
        bool _isShown = false;

        /// <summary>
        /// Indicates that grayscale filtration should be used.
        /// </summary>
        bool _grayscaleFiltration = true;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfImageSharpeningWindow"/> class.
        /// </summary>
        public WpfImageSharpeningWindow(WpfImageViewer viewer)
        {
            InitializeComponent();
            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);
            this.Title = "Image Sharpening";

            // preview check box
            previewCheckBox.IsChecked = true;

            // blending mode combo box
            blendingModeComboBox.Items.Add(BlendingMode.Normal);
            blendingModeComboBox.Items.Add(BlendingMode.Multiply);
            blendingModeComboBox.Items.Add(BlendingMode.Screen);
            blendingModeComboBox.Items.Add(BlendingMode.Overlay);
            blendingModeComboBox.Items.Add(BlendingMode.Darken);
            blendingModeComboBox.Items.Add(BlendingMode.Lighten);
            blendingModeComboBox.Items.Add(BlendingMode.ColorDodge);
            blendingModeComboBox.Items.Add(BlendingMode.ColorBurn);
            blendingModeComboBox.Items.Add(BlendingMode.HardLight);
            blendingModeComboBox.Items.Add(BlendingMode.SoftLight);
            blendingModeComboBox.Items.Add(BlendingMode.Difference);
            blendingModeComboBox.Items.Add(BlendingMode.Exclusion);
            blendingModeComboBox.Items.Add(BlendingMode.Hue);
            blendingModeComboBox.Items.Add(BlendingMode.Saturation);
            blendingModeComboBox.Items.Add(BlendingMode.Color);
            blendingModeComboBox.Items.Add(BlendingMode.Luminosity);
            blendingModeComboBox.Items.Add(BlendingMode.Brightness);
            blendingModeComboBox.Items.Add(BlendingMode.Contrast);
            blendingModeComboBox.Items.Add(BlendingMode.Gamma);
            blendingModeComboBox.Items.Add(BlendingMode.Min);
            blendingModeComboBox.Items.Add(BlendingMode.Max);
            blendingModeComboBox.Items.Add(BlendingMode.Sum);
            blendingModeComboBox.Items.Add(BlendingMode.Sub);
            blendingModeComboBox.Items.Add(BlendingMode.Division);
            blendingModeComboBox.SelectedItem = BlendingMode.SoftLight;

            // filter type combo box
            filterTypeComboBox.Items.Add(FrequencyFilterType.Ideal);
            filterTypeComboBox.Items.Add(FrequencyFilterType.Butterworth);
            filterTypeComboBox.Items.Add(FrequencyFilterType.Gaussian);
            filterTypeComboBox.SelectedItem = FrequencyFilterType.Gaussian;

            grayscaleFiltrationCheckBox.IsChecked = true;
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
        public ProcessingCommandBase GetProcessingCommand()
        {
            ImageSharpeningCommand command = new ImageSharpeningCommand();
            command.Radius = (int)Math.Round(radiusEditorControl.Value);
            command.OverlayAlpha = (float)overlayAlphaEditorControl.Value;
            command.Filter = _filter;
            command.BlendingMode = _blendingMode;
            command.GrayscaleFiltration = _grayscaleFiltration;
            return command;
        }

        #endregion


        #region PRIVATE

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
        /// "OK" button is pressed.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is pressed.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// The selected index of "Blending Mode" combo box is changed.
        /// </summary>
        private void blendingModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _blendingMode = (BlendingMode)blendingModeComboBox.SelectedItem;
            ExecuteProcessing();
        }

        /// <summary>
        /// The selected index of "Filter Type combo box is changed.
        /// </summary>
        private void filterTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _filter = (FrequencyFilterType)filterTypeComboBox.SelectedItem;
            ExecuteProcessing();
        }

        /// <summary>
        /// The preview check box is pressed.
        /// </summary>
        private void previewCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            IsPreviewEnabled = previewCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// The "Use Grayscale Filtration" check box is pressed.
        /// </summary>
        private void grayscaleFiltrationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _grayscaleFiltration = grayscaleFiltrationCheckBox.IsChecked.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// The value in value editor is changed.
        /// </summary>
        private void ValueEditorControl_ValueChanged(object sender, EventArgs e)
        {
            ExecuteProcessing();
        }

        #endregion

        #endregion

    }
}
