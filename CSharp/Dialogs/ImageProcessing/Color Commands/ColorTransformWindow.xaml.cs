using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

using Microsoft.Win32;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ColorManagement;
using Vintasoft.Imaging.ColorManagement.Icc;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;

using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and change settings for the color transform command.
    /// </summary>
    public partial class ColorTransformWindow : Window
    {

        #region Fields

        /// <summary>
        /// Color management decode settings.
        /// </summary>
        ColorManagementDecodeSettings _settings = new ColorManagementDecodeSettings();

        /// <summary>
        /// Image color space format.
        /// </summary>
        ColorSpaceFormat _imageColorSpaceFormat;

        /// <summary>
        /// Open file dialog.
        /// </summary>
        OpenFileDialog _openFileDialog;

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        bool _isShown = false;

        #endregion



        #region Constructors

        public ColorTransformWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);

            _imageColorSpaceFormat = viewer.Image.ColorSpaceFormat;
            this.Title = String.Format("Color Transform ({0})", _imageColorSpaceFormat.ColorSpace);

            renderingIntentComboBox.Items.Add(RenderingIntent.Perceptual);
            renderingIntentComboBox.Items.Add(RenderingIntent.MediaRelativeColorimetric);
            renderingIntentComboBox.Items.Add(RenderingIntent.Saturation);
            renderingIntentComboBox.Items.Add(RenderingIntent.IccAbsoluteColorimetric);

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = "ICC profiles|*.icc;*.icm|All files|*.*";
            _openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ResourceAssembly.Location);
            useBlackPointCompensationCheckBox.IsChecked = _settings.UseBlackPointCompensation;
            renderingIntentComboBox.SelectedItem = _settings.RenderingIntent;

            UseCurrentViewerZoomWhenPreviewProcessing = true;
            previewCheckBox.IsChecked = true;
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
            ColorTransformCommand command = new ColorTransformCommand();
            _settings.ConstructThreadSafeColorTransforms = true;
            command.ColorTransform = _settings.GetColorTransform(_imageColorSpaceFormat, _imageColorSpaceFormat);
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
        /// Sets input ICC profile.
        /// </summary>
        private void inputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                string filePath = _openFileDialog.FileName;
                _openFileDialog.InitialDirectory = Path.GetDirectoryName(filePath);

                IccProfile iccProfile = new IccProfile(filePath);
                IccProfile oldProfile = null;

                if (iccProfile.DeviceColorSpace != _imageColorSpaceFormat.ColorSpace)
                {
                    DemosTools.ShowErrorMessage(string.Format("Unexpected profile color space: {0}.\nRequired profile color space: {1}.",
                        iccProfile.DeviceColorSpace, _imageColorSpaceFormat.ColorSpace));
                    oldProfile = iccProfile;
                }
                else
                {
                    if (iccProfile.DeviceColorSpace == ColorSpaceType.sRGB)
                    {
                        oldProfile = _settings.InputRgbProfile;
                        _settings.InputRgbProfile = iccProfile;
                    }
                    else
                    {
                        oldProfile = _settings.InputGrayscaleProfile;
                        _settings.InputGrayscaleProfile = iccProfile;
                    }
                    ExecuteProcessing();
                }

                if (iccProfile != oldProfile)
                    inputProfileTextBox.Text = filePath;

                if (oldProfile != null)
                    oldProfile.Dispose();
            }
        }

        /// <summary>
        /// Sets output ICC profile.
        /// </summary>
        private void outputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                string filePath = _openFileDialog.FileName;
                _openFileDialog.InitialDirectory = Path.GetDirectoryName(filePath);

                IccProfile iccProfile = new IccProfile(filePath);
                IccProfile oldProfile = null;

                if (iccProfile.DeviceColorSpace != _imageColorSpaceFormat.ColorSpace)
                {
                    DemosTools.ShowErrorMessage(string.Format("Unexpected profile color space: {0}.\nRequired profile color space: {1}.",
                        iccProfile.DeviceColorSpace, _imageColorSpaceFormat.ColorSpace));
                    oldProfile = iccProfile;
                }
                else
                {
                    if (iccProfile.DeviceColorSpace == ColorSpaceType.sRGB)
                    {
                        oldProfile = _settings.InputRgbProfile;
                        _settings.OutputRgbProfile = iccProfile;
                    }
                    else
                    {
                        oldProfile = _settings.InputGrayscaleProfile;
                        _settings.OutputGrayscaleProfile = iccProfile;
                    }
                    ExecuteProcessing();
                }

                if (iccProfile != oldProfile)
                    outputProfileTextBox.Text = filePath;

                if (oldProfile != null)
                    oldProfile.Dispose();
            }
        }

        /// <summary>
        /// Remove input ICC profile.
        /// </summary>
        private void removeInputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            IccProfile profile = null;
            if (_settings.InputRgbProfile != null)
            {
                profile = _settings.InputRgbProfile;
                _settings.InputRgbProfile = null;
            }
            else if (_settings.InputGrayscaleProfile != null)
            {
                profile = _settings.InputGrayscaleProfile;
                _settings.InputGrayscaleProfile = null;
            }
            if (profile != null)
                profile.Dispose();

            inputProfileTextBox.Text = string.Empty;
            ExecuteProcessing();
        }

        /// <summary>
        /// Remove output ICC profile.
        /// </summary>
        private void removeOutputProfileButton_Click(object sender, RoutedEventArgs e)
        {
            IccProfile profile = null;
            if (_settings.OutputRgbProfile != null)
            {
                profile = _settings.OutputRgbProfile;
                _settings.OutputRgbProfile = null;
            }
            else if (_settings.OutputGrayscaleProfile != null)
            {
                profile = _settings.OutputGrayscaleProfile;
                _settings.OutputGrayscaleProfile = null;
            }
            if (profile != null)
                profile.Dispose();

            outputProfileTextBox.Text = string.Empty;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the SelectionChanged event of renderingIntentComboBox object.
        /// </summary>
        private void renderingIntentComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _settings.RenderingIntent = (RenderingIntent)renderingIntentComboBox.SelectedItem;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the CheckedChanged event of useBlackPointCompensationCheckBox object.
        /// </summary>
        void useBlackPointCompensationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _settings.UseBlackPointCompensation = useBlackPointCompensationCheckBox.IsChecked.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Handles the CheckedChanged event of previewCheckBox object.
        /// </summary>
        private void previewCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            IsPreviewEnabled = previewCheckBox.IsChecked.Value;
        }

        #endregion

        #endregion

    }
}
