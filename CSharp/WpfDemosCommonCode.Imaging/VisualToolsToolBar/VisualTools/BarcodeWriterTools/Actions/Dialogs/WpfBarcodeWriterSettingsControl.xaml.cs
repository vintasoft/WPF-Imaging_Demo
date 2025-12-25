using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if !REMOVE_BARCODE_SDK
using Vintasoft.Barcode;
using Vintasoft.Barcode.BarcodeInfo;
using Vintasoft.Barcode.GS1;
using Vintasoft.Barcode.SymbologySubsets;
#endif

using WpfDemosCommonCode.Imaging;

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// A control that contains settings of barcode writer.
    /// </summary>
    public partial class WpfBarcodeWriterSettingsControl : UserControl
    {

        #region Fields

#if !REMOVE_BARCODE_SDK
        GS1ApplicationIdentifierValue[] _GS1ApplicationIdentifierValues;

        MailmarkCmdmValueItem _mailmarkCmdmValueItem = new MailmarkCmdmValueItem();

        PpnBarcodeValue _ppnBarcodeValue = new PpnBarcodeValue(); 
#endif

        #endregion



        #region Constructor

        public WpfBarcodeWriterSettingsControl()
        {
            InitializeComponent();

#if !REMOVE_BARCODE_SDK
            // default GS1 value
            _GS1ApplicationIdentifierValues = new GS1ApplicationIdentifierValue[] {
                new GS1ApplicationIdentifierValue(
                    GS1ApplicationIdentifiers.FindApplicationIdentifier("01"),
                    "0123456789012C")
            };

            // common
            barcodeValueTextBox.Text = "01234567";
            barcodeValueTextBox.TextChanged += new TextChangedEventHandler(barcodeValueTextBox_TextChanged);

            AddEnumValues(pixelFormatComboBox, typeof(BarcodeImagePixelFormat));
            pixelFormatComboBox.SelectedItem = BarcodeImagePixelFormat.Bgr24;
            pixelFormatComboBox.SelectionChanged += new SelectionChangedEventHandler(pixelFormatComboBox_SelectionChanged);

            minWidthNumericUpDown.ValueChanged += new EventHandler<EventArgs>(minWidthNumericUpDown_ValueChanged);
            paddingNumericUpDown.ValueChanged += new EventHandler<EventArgs>(paddingNumericUpDown_ValueChanged);
            widthAdjustNumericUpDown.ValueChanged += new EventHandler<EventArgs>(widthAdjustNumericUpDown_ValueChanged);

            foregroundColorPicker.SelectedColor = Colors.Black;
            foregroundColorPicker.SelectedColorChanged += new EventHandler(foregroundColorPicker_SelectedColorChanged);

            backgroundColorPicker.SelectedColor = Colors.White;
            backgroundColorPicker.SelectedColorChanged += new EventHandler(backgroundColorPicker_SelectedColorChanged);

            // linear
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Code128);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.SSCC18);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.SwissPostParcel);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.FedExGround96);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.VicsBol);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.VicsScacPro);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Code16K);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Code93);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Code39);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.Code39Extended);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.Code32);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.VIN);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.PZN);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.NumlyNumber);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.DhlAwb);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Code11);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Codabar);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.EAN13);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.EAN13Plus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.EAN13Plus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.JAN13);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.JAN13Plus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.JAN13Plus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.EAN8);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.EAN8Plus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.EAN8Plus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.JAN8);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.JAN8Plus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.JAN8Plus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.EANVelocity);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISBN);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISBNPlus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISBNPlus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISMN);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISMNPlus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISMNPlus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISSN);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISSNPlus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ISSNPlus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Interleaved2of5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.OPC);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.DeutschePostIdentcode);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.DeutschePostLeitcode);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.MSI);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.PatchCode);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Pharmacode);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.RSS14);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.RSS14Stacked);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.RSSLimited);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.RSSExpanded);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.RSSExpandedStacked);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Standard2of5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.IATA2of5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Matrix2of5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Telepen);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.UPCA);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.UPCAPlus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.UPCAPlus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.UPCE);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.UPCEPlus2);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.UPCEPlus5);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.AustralianPost);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.IntelligentMail);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Planet);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Postnet);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.DutchKIX);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.RoyalMail);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Mailmark4StateC);
            linearBarcodeTypeComboBox.Items.Add(BarcodeType.Mailmark4StateL);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DataBar);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.GS1_128);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DataBarStacked);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DataBarLimited);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DataBarExpanded);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DataBarExpandedStacked);
            linearBarcodeTypeComboBox.Items.Add(BarcodeSymbologySubsets.ITF14);

            // sort supported barcode list
            object[] barcodes = new object[linearBarcodeTypeComboBox.Items.Count];
            linearBarcodeTypeComboBox.Items.CopyTo(barcodes, 0);
            string[] names = new string[barcodes.Length];
            for (int i = 0; i < barcodes.Length; i++)
                names[i] = barcodes[i].ToString();
            Array.Sort(names, barcodes);
            linearBarcodeTypeComboBox.Items.Clear();
            foreach (object barcode in barcodes)
                linearBarcodeTypeComboBox.Items.Add(barcode);
            linearBarcodeTypeComboBox.SelectionChanged += new SelectionChangedEventHandler(linearBarcodeTypeComboBox_SelectionChanged);

            // 2D
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.Aztec);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.DataMatrix);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.DotCode);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.PDF417);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.MicroPDF417);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.HanXinCode);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.QR);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.MicroQR);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeType.MaxiCode);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.GS1Aztec);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DataMatrix);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.GS1DotCode);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.GS1QR);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.MailmarkCmdmType7);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.MailmarkCmdmType9);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.MailmarkCmdmType29);
            twoDimensionalBarcodeComboBox.Items.Add(BarcodeSymbologySubsets.PPN);
            twoDimensionalBarcodeComboBox.SelectedItem = BarcodeType.DataMatrix;
            twoDimensionalBarcodeComboBox.SelectionChanged += new SelectionChangedEventHandler(twoDimensionalBarcodeComboBox_SelectionChanged);

            linearBarcodeHeight.ValueChanged += new EventHandler<EventArgs>(linearBarcodeHeight_ValueChanged);

            valueAutoLetterSpacingCheckBox.Checked += new RoutedEventHandler(valueAutoLetterSpacingCheckBox_CheckedChanged);
            valueAutoLetterSpacingCheckBox.Unchecked += new RoutedEventHandler(valueAutoLetterSpacingCheckBox_CheckedChanged);

            valueVisibleCheckBox.Checked += new RoutedEventHandler(valueVisibleCheckBox_CheckedChanged);
            valueVisibleCheckBox.Unchecked += new RoutedEventHandler(valueVisibleCheckBox_CheckedChanged);

            valueGapNumericUpDown.ValueChanged += new EventHandler<EventArgs>(valueGapNumericUpDown_ValueChanged);

            fontFamilySelector.SelectedFamily = new FontFamily("Courier New");
            fontFamilySelector.SelectedFamilyChanged += new EventHandler(fontFamilySelector_SelectedFamilyChanged);

            valueFontSizeNumericUpDown.ValueChanged += new EventHandler<EventArgs>(valueFontSizeNumericUpDown_ValueChanged);

            // MSI checksum
            msiChecksumComboBox.Items.Add(MSIChecksumType.Mod10);
            msiChecksumComboBox.Items.Add(MSIChecksumType.Mod10Mod10);
            msiChecksumComboBox.Items.Add(MSIChecksumType.Mod11);
            msiChecksumComboBox.Items.Add(MSIChecksumType.Mod11Mod10);
            msiChecksumComboBox.SelectionChanged += new SelectionChangedEventHandler(msiChecksumComboBox_SelectionChanged);

            // Code 128 encoding mode
            AddEnumValues(code128ModeComboBox, typeof(Code128EncodingMode));
            code128ModeComboBox.SelectionChanged += new SelectionChangedEventHandler(code128ModeComboBox_SelectionChanged);

            // RSS
            for (int i = 2; i <= 20; i += 2)
                rssExpandedStackedSegmentPerRow.Items.Add(i);
            rssLinkageFlag.Checked += new RoutedEventHandler(rssLinkageFlag_Checked);
            rssLinkageFlag.Unchecked += new RoutedEventHandler(rssLinkageFlag_Checked);
            rss14StackedOmni.Checked += new RoutedEventHandler(rss14StackedOmni_Checked);
            rss14StackedOmni.Unchecked += new RoutedEventHandler(rss14StackedOmni_Checked);
            rssExpandedStackedSegmentPerRow.SelectionChanged += new SelectionChangedEventHandler(rssExpandedStackedSegmentPerRow_SelectionChanged);

            // Postal
            postalADMiltiplierNumericUpDown.ValueChanged += new EventHandler<EventArgs>(postalADMiltiplierNumericUpDown_ValueChanged);
            AddEnumValues(australianPostCustomInfoComboBox, typeof(AustralianPostCustomerInfoFormat));
            australianPostCustomInfoComboBox.SelectionChanged += new SelectionChangedEventHandler(australianPostCustomInfoComboBox_SelectionChanged);

            // Aztec
            AddEnumValues(aztecSymbolComboBox, typeof(AztecSymbolType));
            aztecSymbolComboBox.SelectionChanged += new SelectionChangedEventHandler(aztecSymbolComboBox_SelectionChanged);
            for (int i = 0; i <= 32; i++)
                aztecLayersCountComboBox.Items.Add(i);
            aztecLayersCountComboBox.SelectionChanged += new SelectionChangedEventHandler(aztecLayersCountComboBox_SelectionChanged);

            aztecEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(aztecEncodingModeComboBox_SelectionChanged);
            aztecEncodingModeComboBox.Items.Add(AztecEncodingMode.Undefined);
            aztecEncodingModeComboBox.Items.Add(AztecEncodingMode.Text);
            aztecEncodingModeComboBox.Items.Add(AztecEncodingMode.Byte);

            aztecErrorCorrectionNumericUpDown.ValueChanged += new EventHandler<EventArgs>(aztecErrorCorrectionNumericUpDown_ValueChanged);

            // DataMatrix
            AddEnumValues(datamatrixEncodingModeComboBox, typeof(DataMatrixEncodingMode));
            datamatrixEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(datamatrixEncodingModeComboBox_SelectionChanged);

            AddEnumValues(datamatrixSymbolSizeComboBox, typeof(DataMatrixSymbolType));
            datamatrixSymbolSizeComboBox.SelectionChanged += new SelectionChangedEventHandler(datamatrixSymbolSizeComboBox_SelectionChanged);

            // DotCode
            dotCodeWidthNumericUpDown.ValueChanged += DotCodeWidthNumericUpDown_ValueChanged;
            dotCodeHeightNumericUpDown.ValueChanged += DotCodeHeightNumericUpDown_ValueChanged;
            dotCodeAspectRatioNumericUpDown.ValueChanged += DotCodeAspectRatioNumericUpDown_ValueChanged;
            dotCodeRectangularModulesCheckBox.Checked += DotCodeRectangularModulesCheckBox_CheckedChanged;
            dotCodeRectangularModulesCheckBox.Unchecked += DotCodeRectangularModulesCheckBox_CheckedChanged;

            // QR Code
            AddEnumValues(qrEncodingModeComboBox, typeof(QREncodingMode));
            qrEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(qrEncodingModeComboBox_SelectionChanged);

            qrSymbolSizeComboBox.Items.Add(QRSymbolVersion.Undefined);
            for (int i = 1; i <= 40; i++)
                qrSymbolSizeComboBox.Items.Add((QRSymbolVersion)i);
            qrSymbolSizeComboBox.SelectionChanged += new SelectionChangedEventHandler(qrSymbolSizeComboBox_SelectionChanged);

            AddEnumValues(qrECCLevelComboBox, typeof(QRErrorCorrectionLevel));
            qrECCLevelComboBox.SelectionChanged += new SelectionChangedEventHandler(qrECCLevelComboBox_SelectionChanged);

            // Micro QR Code
            AddEnumValues(microQrEncodingModeComboBox, typeof(QREncodingMode));
            microQrEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(microQrEncodingModeComboBox_SelectionChanged);

            microQrSymbolSizeComboBox.Items.Add(QRSymbolVersion.Undefined);
            microQrSymbolSizeComboBox.Items.Add(QRSymbolVersion.VersionM1);
            microQrSymbolSizeComboBox.Items.Add(QRSymbolVersion.VersionM2);
            microQrSymbolSizeComboBox.Items.Add(QRSymbolVersion.VersionM3);
            microQrSymbolSizeComboBox.Items.Add(QRSymbolVersion.VersionM4);
            microQrSymbolSizeComboBox.SelectionChanged += new SelectionChangedEventHandler(microQrSymbolSizeComboBox_SelectionChanged);

            microQrECCLevelComboBox.Items.Add(QRErrorCorrectionLevel.L);
            microQrECCLevelComboBox.Items.Add(QRErrorCorrectionLevel.M);
            microQrECCLevelComboBox.Items.Add(QRErrorCorrectionLevel.Q);
            microQrECCLevelComboBox.SelectionChanged += new SelectionChangedEventHandler(microQrECCLevelComboBox_SelectionChanged);

            // MaxiCode
            maxiCodeResolutonNumericUpDown.ValueChanged += new EventHandler<EventArgs>(maxiCodeResolutonNumericUpDown_ValueChanged);

            maxiCodeEncodingModeComboBox.Items.Add(MaxiCodeEncodingMode.Mode2);
            maxiCodeEncodingModeComboBox.Items.Add(MaxiCodeEncodingMode.Mode3);
            maxiCodeEncodingModeComboBox.Items.Add(MaxiCodeEncodingMode.Mode4);
            maxiCodeEncodingModeComboBox.Items.Add(MaxiCodeEncodingMode.Mode5);
            maxiCodeEncodingModeComboBox.Items.Add(MaxiCodeEncodingMode.Mode6);
            maxiCodeEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(maxiCodeEncodingModeComboBox_SelectionChanged);

            // PDF417
            AddEnumValues(pdf417EncodingModeComboBox, typeof(PDF417EncodingMode));
            pdf417EncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(pdf417EncodingModeComboBox_SelectionChanged);

            AddEnumValues(pdf417ErrorCorrectionComboBox, typeof(PDF417ErrorCorrectionLevel));
            pdf417ErrorCorrectionComboBox.SelectionChanged += new SelectionChangedEventHandler(pdf417ErrorCorrectionComboBox_SelectionChanged);

            pdf417RowsNumericUpDown.ValueChanged += new EventHandler<EventArgs>(pdf417RowsNumericUpDown_ValueChanged);

            pdf417ColsNumericUpDown.ValueChanged += new EventHandler<EventArgs>(pdf417ColsNumericUpDown_ValueChanged);

            pdf417RowHeightNumericUpDown.ValueChanged += new EventHandler<EventArgs>(pdf417RowHeightNumericUpDown_ValueChanged);

            pdf417CompactCheckBox.Checked += new RoutedEventHandler(pdf417CompactCheckBox_Checked);
            pdf417CompactCheckBox.Unchecked += new RoutedEventHandler(pdf417CompactCheckBox_Checked);

            // Micro PDF417
            AddEnumValues(microPdf417EncodingModeComboBox, typeof(PDF417EncodingMode));
            microPdf417EncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(microPdf417EncodingModeComboBox_SelectionChanged);

            AddEnumValues(microPdf417SymbolSizeComboBox, typeof(MicroPDF417SymbolType));
            microPdf417SymbolSizeComboBox.SelectionChanged += new SelectionChangedEventHandler(microPdf417SymbolSizeComboBox_SelectionChanged);

            microPdf417ColumnsNumericUpDown.ValueChanged += new EventHandler<EventArgs>(microPdf417ColumnsNumericUpDown_ValueChanged);
            microPdf417RowHeightNumericUpDown.ValueChanged += new EventHandler<EventArgs>(microPdf417RowHeightNumericUpDown_ValueChanged);

            // ECI
            EciCharacterEncoding[] eciCharacterEncodings = EciCharacterEncoding.GetEciCharacterEncodings();
            foreach (EciCharacterEncoding characterEncoding in eciCharacterEncodings)
            {
                encodingInfoComboBox.Items.Add(characterEncoding);
                if (characterEncoding.EciAssignmentNumber == 3)
                    encodingInfoComboBox.SelectedItem = characterEncoding;
            }
            encodingInfoComboBox.SelectionChanged += new SelectionChangedEventHandler(encodingInfoComboBox_SelectionChanged);

            // PPN
            _ppnBarcodeValue.PharmacyProductNumber = "110375286414";
            _ppnBarcodeValue.BatchNumber = "12345ABCD";
            _ppnBarcodeValue.ExpiryDate = "150617";
            _ppnBarcodeValue.SerialNumber = "12345ABCDEF98765";

            // Code 16K
            code16KRowsComboBox.Items.Add(0);
            for (int i = 2; i <= 16; i++)
                code16KRowsComboBox.Items.Add(i);
            code16KRowsComboBox.SelectionChanged += new SelectionChangedEventHandler(code16KRowsComboBox_SelectionChanged);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.Undefined);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.ModeA);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.ModeB);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.ModeC);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.Code16K_Mode3);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.Code16K_Mode4);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.Code16K_Mode5);
            code16KEncodingModeComboBox.Items.Add(Code128EncodingMode.Code16K_Mode6);
            code16KEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(code16KEncodingModeComboBox_SelectionChanged);

            // Han Xin Code
            AddEnumValues(hanXinCodeEncodingModeComboBox, typeof(HanXinCodeEncodingMode));
            hanXinCodeSymbolVersionComboBox.Items.Add(HanXinCodeSymbolVersion.Undefined);
            for (int i = 1; i <= 84; i++)
                hanXinCodeSymbolVersionComboBox.Items.Add((HanXinCodeSymbolVersion)i);
            AddEnumValues(hanXinCodeECCLevelComboBox, typeof(HanXinCodeErrorCorrectionLevel));

            encodingInfoCheckBox.Foreground = new SolidColorBrush(Color.FromRgb(220, 100, 0));
            encodingInfoCheckBox.Checked += new RoutedEventHandler(encodingInfoCheckBox_Checked);
            encodingInfoCheckBox.Unchecked += new RoutedEventHandler(encodingInfoCheckBox_Checked);

            useOptionalCheckSum.Checked += new RoutedEventHandler(useOptionalCheckSum_Checked);
            useOptionalCheckSum.Unchecked += new RoutedEventHandler(useOptionalCheckSum_Checked);

            enableTelepenNumericMode.Checked += new RoutedEventHandler(enableTelepenNumericMode_Checked);
            enableTelepenNumericMode.Unchecked += new RoutedEventHandler(enableTelepenNumericMode_Checked);

            barsRatioNumericUpDown.ValueChanged += new EventHandler<EventArgs>(barsRatioNumericUpDown_ValueChanged);

            barcodeGroupsTabPages.SelectionChanged += new SelectionChangedEventHandler(barcodeGroupsTabPages_SelectionChanged);
            linearBarcodeTypeComboBox.SelectedItem = BarcodeType.Code128;
            datamatrixSymbolSizeComboBox.SelectedItem = DataMatrixSymbolType.Undefined;
            twoDimensionalBarcodeComboBox.SelectedItem = BarcodeType.DataMatrix;
            qrEncodingModeComboBox.SelectedItem = QREncodingMode.Undefined;
            datamatrixEncodingModeComboBox.SelectedItem = DataMatrixEncodingMode.Undefined;
            aztecEncodingModeComboBox.SelectedIndex = 0;
            aztecSymbolComboBox.SelectedItem = AztecSymbolType.Undefined;
            australianPostCustomInfoComboBox.SelectedItem = AustralianPostCustomerInfoFormat.None;
            msiChecksumComboBox.SelectedItem = MSIChecksumType.Mod10;
            code128ModeComboBox.SelectedItem = Code128EncodingMode.Undefined;
            microPdf417SymbolSizeComboBox.SelectedItem = MicroPDF417SymbolType.Undefined;
            microPdf417EncodingModeComboBox.SelectedItem = PDF417EncodingMode.Undefined;
            pdf417ErrorCorrectionComboBox.SelectedItem = PDF417ErrorCorrectionLevel.Undefined;
            pdf417EncodingModeComboBox.SelectedItem = PDF417EncodingMode.Undefined;
            maxiCodeEncodingModeComboBox.SelectedItem = MaxiCodeEncodingMode.Mode4;
            microQrECCLevelComboBox.SelectedItem = QRErrorCorrectionLevel.M;
            microQrSymbolSizeComboBox.SelectedItem = QRSymbolVersion.Undefined;
            microQrEncodingModeComboBox.SelectedItem = QREncodingMode.Undefined;
            qrECCLevelComboBox.SelectedItem = QRErrorCorrectionLevel.M;
            qrSymbolSizeComboBox.SelectedItem = QRSymbolVersion.Undefined;
            aztecLayersCountComboBox.SelectedIndex = 0;
            rssExpandedStackedSegmentPerRow.SelectedItem = 4;
            hanXinCodeEncodingModeComboBox.SelectionChanged += new SelectionChangedEventHandler(hanXinCodeEncodingModeComboBox_SelectionChanged);
            hanXinCodeSymbolVersionComboBox.SelectionChanged += new SelectionChangedEventHandler(hanXinCodeSymbolVersionComboBox_SelectionChanged);
            hanXinCodeECCLevelComboBox.SelectionChanged += new SelectionChangedEventHandler(hanXinCodeECCLevelComboBox_SelectionChanged);
            mainDockPanel.IsEnabled = false; 
