using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Info;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfImagingDemo
{
    class WpfGetImageColorDepthWindow : WpfOneParamConfigWindow
    {

        #region Constructors

        public WpfGetImageColorDepthWindow(WpfImageViewer viewer)
            : base(viewer,
            "Get Image Color Depth",
            new WpfImageProcessingParameter("Max Inaccuracy", 0, 128, 0))
        {
            CanPreview = false;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Gets the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public override ProcessingCommandBase GetProcessingCommand()
        {
            GetImageColorDepthCommand command = new GetImageColorDepthCommand();
            command.MaxInaccuracy = this.Parameter1;
            return command;
        }

        #endregion

    }
}
