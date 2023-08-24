using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfImagingDemo
{
    class WpfTileReflectionWindow : WpfThreeParamsConfigWindow
    {

		#region Constructor

        public WpfTileReflectionWindow(WpfImageViewer viewer)
			: base(viewer,
            "Tile / reflection",
            new WpfImageProcessingParameter("Rotation angle (degrees)", -45, 45, 30),
            new WpfImageProcessingParameter("Tile size (pixels)", 2, 200, 40),
            new WpfImageProcessingParameter("Curvature", -20, 20, 8))
		{
		}

		#endregion



		#region Properties

        /// <summary>
        /// Gets the rotation angle in degrees.
        /// </summary>
		public int RotationAngle
		{
			get
			{
				return Parameter1;
			}
		}

        /// <summary>
        /// Gets the tile size in pixels.
        /// </summary>
		public int SquareSize
		{
			get
			{
				return Parameter2;
			}
		}

        /// <summary>
        /// Gets the level of curvature at the borders of the tile.
        /// </summary>
		public int Curvature
		{
			get
			{
				return Parameter3;
			}
		}

		#endregion



        #region Methods

        /// <summary>
        /// Gets the current image processing command.
        /// </summary>
        /// <returns>Current image processing command.</returns>
        public override ProcessingCommandBase GetProcessingCommand()
        {
            return new TileReflectionCommand(RotationAngle, SquareSize, Curvature);
        }

        #endregion 

    }
}