#endif

            BarcodeImage = null;
        }

        #endregion



        #region Properties

#if !REMOVE_BARCODE_SDK
        BarcodeSymbologySubset _selectedBarcodeSubset = null;
        /// <summary>
        /// Gets or sets a selected barcode subset.
        /// </summary>
        public BarcodeSymbologySubset SelectedBarcodeSubset
        {
            get
            {
                return _selectedBarcodeSubset;
            }
            set
            {
                _selectedBarcodeSubset = value;
                UpdateUI();
            }
        }

        private WriterSettings _barcodeWriterSettings = new WriterSettings();
        /// <summary>
        /// Gest or sets a writer settings.
        /// </summary>
        public WriterSettings BarcodeWriterSettings
        {
            get
            {
                return _barcodeWriterSettings;
            }
            set
            {
                _barcodeWriterSettings = value;

                if (value != null)
                {
                    mainDockPanel.IsEnabled = true;

                    linearBarcodeTypeComboBox.SelectedItem = value.Barcode;

                    barcodeValueTextBox.Text = value.Value;
                    foregroundColorPicker.SelectedColor = Vintasoft.Barcode.WpfConverter.Convert(value.ForeColor);
                    backgroundColorPicker.SelectedColor = Vintasoft.Barcode.WpfConverter.Convert(value.BackColor);
                    pixelFormatComboBox.SelectedItem = value.PixelFormat;
                    minWidthNumericUpDown.Value = value.MinWidth;
                    paddingNumericUpDown.Value = value.Padding;
                    widthAdjustNumericUpDown.Value = (int)(value.BarsWidthAdjustment * 10);
                    linearBarcodeHeight.Value = value.Height;
                    valueAutoLetterSpacingCheckBox.IsChecked = value.ValueAutoLetterSpacing;
                    if (barcodeGroupsTabPages.SelectedItem == linearBarcodesTabItem)
                        valueVisibleCheckBox.IsChecked = value.ValueVisible;
                    else
                        valueVisibleCheckBox.IsChecked = value.Value2DVisible;
                    valueGapNumericUpDown.Value = value.ValueGap;
                    fontFamilySelector.SelectedFamily = new FontFamily(value.ValueFont.Name);
                    valueFontSizeNumericUpDown.Value = (int)value.ValueFont.Size;
                    msiChecksumComboBox.SelectedItem = value.MSIChecksum;
                    code128ModeComboBox.SelectedItem = value.Code128EncodingMode;
                    postalADMiltiplierNumericUpDown.Value = (int)(value.PostBarcodesADMultiplier * 10.0);
                    australianPostCustomInfoComboBox.SelectedItem = value.AustralianPostCustomerInfoFormat;
                    aztecSymbolComboBox.SelectedItem = value.AztecSymbol;
                    aztecLayersCountComboBox.SelectedIndex = value.AztecDataLayers;
                    aztecEncodingModeComboBox.SelectedItem = value.AztecEncodingMode;
                    aztecErrorCorrectionNumericUpDown.Value = (int)value.AztecErrorCorrectionDataPercent;
                    datamatrixEncodingModeComboBox.SelectedItem = value.DataMatrixEncodingMode;
                    datamatrixSymbolSizeComboBox.SelectedItem = value.DataMatrixSymbol;
                    qrEncodingModeComboBox.SelectedItem = value.QREncodingMode;
                    qrSymbolSizeComboBox.SelectedItem = value.QRSymbol;
                    qrECCLevelComboBox.SelectedItem = value.QRErrorCorrectionLevel;
                    microQrEncodingModeComboBox.SelectedItem = value.QREncodingMode;
                    microQrSymbolSizeComboBox.SelectedItem = value.QRSymbol;
                    microQrECCLevelComboBox.SelectedItem = value.QRErrorCorrectionLevel;
                    maxiCodeResolutonNumericUpDown.Value = (int)value.MaxiCodeResolution;
                    maxiCodeEncodingModeComboBox.SelectedItem = value.MaxiCodeEncodingMode;
                    pdf417EncodingModeComboBox.SelectedItem = value.PDF417EncodingMode;
                    pdf417ErrorCorrectionComboBox.SelectedItem = value.PDF417ErrorCorrectionLevel;
                    pdf417RowsNumericUpDown.Value = value.PDF417Rows;
                    pdf417ColsNumericUpDown.Value = value.PDF417Columns;
                    pdf417RowHeightNumericUpDown.Value = value.PDF417RowHeight;
                    microPdf417ColumnsNumericUpDown.Value = value.MicroPDF417Columns;
                    microPdf417EncodingModeComboBox.SelectedItem = value.MicroPDF417EncodingMode;
                    microPdf417SymbolSizeComboBox.SelectedItem = value.MicroPDF417Symbol;
                    microPdf417RowHeightNumericUpDown.Value = value.MicroPDF417RowHeight;
                    rss14StackedOmni.IsChecked = value.RSS14StackedOmnidirectional;
                    rssExpandedStackedSegmentPerRow.SelectedItem = value.RSSExpandedStackedSegmentPerRow;
                    rssLinkageFlag.IsChecked = value.RSSLinkageFlag;
                    useOptionalCheckSum.IsChecked = value.OptionalCheckSum;
                    enableTelepenNumericMode.IsChecked = value.EnableTelepenNumericMode;
                    code16KRowsComboBox.SelectedItem = value.Code16KRows;
                    code16KEncodingModeComboBox.SelectedItem = value.Code16KEncodingMode;
                    hanXinCodeEncodingModeComboBox.SelectedItem = value.HanXinCodeEncodingMode;
                    hanXinCodeSymbolVersionComboBox.SelectedItem = value.HanXinCodeSymbol;
                    hanXinCodeECCLevelComboBox.SelectedItem = value.HanXinCodeErrorCorrectionLevel;
                }
                else
                {
                    mainDockPanel.IsEnabled = false;
                }
            }
        } 
