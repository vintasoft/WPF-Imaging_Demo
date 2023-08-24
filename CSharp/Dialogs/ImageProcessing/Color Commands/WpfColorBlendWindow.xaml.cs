using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging;

namespace WpfImagingDemo
{
    /// <summary>
    /// A form that allows to specify parameters for color blend command.
    /// </summary>
    public partial class WpfColorBlendWindow : Window
    {

        #region Fields

        // color components
        byte _r;
        byte _g;
        byte _b;
        byte _a;

        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        bool _disableBlending = true;

        #endregion



        #region Constructor

        public WpfColorBlendWindow(WpfImageViewer viewer, System.Drawing.Color blendColor, BlendingMode blendMode)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);
            _imageProcessingPreviewInViewer.UseCurrentViewerZoomWhenPreviewProcessing = true;

            _blendMode = blendMode;

            // set available blending modes 
            foreach (object value in Enum.GetValues(typeof(BlendingMode)))
                blendModeComboBox.Items.Add(value);
            blendModeComboBox.SelectedItem = blendMode;

            // set blend color
            alphaSlider.Value = blendColor.A;
            redSlider.Value = blendColor.R;
            greenSlider.Value = blendColor.G;
            blueSlider.Value = blendColor.B;

            _disableBlending = false;
            SetColor();
        }

        #endregion



        #region Properties

        System.Drawing.Color _blendColor;
        public System.Drawing.Color BlendColor
        {
            get
            {
                return _blendColor;
            }
        }

        BlendingMode _blendMode;
        public BlendingMode BlendMode
        {
            get
            {
                return _blendMode;
            }
        }

        #endregion



        #region Methods

        public bool ShowProcessingDialog(Window windowOwner)
        {
            Owner = windowOwner;
            try
            {
                _imageProcessingPreviewInViewer.StartPreview();
                Blend();
            }
            catch (ImageProcessingException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            bool result = false;
            if (ShowDialog() == true)
                result = true;
            _imageProcessingPreviewInViewer.StopPreview();
            return result;
        }

        /// <summary>
        /// Blend the thumbnail.
        /// </summary>
        public void Blend()
        {
            if (_disableBlending)
                return;

            ColorBlendCommand command = new ColorBlendCommand(_blendMode, _blendColor);
            _imageProcessingPreviewInViewer.SetCommand(command);
        }

        /// <summary>
        /// Handles the ValueChanged event of Channel object.
        /// </summary>
        private void channel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_disableBlending)
                return;
            SetColor();
            Blend();
        }

        private void SetColor()
        {
            if (lockRGBCheckBox.IsChecked.Value == true)
            {
                // lock RGB channels
                _disableBlending = true;
                int dx = 0;
                if (_r != redSlider.Value)
                {
                    dx = (int)(redSlider.Value - _r);
                }
                else if (_g != greenSlider.Value)
                {
                    dx = (int)(greenSlider.Value - _g);
                }
                else if (_b != blueSlider.Value)
                {
                    dx = (int)(blueSlider.Value - _b);
                }
                _r = (byte)Math.Min(255, Math.Max(0, _r + dx));
                _g = (byte)Math.Min(255, Math.Max(0, _g + dx));
                _b = (byte)Math.Min(255, Math.Max(0, _b + dx));
                redSlider.Value = _r;
                greenSlider.Value = _g;
                blueSlider.Value = _b;
                _disableBlending = false;
            }
            else
            {
                _r = (byte)redSlider.Value;
                _g = (byte)greenSlider.Value;
                _b = (byte)blueSlider.Value;
            }

            // show colors information

            _a = (byte)alphaSlider.Value;
            _blendColor = System.Drawing.Color.FromArgb(_a, System.Drawing.Color.FromArgb(_r, _g, _b));

            alphaLabel.Content = _a.ToString();
            alphaLabel.Foreground = new SolidColorBrush(BlendColors((this.Foreground as SolidColorBrush).Color,
                System.Drawing.Color.FromArgb(_a, System.Drawing.Color.Black)));

            redLabel.Content = _r.ToString();
            redLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, _r, 0, 0));

            greenLabel.Content = _g.ToString();
            greenLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, _g, 0));

            blueLabel.Content = _b.ToString();
            blueLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, _b));

            blackColorLabel.Background = new SolidColorBrush(BlendColors(Colors.Black, _blendColor));
            whiteColorLabel.Background = new SolidColorBrush(BlendColors(Colors.White, _blendColor));
        }

        /// <summary>
        /// Blend two color with alpha component.
        /// </summary>
        private Color BlendColors(Color sourceColor, System.Drawing.Color blendColor)
        {
            int alpha = blendColor.A;
            return Color.FromArgb(255,
               (byte)((sourceColor.R * (255 - alpha)) / 255 + (blendColor.R * alpha) / 255),
               (byte)((sourceColor.G * (255 - alpha)) / 255 + (blendColor.G * alpha) / 255),
               (byte)((sourceColor.B * (255 - alpha)) / 255 + (blendColor.B * alpha) / 255));
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

        /// <summary>
        /// Handles the SelectionChanged event of BlendModeComboBox object.
        /// </summary>
        private void blendModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _blendMode = (BlendingMode)blendModeComboBox.SelectedItem;
            Blend();
        }

        #endregion

    }
}
