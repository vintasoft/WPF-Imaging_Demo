using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.UI.VisualTools.UserInteraction;

using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;


namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Represents an interaction button.
    /// </summary>
    internal class InteractionButton : WpfInteractionArea
    {

        #region Fields

        int _distance = 6;

        IWpfRectangularInteractiveObject _interactiveObject;

        #endregion



        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionButton"/> class.
        /// </summary>
        internal InteractionButton(IWpfRectangularInteractiveObject interactiveObject)
            : base("ExecuteButton", InteractionAreaType.Clickable)
        {
            _interactiveObject = interactiveObject;
            Cursor = Cursors.Arrow;
        }

        #endregion



        #region Properties

        VintasoftImage _image;
        /// <summary>
        /// Gets or sets the button image.
        /// </summary>
        internal VintasoftImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        float _x;
        /// <summary>
        /// Gets or sets X coordinate offset.
        /// </summary>
        internal float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Draws interaction area on specified <see cref="DrawingContext"/>.
        /// </summary>
        public override void Render(WpfImageViewer viewer, DrawingContext drawingContext)
        {
            if (_image != null)
            {
                Rect buttonRect = GetBoundingBox(viewer);
                VintasoftImageRenderer.Draw(_image, drawingContext, new Rect(0, 0, _image.Width, _image.Height), buttonRect);
            }
        }

        /// <summary>
        /// Returns a bounding box of interaction area in viewer space.
        /// </summary>
        public override Rect GetBoundingBox(WpfImageViewer viewer)
        {
            if (_image == null)
                return Rect.Empty;
            double x0, y0, x1, y1;
            _interactiveObject.GetRectangle(out x0, out y0, out x1, out y1);
            Rect objectRect = new Rect(x0, y0, x1 - x0, y1 - y0);
            objectRect = TransformRect(objectRect, _interactiveObject.GetPointTransform(viewer, viewer.Image));
            return new Rect(objectRect.X + X, objectRect.Y - _distance - _image.Height, _image.Width, _image.Height);
        }

        /// <summary>
        /// Transforms the rectangle and returns the bounding box.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="pointTransform">The point transform.</param>
        /// <returns>The bounding box of transformed rectangle.</returns>
        private Rect TransformRect(Rect rect, WpfPointTransform pointTransform)
        {
            Point point1 = pointTransform.TransformPoint(rect.TopLeft);
            Point point2 = pointTransform.TransformPoint(rect.TopRight);
            Point point3 = pointTransform.TransformPoint(rect.BottomRight);
            Point point4 = pointTransform.TransformPoint(rect.BottomLeft);
            double x = Math.Min(Math.Min(point1.X, point2.X), Math.Min(point3.X, point4.X));
            double y = Math.Min(Math.Min(point1.Y, point2.Y), Math.Min(point3.Y, point4.Y));
            double width = Math.Max(Math.Max(point1.X, point2.X), Math.Max(point3.X, point4.X)) - x;
            double height = Math.Max(Math.Max(point1.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - y;
            return new Rect(x, y, width, height);
        }

        #endregion

    }
}
