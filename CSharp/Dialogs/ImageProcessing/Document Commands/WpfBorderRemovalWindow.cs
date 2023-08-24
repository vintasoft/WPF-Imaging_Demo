using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfBorderRemovalWindow : WpfTwoParamsConfigWindow
    {

		#region Constructor

        public WpfBorderRemovalWindow(WpfImageViewer viewer)
            : base(viewer,
            "Border removal",
            new WpfImageProcessingParameter("BorderSize", 0, 100, 5),
            new WpfImageProcessingParameter("Max border noise", 0, 100, 0))
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
            BorderRemovalCommand borderRemoval = new BorderRemovalCommand();
            borderRemoval.BorderSize = Parameter1;
            borderRemoval.MaxBorderNoise = Parameter2;
            return borderRemoval;
        }

        #endregion

    }
}
