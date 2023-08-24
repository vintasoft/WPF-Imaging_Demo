using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A control that allows to select color.
    /// </summary>
    public partial class SimpleColorPicker : UserControl
    {

        #region DataItem class

        internal class DataItem
        {

            #region Constructors

            public DataItem(string name, Color color)
            {
                _name = name;
                _color = color;
                _brush = new SolidColorBrush(color);
            }

            #endregion



            #region Properties

            Color _color;
            public Color Color
            {
                get
                {
                    return _color;
                }
            }
            
            Brush _brush;
            public Brush Brush
            {
                get
                {
                    return _brush;
                }

            }
            
            string _name;
            public string Name
            {
                get
                {
                    return _name;
                }
            }

            #endregion

        }

        #endregion



        #region Fields

        Dictionary<Color, DataItem> _colorToDataItems = new Dictionary<Color, DataItem>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleColorPicker"/> class.
        /// </summary>
        public SimpleColorPicker()
        {
            InitializeComponent();

            PropertyInfo[] pi = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < pi.Length; i++)
            {
                Color color = (Color)pi[i].GetValue(null, null);
                DataItem item = new DataItem(pi[i].Name, color);
                colorComboBox.Items.Add(item);
                if (!_colorToDataItems.ContainsKey(color))
                    _colorToDataItems.Add(color, item);
            }

            colorComboBox.SelectionChanged += new SelectionChangedEventHandler(colorComboBox_SelectionChanged);
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color SelectedColor
        {
            get 
            {
                if (colorComboBox.SelectedItem == null)
                    return Colors.Black;
                return ((DataItem)colorComboBox.SelectedItem).Color; 
            }
            set
            {
                if (_colorToDataItems.ContainsKey(value))
                    colorComboBox.SelectedItem = _colorToDataItems[value];
                else
                    colorComboBox.SelectedItem = new DataItem(value.ToString(), value);
                OnSelectedColorChanged();
            }
        }

        #endregion



        #region Methods

        protected virtual void OnSelectedColorChanged()
        {
            if (SelectedColorChanged != null)
                SelectedColorChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the SelectionChanged event of ColorComboBox object.
        /// </summary>
        private void colorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedColorChanged();
        }


        #endregion



        #region Events

        public event EventHandler SelectedColorChanged;

        #endregion

    }
}
