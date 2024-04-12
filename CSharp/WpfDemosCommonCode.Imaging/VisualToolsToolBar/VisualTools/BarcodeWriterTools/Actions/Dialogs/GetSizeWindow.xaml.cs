using System;
using System.Globalization;
using System.Windows;
#if !REMOVE_BARCODE_SDK
using Vintasoft.Barcode; 
#endif

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// GetSizeWindow logic.
    /// </summary>
    public partial class GetSizeWindow : Window
    {
#if !REMOVE_BARCODE_SDK
        public GetSizeWindow(string name, double value, int resolution, UnitOfMeasure units)
        {
            InitializeComponent();

            Title = string.Format(Title, name);
            sizeLabel.Content = string.Format((string)sizeLabel.Content, name);

            valueTextBox.Text = value.ToString();

            unitsComboBox.Items.Add(UnitOfMeasure.Inches);
            unitsComboBox.Items.Add(UnitOfMeasure.Centimeters);
            unitsComboBox.Items.Add(UnitOfMeasure.Millimeters);
            unitsComboBox.Items.Add(UnitOfMeasure.Pixels);
            unitsComboBox.SelectedItem = units;

            resolutionNumericUpDown.Value = (int)resolution;
        } 
#else
        public GetSizeWindow()
        {
            InitializeComponent();
        }
#endif

        double _value;
        public double Value
        {
            get
            {
                return _value;
            }
        }

#if !REMOVE_BARCODE_SDK
        UnitOfMeasure _units;
        public UnitOfMeasure Units
        {

            get
            {
                return _units;
            }
        } 
#endif

        int _resolution;
        public int Resolution
        {
            get
            {
                return _resolution;
            }
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            _units = (UnitOfMeasure)unitsComboBox.SelectedItem;
            _resolution = (int)resolutionNumericUpDown.Value;
            try
            {
                _value = double.Parse(valueTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true; 
#endif
        }
    }
}
