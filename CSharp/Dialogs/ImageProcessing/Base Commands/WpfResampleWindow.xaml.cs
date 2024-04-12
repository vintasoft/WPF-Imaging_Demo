using System;
using System.Windows;

using Vintasoft.Imaging;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to resample the image.
    /// </summary>
    public partial class WpfResampleWindow : Window
    {

        #region Constructor

        public WpfResampleWindow(float horizontalResolution, float verticalResolution,
            ImageInterpolationMode interpolationMode, string dlgCaption, bool resample)
        {
            InitializeComponent();

            this.Title = dlgCaption;
            _horizontalResolution = horizontalResolution;
            _verticalResolution = verticalResolution;

            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bicubic);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.NearestNeighbor);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBicubic);

            interpolationModeComboBox.SelectedItem = interpolationMode;

            horizontalResolutionNumericUpDown.Value = (double)Math.Round(Math.Min((double)horizontalResolution, horizontalResolutionNumericUpDown.Maximum));
            verticalResolutionNumericUpDown.Value = (double)Math.Round(Math.Min((double)verticalResolution, verticalResolutionNumericUpDown.Maximum));
        }

        #endregion



        #region Properties

        private float _horizontalResolution;
        public float HorizontalResolution
        {
            get
            {
                return _horizontalResolution;
            }
        }

        private float _verticalResolution;
        public float VerticalResolution
        {
            get
            {
                return _verticalResolution;
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

        public bool ShowInterpolationComboBox
        {
            get
            {
                return label1.Visibility == Visibility.Visible &&
                    interpolationModeComboBox.Visibility == Visibility.Visible;
            }
            set
            {
                Visibility visibility = value ? Visibility.Visible : Visibility.Hidden;
                label1.Visibility = visibility;
                interpolationModeComboBox.Visibility = visibility;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            _horizontalResolution = (float)horizontalResolutionNumericUpDown.Value;
            _verticalResolution = (float)verticalResolutionNumericUpDown.Value;
            _interpolationMode = (ImageInterpolationMode)interpolationModeComboBox.SelectedItem;
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
