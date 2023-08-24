using System.Windows;
using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Transforms;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and change settings for the image scaling command.
    /// </summary>
    public partial class WpfImageScalingWindow : Window
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfImageScalingWindow"/> class.
        /// </summary>
        public WpfImageScalingWindow()
        {
            InitializeComponent();

            // initialize interpolation mode combo box
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bicubic);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.NearestNeighbor);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBicubic);
            interpolationModeComboBox.SelectedItem = ImageInterpolationMode.HighQualityBicubic;
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets scale of the image.
        /// </summary>
        public float ImageScale
        {
            get
            {
                return (float)scaleEditorControl.Value / 100;
            }
        }

        ImageScalingCommand _command;
        /// <summary>
        /// Gets or sets the image scaling command.
        /// </summary>
        public ImageScalingCommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
            }
        }


        #endregion



        #region Methods

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
        /// Gets the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public ProcessingCommandBase GetProcessingCommand()
        {
            Command.Scale = ImageScale;
            Command.InterpolationMode = (ImageInterpolationMode)interpolationModeComboBox.SelectedItem;
            return Command;
        }

        /// <summary>
        /// Show processing dialog.
        /// </summary>
        /// <returns>
        /// <b>true</b> if form is closed and OK button is pressed;
        /// <b>false</b> if form is closed and not OK button is pressed.</returns>
        public bool ShowProcessingDialog()
        {
            return ShowDialog().Value;
        }

        #endregion

    }
}
