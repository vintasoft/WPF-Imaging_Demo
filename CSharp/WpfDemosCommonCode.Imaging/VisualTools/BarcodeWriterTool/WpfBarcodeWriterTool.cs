using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.UI.VisualTools.UserInteraction;

using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;

#if !REMOVE_BARCODE_SDK
using Vintasoft.Barcode; 
#endif


namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Visual tool for barcode generation on image in image viewer.
    /// </summary>
    public class WpfBarcodeWriterTool : WpfRectangularSelectionTool
    {

        #region Fields

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Barcode writer.
        /// </summary>
        BarcodeWriter _writer; 
#endif

        /// <summary>
        /// Panel with buttons.
        /// </summary>
        InteractionButtonsPanel _controlButtonsPanel;

        /// <summary>
        /// Button for refreshing the barcode image.
        /// </summary>
        InteractionButton _refreshBarcodeButton;

        /// <summary>
        /// Button for drawing the barcode image.
        /// </summary>
        InteractionButton _drawBarcodeButton;

        /// <summary>
        /// Barcode image rectangle which was used for aligning the selection region of tool
        /// on previous step.
        /// </summary>
        Rect _alignedRectangle;

        /// <summary>
        /// Determines that barcode image must be recreated when transformation is finished.
        /// </summary>
        bool _needBuildBarcodeImage = false;

        /// <summary>
        /// Determines when barcode generator settings is changing.
        /// </summary>
        bool _settingsChanging = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes the <see cref="WpfBarcodeWriterTool"/> class.
        /// </summary>
        static WpfBarcodeWriterTool()
        {
            Vintasoft.Barcode.WpfAssembly.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfBarcodeWriterTool"/> class. 
        /// </summary>
        public WpfBarcodeWriterTool()
            : base()
        {
#if !REMOVE_BARCODE_SDK
            _writer = new BarcodeWriter();
            _writer.Settings.Changed += new EventHandler(Settings_Changed);
            _writer.Settings.PixelFormat = BarcodeImagePixelFormat.Bgra32; 
#endif

            base.DefaultCursor = Cursors.Cross;
            base.ActionCursor = base.DefaultCursor;

            WpfRectangularObjectTransformer transformer = new WpfRectangularObjectTransformer(this);
            transformer.HideInteractionPointsWhenMoving = true;
            TransformController = transformer;

            // create the "Refresh barcode" button
            _refreshBarcodeButton = new InteractionButton(this);
            _refreshBarcodeButton.Image = DemosResourcesManager.GetResourceAsImage("RefreshBarcodeImage.png");
            // create the "Draw barcode" button
            _drawBarcodeButton = new InteractionButton(this);
            _drawBarcodeButton.Image = DemosResourcesManager.GetResourceAsImage("DrawBarcodeImage.png");
            // create the panel with buttons
            _controlButtonsPanel = new InteractionButtonsPanel(this, _refreshBarcodeButton, _drawBarcodeButton);
            _controlButtonsPanel.Interaction += new EventHandler<WpfInteractionEventArgs>(ControlButtonsPanel_Interaction);
        }

        #endregion



        #region Properties

        #region PUBLIC

        /// <summary>
        /// Gets the name of visual tool.
        /// </summary>
        public override string ToolName
        {
            get
            {
                return "Barcode writer tool";
            }
        }

        /// <summary>
        /// Gets the value indicating whether this visual tool can modify the image.
        /// </summary>
        public override bool CanModifyImage
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// Gets or sets an interaction controller of barcode writer tool.
        /// </summary>
        public override IWpfInteractionController InteractionController
        {
            get
            {
                return base.InteractionController;
            }
            set
            {
                if (value != null && value == TransformController)
                    value = new WpfCompositeInteractionController(value, _controlButtonsPanel);
                base.InteractionController = value;
            }
        }

        /// <summary>
        /// Gets or sets an image for button that refreshes the barcode image.
        /// </summary>
        public VintasoftImage RefreshBarcodeButtonImage
        {
            get
            {
                return _refreshBarcodeButton.Image;
            }
            set
            {
                if (RefreshBarcodeButtonImage != value)
                {
                    _refreshBarcodeButton.Image = value;
                    InvalidateVisual();
                }
            }
        }


        /// <summary>
        /// Gets or sets an image for button that draws a barcode image on the image.
        /// </summary>
        public VintasoftImage DrawBarcodeButtonImage
        {
            get
            {
                return _drawBarcodeButton.Image;
            }
            set
            {
                if (DrawBarcodeButtonImage != value)
                {
                    _drawBarcodeButton.Image = value;
                    InvalidateVisual();
                }
            }
        }

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Gets or sets the barcode writer settings.
        /// </summary>
        public WriterSettings WriterSettings
        {
            get
            {
                return _writer.Settings;
            }
            set
            {
                if (_writer.Settings != value)
                {
                    _writer.Settings.Changed -= new EventHandler(Settings_Changed);
                    _writer.Settings = value;
                    _writer.Settings.Changed += new EventHandler(Settings_Changed);
                }
            }
        } 
#endif

        #endregion


        #region PRIVATE

        VintasoftImage _barcodeImage;
        /// <summary>
        /// Gets or sets a current barcode image.
        /// </summary>
        private VintasoftImage BarcodeImage
        {
            get
            {
                return _barcodeImage;
            }
            set
            {
                VintasoftImage image = _barcodeImage;
                _barcodeImage = value;
                if (image != null)
                    image.Dispose();
            }
        }

        #endregion

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Returns barcode as <see cref="VintasoftImage"/>.
        /// </summary>
        public VintasoftImage GetBarcodeImage()
        {
#if !REMOVE_BARCODE_SDK
            return VintasoftImageConverter.FromBitmapSource(_writer.GetBarcodeAsBitmapSource()); 
#else
            return null;
#endif
        }

        /// <summary>
        /// Returns barcode as <see cref="PathGeometry"/>.
        /// </summary>
        public PathGeometry GetBarcodePathGeometry()
        {
#if !REMOVE_BARCODE_SDK
            return _writer.GetBarcodeAsPathGeometry(); 
#else
            return null;
#endif
        }

        /// <summary>
        /// Refreshes the current barcode image.
        /// </summary>
        /// <param name="needAlignRectangleSize">
        /// Indicates that the selection rectangle of this tool must be aligned to the barcode image.
        /// </param>
        public void RefreshBarcodeImage(bool needAlignRectangleSize)
        {
            // if selection is empty
            if (Rectangle.Width <= 0 || Rectangle.Height <= 0)
            {
                BarcodeImage = null;
                return;
            }

            // determines that selection region of tool was aligned to the barcode image on previous step
            bool currentRectangeIsNotAligned = _alignedRectangle.Size != Rectangle.Size;

            // if selection region of tool is not aligned on barcode image
            if (currentRectangeIsNotAligned)
            {
                _settingsChanging = true;
#if !REMOVE_BARCODE_SDK
                if (WriterSettings.Barcode == BarcodeType.MaxiCode)
                {
                    _writer.Settings.MaxiCodeResolution = (int)(Rectangle.Width / 30.0) * 30;
                }
                else
                {
                    double width = Rectangle.Width - WriterSettings.Padding * WriterSettings.MinWidth * 2;
                    double height = Rectangle.Height - WriterSettings.Padding * WriterSettings.MinWidth * 2;

                    if (width <= 0 || height <= 0)
                    {
                        width = Rectangle.Width;
                        height = Rectangle.Height;
                    }

                    bool is2dBarcode =
                        WriterSettings.Barcode == BarcodeType.MicroQR ||
                        WriterSettings.Barcode == BarcodeType.QR ||
                        WriterSettings.Barcode == BarcodeType.PDF417 ||
                        WriterSettings.Barcode == BarcodeType.PDF417Compact ||
                        WriterSettings.Barcode == BarcodeType.DataMatrix ||
                        WriterSettings.Barcode == BarcodeType.Aztec ||
                        WriterSettings.Barcode == BarcodeType.MaxiCode ||
                        WriterSettings.Barcode == BarcodeType.HanXinCode;

                    if ((is2dBarcode && WriterSettings.Value2DVisible) ||
                        (!is2dBarcode && WriterSettings.ValueVisible))
                    {
                        int valueGap = 0;
                        if (WriterSettings.ValueGap > 0)
                            valueGap = WriterSettings.ValueGap;
                        height -= valueGap + WriterSettings.ValueFont.LineHeight;
                    }

                    _writer.Settings.SetWidth((int)width);
                    _writer.Settings.Height = (int)height;
                } 
#endif
                _settingsChanging = false;
            }

#if !REMOVE_BARCODE_SDK
            // generate the barcode image
            try
            {
                BarcodeImage = VintasoftImageConverter.FromBitmapSource(_writer.GetBarcodeAsBitmapSource());
            }
            catch (WriterSettingsException ex)
            {
                // generate image with error message
                int errorMessageImageWidth = (int)Math.Round(Rectangle.Width);
                int errorMessageImageHeigh = (int)Math.Round(Rectangle.Height);
                VintasoftImage errorMessageImage = new VintasoftImage(errorMessageImageWidth, errorMessageImageHeigh);
                Graphics g = errorMessageImage.OpenGraphics();
                g.Clear(System.Drawing.Color.White);
                Rectangle rect = new Rectangle(0, 0, errorMessageImage.Width - 1, errorMessageImage.Height - 1);
                g.DrawRectangle(Pens.Red, rect);
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                using (Font font = new Font(ImageViewer.FontFamily.Source, (float)ImageViewer.FontSize))
                {
                    g.DrawString(string.Format("WriterSettingsException:\n{0}", ex.Message), font, System.Drawing.Brushes.Red, rect, format);
                }
                errorMessageImage.CloseGraphics();
                BarcodeImage = errorMessageImage;
                InvalidateVisual();
            } 
#endif

            // if selection region of tool must be aligned to the barcode image AND
            // selection region of tool is not aligned on barcode image
            if (needAlignRectangleSize && currentRectangeIsNotAligned)
            {
                Rectangle = new Rect(Rectangle.X, Rectangle.Y, BarcodeImage.Width, BarcodeImage.Height);
                _alignedRectangle = Rectangle;
            }
            else
                InvalidateVisual();
        }

        /// <summary>
        /// Draws the barcode image on an image in image viewer.
        /// </summary>
        public void DrawBarcodeImage()
        {
            if (ImageViewer != null &&
                ImageViewer.Image != null &&
                BarcodeImage != null &&
                !Rectangle.Size.IsEmpty)
            {
                VintasoftImage image = ImageViewer.Image;

                OnImageChanging(new ImageChangedEventArgs(image));

                VintasoftImage barcodeImage = BarcodeImage;
                bool needDisposeBarcodeImage = false;

                Rect barcodeRect = GetDestBarcodeImageRectangle();

                if ((int)barcodeRect.Width > 0 && (int)barcodeRect.Height > 0)
                {
                    // if image size not equals with rectangle size
                    if (barcodeRect.Width != BarcodeImage.Width || barcodeRect.Height != BarcodeImage.Height)
                    {
                        // resize barcode image
                        needDisposeBarcodeImage = true;
                        ResizeCommand resize = new ResizeCommand((int)barcodeRect.Width, (int)barcodeRect.Height);
                        barcodeImage = resize.Execute(barcodeImage);
                    }

                    // draw barcode image on viewer image
                    OverlayCommand overaly = new OverlayCommand(barcodeImage, new System.Drawing.Point((int)barcodeRect.X, (int)barcodeRect.Y));
                    overaly.ExecuteInPlace(image);

                    if (needDisposeBarcodeImage)
                        barcodeImage.Dispose();

                    Rectangle = Rect.Empty;

                    OnImageChanged(new ImageChangedEventArgs(image));
                }
            }
        }

        /// <summary>
        /// Renders the content of <see cref="WpfBarcodeWriterTool"/>.
        /// </summary>
        /// <param name="dc">An instance of <see cref="System.Windows.Media.DrawingContext" />
        /// used to render the tool.</param>
        protected override void RenderContent(DrawingContext dc)
        {
            if (BarcodeImage == null)
                RefreshBarcodeImage(false);
            if (BarcodeImage != null)
            {
                AffineMatrix transform = ImageViewer.ViewerState.GetTransformFromImageToVisualTool();
                dc.PushTransform(new MatrixTransform(VintasoftWpfConverter.Convert(transform)));
                VintasoftImageRenderer.Draw(
                    BarcodeImage,
                    dc,
                    new Rect(0, 0, BarcodeImage.Width, BarcodeImage.Height),
                    GetDestBarcodeImageRectangle());
                dc.Pop();
            }
            else
            {
                base.RenderContent(dc);
            }
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Raises the interaction event for specified interactive object.
        /// </summary>
        /// <param name="item">The interactive object.</param>
        /// <param name="args">The interaction event args.</param>
        protected override bool OnInteraction(IWpfInteractiveObject item, WpfInteractionEventArgs args)
        {
            bool result = base.OnInteraction(item, args);
            if ((args.Action & InteractionAreaAction.Begin) != 0)
            {
                string interactionName = args.Area.InteractionName;
                _needBuildBarcodeImage = interactionName == "Resize" || interactionName == "Build";
            }
            return result;
        }

        /// <summary>
        /// Finishes an active interaction.
        /// </summary>
        /// <param name="item">Active item.</param>
        protected override void FinishInteraction(IWpfInteractiveObject item)
        {
            base.FinishInteraction(item);
            if (_needBuildBarcodeImage)
            {
                RefreshBarcodeImage(false);
            }
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Returns a rectangle where barcode must be drawn.
        /// </summary>
        private Rect GetDestBarcodeImageRectangle()
        {
#if !REMOVE_BARCODE_SDK
            BarcodeType barcode = WriterSettings.Barcode; 
#endif
            bool needMaintainAspectRatio = false;
#if !REMOVE_BARCODE_SDK
            switch (barcode)
            {
                case BarcodeType.Aztec:
                case BarcodeType.QR:
                case BarcodeType.MicroQR:
                case BarcodeType.DataMatrix:
                case BarcodeType.PDF417:
                case BarcodeType.PDF417Compact:
                case BarcodeType.MaxiCode:
                case BarcodeType.HanXinCode:
                    needMaintainAspectRatio = true;
                    break;
            } 
#endif
            // if we do not need to maintain the aspect ratio
            if (!needMaintainAspectRatio)
                // return the selection region of this tool as rectangle where barcode must be drawn
                return Rectangle;

            double imageWidth = BarcodeImage.Width;
            double imageHeight = BarcodeImage.Height;
            double rectWidth = Rectangle.Width;
            double rectHeight = Rectangle.Height;
            double k = Math.Min(rectWidth / imageWidth, rectHeight / imageHeight);
            imageWidth *= k;
            imageHeight *= k;
            return new Rect(
                (int)Math.Round(Rectangle.X + (Rectangle.Width - imageWidth) / 2),
                (int)Math.Round(Rectangle.Y + (Rectangle.Height - imageHeight) / 2),
                (int)Math.Round(imageWidth),
                (int)Math.Round(imageHeight)
                );
        }

        /// <summary>
        /// User is interacted with control panel.
        /// </summary>
        private void ControlButtonsPanel_Interaction(object sender, WpfInteractionEventArgs e)
        {
            if (e.Action == InteractionAreaAction.Begin)
            {
                if (e.Area == _refreshBarcodeButton)
                    RefreshBarcodeImage(true);
                else if (e.Area == _drawBarcodeButton)
                    DrawBarcodeImage();
            }
        }

        /// <summary>
        /// Barcode writer settings are changed.
        /// </summary>
        private void Settings_Changed(object sender, EventArgs e)
        {
            if (!_settingsChanging)
            {
                _alignedRectangle = Rect.Empty;
                RefreshBarcodeImage(false);
                InvalidateVisual();
            }
        }

#endregion

#endregion

    }
}
