using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfMaximumWindow : WpfOneParamConfigWindow
    {

		#region Constructor

        public WpfMaximumWindow(WpfImageViewer viewer)
            : base(viewer, "Arithmetic.Maximum",
            new WpfImageProcessingParameter("Window Radius", 1, 16, 3))
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
            return new MaximumCommand(WindowSize);
        }

        #endregion

    }
}