#endif

        /// <summary>
        /// Gets a barcode image.
        /// </summary>
        public BitmapSource BarcodeImage
        {
            set
            {
#if !REMOVE_BARCODE_SDK
                if (value == null)
                {
                    barcodeImageLabel.Visibility = Visibility.Hidden;
                    imageSizeLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    barcodeImageLabel.Visibility = Visibility.Visible;
                    imageSizeLabel.Visibility = Visibility.Visible;
                    imageSizeLabel.Content = string.Format(
                        "{0}x{1} px; {2}x{3} DPI", value.PixelWidth, value.PixelHeight,
                        value.DpiX, value.DpiY);
                } 
#endif
            }
        }

        bool _canChangeBarcodeSize = true;
        /// <summary>
        /// Gets or sets a value that indicating when can change barcode size.
        /// </summary>
        [DefaultValue(true)]
        public bool CanChangeBarcodeSize
        {
            get
            {
                return _canChangeBarcodeSize;
            }
            set
            {
                _canChangeBarcodeSize = value;

#if !REMOVE_BARCODE_SDK
                barcodeWidthDockPanel.Visibility = (value ? Visibility.Visible : Visibility.Hidden);
                Visibility visibility = (value ? Visibility.Visible : Visibility.Collapsed);
                linearBarcodeHeight.Visibility = visibility;
                linearBarcodeHeightLabel.Visibility = visibility; 
#endif
            }
        }

        /// <summary>
        /// Gets a value that indicating when need encode ECI character.
        /// </summary>
        public bool EncodeEciCharacter
        {
            get
            {
#if !REMOVE_BARCODE_SDK
                if (encodingInfoCheckBox.IsChecked.Value == true &&
                            encodingInfoCheckBox.IsEnabled)
                    return true;
                else
                    return false; 
#else
                return false;
#endif
            }
        }

        #endregion



        #region Methods

        #region INTERNAL

        internal void UpdateBarcodeWriterSettings()
        {
#if !REMOVE_BARCODE_SDK
            minWidthNumericUpDown.Value = Math.Max(BarcodeWriterSettings.MinWidth, minWidthNumericUpDown.Minimum); 
#endif
        }

        #endregion


        #region PRIVATE

        #region Common

        /// <summary>
        /// Handles the ValueChanged event of valueFontSizeNumericUpDown object.
        /// </summary>
        void valueFontSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.ValueFont = Vintasoft.Barcode.TextRendering.TextRenderingFactory.Default.CreateTextFont(
                BarcodeWriterSettings.ValueFont, (float)valueFontSizeNumericUpDown.Value);
