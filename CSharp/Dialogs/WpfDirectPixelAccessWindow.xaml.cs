using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageColors;
using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;
using WpfDemosCommonCode.CustomControls;
using WpfDemosCommonCode.Imaging;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to get direct access to an image pixels.
    /// </summary>
    public partial class WpfDirectPixelAccessWindow : Window
    {

        #region Fields

        ColorBase _selectedColor;
        int _selectedColorX;
        int _selectedColorY;
        bool _pixelSelect = false;
        WpfImageViewer _viewer;

        #endregion



        #region Constructor

        public WpfDirectPixelAccessWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            _viewer = viewer;
            _viewer.MouseMove += new MouseEventHandler(viewer_MouseMove);
            _viewer.MouseUp += new MouseButtonEventHandler(viewer_MouseUp);
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Closed event of Window object.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            _viewer.MouseMove -= viewer_MouseMove;
            _viewer.MouseUp -= viewer_MouseUp;
        }

        // Updates information about selected pixel.
        /// <summary>
        /// Handles the MouseUp event of viewer object.
        /// </summary>
        void viewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (pixelsGroupBox.IsEnabled == true)
            {
                if (_viewer.FocusedIndex >= 0)
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        Point pt = _viewer.PointFromControlToImage(e.GetPosition(_viewer));
                        SelectPixel((int)pt.X, (int)pt.Y);
                    }
                }
            }
        }

        // Updates information about focused pixel.
        /// <summary>
        /// Handles the MouseMove event of viewer object.
        /// </summary>
        void viewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_viewer.FocusedIndex >= 0)
            {
                Point pt = _viewer.PointFromControlToImage(e.GetPosition(_viewer));
                string text = string.Format("Pixel (X={0},Y={1})", (int)pt.X, (int)pt.Y);
                if (text != pixelsGroupBox.Header.ToString())
                {
                    pixelsGroupBox.Header = text;
                    if (e.LeftButton == MouseButtonState.Pressed && pixelsGroupBox.IsEnabled == true && _viewer.IsEntireImageLoaded)
                    {
                        SelectPixel((int)pt.X, (int)pt.Y);
                    }
                }
            }
            else
            {
                pixelsGroupBox.Header = string.Format("Pixel");
            }
        }

        internal void SelectPixel(int x, int y)
        {
            if (x >= 0 && y >= 0)
            {
                VintasoftImage image = _viewer.Image;
                if (x < image.Width && y < image.Height)
                {
                    try
                    {
                        ColorBase pixelColor = _viewer.Image.GetPixelColor(x, y);
                        ShowSelectedPixelColor(x, y, pixelColor);
                    }
                    catch (Exception e)
                    {
                        DemosTools.ShowErrorMessage(e);
                    }
                    return;
                }
            }
            selectedPixelLabel.Content = "Selected Pixel: click on image to select";
            argbPanel.Visibility = Visibility.Hidden;
            indexedPanel.Visibility = Visibility.Hidden;
            gray16Panel.Visibility = Visibility.Hidden;
            selectedPixelColorLabel.Background = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Handles the Click event of selectInPaletteButton object.
        /// </summary>
        private void selectInPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            PaletteWindow paletteDlg = new PaletteWindow();
            paletteDlg.Topmost = true;
            paletteDlg.PaletteViewer.CanChangePalette = false;
            IndexedColor indColor = ((IndexedColor)_selectedColor);
            paletteDlg.PaletteViewer.Palette = indColor.Palette;
            paletteDlg.PaletteViewer.SelectedColorIndex = indColor.Index;
            if (paletteDlg.ShowDialog().Value)
            {
                indexNumericUpDown.Value = (int)paletteDlg.PaletteViewer.SelectedColorIndex;
                ShowSelectedPixelColor(_selectedColorX, _selectedColorY, new IndexedColor(paletteDlg.PaletteViewer.Palette, paletteDlg.PaletteViewer.SelectedColorIndex));
                SetColorOfSelectedPixel();
            }
        }

        private void UpdateSelectedPixelColorValue(int alpha, int red, int green, int blue, bool isNativeValues)
        {
            byte alpha8 = (byte)alpha;
            byte red8 = (byte)red;
            byte green8 = (byte)green;
            byte blue8 = (byte)blue;
            if (isNativeValues)
            {
                // create color
                if (_selectedColor is Argb32Color)
                    _selectedColor = new Argb32Color(alpha8, red8, green8, blue8);
                else if (_selectedColor is Rgb24Color)
                    _selectedColor = new Rgb24Color(red8, green8, blue8);
                else if (_selectedColor is Rgb16Color555)
                    _selectedColor = new Rgb16Color555(red8, green8, blue8);
                else if (_selectedColor is Rgb16Color565)
                    _selectedColor = new Rgb16Color565(red8, green8, blue8);
                else if (_selectedColor is Argb64Color)
                    _selectedColor = new Argb64Color(alpha, red, green, blue);
                else if (_selectedColor is Rgb48Color)
                    _selectedColor = new Rgb48Color(red, green, blue);
                else
                    _selectedColor = new Rgb24Color(red8, green8, blue8);
            }
            else
            {
                // convert from 8-bit per color to dest color depth and create color
                if (_selectedColor is Argb32Color)
                    _selectedColor = new Argb32Color(alpha8, red8, green8, blue8);
                else if (_selectedColor is Rgb24Color)
                    _selectedColor = new Rgb24Color(red8, green8, blue8);
                else if (_selectedColor is Rgb16Color555)
                    _selectedColor = new Rgb16Color555(new Rgb24Color(red8, green8, blue8));
                else if (_selectedColor is Rgb16Color565)
                    _selectedColor = new Rgb16Color565(new Rgb24Color(red8, green8, blue8));
                else if (_selectedColor is Argb64Color)
                    _selectedColor = new Argb64Color(new Argb32Color(alpha8, red8, green8, blue8));
                else if (_selectedColor is Rgb48Color)
                    _selectedColor = new Rgb48Color(new Rgb24Color(red8, green8, blue8));
                else
                    _selectedColor = new Rgb24Color(red8, green8, blue8);
            }
        }

        /// <summary>
        /// Handles the Click event of changeRGBComponentsButton object.
        /// </summary>
        private void changeRGBComponentsButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog dialog = new ColorPickerDialog();
            dialog.Topmost = true;
            System.Drawing.Color selectedColor = _selectedColor.ToColor();
            dialog.StartingColor = Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
            if (dialog.ShowDialog().Value)
            {
                Color newColor = dialog.SelectedColor;
                UpdateSelectedPixelColorValue(newColor.A, newColor.R, newColor.G, newColor.B, false);

                ShowSelectedPixelColor(
                    _selectedColorX,
                    _selectedColorY,
                    _selectedColor);

                SetColorOfSelectedPixel();
            }
        }

        private void ShowSelectedPixelColor(int x, int y, ColorBase pixelColor)
        {
            VintasoftImage image = _viewer.Image;
            _selectedColorX = x;
            _selectedColorY = y;
            _selectedColor = pixelColor;
            selectedPixelLabel.Content = string.Format("Selected Pixel: X={0},Y={1}; ColorType={2}", x, y, pixelColor.GetType().Name);
            _pixelSelect = true;
            if (pixelColor is Rgb24Color)
            {
                Rgb24Color rgbColor = (Rgb24Color)pixelColor;
                argbPanel.Visibility = Visibility.Visible;
                indexedPanel.Visibility = Visibility.Hidden;
                gray16Panel.Visibility = Visibility.Hidden;
                redNumericUpDown.Maximum = 255;
                redNumericUpDown.Value = rgbColor.Red;
                greenNumericUpDown.Maximum = 255;
                greenNumericUpDown.Value = rgbColor.Green;
                blueNumericUpDown.Maximum = 255;
                blueNumericUpDown.Value = rgbColor.Blue;
                if (pixelColor is Argb32Color)
                {
                    alphaNumericUpDown.Maximum = 255;
                    alphaNumericUpDown.Value = ((Argb32Color)pixelColor).Alpha;
                    alphaNumericUpDown.Visibility = Visibility.Visible;
                    rgbColorTypeLabel.Content = "ARGB32 =";
                }
                else
                {
                    alphaNumericUpDown.Visibility = Visibility.Hidden;
                    rgbColorTypeLabel.Content = "RGB (24bpp) =";
                }
            }
            else if (pixelColor is Rgb48Color)
            {
                Rgb48Color rgb48Color = (Rgb48Color)pixelColor;
                argbPanel.Visibility = Visibility.Visible;
                indexedPanel.Visibility = Visibility.Hidden;
                gray16Panel.Visibility = Visibility.Hidden;
                redNumericUpDown.Maximum = 65535;
                redNumericUpDown.Value = rgb48Color.Red;
                greenNumericUpDown.Maximum = 65535;
                greenNumericUpDown.Value = rgb48Color.Green;
                blueNumericUpDown.Maximum = 65535;
                blueNumericUpDown.Value = rgb48Color.Blue;
                if (pixelColor is Argb64Color)
                {
                    alphaNumericUpDown.Maximum = 65535;
                    alphaNumericUpDown.Value = ((Argb64Color)pixelColor).Alpha;
                    alphaNumericUpDown.Visibility = Visibility.Visible;
                    rgbColorTypeLabel.Content = "ARGB64 =";
                }
                else
                {
                    alphaNumericUpDown.Visibility = Visibility.Hidden;
                    rgbColorTypeLabel.Content = "RGB (48bpp) =";
                }
            }
            else if (pixelColor is Rgb16ColorBase)
            {
                Rgb16ColorBase rgb16Color = (Rgb16ColorBase)pixelColor;
                argbPanel.Visibility = Visibility.Visible;
                indexedPanel.Visibility = Visibility.Hidden;
                gray16Panel.Visibility = Visibility.Hidden;
                redNumericUpDown.Maximum = 31;
                redNumericUpDown.Value = rgb16Color.Red;
                blueNumericUpDown.Maximum = 31;
                blueNumericUpDown.Value = rgb16Color.Blue;
                alphaNumericUpDown.Visibility = Visibility.Hidden;
                if (pixelColor is Rgb16Color555)
                {
                    greenNumericUpDown.Maximum = 31;
                    rgbColorTypeLabel.Content = "RGB16 (5-5-5) =";
                }
                else
                {
                    greenNumericUpDown.Maximum = 63;
                    rgbColorTypeLabel.Content = "RGB16 (5-6-5) =";
                }
                greenNumericUpDown.Value = rgb16Color.Green;
            }
            else if (pixelColor is IndexedColor)
            {
                IndexedColor indexedColor = (IndexedColor)pixelColor;
                argbPanel.Visibility = Visibility.Hidden;
                indexedPanel.Visibility = Visibility.Visible;
                gray16Panel.Visibility = Visibility.Hidden;
                indexNumericUpDown.Maximum = indexedColor.Palette.ColorCount - 1;
                indexNumericUpDown.Value = indexedColor.Index;
            }
            else if (pixelColor is Gray16Color)
            {
                Gray16Color gray16 = (Gray16Color)pixelColor;
                argbPanel.Visibility = Visibility.Hidden;
                indexedPanel.Visibility = Visibility.Hidden;
                gray16Panel.Visibility = Visibility.Visible;
                gray16LumNumericUpDown.Value = gray16.Luminance;
            }
            SetSelectedPixelColorLabel(pixelColor);
            _pixelSelect = false;
        }

        private void SetColorOfSelectedPixel()
        {
            try
            {
                _viewer.Image.SetPixelColor(_selectedColorX, _selectedColorY, _selectedColor);
            }
            catch (Exception e)
            {
                DemosTools.ShowErrorMessage(e);
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of colorChannel object.
        /// </summary>
        private void colorChannel_ValueChanged(object sender, EventArgs e)
        {
            if (!_pixelSelect)
            {
                UpdateSelectedPixelColorValue(
                    (int)alphaNumericUpDown.Value,
                    (int)redNumericUpDown.Value,
                    (int)greenNumericUpDown.Value,
                    (int)blueNumericUpDown.Value,
                    true);
                SetSelectedPixelColorLabel(_selectedColor);
                SetColorOfSelectedPixel();
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of indexNumericUpDown object.
        /// </summary>
        private void indexNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_pixelSelect)
            {
                _selectedColor = new IndexedColor(_viewer.Image.Palette, (byte)indexNumericUpDown.Value);
                SetSelectedPixelColorLabel(_selectedColor);
                SetColorOfSelectedPixel();
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of gray16LumNumericUpDown object.
        /// </summary>
        private void gray16LumNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_pixelSelect)
            {
                _selectedColor = new Gray16Color((int)gray16LumNumericUpDown.Value);
                SetSelectedPixelColorLabel(_selectedColor);
                SetColorOfSelectedPixel();
            }
        }

        private void SetSelectedPixelColorLabel(ColorBase pixelColor)
        {
            System.Drawing.Color backColor = pixelColor.ToColor();
            selectedPixelColorLabel.Background = new SolidColorBrush(Color.FromArgb(backColor.A, backColor.R, backColor.G, backColor.B));
        }

        #endregion

    }
}
