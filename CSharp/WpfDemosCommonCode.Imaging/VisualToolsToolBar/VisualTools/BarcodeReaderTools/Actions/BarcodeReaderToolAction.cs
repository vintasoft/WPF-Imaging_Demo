#if !REMOVE_BARCODE_SDK
using System.Windows;
using System.Windows.Media.Imaging;

using Vintasoft.Barcode;

using WpfDemosCommonCode.Imaging;

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Contains information about <see cref="WpfBarcodeReaderTool"/>, which is used in <see cref="VisualToolsToolBar"/>.
    /// </summary>
    public class BarcodeReaderToolAction : VisualToolAction
    {

        #region Fields

        /// <summary>
        /// A form that allows to set the barcode reader settings and
        /// see the barcode reading results.
        /// </summary>
        Window _barcodeReaderToolWindow;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeReaderToolAction"/> class.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subactions">The sub-actions of the action.</param>
        public BarcodeReaderToolAction(
            WpfBarcodeReaderTool visualTool,
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subactions)
            : base(visualTool, text, toolTip, icon, subactions)
        {
            visualTool.ReaderSettings.AutomaticRecognition = true;
            visualTool.ReaderSettings.ScanDirection = ScanDirection.Horizontal | ScanDirection.Vertical;
            visualTool.ReaderSettings.ScanBarcodeTypes = BarcodeType.Code128 | BarcodeType.Code39;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Activates this item.
        /// </summary>
        public override void Activate()
        {
            base.Activate();

            if (_barcodeReaderToolWindow == null)
            {
                _barcodeReaderToolWindow = new BarcodeReaderToolWindow((WpfBarcodeReaderTool)VisualTool);
                _barcodeReaderToolWindow.Owner = Application.Current.MainWindow;
                _barcodeReaderToolWindow.Topmost = true;
                _barcodeReaderToolWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                _barcodeReaderToolWindow.Left = 5;
                _barcodeReaderToolWindow.Top = 70;
                _barcodeReaderToolWindow.Show();
            }
        }

        /// <summary>
        /// Deactivates this item.
        /// </summary>
        public override void Deactivate()
        {
            if (_barcodeReaderToolWindow != null)
            {
                _barcodeReaderToolWindow.Close();
                _barcodeReaderToolWindow = null;
            }

            base.Deactivate();
        }

        #endregion

    }
}
#endif
