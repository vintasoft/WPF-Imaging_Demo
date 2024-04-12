using System.Windows;

using Vintasoft.Imaging;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to specify parameters of new image.
    /// </summary>
    public partial class CreateNewImageWindow : Window
    {

        #region Fields

        ImageSize _imageSize = null;
        PixelFormat _pixelFormat;

        #endregion



        #region Constructor

        public CreateNewImageWindow()
        {
            InitializeComponent();

            pixelFormatComboBox.Items.Add(PixelFormat.BlackWhite);
            pixelFormatComboBox.Items.Add(PixelFormat.Indexed1);
            pixelFormatComboBox.Items.Add(PixelFormat.Indexed4);
            pixelFormatComboBox.Items.Add(PixelFormat.Indexed8);
            pixelFormatComboBox.Items.Add(PixelFormat.Gray8);
            pixelFormatComboBox.Items.Add(PixelFormat.Gray16);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgr555);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgr565);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgr24);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgr32);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgra32);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgr48);
            pixelFormatComboBox.Items.Add(PixelFormat.Bgra64);

            pixelFormatComboBox.SelectedItem = PixelFormat.Bgr24;

            horizontalResolutionTextBox.Text = ImagingEnvironment.ScreenResolution.Horizontal.ToString();
            verticalResolutionTextBox.Text = ImagingEnvironment.ScreenResolution.Vertical.ToString();

            //SetImageParams(ImagingEnvironment.ScreenResolution);
        }

        #endregion



        #region Methods

        #region PUBLIC

        public VintasoftImage CreateImage()
        {
            VintasoftImage image = new VintasoftImage(_imageSize, _pixelFormat);

            Palette palette = null;
            if (_pixelFormat == PixelFormat.Indexed1)
                palette = Palette.CreateBlackWhitePalette();
            else if (_pixelFormat == PixelFormat.Indexed4)
                palette = Palette.CreateGrayscalePalette(16);
            else if (_pixelFormat == PixelFormat.Indexed8)
                palette = Palette.CreateGrayscalePalette();

            if (palette != null)
                image.Palette.SetColors(palette.GetAsArray());

            return image;
        }

        #endregion


        #region PRIVATE

        private void SetImageParams(Resolution resolution)
        {
            int widthImage = (int)widthImageNumericUpDown.Value;
            int heightImage = (int)heightImageNumericUpDown.Value;
            _imageSize = ImageSize.FromPixels(widthImage, heightImage, resolution);

            _pixelFormat = (PixelFormat)pixelFormatComboBox.SelectedItem;
        }

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            float horizontalResolution;
            if (!float.TryParse(horizontalResolutionTextBox.Text, out horizontalResolution))
            {
                MessageBox.Show("Horizontal resolution value is incorrect!", "Create new image");
                horizontalResolutionTextBox.Focus();
                horizontalResolutionTextBox.SelectAll();
                return;
            }
            else if (horizontalResolution <= 0)
            {
                MessageBox.Show("Horizontal resolution value is incorrect!", "Create new image");
                horizontalResolutionTextBox.Focus();
                horizontalResolutionTextBox.SelectAll();
                return;
            }

            float verticalResolution;
            if (!float.TryParse(verticalResolutionTextBox.Text, out verticalResolution))
            {
                MessageBox.Show("Vertical resolution value is incorrect!", "Create new image");
                verticalResolutionTextBox.Focus();
                verticalResolutionTextBox.SelectAll();
                return;
            }
            else if (verticalResolution <= 0)
            {
                MessageBox.Show("Vertical resolution value is incorrect!", "Create new image");
                verticalResolutionTextBox.Focus();
                verticalResolutionTextBox.SelectAll();
                return;
            }

            Resolution resolution = new Resolution(horizontalResolution, verticalResolution);
            SetImageParams(resolution);

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of buttonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

        #endregion

    }
}
