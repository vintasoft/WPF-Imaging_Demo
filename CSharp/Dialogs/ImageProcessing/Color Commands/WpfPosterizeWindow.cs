using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;

using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfPosterizeWindow : WpfOneParamConfigWindow
    {

        #region Constructor

        public WpfPosterizeWindow(WpfImageViewer viewer)
            : base(viewer,
            "Posterize",
            new WpfImageProcessingParameter("Levels", 2, 256, 6))
        {
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the levels count.
        /// </summary>
        public int Levels
        {
            get
            {
                return Parameter1;
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
            return new PosterizeCommand(Levels);
        }

        #endregion

    }
}