#endif
        }

        /// <summary>
        /// Handles the SelectedFamilyChanged event of fontFamilySelector object.
        /// </summary>
        void fontFamilySelector_SelectedFamilyChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            Typeface typeface = new Typeface(fontFamilySelector.SelectedFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            BarcodeWriterSettings.ValueFont = new Vintasoft.Barcode.Wpf.TextRendering.WpfTextFont(typeface, (float)valueFontSizeNumericUpDown.Value);
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of barsRatioNumericUpDown object.
        /// </summary>
        void barsRatioNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.BarsRatio = (double)barsRatioNumericUpDown.Value / 10.0; 
#endif
        }

        /// <summary>
        /// Handles the Checked event of useOptionalCheckSum object.
        /// </summary>
        void useOptionalCheckSum_Checked(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.OptionalCheckSum = useOptionalCheckSum.IsChecked.Value; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of valueGapNumericUpDown object.
        /// </summary>
        void valueGapNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.ValueGap = (int)valueGapNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the SelectedColorChanged event of backgroundColorPicker object.
        /// </summary>
        void backgroundColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.BackColor = Vintasoft.Barcode.WpfConverter.Convert(backgroundColorPicker.SelectedColor); 
#endif
        }

        /// <summary>
        /// Handles the SelectedColorChanged event of foregroundColorPicker object.
        /// </summary>
        void foregroundColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.ForeColor = Vintasoft.Barcode.WpfConverter.Convert(foregroundColorPicker.SelectedColor); 
#endif
        }

        /// <summary>
        /// Handles the CheckedChanged event of valueVisibleCheckBox object.
        /// </summary>
        void valueVisibleCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            bool enabled = valueVisibleCheckBox.IsChecked.Value;
            valueAutoLetterSpacingCheckBox.IsEnabled = enabled;
            valueGapNumericUpDown.IsEnabled = enabled;
            valueFontSizeNumericUpDown.IsEnabled = enabled;
            fontFamilySelector.IsEnabled = enabled;
            if (barcodeGroupsTabPages.SelectedItem == linearBarcodesTabItem)
                BarcodeWriterSettings.ValueVisible = valueVisibleCheckBox.IsChecked.Value;
            else
                BarcodeWriterSettings.Value2DVisible = valueVisibleCheckBox.IsChecked.Value; 
