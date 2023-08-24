using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfPixelateWindow : WpfOneParamConfigWindow
    {

        #region Constructor

        public WpfPixelateWindow(WpfImageViewer viewer)
            : base(viewer,
            "Pixelate",
            new WpfImageProcessingParameter("Cell size (pixels)", 2, 100, 4))
        {
        }

        #endregion



        #region Properties

        public int CellSize
        {
            get
            {
                return Parameter1;
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
            return new PixelateCommand(CellSize);
        }

        #endregion

    }
}
