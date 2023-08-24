using System.Windows;

using Vintasoft.Imaging.Media;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to change the webcam settings.
    /// </summary>
    public partial class DirectShowWebcamPropertiesWindow : Window
    {

        public DirectShowWebcamPropertiesWindow()
        {
            InitializeComponent();
        }

        public DirectShowWebcamPropertiesWindow(DirectShowCamera camera)
            : this()
        {
            directShowImageQualityPropertiesControl1.Camera = camera;
            directShowCameraControlPropertiesControl1.Camera = camera;

            this.Title = string.Format("{0} Properties (CustomUI)", camera.FriendlyName);
        }



        /// <summary>
        /// Handles the Click event of Button object.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
