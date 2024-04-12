using System.Windows;

namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to specify parameters for pasting an image.
    /// </summary>
    public partial class WpfPasteImageWindow : Window
    {

        #region Constructor

        public WpfPasteImageWindow(int imageWidth, int imageHeight)
        {
            InitializeComponent();

            xCoordNumericUpDown.Maximum = imageWidth;
            yCoordNumericUpDown.Maximum = imageHeight;
        }

        #endregion



        #region Properties

        int _xCoord = 0;
        public int X
        {
            get
            {
                return _xCoord;
            }
        }

        int _yCoord = 0;
        public int Y
        {
            get
            {
                return _yCoord;
            }
        }

        #endregion
        


        #region Methods

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            _xCoord = (int)xCoordNumericUpDown.Value;
            _yCoord = (int)yCoordNumericUpDown.Value;
            this.DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

    }
}
