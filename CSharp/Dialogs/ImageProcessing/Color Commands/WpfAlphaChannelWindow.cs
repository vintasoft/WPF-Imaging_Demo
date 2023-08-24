using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;

using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    public class WpfAlphaChannelWindow : WpfOneParamConfigWindow
    {

        #region Constructor

        public WpfAlphaChannelWindow(WpfImageViewer viewer)
            : base(viewer,
            "Alpha channel",
            new WpfImageProcessingParameter("Alpha value", 0, 255, 255))
        {
            CanPreview = false;
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the alpha value.
        /// </summary>
        public byte Alpha
        {
            get
            {
                return (byte)Parameter1;
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
            return new SetAlphaChannelValueCommand(Alpha);
        }

        #endregion

    }
}
