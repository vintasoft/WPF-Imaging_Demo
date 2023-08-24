using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to change the pixel format.
    /// </summary>
    public partial class WpfChangePixelFormatWindow : Window
    {

        #region Constructor

        public WpfChangePixelFormatWindow()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        Vintasoft.Imaging.PixelFormat _pixelFormat;
        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public Vintasoft.Imaging.PixelFormat PixelFormat
        {
            get
            {
                return _pixelFormat;
            }
        }

        PaletteType _paletteType = PaletteType.Adaptive;
        /// <summary>
        /// Gets the palette type.
        /// </summary>
        public PaletteType PaletteType
        {
            get
            {
                return _paletteType;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of ConvertButton object.
        /// </summary>
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            switch (pixelFormatComboBox.SelectedIndex)
            {
                case 0:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.BlackWhite;
                    break;

                case 1:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Indexed1;
                    break;

                case 2:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Indexed4;
                    break;

                case 3:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Indexed8;
                    break;

                case 4:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Gray8;
                    break;

                case 5:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Indexed8;
                    _paletteType = PaletteType.Web;
                    break;

                case 6:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Gray16;
                    break;

                case 7:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgr555;
                    break;

                case 8:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgr565;
                    break;

                case 9:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgr24;
                    break;

                case 10:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgra32;
                    break;

                case 11:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgr32;
                    break;

                case 12:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgr48;
                    break;

                case 13:
                    _pixelFormat = Vintasoft.Imaging.PixelFormat.Bgra64;
                    break;

                default:
                    MessageBox.Show("You should select the pixel format first.");
                    return;
            }
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