#endif
        }

        /// <summary>
        /// Handles the CheckedChanged event of valueAutoLetterSpacingCheckBox object.
        /// </summary>
        void valueAutoLetterSpacingCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.ValueAutoLetterSpacing = valueAutoLetterSpacingCheckBox.IsChecked.Value; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of linearBarcodeHeight object.
        /// </summary>
        void linearBarcodeHeight_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.Height = (int)linearBarcodeHeight.Value; 
#endif
        }

        /// <summary>
        /// Handles the Checked event of encodingInfoCheckBox object.
        /// </summary>
        void encodingInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            try
            {
                encodingInfoComboBox.IsEnabled = encodingInfoCheckBox.IsChecked.Value;
            EncodeValue();
            }
            catch
            {
                MessageBox.Show(string.Format("Barcode {0} is not supports encoding info.", BarcodeWriterSettings.Barcode));
                encodingInfoCheckBox.IsChecked = false;
            }
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of encodingInfoComboBox object.
        /// </summary>
        void encodingInfoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EncodeValue();
        }

        /// <summary>
        /// Encodes the barcode value.
        /// </summary>
        private bool EncodeValue()
        {
#if !REMOVE_BARCODE_SDK
            if (BarcodeWriterSettings == null)
                return false;
            string oldValue = BarcodeWriterSettings.Value;
            try
            {
                if (SelectedBarcodeSubset is GS1BarcodeSymbologySubset)
                {
                    //  encode GS1 barcode value
                    SelectedBarcodeSubset.Encode(new GS1ValueItem(_GS1ApplicationIdentifierValues), BarcodeWriterSettings);
                }
                else if (SelectedBarcodeSubset is MailmarkCmdmBarcodeSymbology)
                {
                    //  encode Mailmark barcode value
                    SelectedBarcodeSubset.Encode(_mailmarkCmdmValueItem, BarcodeWriterSettings);
                }
                else if (SelectedBarcodeSubset is PpnBarcodeSymbology)
                {
                    //  encode PPN barcode value
                    SelectedBarcodeSubset.Encode(_ppnBarcodeValue, BarcodeWriterSettings);
                }
                else if (SelectedBarcodeSubset != null)
                {
                    SelectedBarcodeSubset.Encode(new TextValueItem(barcodeValueTextBox.Text), BarcodeWriterSettings);
                }
                else
                {
                    if (encodingInfoCheckBox.IsChecked.Value && encodingInfoCheckBox.IsEnabled)
                    {
                        EciCharacterEncoder encoder = new EciCharacterEncoder(BarcodeWriterSettings.Barcode);
                        encoder.AppendText((EciCharacterEncoding)encodingInfoComboBox.SelectedItem, barcodeValueTextBox.Text);
                        BarcodeWriterSettings.ValueItems = encoder.ToValueItems();
                        BarcodeWriterSettings.PrintableValue = barcodeValueTextBox.Text;
                    }
                    else
                    {
                        BarcodeWriterSettings.PrintableValue = "";
                        BarcodeWriterSettings.Value = barcodeValueTextBox.Text;
                    }
                }
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                return false;
            } 
#endif
            return true;
        }

        /// <summary>
        /// Handles the SelectionChanged event of linearBarcodeTypeComboBox object.
        /// </summary>
        void linearBarcodeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if (BarcodeWriterSettings == null)
                return;
            BarcodeType oldBarcodeType = BarcodeWriterSettings.Barcode;
            try
            {
                BarcodeWriterSettings.BeginInit();
                if (linearBarcodeTypeComboBox.SelectedItem is BarcodeSymbologySubset)
                {
                    SelectedBarcodeSubset = (BarcodeSymbologySubset)linearBarcodeTypeComboBox.SelectedItem;
                }
                else
                {
                    SelectedBarcodeSubset = null;
                    BarcodeWriterSettings.Barcode = (BarcodeType)linearBarcodeTypeComboBox.SelectedItem;
                }
                EncodeValue();
                BarcodeWriterSettings.EndInit();
            }
            catch (WriterSettingsException exc)
            {
                BarcodeWriterSettings.EndInit();
                OnWriterException(exc);
                BarcodeWriterSettings.Barcode = oldBarcodeType;
                linearBarcodeTypeComboBox.SelectedItem = oldBarcodeType;
            }

            australianPostCustomInfoDockPanel.Visibility = Visibility.Collapsed;
            msiChecksumDockPanel.Visibility = Visibility.Collapsed;
            code128ModeDockPanel.Visibility = Visibility.Collapsed;
            postalADMiltiplierDockPanel.Visibility = Visibility.Collapsed;
            barsRatioDockPanel.Visibility = Visibility.Collapsed;
            useOptionalCheckSumDockPanel.Visibility = Visibility.Collapsed;
            enableTelepenNumericModeDockPanel.Visibility = Visibility.Collapsed;
            rssLinkageFlagDockPanel.Visibility = Visibility.Collapsed;
            rss14StackedOmniDockPanel.Visibility = Visibility.Collapsed;
            rssExpandedStackedSegmentPerRowDockPanel.Visibility = Visibility.Collapsed;
            code16KEncodeingModePanel.Visibility = Visibility.Collapsed;
            code16KRowsPanel.Visibility = Visibility.Collapsed;
            if (SelectedBarcodeSubset == null)
            {
                switch (BarcodeWriterSettings.Barcode)
                {
                    case BarcodeType.MSI:
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        msiChecksumDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Code128:
                        code128ModeDockPanel.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.Code16K:
                        code16KEncodeingModePanel.Visibility = Visibility.Visible;
                        code16KRowsPanel.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.AustralianPost:
                        postalADMiltiplierDockPanel.Visibility = Visibility.Visible;
                        australianPostCustomInfoDockPanel.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.RoyalMail:
                    case BarcodeType.DutchKIX:
                    case BarcodeType.IntelligentMail:
                    case BarcodeType.Postnet:
                    case BarcodeType.Planet:
                        postalADMiltiplierDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Code11:
                    case BarcodeType.Codabar:
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Interleaved2of5:
                        useOptionalCheckSumDockPanel.Visibility = Visibility.Visible;
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Standard2of5:
                        useOptionalCheckSumDockPanel.Visibility = Visibility.Visible;
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Matrix2of5:
                        useOptionalCheckSumDockPanel.Visibility = Visibility.Visible;
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.IATA2of5:
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Code39:
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        useOptionalCheckSumDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.Telepen:
                        enableTelepenNumericModeDockPanel.Visibility = Visibility.Visible;
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.RSS14:
                    case BarcodeType.RSS14Stacked:
                    case BarcodeType.RSSExpanded:
                    case BarcodeType.RSSExpandedStacked:
                    case BarcodeType.RSSLimited:
                        rssLinkageFlagDockPanel.Visibility = Visibility.Visible;
                        if (BarcodeWriterSettings.Barcode == BarcodeType.RSS14Stacked)
                            rss14StackedOmniDockPanel.Visibility = Visibility.Visible;
                        else if (BarcodeWriterSettings.Barcode == BarcodeType.RSSExpandedStacked)
                            rssExpandedStackedSegmentPerRowDockPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                switch (SelectedBarcodeSubset.BarcodeType)
                {

                    case BarcodeType.MSI:
                    case BarcodeType.Code11:
                    case BarcodeType.Codabar:
                    case BarcodeType.Interleaved2of5:
                    case BarcodeType.Standard2of5:
                    case BarcodeType.Matrix2of5:
                    case BarcodeType.IATA2of5:
                    case BarcodeType.Code39:
                    case BarcodeType.Telepen:
                        barsRatioDockPanel.Visibility = Visibility.Visible;
                        break;

                    case BarcodeType.RSS14:
                    case BarcodeType.RSS14Stacked:
                    case BarcodeType.RSSExpanded:
                    case BarcodeType.RSSExpandedStacked:
                    case BarcodeType.RSSLimited:
                        rssLinkageFlagDockPanel.Visibility = Visibility.Visible;
                        if (BarcodeWriterSettings.Barcode == BarcodeType.RSS14Stacked)
                            rss14StackedOmniDockPanel.Visibility = Visibility.Visible;
                        else if (BarcodeWriterSettings.Barcode == BarcodeType.RSSExpandedStacked)
                            rssExpandedStackedSegmentPerRowDockPanel.Visibility = Visibility.Visible;
                        break;

                }
                if (SelectedBarcodeSubset is Code39ExtendedBarcodeSymbology)
                    useOptionalCheckSumDockPanel.Visibility = Visibility.Visible;
            } 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of twoDimensionalBarcodeComboBox object.
        /// </summary>
        void twoDimensionalBarcodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            pdf417SettingsGrid.Visibility = Visibility.Collapsed;
            aztecSettingsGrid.Visibility = Visibility.Collapsed;
            maxiCodeSettingsGrid.Visibility = Visibility.Collapsed;
            dataMatrixSettingsGrid.Visibility = Visibility.Collapsed;
            microQrSettingsGrid.Visibility = Visibility.Collapsed;
            qrSettingsGrid.Visibility = Visibility.Collapsed;
            microPDF417SettingsGrid.Visibility = Visibility.Collapsed;
            hanXinCodeSettingsGrid.Visibility = Visibility.Collapsed;
            dotCodeSettingsGrid.Visibility = Visibility.Collapsed;

            BarcodeSymbologySubset barcodeSubset = twoDimensionalBarcodeComboBox.SelectedItem as BarcodeSymbologySubset;
            BarcodeType baseBarcodeType;
            if (barcodeSubset != null)
                baseBarcodeType = barcodeSubset.BarcodeType;
            else
                baseBarcodeType = (BarcodeType)twoDimensionalBarcodeComboBox.SelectedItem;

            SelectedBarcodeSubset = barcodeSubset;
            try
            {
                if (BarcodeWriterSettings != null)
                {
                    if (SelectedBarcodeSubset == null)
                    {
                        BarcodeWriterSettings.BeginInit();
                        BarcodeWriterSettings.Barcode = baseBarcodeType;
                    }
                }

                UpdateUI();

                // select settings panel
                switch (baseBarcodeType)
                {
                    case BarcodeType.Aztec:
                        aztecSettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.QR:
                        qrSettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.MicroQR:
                        microQrSettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.PDF417:
                        pdf417SettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.MicroPDF417:
                        microPDF417SettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.DataMatrix:
                        if (!(SelectedBarcodeSubset is MailmarkCmdmBarcodeSymbology))
                        {
                            dataMatrixSettingsGrid.Visibility = Visibility.Visible;
                            datamatrixSymbolSizeComboBox_SelectionChanged(this, null);

                        }
                        break;
                    case BarcodeType.HanXinCode:
                        hanXinCodeSettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.MaxiCode:
                        maxiCodeSettingsGrid.Visibility = Visibility.Visible;
                        break;
                    case BarcodeType.DotCode:
                        dotCodeSettingsGrid.Visibility = Visibility.Visible;
                        break;
                }
                EncodeValue();

                e.Handled = true;
            }
            finally
            {
                if (SelectedBarcodeSubset == null)
                    if (BarcodeWriterSettings != null)
                        BarcodeWriterSettings.EndInit();
            } 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of widthAdjustNumericUpDown object.
        /// </summary>
        void widthAdjustNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.BarsWidthAdjustment = widthAdjustNumericUpDown.Value * 0.1; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of paddingNumericUpDown object.
        /// </summary>
        void paddingNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.Padding = (int)paddingNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of minWidthNumericUpDown object.
        /// </summary>
        void minWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MinWidth = (int)minWidthNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of pixelFormatComboBox object.
        /// </summary>
        void pixelFormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.PixelFormat = (BarcodeImagePixelFormat)pixelFormatComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the TextChanged event of barcodeValueTextBox object.
        /// </summary>
        void barcodeValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK

            BarcodeWriterSettings.Value = barcodeValueTextBox.Text; 
#endif
        }



        /// <summary>
        /// Handles the SelectionChanged event of barcodeGroupsTabPages object.
        /// </summary>
        void barcodeGroupsTabPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if (!(e.OriginalSource is TabControl))
                return;
            barcodeWidthDockPanel.Visibility = (CanChangeBarcodeSize ? Visibility.Visible : Visibility.Hidden);
            SelectedBarcodeSubset = null;
            BarcodeType oldValue = BarcodeWriterSettings.Barcode;
            try
            {

                BarcodeWriterSettings.BeginInit();
                if (barcodeGroupsTabPages.SelectedItem == linearBarcodesTabItem)
                {
                    if (linearBarcodeTypeComboBox.SelectedItem is BarcodeSymbologySubset)
                    {
                        SelectedBarcodeSubset = (BarcodeSymbologySubset)linearBarcodeTypeComboBox.SelectedItem;
                    }
                    else
                    {
                        BarcodeWriterSettings.Barcode = (BarcodeType)linearBarcodeTypeComboBox.SelectedItem;
                    }
                    valueVisibleCheckBox.IsChecked = BarcodeWriterSettings.ValueVisible;
                    barcodeValueTextBox.IsEnabled = true;
                }
                else
                {
                    BarcodeSymbologySubset barcodeSubset = twoDimensionalBarcodeComboBox.SelectedItem as BarcodeSymbologySubset;
                    BarcodeType baseBarcodeType;
                    if (barcodeSubset != null)
                        baseBarcodeType = barcodeSubset.BarcodeType;
                    else
                        baseBarcodeType = (BarcodeType)twoDimensionalBarcodeComboBox.SelectedItem;

                    if (baseBarcodeType == BarcodeType.PDF417)
                    {
                        if (pdf417CompactCheckBox.IsChecked.Value)
                            BarcodeWriterSettings.Barcode = BarcodeType.PDF417Compact;
                        else
                            BarcodeWriterSettings.Barcode = BarcodeType.PDF417;
                    }
                    else if (baseBarcodeType == BarcodeType.MicroPDF417)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.MicroPDF417;
                    }
                    else if (baseBarcodeType == BarcodeType.DataMatrix)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.DataMatrix;
                        datamatrixSymbolSizeComboBox_SelectionChanged(this, null);
                    }
                    else if (baseBarcodeType == BarcodeType.Aztec)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.Aztec;
                    }
                    else if (baseBarcodeType == BarcodeType.QR)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.QR;
                        BarcodeWriterSettings.QRSymbol = (QRSymbolVersion)qrSymbolSizeComboBox.SelectedItem;
                    }
                    else if (baseBarcodeType == BarcodeType.MicroQR)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.MicroQR;
                        BarcodeWriterSettings.QRSymbol = (QRSymbolVersion)microQrSymbolSizeComboBox.SelectedItem;
                    }
                    else if (baseBarcodeType == BarcodeType.MaxiCode)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.MaxiCode;
                        barcodeWidthDockPanel.Visibility = Visibility.Collapsed;
                    }
                    else if (baseBarcodeType == BarcodeType.HanXinCode)
                    {
                        BarcodeWriterSettings.Barcode = BarcodeType.HanXinCode;
                    }

                    valueVisibleCheckBox.IsChecked = BarcodeWriterSettings.Value2DVisible;
                }
                UpdateUI();
                EncodeValue();
                BarcodeWriterSettings.EndInit();
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.Barcode = oldValue;
                BarcodeWriterSettings.EndInit();
            } 
