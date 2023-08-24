using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Info;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfImagingDemo
{
    class WpfIsImageBlackWhiteWindow : WpfOneParamConfigWindow
    {

        #region Constructors

        public WpfIsImageBlackWhiteWindow(WpfImageViewer viewer)
            : base(viewer,
            "Is Image Black-White",
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
            IsImageBlackWhiteCommand command = new IsImageBlackWhiteCommand();
            command.MaxInaccuracy = this.Parameter1;
            return command;
        }

        #endregion

    }
}
