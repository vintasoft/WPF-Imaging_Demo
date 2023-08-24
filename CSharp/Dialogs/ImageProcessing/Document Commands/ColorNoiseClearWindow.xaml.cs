using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageColors;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.Wpf.UI;

using WpfDemosCommonCode;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to view and edit settings of the ColorNoiseClearCommand.
    /// </summary>
    public partial class ColorNoiseClearWindow : Window
    {

        #region Fields

        /// <summary>
        /// Image processing preview in ImageViewer.
        /// </summary>
        WpfImageProcessingPreviewInViewer _imageProcessingPreviewInViewer;

        bool _isShown = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorNoiseClearWindow"/> class.
        /// </summary>
        public ColorNoiseClearWindow(WpfImageViewer viewer)
        {
            InitializeComponent();

            _imageProcessingPreviewInViewer = new WpfImageProcessingPreviewInViewer(viewer);
            UseCurrentViewerZoomWhenPreviewProcessing = true;
            previewCheckBox.IsChecked = true;
        }

        #endregion



        #region Properties

        bool _isPreviewEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether the image preview in image viewer is enabled.
        /// </summary>
        [Browsable(false)]
        public bool IsPreviewEnabled
        {
            get
            {
                return _isPreviewEnabled;
            }
            set
            {
                if (IsPreviewEnabled != value)
                {
                    if (_isPreviewEnabled != value)
                    {
                        _isPreviewEnabled = value;
                        if (_isShown)
                        {
                            if (_isPreviewEnabled)
                            {
                                _imageProcessingPreviewInViewer.StartPreview();
                                ExecuteProcessing();
                            }
                            else
                            {
                                _imageProcessingPreviewInViewer.StopPreview();
                            }
                        }
                    }

                    previewCheckBox.IsChecked = value;
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value indicating whether the image processing must be previewed
        /// with zoom from image viewer.
        /// </summary>
        [Browsable(false)]
        public bool UseCurrentViewerZoomWhenPreviewProcessing
        {
            get
            {
                return _imageProcessingPreviewInViewer.UseCurrentViewerZoomWhenPreviewProcessing;
            }
            set
            {
                _imageProcessingPreviewInViewer.UseCurrentViewerZoomWhenPreviewProcessing = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the processing command need to
        /// convert the processing image to the nearest pixel format without color loss
        /// if processing command does not support pixel format
        /// of the processing image.
        /// </summary>
        [Browsable(false)]
        public bool ExpandSupportedPixelFormats
        {
            get
            {
                return _imageProcessingPreviewInViewer.ExpandSupportedPixelFormats;
            }
            set
            {
                _imageProcessingPreviewInViewer.ExpandSupportedPixelFormats = value;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Shows the image processing dialog.
        /// </summary>
        /// <returns>
        /// <b>true</b> if form is closed and OK button is pressed;
        /// <b>false</b> if form is closed and not OK button is pressed.</returns>
        public bool ShowProcessingDialog()
        {
            try
            {
                if (IsPreviewEnabled)
                {
                    _imageProcessingPreviewInViewer.StartPreview();
                    ExecuteProcessing();
                }
                _isShown = true;
                return ShowDialog().Value;
            }
            catch (ImageProcessingException ex)
            {
                DemosTools.ShowErrorMessage(ex);
                return false;
            }
            finally
            {
                if (IsPreviewEnabled)
                {
                    if (IsPreviewEnabled)
                        _imageProcessingPreviewInViewer.StopPreview();
                    _isShown = false;
                }
            }
        }

#if !REMOVE_DOCCLEANUP_PLUGIN
        /// <summary> 
        /// Returns the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public ProcessingCommandBase GetProcessingCommand()
        {
            ColorNoiseClearCommand command = new ColorNoiseClearCommand();
            command.InterpolationRadius = 1.0 - interpolationSlider.Value / interpolationSlider.Maximum;

            List<ColorSphere> colorSpheres = new List<ColorSphere>();
            if (whiteCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.White), whiteSlider.Value));
            if (blackCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Black), blackSlider.Value));
            if (redCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Red), redSlider.Value));
            if (greenCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Green), greenSlider.Value));
            if (blueCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Blue), blueSlider.Value));
            if (cyanCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Cyan), cyanSlider.Value));
            if (magentaCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Magenta), magentaSlider.Value));
            if (yellowCheckBox.IsChecked.Value)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.Yellow), yellowSlider.Value));

            if (colorSpheres.Count == 0)
                colorSpheres.Add(new ColorSphere(new Rgb24Color(System.Drawing.Color.White), 0));

            command.ColorSpheres = colorSpheres.ToArray();
            return command;
        }
#endif

        #endregion


        #region PRIVATE

        /// <summary>
        /// Executes the image processing command.
        /// </summary>
        private void ExecuteProcessing()
        {
            if (!IsInitialized)
                return;
#if !REMOVE_DOCCLEANUP_PLUGIN
            ProcessingCommandBase command = GetProcessingCommand();
            _imageProcessingPreviewInViewer.SetCommand(command);
#endif
        }

        /// <summary>
        /// 'OK' button is pressed.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// 'Cancel' button is pressed.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsPreviewEnabled = false;
            DialogResult = false;
        }

        /// <summary>
        /// Enabled/disabled the color noise removal.
        /// </summary>
        private void colorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
                return;

            whiteSlider.IsEnabled = whiteCheckBox.IsChecked.Value;
            blackSlider.IsEnabled = blackCheckBox.IsChecked.Value;
            redSlider.IsEnabled = redCheckBox.IsChecked.Value;
            greenSlider.IsEnabled = greenCheckBox.IsChecked.Value;
            blueSlider.IsEnabled = blueCheckBox.IsChecked.Value;
            cyanSlider.IsEnabled = cyanCheckBox.IsChecked.Value;
            magentaSlider.IsEnabled = magentaCheckBox.IsChecked.Value;
            yellowSlider.IsEnabled = yellowCheckBox.IsChecked.Value;
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of white sphere is changed.
        /// </summary>
        private void whiteSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            whiteLabel.Content = ((int)Math.Round(whiteSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of black sphere is changed.
        /// </summary>
        private void blackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blackLabel.Content = ((int)Math.Round(blackSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of red sphere is changed.
        /// </summary>
        private void redSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            redLabel.Content = ((int)Math.Round(redSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of green sphere is changed.
        /// </summary>
        private void greenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            greenLabel.Content = ((int)Math.Round(greenSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of blue sphere is changed.
        /// </summary>
        private void blueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blueLabel.Content = ((int)Math.Round(blueSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of cyan sphere is changed.
        /// </summary>
        private void cyanSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cyanLabel.Content = ((int)Math.Round(cyanSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of magenta sphere is changed.
        /// </summary>
        private void magentaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            magentaLabel.Content = ((int)Math.Round(magentaSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Radius of yellow sphere is changed.
        /// </summary>
        private void yellowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            yellowLabel.Content = ((int)Math.Round(yellowSlider.Value)).ToString();
            ExecuteProcessing();
        }

        /// <summary>
        /// Interpolation radius is changed.
        /// </summary>
        private void interpolationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double interpolation = interpolationSlider.Value / interpolationSlider.Maximum;
            interpolationLabel.Content = interpolation.ToString("0.00");
            ExecuteProcessing();
        }

        /// <summary>
        /// Image processing preview on image viewer is enabled/disabled.
        /// </summary>
        private void previewCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
                IsPreviewEnabled = previewCheckBox.IsChecked.Value;
        }

        #endregion

        #endregion

    }
}