#endif
        }

        private void UpdateUI()
        {
#if !REMOVE_BARCODE_SDK
            if (BarcodeWriterSettings == null)
                return;
            bool canEncodeECI = false;
            if (SelectedBarcodeSubset == null)
                switch (BarcodeWriterSettings.Barcode)
                {
                    case BarcodeType.Aztec:
                    case BarcodeType.DataMatrix:
                    case BarcodeType.QR:
                    case BarcodeType.PDF417:
                    case BarcodeType.PDF417Compact:
                    case BarcodeType.MicroPDF417:
                    case BarcodeType.HanXinCode:
                        canEncodeECI = true;
                        break;
                }
            encodingInfoCheckBox.IsEnabled = canEncodeECI;
            encodingInfoComboBox.IsEnabled = canEncodeECI && encodingInfoCheckBox.IsChecked.Value;
            Visibility useCustomValueDialog = Visibility.Collapsed;
            if (SelectedBarcodeSubset != null &&
                SelectedBarcodeSubset is GS1BarcodeSymbologySubset ||
                SelectedBarcodeSubset is MailmarkCmdmBarcodeSymbology ||
                SelectedBarcodeSubset is PpnBarcodeSymbology)
                useCustomValueDialog = Visibility.Visible;
            subsetBarcodeValueDockPanel.Visibility = useCustomValueDialog;
            barcodeValueDockPanel.Visibility = useCustomValueDialog == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed; 
#endif
        }

        /// <summary>
        /// Handles the Click event of subsetBarcodeValueButton object.
        /// </summary>
        private void subsetBarcodeValueButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if (SelectedBarcodeSubset is GS1BarcodeSymbologySubset)
            {
                GS1ValueEditorWindow gs1Editor = new GS1ValueEditorWindow(_GS1ApplicationIdentifierValues, false);
                if (gs1Editor.ShowDialog().Value)
                {
                    _GS1ApplicationIdentifierValues = gs1Editor.GS1ApplicationIdentifierValues;
                    EncodeValue();
                }
            }
            else if (SelectedBarcodeSubset is MailmarkCmdmBarcodeSymbology)
            {
                PropertyGridWindow form = new PropertyGridWindow(_mailmarkCmdmValueItem, "Mailmark CMDM value", false);
                form.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
                if (form.ShowDialog().Value)
                {
                    EncodeValue();
                }
            }
            else if (SelectedBarcodeSubset is PpnBarcodeSymbology)
            {
                PropertyGridWindow form = new PropertyGridWindow(_ppnBarcodeValue, "PPN value", false);
                form.PropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
                if (form.ShowDialog().Value)
                {
                    EncodeValue();
                }
            }
            else
            {
                throw new NotImplementedException();
            } 
