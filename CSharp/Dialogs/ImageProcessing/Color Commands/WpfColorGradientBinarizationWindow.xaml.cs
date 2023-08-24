using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging.ImageColors;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and edit settings of the AdvancedReplaceColorCommand (CreateColorGradientBinarizationCommand).
    /// </summary>
    public partial class WpfColorGradientBinarizationWindow : Window
    {

        #region Fields

        /// <summary>
        /// Image processing preview in WpfImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        bool _isShown = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfColorGradientBinarizationWindow"/> class.
        /// </summary>
        public WpfColorGradientBinarizationWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);

            this.previewCheckBox.IsChecked = this.IsPreviewEnabled;
            previewCheckBox_Click(previewCheckBox, null);

            startColorPanelControl.Color = Colors.Black;
            stopColorPanelControl.Color = Colors.Gray;
            gradientRadiusNumericUpDown.Value = 16;
        }

        #endregion



        #region Properties

        bool _isPreviewEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether the image preview in image viewer is enabled.
        /// </summary>
        [Browsable(false)]
        public virtual bool IsPreviewEnabled
        {
            get
            {
                return _isPreviewEnabled;
            }
            set
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

        bool _changePixelFormatToBlackWhite = false;
        /// <summary>
        /// Gets or sets a value indicating whether need change pixel format to black white.
        /// </summary>
        /// <value>
        ///   <c>True</c> if need change pixel format to black white; otherwise, <c>false</c>.
        /// </value>
        public bool ChangePixelFormatToBlackWhite
        {
            get
            {
                return _changePixelFormatToBlackWhite;
            }
            set
            {
                _changePixelFormatToBlackWhite = value;
            }
        }

        #endregion



        #region Methods

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
#if !REMOVE_DOCCLEANUP_PLUGIN
            System.Drawing.Color startColor = WpfObjectConverter.CreateDrawingColor(startColorPanelControl.Color);
            System.Drawing.Color stopColor = WpfObjectConverter.CreateDrawingColor(stopColorPanelControl.Color);


            AdvancedReplaceColorCommand result = AdvancedReplaceColorCommand.CreateColorGradientBinarizationCommand(
                new Rgb24Color(startColor),
                new Rgb24Color(stopColor),
                (int)gradientRadiusNumericUpDown.Value);
            if (ChangePixelFormatToBlackWhite)
                return new CompositeCommand(result, new ChangePixelFormatToBlackWhiteCommand(BinarizationMode.Threshold));
            return result;
#else
            throw new NotImplementedException();
#endif
        }


        /// <summary>
        /// Executes the processing command.
        /// </summary>
        protected void ExecuteProcessing()
        {
            ProcessingCommandBase command = GetProcessingCommand();
            if (command != null)
                _imageProcessingPreviewInViewer.SetCommand(command);
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

        /// <summary>
        /// Handles the ValueChanged event of Property object.
        /// </summary>
        private void property_ValueChanged(object sender, EventArgs e)
        {
            ExecuteProcessing();
        }

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

        #endregion

    }
}
