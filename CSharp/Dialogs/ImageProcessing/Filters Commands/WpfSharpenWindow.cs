using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfSharpenWindow : WpfOneParamConfigWindow
    {

        #region Constructor

        public WpfSharpenWindow(WpfImageViewer viewer)
            : base(viewer, "Convolution.Sharpen",
            new WpfImageProcessingParameter("Amount", 1, 10, 1))
        {
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the amount.
        /// </summary>
        public int Amount
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
            return new SharpenCommand(Amount);
        }

        #endregion

    }
}
