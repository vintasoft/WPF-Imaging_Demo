using System;
using System.Windows;

using Vintasoft.Imaging.Wpf;


namespace WpfImagingDemo
{
    /// <summary>
    /// A window that allows to resize canvas of image.
    /// </summary>
    public partial class WpfResizeCanvasWindow : Window
    {

        #region Constructor

        public WpfResizeCanvasWindow(int width, int height)
        {
            InitializeComponent();

            widthNumericUpDown.Value = width;
            heightNumericUpDown.Value = height;
        }

        #endregion



        #region Properties

        public System.Drawing.Point ImagePosition
        {
            get
            {
                return new System.Drawing.Point((int)Math.Round(xPositionNumericUpDown.Value), (int)Math.Round(yPositionNumericUpDown.Value));
            }
        }

        public int CanvasWidth
        {
            get
            {
                return (int)Math.Round(widthNumericUpDown.Value);
            }
        }

        public int CanvasHeight
        {
            get
            {
                return (int)Math.Round(heightNumericUpDown.Value);
            }
        }

        public System.Drawing.Color CanvasColor
        {
            get
            {
                return WpfObjectConverter.CreateDrawingColor(canvasColorPanelControl.Color);
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of OkButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion

    }
}
