using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfMotionBlurWindow : WpfTwoParamsConfigWindow
    {

        #region Constructor

        public WpfMotionBlurWindow(WpfImageViewer viewer)
            : base(viewer,
            "Motion blur",
            new WpfImageProcessingParameter("Direction", 0, 180, 45),
            new WpfImageProcessingParameter("Depth", 1, 10, 4))
        {
        }

        #endregion



        #region Methods

        /// <summary>
        /// Gets the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public override ProcessingCommandBase GetProcessingCommand()
        {
            return new MotionBlurCommand(Parameter1, Parameter2);
        }

        #endregion

    }
}