#endif
        }

        #endregion

        #region MSI

        /// <summary>
        /// Handles the SelectionChanged event of msiChecksumComboBox object.
        /// </summary>
        void msiChecksumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MSIChecksum = (MSIChecksumType)msiChecksumComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region Code128

        /// <summary>
        /// Handles the SelectionChanged event of code128ModeComboBox object.
        /// </summary>
        void code128ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.Code128EncodingMode = (Code128EncodingMode)code128ModeComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region Telepen

        /// <summary>
        /// Handles the Checked event of enableTelepenNumericMode object.
        /// </summary>
        void enableTelepenNumericMode_Checked(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.EnableTelepenNumericMode = enableTelepenNumericMode.IsChecked.Value; 
#endif
        }

        #endregion

        #region RSS

        /// <summary>
        /// Handles the Checked event of rssLinkageFlag object.
        /// </summary>
        void rssLinkageFlag_Checked(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.RSSLinkageFlag = rssLinkageFlag.IsChecked.Value; 
#endif
        }

        /// <summary>
        /// Handles the Checked event of rss14StackedOmni object.
        /// </summary>
        void rss14StackedOmni_Checked(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.RSS14StackedOmnidirectional = rss14StackedOmni.IsChecked.Value; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of rssExpandedStackedSegmentPerRow object.
        /// </summary>
        void rssExpandedStackedSegmentPerRow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.RSSExpandedStackedSegmentPerRow = (int)rssExpandedStackedSegmentPerRow.SelectedItem; 
#endif
        }

        #endregion

        #region Postal

        /// <summary>
        /// Handles the ValueChanged event of postalADMiltiplierNumericUpDown object.
        /// </summary>
        void postalADMiltiplierNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.PostBarcodesADMultiplier = postalADMiltiplierNumericUpDown.Value / 10.0; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of australianPostCustomInfoComboBox object.
        /// </summary>
        void australianPostCustomInfoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.AustralianPostCustomerInfoFormat = (AustralianPostCustomerInfoFormat)australianPostCustomInfoComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region Aztec

        /// <summary>
        /// Handles the SelectionChanged event of aztecSymbolComboBox object.
        /// </summary>
        void aztecSymbolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            AztecSymbolType oldValue = BarcodeWriterSettings.AztecSymbol;
            try
            {
                BarcodeWriterSettings.AztecSymbol = (AztecSymbolType)aztecSymbolComboBox.SelectedItem;
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.AztecSymbol = oldValue;
                aztecSymbolComboBox.SelectedItem = oldValue;
            } 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of aztecLayersCountComboBox object.
        /// </summary>
        void aztecLayersCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            int oldValue = BarcodeWriterSettings.AztecDataLayers;
            try
            {
                BarcodeWriterSettings.AztecDataLayers = aztecLayersCountComboBox.SelectedIndex;
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.AztecDataLayers = oldValue;
                aztecLayersCountComboBox.SelectedIndex = oldValue;
            } 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of aztecEncodingModeComboBox object.
        /// </summary>
        void aztecEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.AztecEncodingMode = (AztecEncodingMode)aztecEncodingModeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of aztecErrorCorrectionNumericUpDown object.
        /// </summary>
        void aztecErrorCorrectionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.AztecErrorCorrectionDataPercent = aztecErrorCorrectionNumericUpDown.Value; 
#endif
        }

        #endregion

        #region DataMatrix

        /// <summary>
        /// Handles the SelectionChanged event of datamatrixEncodingModeComboBox object.
        /// </summary>
        void datamatrixEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.DataMatrixEncodingMode = (DataMatrixEncodingMode)datamatrixEncodingModeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of datamatrixSymbolSizeComboBox object.
        /// </summary>
        void datamatrixSymbolSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if (BarcodeWriterSettings == null)
                return;
            DataMatrixSymbolType oldValue = BarcodeWriterSettings.DataMatrixSymbol;
            try
            {
                BarcodeWriterSettings.DataMatrixSymbol = (DataMatrixSymbolType)datamatrixSymbolSizeComboBox.SelectedItem;
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.DataMatrixSymbol = oldValue;
                datamatrixSymbolSizeComboBox.SelectedItem = oldValue;
            } 
#endif
        }

        #endregion

        #region QR

        /// <summary>
        /// Handles the SelectionChanged event of qrEncodingModeComboBox object.
        /// </summary>
        void qrEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            QREncodingMode oldValue = BarcodeWriterSettings.QREncodingMode;
            try
            {
                BarcodeWriterSettings.BeginInit();
                try
                {
                    BarcodeWriterSettings.QREncodingMode = (QREncodingMode)qrEncodingModeComboBox.SelectedItem;
                    EncodeValue();
                }
                finally
                {
                    BarcodeWriterSettings.EndInit();
                }
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.QREncodingMode = oldValue;
                qrEncodingModeComboBox.SelectedItem = oldValue;
            } 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of qrSymbolSizeComboBox object.
        /// </summary>
        void qrSymbolSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.QRSymbol = (QRSymbolVersion)qrSymbolSizeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of qrECCLevelComboBox object.
        /// </summary>
        void qrECCLevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.QRErrorCorrectionLevel = (QRErrorCorrectionLevel)qrECCLevelComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region MicroQR

        /// <summary>
        /// Handles the SelectionChanged event of microQrEncodingModeComboBox object.
        /// </summary>
        void microQrEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.QREncodingMode = (QREncodingMode)microQrEncodingModeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of microQrSymbolSizeComboBox object.
        /// </summary>
        void microQrSymbolSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            QRSymbolVersion oldValue = BarcodeWriterSettings.QRSymbol;
            try
            {
                BarcodeWriterSettings.QRSymbol = (QRSymbolVersion)microQrSymbolSizeComboBox.SelectedItem;
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.QRSymbol = oldValue;
                microQrSymbolSizeComboBox.SelectedItem = oldValue;
            } 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of microQrECCLevelComboBox object.
        /// </summary>
        void microQrECCLevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.QRErrorCorrectionLevel = (QRErrorCorrectionLevel)microQrECCLevelComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region MaxiCode

        /// <summary>
        /// Handles the ValueChanged event of maxiCodeResolutonNumericUpDown object.
        /// </summary>
        void maxiCodeResolutonNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MaxiCodeResolution = maxiCodeResolutonNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of maxiCodeEncodingModeComboBox object.
        /// </summary>
        void maxiCodeEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            MaxiCodeEncodingMode oldValue = BarcodeWriterSettings.MaxiCodeEncodingMode;
            try
            {
                BarcodeWriterSettings.MaxiCodeEncodingMode = (MaxiCodeEncodingMode)maxiCodeEncodingModeComboBox.SelectedItem;
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.MaxiCodeEncodingMode = oldValue;
                maxiCodeEncodingModeComboBox.SelectedItem = oldValue;
            } 
