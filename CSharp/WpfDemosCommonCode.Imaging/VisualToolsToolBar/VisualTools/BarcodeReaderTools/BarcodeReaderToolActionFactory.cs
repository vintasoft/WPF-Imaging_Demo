using System.Windows.Media.Imaging;

using WpfDemosCommonCode.Imaging;

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Creates visual tool action, which allows to enable/disable visual tool <see cref="WpfBarcodeReaderTool"/> in image viewer, and adds action to the toolstrip.
    /// </summary>    
    public class BarcodeReaderToolActionFactory
    {

        #region Methods

        #region PUBLIC

        /// <summary>
        /// Creates visual tool action, which allows to enable/disable visual tool <see cref="WpfBarcodeReaderTool"/> in image viewer, and adds action to the toolstrip.
        /// </summary>
        /// <param name="toolBar">The toolbar, where actions must be added.</param>
        public static void CreateActions(VisualToolsToolBar toolBar)
        {
#if !REMOVE_BARCODE_SDK
            // create action, which allows to enable the barcode reader tool in image viewer
            BarcodeReaderToolAction barcodeReaderToolAction = new BarcodeReaderToolAction(
                new WpfBarcodeReaderTool(),
                "Barcode Reader",
                "Barcode Reader",
                GetIcon("BarcodeReaderTool.png"));
            // add the action to the toolstrip
            toolBar.AddAction(barcodeReaderToolAction); 
#endif
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Returns the visual tool icon of specified name.
        /// </summary>
        /// <param name="iconName">The visual tool icon name.</param>
        /// <returns>
        /// The visual tool icon.
        /// </returns>
        private static BitmapSource GetIcon(string iconName)
        {
            string iconPath =
                string.Format("WpfDemosCommonCode.Imaging.VisualToolsToolBar.VisualTools.BarcodeReaderTools.Resources.{0}", iconName);

            return DemosResourcesManager.GetResourceAsBitmap(iconPath);
        }

        #endregion

        #endregion
    }
}
