using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

#if !REMOVE_BARCODE_SDK
using Vintasoft.Barcode.GS1; 
#endif

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// A window that allows to edit the GS1 value.
    /// </summary>
    public partial class GS1ValueEditorWindow : Window
    {

        #region Nested class

        private class ListViewData
        {

            #region Constructor

#if !REMOVE_BARCODE_SDK
            public ListViewData(GS1ApplicationIdentifierValue value)
            {
                _ai = value.ApplicationIdentifier.ApplicationIdentifier;
                _aiTitle = value.ApplicationIdentifier.DataTitle;
                _aiValue = value.Value;
            } 
#endif

            #endregion



            #region Properties

            private string _ai;
            public string Ai
            {
                get
                {
                    return _ai;
                }
                set
                {
                    _ai = value;
                }
            }

            private string _aiTitle;
            public string AiTitle
            {
                get
                {
                    return _aiTitle;
                }
                set
                {
                    _aiTitle = value;
                }
            }

            private string _aiValue;
            public string AiValue
            {
                get
                {
                    return _aiValue;
                }
                set
                {
                    _aiValue = value;
                }
            }

            #endregion

        }

        #endregion



        #region Fields

        bool _readOnly = false;
        bool _existsAISelected = false;
#if !REMOVE_BARCODE_SDK
        List<GS1ApplicationIdentifierValue> _identifierValuesList = new List<GS1ApplicationIdentifierValue>(); 
#endif

        #endregion



        #region Constructor

#if !REMOVE_BARCODE_SDK
        public GS1ValueEditorWindow(GS1ApplicationIdentifierValue[] gs1ApplicationIdentifierValues, bool readOnly)
        {
            InitializeComponent();

            formatStructureValueLabel.Content = "'Nk' - k numeric digits, fixed length\r" +
                    "'N..k' - up to k numeric digits\r'X..k' - up to k characters ISO646";

            aiNumberComboBox.SelectionChanged += new SelectionChangedEventHandler(aiNumberComboBox_SelectionChanged);
            aiListView.SelectionChanged += new SelectionChangedEventHandler(aiListView_SelectionChanged);

            if (readOnly)
            {
                addButton.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;
            }
            aiNumberComboBox.IsEnabled = !readOnly;
            GS1ApplicationIdentifier[] applicationIdentifiers = GS1ApplicationIdentifiers.ApplicationIdentifiers;
            for (int i = 0; i < applicationIdentifiers.Length; i++)
                aiNumberComboBox.Items.Add(string.Format("{0}: {1}", applicationIdentifiers[i].ApplicationIdentifier, applicationIdentifiers[i].DataTitle));
            _GS1ApplicationIdentifierValues = gs1ApplicationIdentifierValues;
            _identifierValuesList.AddRange(gs1ApplicationIdentifierValues);
            _readOnly = readOnly;
            aiValueTextBox.IsReadOnly = readOnly;

            if (readOnly)
                Title = "GS1 Value Decoder";
            else
                Title = "GS1 Value Editor";

            ShowPrintableValue();
            ShowAI();
        } 
#else
        public GS1ValueEditorWindow()
        {
            InitializeComponent();
        }
#endif

        #endregion



        #region Properties

#if !REMOVE_BARCODE_SDK
        GS1ApplicationIdentifierValue[] _GS1ApplicationIdentifierValues;
        public GS1ApplicationIdentifierValue[] GS1ApplicationIdentifierValues
        {
            get
            {
                return _GS1ApplicationIdentifierValues;
            }
        } 
#endif


        #endregion



        #region Methods

        /// <summary>
        /// Handles the SelectionChanged event of aiNumberComboBox object.
        /// </summary>
        void aiNumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_existsAISelected)
            {
#if !REMOVE_BARCODE_SDK
                GS1ApplicationIdentifier ai = GS1ApplicationIdentifiers.ApplicationIdentifiers[aiNumberComboBox.SelectedIndex];
                aiDataContentLabel.Content = ai.DataContent;
                string format = ai.Format;
                if (ai.IsContainsDecimalPoint)
                    format += " (with decimal point)";
                aiDataFormatLabel.Content = format;
                aiValueTextBox.Text = ""; 
#endif
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of aiListView object.
        /// </summary>
        void aiListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if (aiListView.SelectedIndex >= 0)
            {
                ListViewData listViewData = aiListView.SelectedItem as ListViewData;
                aiNumberComboBox.SelectedIndex = GS1ApplicationIdentifiers.IndexOfApplicationIdentifier(listViewData.Ai);
                aiValueTextBox.Text = listViewData.AiValue; 
            }
#endif
        }

        /// <summary>
        /// Handles the Click event of addButton object.
        /// </summary>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            AddAI(GS1ApplicationIdentifiers.ApplicationIdentifiers[aiNumberComboBox.SelectedIndex].ApplicationIdentifier, aiValueTextBox.Text); 
#endif
        }

        /// <summary>
        /// Handles the Click event of deleteButton object.
        /// </summary>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            if(aiListView.Items.Count > 0 && aiListView.SelectedIndex >= 0)
            {
                int index = aiListView.SelectedIndex;
                _identifierValuesList.RemoveAt(index);
                aiListView.Items.RemoveAt(index);
                ShowPrintableValue(); 
            }
#endif
        }

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_BARCODE_SDK
            _GS1ApplicationIdentifierValues = _identifierValuesList.ToArray(); 
#endif
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of buttonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ShowAI()
        {
#if !REMOVE_BARCODE_SDK
            if (_GS1ApplicationIdentifierValues.Length == 0)
            {
                aiNumberComboBox.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < _identifierValuesList.Count; i++)
                    AddAIToTable(_identifierValuesList[i]);
            } 
#endif
        }

#if !REMOVE_BARCODE_SDK
        private void AddAIToTable(GS1ApplicationIdentifierValue value)
        {
            aiListView.Items.Add(new ListViewData(value));
        } 
#endif

        private void AddAI(string number, string value)
        {
#if !REMOVE_BARCODE_SDK
            GS1ApplicationIdentifierValue ai = null;
            try
            {
                ai = new GS1ApplicationIdentifierValue(GS1ApplicationIdentifiers.FindApplicationIdentifier(number), value);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _identifierValuesList.Add(ai); 
            ShowPrintableValue();
            AddAIToTable(ai);
#endif
        }

        private void ShowPrintableValue()
        {
#if !REMOVE_BARCODE_SDK
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _identifierValuesList.Count; i++)
                sb.Append(_identifierValuesList[i].ToString()); 
            gs1BarcodePrintableValueTextBox.Text = sb.ToString();
#endif
        }

        #endregion

    }
}
