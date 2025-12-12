using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.ImageProcessing.Transforms;
using Vintasoft.Imaging.Media;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to preview image from webcam, change webcam settings and
    /// process the captured images.
    /// </summary>
    public partial class WebcamPreviewWindow : Window
    {

        #region Fields

        /// <summary>
        /// Available image capture device monitor.
        /// </summary>
        ImageCaptureDevicesMonitor _imageCaptureDeviceMonitor;

        /// <summary>
        /// Image capture device.
        /// </summary>
        ImageCaptureDevice _imageCaptureDevice;

        /// <summary>
        /// Image capture source.
        /// </summary>
        ImageCaptureSource _imageCaptureSource;

        /// <summary>
        /// Image capture timeout.
        /// </summary>
        int _imageCaptureTimeout = 0;

        /// <summary>
        /// Processing command that uses to process current frame.
        /// </summary>
        ProcessingCommandBase _processingCommand;

        /// <summary>
        /// Indicates when image was painted.
        /// </summary>
        bool _imagePainted = false;

        #endregion



        #region Constructor

        public WebcamPreviewWindow()
        {
            InitializeComponent();
        }

        public WebcamPreviewWindow(ImageCaptureDevice device)
            : this()
        {
            _imageCaptureDevice = device;
            Title = device.FriendlyName;

            _imageCaptureSource = new ImageCaptureSource();
            _imageCaptureSource.CaptureCompleted += new EventHandler<ImageCaptureCompletedEventArgs>(CaptureSource_CaptureCompleted);
            _imageCaptureSource.CaptureDevice = _imageCaptureDevice;

            _imageCaptureDeviceMonitor = new ImageCaptureDevicesMonitor();
            _imageCaptureDeviceMonitor.CaptureDevicesChanged += new EventHandler<ImageCaptureDevicesChangedEventArgs>(DeviceMonitor_CaptureDevicesChanged);

            HashSet<string> imageCaptureFormatKeys = new HashSet<string>();
            // for each supported format
            for (int i = 0; i < _imageCaptureDevice.SupportedFormats.Count; i++)
            {
                ImageCaptureFormat captureFormat = _imageCaptureDevice.SupportedFormats[i];

                // if format has bit depth less or equal than 12 bit
                if (captureFormat.BitsPerPixel <= 12)
                    // ignore formats with bit depth less or equal than 12 bit because they may cause issues on Windows 8
                    continue;
                string imageCaptureFormatSKey = captureFormat.Width + "X" + captureFormat.Height + " " + captureFormat.FramesPerSecond;
                if (!imageCaptureFormatKeys.Contains(imageCaptureFormatSKey))
                {
                    imageCaptureFormatKeys.Add(imageCaptureFormatSKey);
                    formatsComboBox.Items.Add(captureFormat);
                }
            }

            formatsComboBox.SelectedItem = _imageCaptureDevice.DesiredFormat;

            invertComboBox.SelectedIndex = 0;

            videoPreviewImageViewer.ImagePainted += VideoPreviewImageViewer_ImagePainted;
        }

        #endregion



        #region Properties

        WpfImageViewer _snapshotViewer;
        /// <summary>
        /// Gets or sets an image viewer that stores images captured from webcam.
        /// </summary>
        public WpfImageViewer SnapshotViewer
        {
            get
            {
                return _snapshotViewer;
            }
            set
            {
                _snapshotViewer = value;
                if (_snapshotViewer != null)
                    captureImageButton.Visibility = Visibility.Visible;
                else
                    captureImageButton.Visibility = Visibility.Hidden;
            }
        }

        #endregion



        #region Methods

        #region Common

        /// <summary>
        /// Changes frame delay.
        /// </summary>
        private void frameDelayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _imageCaptureTimeout = (int)frameDelayNumericUpDown.Value;
        }

        /// <summary>
        /// Shows camera properties dialog (default UI).
        /// </summary>
        private void propertiesDefaultUiButton_Click(object sender, RoutedEventArgs e)
        {
            _imageCaptureDevice.ShowPropertiesDialog();
        }

        /// <summary>
        /// Shows camera properties dialog (custom UI).
        /// </summary>
        private void propertiesCustomUiButton_Click(object sender, RoutedEventArgs e)
        {
            DirectShowWebcamPropertiesWindow dlg = new DirectShowWebcamPropertiesWindow((DirectShowCamera)_imageCaptureDevice);
            dlg.ShowDialog();
        }

        /// <summary>
        /// Captures image from webcam and adds image into image viewer (SnapshotViewer).
        /// </summary>
        private void captureImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (SnapshotViewer != null)
            {
                VintasoftImage image = videoPreviewImageViewer.Image;
                if (image != null)
                {
                    SnapshotViewer.Images.Add((VintasoftImage)image.Clone());
                    SnapshotViewer.FocusedIndex = SnapshotViewer.Images.Count - 1;
                }
            }
        }

        #endregion


        #region Camera

        /// <summary>
        /// Handles the ImagePainted event of the VideoPreviewImageViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ImageLoadedEventArgs"/> instance containing the event data.</param>
        private void VideoPreviewImageViewer_ImagePainted(object sender, ImageLoadedEventArgs e)
        {
            _imagePainted = true;
        }

        /// <summary>
        /// Starts capturing from camera.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // start the device monitor
                _imageCaptureDeviceMonitor.Start();
                // start the capture source
                _imageCaptureSource.Start();
                // initialize new image capture request
                _imageCaptureSource.CaptureAsync();
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
                Close();
            }
        }


        /// <summary>
        /// Image is captured.
        /// </summary>       
        private void CaptureSource_CaptureCompleted(object sender, ImageCaptureCompletedEventArgs e)
        {
            // if previous image was painted
            if (videoPreviewImageViewer.Image == null || _imagePainted)
            {
                // get captured image
                VintasoftImage newImage = e.GetCapturedImage();

                // apply processing
                if (_processingCommand != null)
                    _processingCommand.ExecuteInPlace(newImage);

                // show captured image in the preview viewer           
                _imagePainted = false;
                if (videoPreviewImageViewer.Image != null)
                    videoPreviewImageViewer.Image.SetImage(newImage);
                else
                    videoPreviewImageViewer.Image = newImage;
            }

            // if capture source is started
            if (_imageCaptureSource.State == ImageCaptureState.Started)
            {
                // sleep...
                if (_imageCaptureTimeout > 0)
                    Thread.Sleep(_imageCaptureTimeout);
                // if capture source is started
                if (_imageCaptureSource.State == ImageCaptureState.Started)
                    // initialize new image capture request
                    _imageCaptureSource.CaptureAsync();
            }
        }

        /// <summary>
        /// Changes video format.
        /// </summary>
        private void formatsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            propertiesDefaultUiButton.IsEnabled = false;
            captureImageButton.IsEnabled = false;

            if (_imageCaptureSource.State == ImageCaptureState.Started)
            {
                try
                {
                    // stop capture source
                    _imageCaptureSource.Stop();

                    // change desired format
                    try
                    {
                        _imageCaptureDevice.DesiredFormat = (ImageCaptureFormat)formatsComboBox.SelectedItem;
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }

                    // start capture source
                    _imageCaptureSource.Start();

                    // initialize new image capture request
                    _imageCaptureSource.CaptureAsync();
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }

            propertiesDefaultUiButton.IsEnabled = true;
            captureImageButton.IsEnabled = true;
        }

        /// <summary>
        /// List of available capturing devices is changed.
        /// </summary>
        private void DeviceMonitor_CaptureDevicesChanged(object sender, ImageCaptureDevicesChangedEventArgs e)
        {
            // if current capture device is removed then
            if (Array.IndexOf(e.RemovedDevices, _imageCaptureDevice) >= 0)
            {
                // close form
                Dispatcher.Invoke(new ThreadStart(Close));
            }
        }


        /// <summary>
        /// Handles the Closed event of Window object.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // stop monitor and capture source
            _imageCaptureDeviceMonitor.Stop();
            _imageCaptureSource.Stop();
        }

        #endregion


        #region Captured Image Processing

        /// <summary>
        /// Builds proccessing command to process current captured frame.
        /// </summary>
        private void BuildProcessingCommand()
        {
            List<ProcessingCommandBase> commands = new List<ProcessingCommandBase>();

            // Invert
            switch (invertComboBox.SelectedIndex)
            {
                case 1:
                    commands.Add(new InvertCommand());
                    break;
                case 2:
                    commands.Add(new ColorBlendCommand(BlendingMode.Difference, System.Drawing.Color.FromArgb(255, 0, 0)));
                    break;
                case 3:
                    commands.Add(new ColorBlendCommand(BlendingMode.Difference, System.Drawing.Color.FromArgb(0, 255, 0)));
                    break;
                case 4:
                    commands.Add(new ColorBlendCommand(BlendingMode.Difference, System.Drawing.Color.FromArgb(0, 0, 255)));
                    break;
            }

            // Grayscale
            if (grayscaleCheckBox.IsChecked.Value)
                commands.Add(new ChangePixelFormatToGrayscaleCommand(PixelFormat.Gray8));

            // Rotate
            int rotate;
            if (int.TryParse(rotateComboBox.Text, out rotate))
            {
                if (rotate != 0)
                {
                    RotateCommand rotateCommand = new RotateCommand(rotate);
                    rotateCommand.BorderColorType = BorderColorType.Black;
                    commands.Add(rotateCommand);
                }
            }

            if (commands.Count == 0)
                _processingCommand = null;
            else if (commands.Count == 1)
                _processingCommand = commands[0];
            else
                _processingCommand = new CompositeCommand(commands.ToArray());
        }

        /// <summary>
        /// Handles the SelectionChanged event of invertComboBox object.
        /// </summary>
        private void invertComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized)
                BuildProcessingCommand();
        }

        /// <summary>
        /// Handles the TextChanged event of rotateComboBox object.
        /// </summary>
        private void rotateComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            invertComboBox_SelectionChanged(sender, null);
        }

        /// <summary>
        /// Handles the CheckChanged event of grayscaleCheckBox object.
        /// </summary>
        private void grayscaleCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            invertComboBox_SelectionChanged(sender, null);
        }

        #endregion

        #endregion

    }
}
