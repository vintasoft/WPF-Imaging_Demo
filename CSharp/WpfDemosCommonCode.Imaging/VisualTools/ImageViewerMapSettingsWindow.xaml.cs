using System;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and edit settings of image map.
    /// </summary>
    public partial class ImageViewerMapSettingsWindow : Window
    {

        #region Fields

        WpfImageMapTool _imageMap;

        #endregion



        #region Constructor

        public ImageViewerMapSettingsWindow()
        {
            InitializeComponent();

            sizeComboBox.Items.Add("64");
            sizeComboBox.Items.Add("120");
            sizeComboBox.Items.Add("150x200");
            sizeComboBox.Items.Add("200");
            sizeComboBox.Items.Add("320x240");
            sizeComboBox.Items.Add("400x300");
            sizeComboBox.Items.Add("640x480");

            zoomComboBox.Items.Add("1/20");
            zoomComboBox.Items.Add("1/25");
            zoomComboBox.Items.Add("1/40");
            zoomComboBox.Items.Add("1/80");
            zoomComboBox.Items.Add("1/100");
            zoomComboBox.Items.Add("Best fit");

            object[] aStyles = new object[]
            {
                AnchorType.None,
                AnchorType.Left | AnchorType.Top,
                AnchorType.Top,
                AnchorType.Top | AnchorType.Right,
                AnchorType.Right,
                AnchorType.Bottom| AnchorType.Right,
                AnchorType.Bottom,
                AnchorType.Bottom | AnchorType.Left,
                AnchorType.Left,
            };

            foreach (AnchorType anchorType in aStyles)
                locationComboBox.Items.Add(anchorType);
        }

        public ImageViewerMapSettingsWindow(WpfImageMapTool imageMap)
            : this()
        {
            _imageMap = imageMap;

            ShowSettings();
        }

        #endregion



        #region Methods

        private void ShowSettings()
        {
            enabledCheckBox.IsChecked = _imageMap.Enabled;
            enabledCheckBox_Click(enabledCheckBox, null);
            alwaysVisibleCheckBox.IsChecked = _imageMap.IsAlwaysVisible;
            locationComboBox.SelectedItem = _imageMap.Anchor;
            sizeComboBox.Text = string.Format("{0}x{1}", _imageMap.Size.Width, _imageMap.Size.Height);
            if (_imageMap.Zoom == 0)
                zoomComboBox.Text = "Best fit";
            else
                zoomComboBox.Text = string.Format("1/{0}", Math.Round(1 / _imageMap.Zoom));

            canvasPenCheckBox.IsChecked = _imageMap.CanvasPenColor != Colors.Transparent;
            canvasPenCheckBox_Click(canvasPenCheckBox, null);
            canvasColorPanelControl.Color = _imageMap.CanvasPenColor;
            canvasPenThicknessNumericUpDown.Value = (int)Math.Round(_imageMap.CanvasPenThickness);

            imageBufferPenCheckBox.IsChecked = _imageMap.ImageBufferPenColor != Colors.Transparent;
            imageBufferPenCheckBox_Click(imageBufferPenCheckBox, null);
            imageBufferColorPanelControl.Color = _imageMap.ImageBufferPenColor;
            imageBufferPenThicknessNumericUpDown.Value = (int)Math.Round(_imageMap.ImageBufferPenThickness);

            visibleRectPenCheckBox.IsChecked = _imageMap.VisibleRectPenColor != Colors.Transparent;
            visibleRectPenCheckBox_Click(visibleRectPenCheckBox, null);
            visibleRectColorPanelControl.Color = _imageMap.VisibleRectPenColor;
            visibleRectPenThicknessNumericUpDown.Value = (int)Math.Round(_imageMap.VisibleRectPenThickness);
        }

        private bool SetSettings()
        {
            _imageMap.Enabled = enabledCheckBox.IsChecked.Value == true;
            _imageMap.IsAlwaysVisible = alwaysVisibleCheckBox.IsChecked.Value == true;
            _imageMap.Anchor = (AnchorType)locationComboBox.SelectedItem;

            try
            {
                // Size
                string[] sizeStrings = sizeComboBox.Text.Split('x');
                int width;
                int height;
                if (sizeStrings.Length == 1)
                {
                    width = Convert.ToInt32(sizeStrings[0]);
                    height = width;
                }
                else
                {
                    width = Convert.ToInt32(sizeStrings[0]);
                    height = Convert.ToInt32(sizeStrings[1]);
                }
                _imageMap.Size = new Size(width, height);

                // Zoom
                if (zoomComboBox.Text == "Best fit")
                {
                    _imageMap.Zoom = 0;
                }
                else
                {
                    string[] zoomStrings = zoomComboBox.Text.Split('/');
                    if (zoomStrings.Length != 2)
                        throw new Exception("Invalid zoom value.");
                    _imageMap.Zoom = 1f / Convert.ToInt32(zoomStrings[1]);
                }

                // Pens
                if (canvasPenCheckBox.IsChecked.Value == true)
                {
                    _imageMap.CanvasPenColor = canvasColorPanelControl.Color;
                    _imageMap.CanvasPenThickness = canvasPenThicknessNumericUpDown.Value;
                }
                else
                    _imageMap.CanvasPenColor = Colors.Transparent;

                if (visibleRectPenCheckBox.IsChecked.Value == true)
                {
                    _imageMap.VisibleRectPenColor = visibleRectColorPanelControl.Color;
                    _imageMap.VisibleRectPenThickness = visibleRectPenThicknessNumericUpDown.Value;
                }
                else
                    _imageMap.VisibleRectPenColor = Colors.Transparent;

                if (imageBufferPenCheckBox.IsChecked.Value == true)
                {
                    _imageMap.ImageBufferPenColor = imageBufferColorPanelControl.Color;
                    _imageMap.ImageBufferPenThickness = imageBufferPenThicknessNumericUpDown.Value;
                }
                else
                    _imageMap.ImageBufferPenColor = Colors.Transparent;
            }
            catch (Exception e)
            {
                DemosTools.ShowErrorMessage(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Handles the Click event of ButtonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (SetSettings())
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
        /// Handles the Click event of EnabledCheckBox object.
        /// </summary>
        private void enabledCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = enabledCheckBox.IsChecked.Value == true;
            alwaysVisibleCheckBox.IsEnabled = enabled;

            locationComboBox.IsEnabled = enabled;
            sizeComboBox.IsEnabled = enabled;
            zoomComboBox.IsEnabled = enabled;

            canvasPenCheckBox.IsEnabled = enabled;
            imageBufferPenCheckBox.IsEnabled = enabled;
            visibleRectPenCheckBox.IsEnabled = enabled;
        }

        /// <summary>
        /// Handles the Click event of CanvasPenCheckBox object.
        /// </summary>
        private void canvasPenCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = canvasPenCheckBox.IsChecked.Value == true;
            canvasColorPanelControl.IsEnabled = enabled;
            canvasPenThicknessNumericUpDown.IsEnabled = enabled;
        }

        /// <summary>
        /// Handles the Click event of VisibleRectPenCheckBox object.
        /// </summary>
        private void visibleRectPenCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = visibleRectPenCheckBox.IsChecked.Value == true;
            visibleRectColorPanelControl.IsEnabled = enabled;
            visibleRectPenThicknessNumericUpDown.IsEnabled = enabled;
        }

        /// <summary>
        /// Handles the Click event of ImageBufferPenCheckBox object.
        /// </summary>
        private void imageBufferPenCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = imageBufferPenCheckBox.IsChecked.Value == true;
            imageBufferColorPanelControl.IsEnabled = enabled;
            imageBufferPenThicknessNumericUpDown.IsEnabled = enabled;
        }

        #endregion

    }
}
