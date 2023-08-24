using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfGaussianBlurWindow : WpfOneParamConfigWindow
    {

        #region Constructor

        public WpfGaussianBlurWindow(WpfImageViewer viewer)
            : base(viewer, "Convolution.GaussianBlur",
            new WpfImageProcessingParameter("Window Radius", 1, 20, 3))
        {
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the window size.
        /// </summary>
        public int WindowSize
        {
            get
            {
                return Parameter1 * 2 - 1;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Gets the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public override ProcessingCommandBase GetProcessingCommand()
        {
            return new GaussianBlurCommand(WindowSize);
        }

        #endregion

    }
}