#endif
        }

        #endregion

        #region PDF417

        /// <summary>
        /// Handles the SelectionChanged event of pdf417EncodingModeComboBox object.
        /// </summary>
        void pdf417EncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.PDF417EncodingMode = (PDF417EncodingMode)pdf417EncodingModeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of pdf417ErrorCorrectionComboBox object.
        /// </summary>
        void pdf417ErrorCorrectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.PDF417ErrorCorrectionLevel = (PDF417ErrorCorrectionLevel)pdf417ErrorCorrectionComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of pdf417RowsNumericUpDown object.
        /// </summary>
        void pdf417RowsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.PDF417Rows = (int)pdf417RowsNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of pdf417ColsNumericUpDown object.
        /// </summary>
        void pdf417ColsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            int oldValue = BarcodeWriterSettings.PDF417Columns;
            try
            {
                BarcodeWriterSettings.PDF417Columns = (int)pdf417ColsNumericUpDown.Value;
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.PDF417Columns = oldValue;
                pdf417ColsNumericUpDown.Value = oldValue;
            } 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of pdf417RowHeightNumericUpDown object.
        /// </summary>
        void pdf417RowHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.PDF417RowHeight = (int)pdf417RowHeightNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the Checked event of pdf417CompactCheckBox object.
        /// </summary>
        void pdf417CompactCheckBox_Checked(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if (pdf417CompactCheckBox.IsChecked.Value == true)
                BarcodeWriterSettings.Barcode = BarcodeType.PDF417Compact;
            else
                BarcodeWriterSettings.Barcode = BarcodeType.PDF417; 
#endif
        }

        #endregion

        #region Micro PDF417

        /// <summary>
        /// Handles the SelectionChanged event of microPdf417EncodingModeComboBox object.
        /// </summary>
        void microPdf417EncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MicroPDF417EncodingMode = (PDF417EncodingMode)microPdf417EncodingModeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of microPdf417SymbolSizeComboBox object.
        /// </summary>
        void microPdf417SymbolSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MicroPDF417Symbol = (MicroPDF417SymbolType)microPdf417SymbolSizeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of microPdf417ColumnsNumericUpDown object.
        /// </summary>
        void microPdf417ColumnsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MicroPDF417Columns = (int)microPdf417ColumnsNumericUpDown.Value; 
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of microPdf417RowHeightNumericUpDown object.
        /// </summary>
        void microPdf417RowHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.MicroPDF417RowHeight = (int)microPdf417RowHeightNumericUpDown.Value; 
#endif
        }

        #endregion

        #region Code 16K

        /// <summary>
        /// Handles the SelectionChanged event of code16KEncodingModeComboBox object.
        /// </summary>
        void code16KEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.Code16KEncodingMode = (Code128EncodingMode)code16KEncodingModeComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of code16KRowsComboBox object.
        /// </summary>
        void code16KRowsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.Code16KRows = (int)code16KRowsComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region Han Xin Code

        /// <summary>
        /// Handles the SelectionChanged event of hanXinCodeEncodingModeComboBox object.
        /// </summary>
        void hanXinCodeEncodingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            HanXinCodeEncodingMode oldValue = BarcodeWriterSettings.HanXinCodeEncodingMode;
            try
            {
                BarcodeWriterSettings.BeginInit();
                try
                {
                    BarcodeWriterSettings.HanXinCodeEncodingMode = (HanXinCodeEncodingMode)hanXinCodeEncodingModeComboBox.SelectedItem;
                    EncodeValue();
                }
                finally
                {
                    BarcodeWriterSettings.EndInit();
                }
            }
            catch (WriterSettingsException exc)
            {
                OnWriterException(exc);
                BarcodeWriterSettings.HanXinCodeEncodingMode = oldValue;
                hanXinCodeEncodingModeComboBox.SelectedItem = oldValue;
            } 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of hanXinCodeSymbolVersionComboBox object.
        /// </summary>
        void hanXinCodeSymbolVersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.HanXinCodeSymbol = (HanXinCodeSymbolVersion)hanXinCodeSymbolVersionComboBox.SelectedItem; 
#endif
        }

        /// <summary>
        /// Handles the SelectionChanged event of hanXinCodeECCLevelComboBox object.
        /// </summary>
        void hanXinCodeECCLevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.HanXinCodeErrorCorrectionLevel = (HanXinCodeErrorCorrectionLevel)hanXinCodeECCLevelComboBox.SelectedItem; 
#endif
        }

        #endregion

        #region DotCode
        /// <summary>
        /// Handles the ValueChanged event of DotCodeHeightNumericUpDown object.
        /// </summary>
        private void DotCodeHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.DotCodeMatrixHeight = (int)dotCodeHeightNumericUpDown.Value;
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of DotCodeWidthNumericUpDown object.
        /// </summary>
        private void DotCodeWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.DotCodeMatrixWidth = (int)dotCodeWidthNumericUpDown.Value;
#endif
        }

        /// <summary>
        /// Handles the ValueChanged event of DotCodeAspectRatioNumericUpDown object.
        /// </summary>
        private void DotCodeAspectRatioNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.DotCodeMatrixWidthHeightRatio = dotCodeAspectRatioNumericUpDown.Value / 10.0;
#endif
        }

        /// <summary>
        /// Handles the CheckedChanged event of DotCodeRectangularModulesCheckBox object.
        /// </summary>
        private void DotCodeRectangularModulesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            BarcodeWriterSettings.DotCodeRectangularModules = dotCodeRectangularModulesCheckBox.IsChecked.Value;
#endif
        }

        #endregion

        #endregion


        #region Tools

#if !REMOVE_BARCODE_SDK
        private void SelectedBarcodeGroupsTab(BarcodeType barcode)
        {
            if (twoDimensionalBarcodeComboBox.Items.Contains(barcode))
            {
                barcodeGroupsTabPages.SelectedItem = barcodes2DTabItem;
                twoDimensionalBarcodeComboBox.SelectedItem = barcode;
            }
            else
            {
                barcodeGroupsTabPages.SelectedItem = linearBarcodesTabItem;
                linearBarcodeTypeComboBox.SelectedItem = barcode;
            }
        } 
#endif

        /// <summary>
        /// Add a enum values to ComboBox items.
        /// </summary>
        private static void AddEnumValues(ComboBox comboBox, Type type)
        {
            Array values = Enum.GetValues(type);
            for (int i = 0; i < values.Length; i++)
                comboBox.Items.Add(values.GetValue(i));
        }

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Called when writer exception occurs.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private void OnWriterException(WriterSettingsException ex)
        {
            if (WriterException != null)
                WriterException(this, new Vintasoft.Imaging.ExceptionEventArgs(ex));
            else
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        } 
#endif

#endregion

#endregion



#region Events

        /// <summary>
        /// Occurs when writer throws exception.        
        /// </summary>
        public event EventHandler<Vintasoft.Imaging.ExceptionEventArgs> WriterException;

#endregion

    }
}
