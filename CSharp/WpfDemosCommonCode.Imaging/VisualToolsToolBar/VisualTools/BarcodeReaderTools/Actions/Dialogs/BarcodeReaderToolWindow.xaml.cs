using System;
using System.Text;
using System.Windows;

#if !REMOVE_BARCODE_SDK
using Vintasoft.Barcode;
using Vintasoft.Barcode.BarcodeInfo;
using Vintasoft.Barcode.GS1;
#endif

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// A windows for barcode reader tool.
    /// </summary>
    public partial class BarcodeReaderToolWindow : Window
    {

        #region Fields

        WpfBarcodeReaderTool _readerTool;

        #endregion



        #region Constructors

        public BarcodeReaderToolWindow()
        {
            InitializeComponent();

#if !REMOVE_BARCODE_SDK
            recognitionResultsTextBox.AppendText(string.Format("VintaSoftBarcode.NET SDK v{0} WPF edition", Vintasoft.Barcode.BarcodeGlobalSettings.AssemblyVersion));
            if (Vintasoft.Barcode.BarcodeGlobalSettings.IsDemoVersion)
                recognitionResultsTextBox.AppendText("\n(Demo Version)"); 
#endif
        }

        public BarcodeReaderToolWindow(WpfBarcodeReaderTool readerTool)
            : this()
        {
            _readerTool = readerTool;
            _readerTool.RecognitionStarted += new EventHandler(readerTool_RecognitionStarted);
#if !REMOVE_BARCODE_SDK
            _readerTool.RecognitionProgress += new EventHandler<BarcodeReaderProgressEventArgs>(readerTool_RecognitionProgress); 
#endif
            _readerTool.RecognitionFinished += new EventHandler(readerTool_RecognitionFinished);
#if !REMOVE_BARCODE_SDK
            barcodeReaderSettingsControl1.RestoreSettings(_readerTool.ReaderSettings); 
#endif
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the RecognitionFinished event of ReaderTool object.
        /// </summary>
        private void readerTool_RecognitionFinished(object sender, EventArgs e)
        {
            recognizeBarcodeButton.IsEnabled = true;
            recognitionResultsTextBox.Text = GetRecognitionResults();
        }

        private string GetRecognitionResults()
        {
#if !REMOVE_BARCODE_SDK
            if (_readerTool.RecognitionResults != null)
            {
                if (_readerTool.RecognitionResults.Length == 0)
                    return string.Format("No barcodes found. ({0} ms)", _readerTool.RecognizeTime.TotalMilliseconds);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0} barcode found. ({1}ms)", _readerTool.RecognitionResults.Length, _readerTool.RecognizeTime.TotalMilliseconds));
                sb.AppendLine();
                for (int i = 0; i < _readerTool.RecognitionResults.Length; i++)
                    sb.AppendLine(GetBarcodeInfo(i, _readerTool.RecognitionResults[i])); 
            return sb.ToString();
            }
#endif
            return "";
        }

#if !REMOVE_BARCODE_SDK
        private string GetBarcodeInfo(int index, IBarcodeInfo info)
        {
            info.ShowNonDataFlagsInValue = true;

            string value = info.Value;
            if (info.BarcodeType == BarcodeType.UPCE)
                value += string.Format(" ({0})", (info as UPCEANInfo).UPCEValue);

            string confidence;
            if (info.Confidence == ReaderSettings.ConfidenceNotAvailable)
                confidence = "N/A";
            else
                confidence = Math.Round(info.Confidence).ToString() + "%";

            if (info is BarcodeSubsetInfo)
            {
                value = string.Format("{0}{1}Base value: {2}",
                    RemoveSpecialCharacters(value), Environment.NewLine,
                    RemoveSpecialCharacters(((BarcodeSubsetInfo)info).BaseBarcodeInfo.Value));
            }
            else
            {
                value = RemoveSpecialCharacters(value);
            }

            string barcodeTypeValue;
            if (info is BarcodeSubsetInfo)
                barcodeTypeValue = ((BarcodeSubsetInfo)info).BarcodeSubset.ToString();
            else
                barcodeTypeValue = info.BarcodeType.ToString();


            StringBuilder result = new StringBuilder();
            result.AppendLine(string.Format("[{0}:{1}]", index + 1, barcodeTypeValue));
            result.AppendLine(string.Format("Value: {0}", value));
            result.AppendLine(string.Format("Confidence: {0}", confidence));
            result.AppendLine(string.Format("Reading quality: {0}", info.ReadingQuality));
            result.AppendLine(string.Format("Threshold: {0}", info.Threshold));
            result.AppendLine(string.Format("Region: {0}", info.Region));
            return result.ToString();
        } 
#endif

        private string GetGS1BarcodeValue(string value)
        {
#if !REMOVE_BARCODE_SDK
            GS1ApplicationIdentifierValue[] ai = GS1Codec.Decode(value);
            if (ai == null)
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ai.Length; i++)
                sb.Append(ai[i].ToString());
            return sb.ToString(); 
#else
            return string.Empty;
#endif
        }

        private string RemoveSpecialCharacters(string text)
        {
            StringBuilder sb = new StringBuilder();
            if (text != null)
                for (int i = 0; i < text.Length; i++)
                    if (text[i] >= ' ' || text[i] == '\n' || text[i] == '\r' || text[i] == '\t')
                        sb.Append(text[i]);
                    else
                        sb.Append('?');
            return sb.ToString();
        }

        /// <summary>
        /// Handles the RecognitionStarted event of ReaderTool object.
        /// </summary>
        private void readerTool_RecognitionStarted(object sender, EventArgs e)
        {
            recognizeBarcodeButton.IsEnabled = false;
#if !REMOVE_BARCODE_SDK
            barcodeReaderSettingsControl1.SetReaderSettings(_readerTool.ReaderSettings); 
#endif
        }

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Handles the RecognitionProgress event of ReaderTool object.
        /// </summary>
        private void readerTool_RecognitionProgress(object sender, BarcodeReaderProgressEventArgs e)
        {
            recognitionProgressBar.Value = e.Progress;
        } 
#endif

        /// <summary>
        /// Handles the Click event of RecognizeBarcodeButton object.
        /// </summary>
        private void recognizeBarcodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_readerTool.IsRecognotionStarted)
            {
                try
                {
                    _readerTool.ReadBarcodesAsync();
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        #endregion

    }
}
