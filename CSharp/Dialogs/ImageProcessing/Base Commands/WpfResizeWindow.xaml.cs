using System;
using System.Windows;

using Vintasoft.Imaging;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to resize an image.
    /// </summary>
    public partial class WpfResizeWindow : Window
    {

        #region Fields

        private bool _firstInit = true;

        #endregion



        #region Constructor

        public WpfResizeWindow(int width, int height, ImageInterpolationMode interpolationMode)
        {
            InitializeComponent();

            _imageWidth = width;
            _imageHeight = height;

            widthNumericUpDown.Value = width;
            heightNumericUpDown.Value = height;

            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bicubic);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.NearestNeighbor);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBicubic);

            interpolationModeComboBox.SelectedItem = interpolationMode;

            _firstInit = false;
        }

        #endregion



        #region Properties

        private int _imageWidth;
        public int ImageWidth
        {
            get
            {
                return _imageWidth;
            }
        }

        private int _imageHeight;
        public int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
        }

        private ImageInterpolationMode _interpolationMode;
        public ImageInterpolationMode InterpolationMode
        {
            get
            {
                return _interpolationMode;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of OkButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            _imageWidth = (int)Math.Round(widthNumericUpDown.Value);
            _imageHeight = (int)Math.Round(heightNumericUpDown.Value);
            _interpolationMode = (ImageInterpolationMode)interpolationModeComboBox.SelectedItem;
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of CancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the ValueChanged event of WidthNumericUpDown object.
        /// </summary>
        private void widthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_firstInit && constrainProportionsCheckBox.IsChecked.Value == true)
            {
                heightNumericUpDown.Value = (int)Math.Round(Math.Min(
                    Math.Max(1, widthNumericUpDown.Value * (double)_imageHeight / _imageWidth), heightNumericUpDown.Maximum));
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of HeightNumericUpDown object.
        /// </summary>
        private void heightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_firstInit && constrainProportionsCheckBox.IsChecked.Value == true)
            {
                widthNumericUpDown.Value = (int)Math.Round(Math.Min(
                    Math.Max(1, heightNumericUpDown.Value * (double)_imageWidth / _imageHeight), widthNumericUpDown.Maximum));
            }
        }

        #endregion

    }
}
