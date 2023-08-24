using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfOilPaintingWindow : WpfTwoParamsConfigWindow
    {

        #region Constructor

        public WpfOilPaintingWindow(WpfImageViewer viewer)
            : base(viewer,
            "Oil painting",
            new WpfImageProcessingParameter("Radius", 1, 32, 3),
            new WpfImageProcessingParameter("Intensity levels", 3, 255, 50))
        {
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the radius value.
        /// </summary>
        public int Radius
        {
            get
            {
                return Parameter1;
            }
        }

        /// <summary>
        /// Gets the intensity levels value.
        /// </summary>
        public int IntensityLevels
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
            return new OilPaintingCommand(Radius, IntensityLevels);
        }

        #endregion

    }
}
