using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and change settings for the levels command.
    /// </summary>
    public partial class WpfLevelsWindow : Window
    {

        #region Fields

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
        /// Initializes a new instance of the <see cref="WpfLevelsWindow"/> class.
        /// </summary>
        public WpfLevelsWindow(WpfImageViewer viewer)
        {
            InitializeComponent();
            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);

            IsPreviewEnabled = true;

            SetDefaultSettings();
        }

        #endregion



        #region Properties

        bool _isPreviewEnabled = false;
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
            LevelsCommand command = new LevelsCommand();
            ChannelRemapSettings settings = new ChannelRemapSettings(
                (int)sourceMinValueEditorControl.Value,
                (int)sourceMaxValueEditorControl.Value,
                (double)gammaValueEditorControl.Value,
                (int)destinationMinValueEditorControl.Value,
                (int)destinationMaxValueEditorControl.Value);

            if (redCheckBox.IsChecked.Value)
                command.RedChannelSettings = settings;

            if (greenCheckBox.IsChecked.Value)
                command.GreenChannelSettings = settings;

            if (blueCheckBox.IsChecked.Value)
                command.BlueChannelSettings = settings;
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
        /// The preview check box is pressed.
        /// </summary>
        private void previewCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            IsPreviewEnabled = previewCheckBox.IsChecked.Value;
            if (IsPreviewEnabled)
                previewCheckBox.Foreground = new SolidColorBrush(Colors.Black);
            else
                previewCheckBox.Foreground = new SolidColorBrush(Colors.Green);
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
        /// Sets the default settings of value editor controls.
        /// </summary>
        private void SetDefaultSettings()
        {
            LevelsCommand command = new LevelsCommand();
            ChannelRemapSettings settings = command.RedChannelSettings;

            sourceMinValueEditorControl.DefaultValue = settings.InputMin;
            sourceMaxValueEditorControl.DefaultValue = settings.InputMax;
            destinationMinValueEditorControl.DefaultValue = settings.OutputMin;
            destinationMaxValueEditorControl.DefaultValue = settings.OutputMax;
            gammaValueEditorControl.DefaultValue = (float)settings.Gamma;

            sourceMinValueEditorControl.Value = settings.InputMin;
            sourceMaxValueEditorControl.Value = settings.InputMax;
            destinationMinValueEditorControl.Value = settings.OutputMin;
            destinationMaxValueEditorControl.Value = settings.OutputMax;
            gammaValueEditorControl.Value = (float)settings.Gamma;
        }

        /// <summary>
        /// The value in value editor is changed.
        /// </summary>
        private void ValueEditorControl_ValueChanged(object sender, EventArgs e)
        {
            ExecuteProcessing();
        }

        /// <summary>
        /// Channel check box is pressed.
        /// </summary>
        private void channelCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ExecuteProcessing();
        }

        #endregion

        #endregion

    }
}
