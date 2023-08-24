using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfBrightnessContrastWindow : WpfTwoParamsConfigWindow
    {

		#region Constructor

        public WpfBrightnessContrastWindow(WpfImageViewer viewer)
			: base(viewer, 
            "Brightness / contrast",
            new WpfImageProcessingParameter("Brightness", -100, 100, 0),
            new WpfImageProcessingParameter("Contrast", -100, 100, 0))
		{
		}

		#endregion



		#region Properties

        /// <summary>
        /// Gets the brightness value.
        /// </summary>
		public int Brightness
		{
			get
			{
				return Parameter1;
			}
		}

        /// <summary>
        /// Gets the contrast value.
        /// </summary>
		public int Contrast
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
        public override Vintasoft.Imaging.ImageProcessing.ProcessingCommandBase GetProcessingCommand()
        {
            return new ChangeBrightnessContrastCommand(Brightness, Contrast);
        }

        #endregion
    }
}
