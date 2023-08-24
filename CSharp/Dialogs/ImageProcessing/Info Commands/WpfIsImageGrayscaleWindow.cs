using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Info;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfImagingDemo
{
    class WpfIsImageGrayscaleWindow : WpfOneParamConfigWindow
    {

        #region Constructors

        public WpfIsImageGrayscaleWindow(WpfImageViewer viewer)
            : base(viewer,
            "Is Image Grayscale",
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
            IsImageGrayscaleCommand command = new IsImageGrayscaleCommand();
            command.MaxInaccuracy = this.Parameter1;
            return command;
        }

        #endregion

    }
}
