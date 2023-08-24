using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfGammaWindow : WpfThreeParamsConfigWindow
    {

        #region Constructor

        public WpfGammaWindow(WpfImageViewer viewer)
            : base(viewer,
            "Gamma",
            new WpfImageProcessingParameter("Red gamma (percents)", 20, 500, 100),
            new WpfImageProcessingParameter("Green gamma (percents)", 20, 500, 100),
            new WpfImageProcessingParameter("Blue gamma (percents)", 20, 500, 100))
        {
        }

        #endregion



        #region Properties

        public double Red
        {
            get
            {
                return (double)Parameter1 / 100;
            }
        }
        public double Green
        {
            get
            {
                return (double)Parameter2 / 100;
            }
        }
        public double Blue
        {
            get
            {
                return (double)Parameter3 / 100;
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
            return new ChangeGammaCommand(Red, Green, Blue);
        }

        #endregion

    }
}
