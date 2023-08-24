using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfRedEyeRemovalWindow : WpfTwoParamsConfigWindow
    {

        #region Constructor

        public WpfRedEyeRemovalWindow(WpfImageViewer viewer)
            : base(viewer,
            "Red Eye Removal",
            new WpfImageProcessingParameter("Tolerance", 35, 100, 70),
            new WpfImageProcessingParameter("Saturation (percents)", 50, 100, 90))
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
            return new RedEyeRemovalCommand(this.Parameter1, this.Parameter2);
        }

        #endregion
    }
}
