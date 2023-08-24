using System;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging.Codecs.ImageFiles.Tiff;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to add new TIFF tag to a TIFF image.
    /// </summary>
    public partial class AddTiffTagWindow : Window
    {

        #region Constructor

        public AddTiffTagWindow()
        {
            InitializeComponent();
            tagDataTypeComboBox.SelectedIndex = 0;
        }

        #endregion



        #region Properties

        ushort _tagId = 58000;
        public ushort TagId
        {
            get
            {
                return _tagId;
            }
        }

        TiffTagDataType _tagDataType = TiffTagDataType.Ascii;
        public TiffTagDataType TagDataType
        {
            get
            {
                return _tagDataType;
            }
        }

        public string StringValue
        {
            get
            {
                return stringValueTextBox.Text;
            }
        }

        public int IntegerValue
        {
            get
            {
                return (int)integerValueNumericUpDown.Value;
            }
        }

        public double DoubleValue
        {
            get
            {
                double value;
                if (double.TryParse(doubleValueTextBox.Text, out value))
                    return value;
                else
                    return 0.0;
            }
        }

        public int NumeratorValue
        {
            get
            {
                return (int)rationalValueNumeratorNumericUpDown.Value;
            }
        }

        public int DenominatorValue
        {
            get
            {
                return (int)rationalValueDenominatorNumericUpDown.Value;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of OkButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Enum.IsDefined(typeof(ReadOnlyTiffTagId), (ReadOnlyTiffTagId)_tagId))
                {
                    MessageBox.Show("Tag with ID from ReadOnlyTiffTagId enumeration cannot be added/updated.", "Add tag");
                    return;
                }
            }
            catch (ArgumentException)
            {
            }

            if (_tagDataType == TiffTagDataType.Float ||
                _tagDataType == TiffTagDataType.Double)
            {
                double value;
                if (!double.TryParse(doubleValueTextBox.Text, out value))
                {
                    MessageBox.Show("Double value is incorrect!", "Add tag");
                    return;
                }
            }

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of CancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the SelectionChanged event of TagDataTypeComboBox object.
        /// </summary>
        private void tagDataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tagDataTypeComboBox.SelectedIndex)
            {
                case 0: _tagDataType = TiffTagDataType.Ascii; break;
                case 1: _tagDataType = TiffTagDataType.SShort; break;
                case 2: _tagDataType = TiffTagDataType.Short; break;
                case 3: _tagDataType = TiffTagDataType.SLong; break;
                case 4: _tagDataType = TiffTagDataType.Long; break;
                case 5: _tagDataType = TiffTagDataType.Float; break;
                case 6: _tagDataType = TiffTagDataType.Double; break;
                case 7: _tagDataType = TiffTagDataType.Rational; break;
                case 8: _tagDataType = TiffTagDataType.SRational; break;
            }
            switch (tagDataTypeComboBox.SelectedIndex)
            {
                case 0:
                    SetGroupBoxsVisible(true, false, false, false);
                    break;

                case 1:
                case 2:
                case 3:
                case 4:
                    SetGroupBoxsVisible(false, true, false, false);
                    break;

                case 5:
                case 6:
                    SetGroupBoxsVisible(false, false, true, false);
                    break;

                case 7:
                case 8:
                    SetGroupBoxsVisible(false, false, false, true);
                    break;
            }
        }

        private void SetGroupBoxsVisible(bool stringVisible, bool integerVisible, bool doubleVisible, bool rationalVisible)
        {
            stringValueGroupBox.Visibility = stringVisible ? Visibility.Visible : Visibility.Collapsed;
            integerValueGroupBox.Visibility = integerVisible ? Visibility.Visible : Visibility.Collapsed;
            doubleValueGroup.Visibility = doubleVisible ? Visibility.Visible : Visibility.Collapsed;
            rationalValueGroupBox.Visibility = rationalVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the ValueChanged event of TagIdNumericUpDown object.
        /// </summary>
        private void tagIdNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _tagId = (ushort)tagIdNumericUpDown.Value;
        }

        #endregion

    }
}
