using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfMeanWindow : WpfOneParamConfigWindow
    {

		#region Constructor

        public WpfMeanWindow(WpfImageViewer viewer)
            : base(viewer, "Arithmetic.Mean",
            new WpfImageProcessingParameter("Window Radius", 2, 16, 3))
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
            return new MeanCommand(WindowSize);
        }

        #endregion

    }
}
