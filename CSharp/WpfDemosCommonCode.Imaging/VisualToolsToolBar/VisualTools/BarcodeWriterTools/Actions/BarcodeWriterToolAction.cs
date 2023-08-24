#if !REMOVE_BARCODE_SDK
using System;
using System.Windows;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Wpf;
using Vintasoft.Barcode;

using WpfDemosCommonCode.Imaging;


namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Contains information about <see cref="WpfBarcodeWriterTool"/>, which is used in <see cref="VisualToolsToolBar"/>.
    /// </summary>
    public class BarcodeWriterToolAction : VisualToolAction
    {

#region Fields

        /// <summary>
        /// A form that allows to set the barcode writer settings.
        /// </summary>
        Window _barcodeWriterToolWindow;

#endregion



#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeWriterToolItem"/> class.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subactions">The sub-actions of the action.</param>
        public BarcodeWriterToolAction(
            WpfBarcodeWriterTool visualTool,
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subactions)
            : base(visualTool, text, toolTip, icon, subactions)
        {
            visualTool.WriterSettings.Barcode = BarcodeType.Code128;
            visualTool.WriterSettings.PixelFormat = BarcodeImagePixelFormat.Bgra32;
            visualTool.WriterSettings.Value = "0123456789";
            visualTool.WriterSettings.Padding = 4;
            visualTool.Rectangle = new Rect(0, 0, 120, 40);
            visualTool.WriterSettings.Changed += new EventHandler(WriterSettings_Changed);
        }

#endregion



#region Methods

#region PUBLIC

        /// <summary>
        /// Activates this item.
        /// </summary>
        public override void Activate()
        {
            base.Activate();

            if (_barcodeWriterToolWindow == null)
            {
                _barcodeWriterToolWindow = new BarcodeWriterToolWindow((WpfBarcodeWriterTool)VisualTool);
                _barcodeWriterToolWindow.Owner = Application.Current.MainWindow;
                _barcodeWriterToolWindow.Topmost = true;
                _barcodeWriterToolWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                _barcodeWriterToolWindow.Left = 5;
                _barcodeWriterToolWindow.Top = 70;
                _barcodeWriterToolWindow.Show();
            }
        }

        /// <summary>
        /// Deactivates this item.
        /// </summary>
        public override void Deactivate()
        {
            if (_barcodeWriterToolWindow != null)
            {
                _barcodeWriterToolWindow.Close();
                _barcodeWriterToolWindow = null;
            }

            base.Deactivate();
        }

#endregion


#region PRIVATE

        /// <summary>
        /// Updates barcode image.
        /// </summary>
        private void WriterSettings_Changed(object sender, EventArgs e)
        {
            if (_barcodeWriterToolWindow == null)
                return;
            BarcodeWriterToolWindow window = _barcodeWriterToolWindow as BarcodeWriterToolWindow;
            try
            {
                WpfBarcodeWriterTool tool = (WpfBarcodeWriterTool)VisualTool;
                using (VintasoftImage barcodeImage = tool.GetBarcodeImage())
                {
                    window.BarcodeImage = VintasoftImageConverter.ToBitmapSource(barcodeImage);
                }
            }
            catch (WriterSettingsException)
            {
                window.BarcodeImage = null;
            }
        }

#endregion

#endregion

    }
}

#endif