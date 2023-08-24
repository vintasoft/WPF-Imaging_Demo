#if !REMOVE_DOCCLEANUP_PLUGIN

using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfAutoTextInvertWindow : WpfSixParamsConfigWindow
    {

		#region Constructor

        public WpfAutoTextInvertWindow(WpfImageViewer viewer)
            : base(viewer,
            "Auto Text Invert",
            new WpfImageProcessingParameter("Min Width", 10, 500, 50),
            new WpfImageProcessingParameter("Max Width", 10, 4000, 1500),
            new WpfImageProcessingParameter("Min Height", 10, 300, 20),
            new WpfImageProcessingParameter("Max Height", 10, 400, 200),
            new WpfImageProcessingParameter("Min White (%)", 0, 49, 1),
            new WpfImageProcessingParameter("Max White (%)", 0, 49, 40))
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
            return new AutoTextInvertCommand(this.Parameter1, this.Parameter2, this.Parameter3, this.Parameter4, this.Parameter5, this.Parameter6);
        }

        #endregion

    }
}

#endif