using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window with property grid for image processing command.
    /// </summary>
    public partial class WpfPropertyGridConfigWindow : Window
    {

        #region Fields

        /// <summary>
        /// Image processing command.
        /// </summary>
        ProcessingCommandBase _command;

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        /// <summary>
        /// Indicates that window is shown.
        /// </summary>
        bool _isShown = false;

        #endregion



        #region Constructors

        public WpfPropertyGridConfigWindow(WpfImageViewer viewer, ProcessingCommandBase command)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);

            _command = command;

            this.Title = command.Name;

            propertyGrid1.SelectedObject = command;

            previewCheckBox.IsChecked = IsPreviewEnabled;
            previewCheckBox_Click(previewCheckBox, null);
        }

        #endregion



        #region Properties

        bool _isPreviewEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether an image preview in image viewer is enabled.
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
                    previewCheckBox.IsChecked = value;
                    previewCheckBox_Click(previewCheckBox, null);
                }
            }
        }

        bool _isPreviewAvailable = true;
        /// <summary>
        /// Gets or sets a value indicating whether an image preview in image viewer is available.
        /// </summary>
        public bool IsPreviewAvailable
        {
            get
            {
                return _isPreviewAvailable;
            }
            set
            {
                _isPreviewAvailable = value;
                IsPreviewEnabled = _isPreviewAvailable;
                previewCheckBox.Visibility = _isPreviewAvailable ? Visibility.Visible : Visibility.Hidden;
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
            return _command;
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Executes the processing command.
        /// </summary>
        protected void ExecuteProcessing()
        {
            if (_command != null)
                _imageProcessingPreviewInViewer.SetCommand(_command);
            propertyGrid1.SelectedObject = null;
            propertyGrid1.SelectedObject = _command;
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

        /// <summary>
        /// Handles the PropertyValueChanged event of propertyGrid1 object.
        /// </summary>
        private void propertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            ExecuteProcessing();
        }

        #endregion
        
        #endregion

    }
}
