using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode.CustomControls;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and edit an image palette.
    /// </summary>
    public partial class PaletteWindow : Window
    {

        #region Fields

        bool _showingColor = false;

        #endregion



        #region Constructors

        public PaletteWindow()
        {
            InitializeComponent();

            _paletteViewer.Margin = new Thickness(5);
            _paletteViewer.PaletteChanged += new EventHandler<PropertyChangedEventArgs>(PaletteViewer_PaletteChanged);
            _paletteViewer.SelectedColorValueChanged += new EventHandler(PaletteViewer_SelectedColorChanged);
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the <see cref="WpfPaletteViewer"/>.
        /// </summary>
        public WpfPaletteViewer PaletteViewer
        {
            get
            {
                return _paletteViewer;
            }
        }

        /// <summary>
        /// Gets value indicating whether the <see cref="PaletteViewer"/> can change palette.
        /// </summary>
        public bool CanChangePalette
        {
            get
            {
                return PaletteViewer.CanChangePalette;
            }
            set
            {
                PaletteViewer.CanChangePalette = value;
                UpdateUI();
            }
        }

        #endregion



        #region Methods

        private void UpdateUI()
        {
            if (PaletteViewer.CanChangePalette)
            {
                toGrayButton.Visibility = Visibility.Visible;
                invertButton.Visibility = Visibility.Visible;
                alphaNumericUpDown.IsEnabled = true;
                redNumericUpDown.IsEnabled = true;
                greenNumericUpDown.IsEnabled = true;
                blueNumericUpDown.IsEnabled = true;
            }
            else
            {
                toGrayButton.Visibility = Visibility.Hidden;
                invertButton.Visibility = Visibility.Hidden;
                alphaNumericUpDown.IsEnabled = false;
                redNumericUpDown.IsEnabled = false;
                greenNumericUpDown.IsEnabled = false;
                blueNumericUpDown.IsEnabled = false;

            }
            colorIndexNumericUpDown.Maximum = PaletteViewer.Palette.ColorCount - 1;
            UpdateColor();
        }

        private void UpdateColor()
        {
            Color selectedColor = PaletteViewer.SelectedColorValue;
            _showingColor = true;
            alphaNumericUpDown.Value = selectedColor.A;
            redNumericUpDown.Value = selectedColor.R;
            greenNumericUpDown.Value = selectedColor.G;
            blueNumericUpDown.Value = selectedColor.B;
            colorIndexNumericUpDown.Value = PaletteViewer.SelectedColorIndex;
            _showingColor = false;
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Color selectedColor = PaletteViewer.SelectedColorValue;
            Title = string.Format("Palette: Selected index = {0}; RGB({1},{2},{3})",
                PaletteViewer.SelectedColorIndex,
                selectedColor.R,
                selectedColor.G,
                selectedColor.B);
        }

        /// <summary>
        /// Handles the SelectedColorChanged event of PaletteViewer object.
        /// </summary>
        private void PaletteViewer_SelectedColorChanged(object sender, System.EventArgs e)
        {
            UpdateColor();
        }

        /// <summary>
        /// Handles the PaletteChanged event of PaletteViewer object.
        /// </summary>
        private void PaletteViewer_PaletteChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// Handles the Click event of ToGrayButton object.
        /// </summary>
        private void toGrayButton_Click(object sender, RoutedEventArgs e)
        {
            PaletteViewer.Palette.ConvertToGrayColors();
            UpdateColor();
        }

        /// <summary>
        /// Handles the Click event of InvertButton object.
        /// </summary>
        private void invertButton_Click(object sender, RoutedEventArgs e)
        {
            PaletteViewer.Palette.Invert();
            UpdateColor();
        }

        /// <summary>
        /// Handles the Click event of ButtonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of ButtonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the ValueChanged event of ColorNumericUpDown object.
        /// </summary>
        private void colorNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_showingColor)
            {
                PaletteViewer.SelectedColorValue = Color.FromArgb(
                    (byte)alphaNumericUpDown.Value,
                    (byte)redNumericUpDown.Value,
                    (byte)greenNumericUpDown.Value,
                    (byte)blueNumericUpDown.Value);
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of ColorIndexNumericUpDown object.
        /// </summary>
        private void colorIndexNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_showingColor)
            {
                PaletteViewer.SelectedColorIndex = (byte)colorIndexNumericUpDown.Value;
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of PaletteViewer object.
        /// </summary>
        private void paletteViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CanChangePalette && e.OriginalSource is WpfPaletteViewerCell)
            {
                WpfPaletteViewerCell cell = (WpfPaletteViewerCell)e.OriginalSource;
                ColorPickerDialog colorDialog = new ColorPickerDialog();
                colorDialog.StartingColor = cell.Color;
                colorDialog.Owner = this;
                bool? dialogResult = colorDialog.ShowDialog();

                if (dialogResult.HasValue && dialogResult.Value)
                {
                    Color selectedColor = colorDialog.SelectedColor;
                    _paletteViewer.SelectedColorValue = selectedColor;
                    UpdateColor();
                }
            }
        }

        #endregion

    }
}
