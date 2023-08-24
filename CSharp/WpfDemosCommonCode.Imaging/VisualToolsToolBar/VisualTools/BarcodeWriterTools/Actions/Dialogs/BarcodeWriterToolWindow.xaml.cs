using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// A window for barcode writer tool.
    /// </summary>
    public partial class BarcodeWriterToolWindow : Window
    {

       


        public BarcodeWriterToolWindow()
        {
            InitializeComponent();
        }

        public BarcodeWriterToolWindow(WpfBarcodeWriterTool writerTool)
            : this()
        {
#if !REMOVE_BARCODE_SDK
            barcodeWriterSettingsControl.BarcodeWriterSettings = writerTool.WriterSettings;
            barcodeWriterSettingsControl.CanChangeBarcodeSize = false; 
#endif
        }



        public BitmapSource BarcodeImage
        {
            set
            {
#if !REMOVE_BARCODE_SDK
                barcodeWriterSettingsControl.BarcodeImage = value; 
#endif
            }
        }

    }
}
