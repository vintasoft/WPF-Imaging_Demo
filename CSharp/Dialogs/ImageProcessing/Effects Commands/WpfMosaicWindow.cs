using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfMosaicWindow : WpfOneParamConfigWindow
    {

		#region Constructor

        public WpfMosaicWindow(WpfImageViewer viewer)
			: base(viewer,
            "Mosaic",
            new WpfImageProcessingParameter("Tile size (pixels)", 2, 100, 4))
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
            return new MosaicCommand(Parameter1);
        }

        #endregion

    }
}
