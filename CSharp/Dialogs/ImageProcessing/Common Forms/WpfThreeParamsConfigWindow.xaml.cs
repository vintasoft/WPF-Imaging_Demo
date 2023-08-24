using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window for image processing function with three parameters.
    /// </summary>
    public partial class WpfThreeParamsConfigWindow : Window
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

        /// <summary>
        /// Indicates that the processing dialog is shown.
        /// </summary>
        bool _isShown = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfThreeParamsConfigWindow"/> class.
        /// </summary>
        public WpfThreeParamsConfigWindow(
            WpfImageViewer viewer,
            string dialogName,
            WpfImageProcessingParameter parameter1,
            WpfImageProcessingParameter parameter2,
            WpfImageProcessingParameter parameter3)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);
            Title = dialogName;

            _initialParameter1 = parameter1;
            _initialParameter2 = parameter2;
            _initialParameter3 = parameter3;

            valueEditorControl1.ValueHeader = parameter1.Name;
            valueEditorControl1.MinValue = parameter1.MinValue;
            valueEditorControl1.MaxValue = parameter1.MaxValue;
            valueEditorControl1.DefaultValue = parameter1.DefaultValue;
            valueEditorControl1.Value = parameter1.DefaultValue;

            valueEditorControl2.ValueHeader = parameter2.Name;
            valueEditorControl2.MinValue = parameter2.MinValue;
            valueEditorControl2.MaxValue = parameter2.MaxValue;
            valueEditorControl2.DefaultValue = parameter2.DefaultValue;
            valueEditorControl2.Value = parameter2.DefaultValue;

            valueEditorControl3.ValueHeader = parameter3.Name;
            valueEditorControl3.MinValue = parameter3.MinValue;
            valueEditorControl3.MaxValue = parameter3.MaxValue;
            valueEditorControl3.DefaultValue = parameter3.DefaultValue;
            valueEditorControl3.Value = parameter3.DefaultValue;

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
        public virtual ProcessingCommandBase GetProcessingCommand()
        {
            return null;
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Executes the processing command.
        /// </summary>
        protected void ExecuteProcessing()
        {
            _parameter1 = (int)Math.Round(valueEditorControl1.Value);
            _parameter2 = (int)Math.Round(valueEditorControl2.Value);
            _parameter3 = (int)Math.Round(valueEditorControl3.Value);
            ProcessingCommandBase command = GetProcessingCommand();
            if (command != null)
                _imageProcessingPreviewInViewer.SetCommand(command);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
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
        /// Preview check box is clicked.
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
        /// Value in a value editor is changed.
        /// </summary>
        private void ValueEditorControl_ValueChanged(object sender, EventArgs e)
        {
            ExecuteProcessing();
        }

        #endregion

        #endregion

    }
}
