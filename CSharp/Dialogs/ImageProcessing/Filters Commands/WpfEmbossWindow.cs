using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfEmbossWindow : WpfTwoParamsConfigWindow
    {

		#region Constructor

        public WpfEmbossWindow(WpfImageViewer viewer)
            : base(viewer,
            "Convolution.Emboss",
            new WpfImageProcessingParameter("Light direction", 0, 359, 45),
            new WpfImageProcessingParameter("Depth", 1, 10, 1))
        {
        }
		
        #endregion



		#region Properties

        /// <summary>
        /// Gets the radius value.
        /// </summary>
        public int LightDirection
		{
			get
			{
				return Parameter1;
			}
		}

        /// <summary>
        /// Gets the intensity levels value.
        /// </summary>
        public int Depth
		{
			get
			{
				return Parameter2;
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
            return new EmbossCommand(LightDirection, Depth);
        }

        #endregion

    }
}
