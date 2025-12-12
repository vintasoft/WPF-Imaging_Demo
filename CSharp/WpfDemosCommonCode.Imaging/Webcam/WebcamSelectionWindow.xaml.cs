using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using Vintasoft.Imaging.Media;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to select the webcam from webcam list.
    /// </summary>
    public partial class WebcamSelectionWindow : Window
    {

        #region Constructor

        public WebcamSelectionWindow()
        {
            InitializeComponent();

            ReadOnlyCollection<ImageCaptureDevice> devices = ImageCaptureDeviceConfiguration.GetCaptureDevices();
            foreach (ImageCaptureDevice device in devices)
                devicesComboBox.Items.Add(device);

            devicesComboBox.SelectedIndex = 0;
        }

        #endregion



        #region Properties

        public ImageCaptureDevice SelectedWebcam
        {
            get
            {
                return (ImageCaptureDevice)devicesComboBox.SelectedItem;
            }
            set
            {
                if (value != null)
                {
                    devicesComboBox.SelectedItem = value;
                }
            }
        }

        public ImageCaptureFormat SelectedFormat
        {
            get
            {
                return (ImageCaptureFormat)videoFormatComboBox.SelectedItem;
            }
        }

        public bool CanSelectFormat
        {
            get
            {
                return videoFormatLabel.Visibility == Visibility.Visible;
            }
            set
            {
                Visibility visibility = value ? Visibility.Visible : Visibility.Hidden;
                videoFormatLabel.Visibility = visibility;
                videoFormatComboBox.Visibility = visibility;
            }
        }

        #endregion



        #region Methods

        public static ImageCaptureDevice SelectWebcam()
        {
            ReadOnlyCollection<ImageCaptureDevice> devices = ImageCaptureDeviceConfiguration.GetCaptureDevices();
            if (devices.Count == 0)
                throw new Exception("Webcam is not found.");
            if (devices.Count == 1)
                return devices[0];

            WebcamSelectionWindow window = new WebcamSelectionWindow();
            window.CanSelectFormat = false;
            if (window.ShowDialog().Value)
            {
                return window.SelectedWebcam;
            }
            return null;
        }

        /// <summary>
        /// Handles the SelectionChanged event of devicesComboBox object.
        /// </summary>
        private void devicesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            videoFormatComboBox.Items.Clear();
            ImageCaptureDevice selectedWebcam = null;
            if (e.AddedItems != null && e.AddedItems.Count > 0)
                selectedWebcam = e.AddedItems[0] as ImageCaptureDevice;

            if (selectedWebcam != null)
            {
                // list with image formats
                HashSet<string> imageCaptureFormatKeys = new HashSet<string>();
                // for each image format in webcam supported formats
                for (int i = 0; i < SelectedWebcam.SupportedFormats.Count; i++)
                {
                    ImageCaptureFormat captureFormat = SelectedWebcam.SupportedFormats[i];

                    // if format has bit depth less or equal than 12 bit
                    if (captureFormat.BitsPerPixel <= 12)
                        // ignore formats with bit depth less or equal than 12 bit because they may cause issues on Windows 8
                        continue;

                    string imageCaptureFormatSKey = captureFormat.Width + "X" + captureFormat.Height + " " + captureFormat.FramesPerSecond;
                    if (!imageCaptureFormatKeys.Contains(imageCaptureFormatSKey))
                    {
                        imageCaptureFormatKeys.Add(imageCaptureFormatSKey);
                        videoFormatComboBox.Items.Add(captureFormat);
                    }
                }

                if (selectedWebcam.DesiredFormat != null && videoFormatComboBox.Items.Contains(selectedWebcam.DesiredFormat))
                    videoFormatComboBox.SelectedItem = selectedWebcam.DesiredFormat;
                else
                    videoFormatComboBox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
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

    }
}
