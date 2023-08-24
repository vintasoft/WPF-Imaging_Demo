using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfBevelEdgeWindow : WpfSixParamsConfigWindow
    {

		#region Constructor

        public WpfBevelEdgeWindow(WpfImageViewer viewer)
            : base(viewer,
            "Bevel Edge",
            new WpfImageProcessingParameter("Edge size", 0, 30, 6),
            new WpfImageProcessingParameter("Smoothness", 0, 20, 2),
            new WpfImageProcessingParameter("Left brightness", -100, 100, 30),
            new WpfImageProcessingParameter("Right brightness", -100, 100, -20),
            new WpfImageProcessingParameter("Top brightness", -100, 100, 30),
            new WpfImageProcessingParameter("Bottom brightness", -100, 100, -20))
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
            return new BevelEdgeCommand(this.Parameter1, this.Parameter2, this.Parameter3, this.Parameter4, this.Parameter5, this.Parameter6);
        }

        #endregion

    }
}
