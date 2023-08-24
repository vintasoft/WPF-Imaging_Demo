using System.Text;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and change settings for rotation operation.
    /// </summary>
    public partial class WpfRotateWindow : Window
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfRotateWindow"/> class.
        /// </summary>
        /// <param name="sourceImagePixelFormat">The pixel format of source image.</param>
        public WpfRotateWindow(PixelFormat sourceImagePixelFormat)
        {
            InitializeComponent();

            _sourceImagePixelFormat = sourceImagePixelFormat;
        }

        #endregion



        #region Properties

        decimal _angle;
        /// <summary>
        /// Gets the rotation angle.
        /// </summary>
        public decimal Angle
        {
            get
            {
                return _angle;
            }
        }

        /// <summary>
        /// Gets the type of border color.
        /// </summary>
        public BorderColorType BorderColorType
        {
            get
            {
                if (backgroundBlackRadioButton.IsChecked.Value == true)
                    return BorderColorType.Black;
                else if (backgroundWhiteRadioButton.IsChecked.Value == true)
                    return BorderColorType.White;
                else if (backgroundTransparentRadioButton.IsChecked.Value == true)
                    return BorderColorType.Transparent;
                return BorderColorType.AutoDetect;
            }
        }

        PixelFormat _sourceImagePixelFormat;
        /// <summary>
        /// Gets the pixel format of source image.
        /// </summary>
        public PixelFormat SourceImagePixelFormat
        {
            get
            {
                return _sourceImagePixelFormat;
            }
        }

        bool _isAntialiasingEnabled = true;
        /// <summary>
        /// Gets a value indicating whether the antialiasing is enabled.
        /// </summary>
        /// <value>
        /// <b>true</b> if the antialiasing is enabled; otherwise, <b>false</b>.
        /// </value>
        public bool IsAntialiasingEnabled
        {
            get
            {
                return _isAntialiasingEnabled;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// "Ok" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            _angle = (decimal)angleNumericUpDown.Value;
            _isAntialiasingEnabled = isAntialiasingEnabledCheckBox.IsChecked.Value == true;

            if ((bool)backgroundTransparentRadioButton.IsChecked &&
                SourceImagePixelFormat != PixelFormat.Bgra32 &&
                SourceImagePixelFormat != PixelFormat.Bgra64)
            {
                PixelFormat pixelFormatWithTransparencySupport = GetPixelFormatWithTransparencySupport(SourceImagePixelFormat);
                StringBuilder message = new StringBuilder();
                message.AppendLine("You have selected a transparent background but image pixel format does not support transparency.");
                message.AppendLine(string.Format("For using transparency the image should be converted to the {0} pixel format.", pixelFormatWithTransparencySupport));
                message.AppendLine("Press 'OK' for converting an image.");
                message.AppendLine("Press 'Cancel' for detecting the color automatically.");

                if (MessageBox.Show(message.ToString(), "Rotate image", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    _sourceImagePixelFormat = pixelFormatWithTransparencySupport;
                }
                else
                {
                    backgroundAutoDetectRadioButton.IsChecked = true;
                }
            }

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
        /// Returns a pixel format, which supports transparency.
        /// </summary>
        /// <param name="sourceImagePixelFormat">The pixel format of source image.</param>
        /// <returns>The pixel format, which supports transparency.</returns>
        private PixelFormat GetPixelFormatWithTransparencySupport(PixelFormat sourceImagePixelFormat)
        {
            switch (sourceImagePixelFormat)
            {
                case PixelFormat.Bgr48:
                case PixelFormat.Bgra64:
                case PixelFormat.Gray16:
                    return PixelFormat.Bgra64;
            }
            return PixelFormat.Bgra32;
        }

        #endregion

    }
}
