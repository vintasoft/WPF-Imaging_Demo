using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.UI.VisualTools.UserInteraction;
using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;
using Vintasoft.Primitives;
#if !REMOVE_BARCODE_SDK
using Vintasoft.Barcode; 
#endif


namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Visual tool for barcode recognition on image in image viewer.
    /// </summary>
    public class WpfBarcodeReaderTool : WpfRectangularSelectionTool
    {

        #region Fields

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Barcode reader.
        /// </summary>
        BarcodeReader _reader; 
#endif

        /// <summary>
        /// Indicates that the barcode recognition process should be started.
        /// </summary>
        bool _needStartBarcodeRecognition = false;

        /// <summary>
        /// Indicates that the barcode recognition process is started.
        /// </summary>
        bool _isRecognitionStarted = false;

        /// <summary>
        /// Indicates that the barcode recognition process is started asynchronously.
        /// </summary>
        bool _isAsyncRecognitionStarted = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfBarcodeReaderTool"/> class. 
        /// </summary>
        public WpfBarcodeReaderTool()
        {
#if !REMOVE_BARCODE_SDK
            _reader = new BarcodeReader();
            _reader.Progress += new EventHandler<BarcodeReaderProgressEventArgs>(Reader_Progress); 
#endif

            base.DefaultCursor = Cursors.Cross;
            
            WpfRectangularObjectTransformer transformer = new WpfRectangularObjectTransformer(this);
            transformer.HideInteractionPointsWhenMoving = true;
            TransformController = transformer;
            TransformController.Interaction += new EventHandler<WpfInteractionEventArgs>(TransformController_Interaction);

            ActionCursor = new Cursor(DemosResourcesManager.GetResourceAsStream("BarcodeScanner.cur"));
            TransformController.MoveArea.Cursor = ActionCursor;

            SolidColorBrush brush;

            brush = new SolidColorBrush(Color.FromArgb(192, Colors.Green.R, Colors.Green.G, Colors.Green.B));
            _recognizedBarcodePen = new Pen(brush, 2);

            brush = new SolidColorBrush(Color.FromArgb(192, Colors.Red.R, Colors.Red.G, Colors.Red.B));
            _unrecognizedBarcodePen = new Pen(brush, 1);
        }
        
        #endregion



        #region Properties

        /// <summary>
        /// Gets a value indicating whether the barcode recognition process is started.
        /// </summary>
        public bool IsRecognotionStarted
        {
            get
            {
                return _isRecognitionStarted || _isAsyncRecognitionStarted;
            }
        }

        /// <summary>
        /// Gets an information about barcodes read time.
        /// </summary>
        public TimeSpan RecognizeTime
        {
            get
            {
#if !REMOVE_BARCODE_SDK
                return _reader.RecognizeTime; 
#else
                return TimeSpan.MinValue;
#endif
            }
        }

        /// <summary>
        /// Gets the name of the visual tool.
        /// </summary>
        public override string ToolName
        {
            get
            {
                return "Barcode reader tool";
            }
        }

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Gets or sets the barcode reader settings.
        /// </summary>
        public ReaderSettings ReaderSettings
        {
            get
            {
                return _reader.Settings;
            }
            set
            {
                _reader.Settings = value;
            }
        }

        IBarcodeInfo[] _recognitionResults;
        /// <summary>
        /// Gets or sets displaying recognition results.
        /// </summary>
        public IBarcodeInfo[] RecognitionResults
        {
            get
            {
                return _recognitionResults;
            }
            set
            {
                _recognitionResults = value;
                Dispatcher.Invoke(new ThreadStart(InvalidateVisual));
            }
        } 
#endif

        Pen _recognizedBarcodePen;
        /// <summary>
        /// Gets or sets a pen for drawing region of recognized barcode.
        /// </summary>
        public Pen RecognizedBarcodePen
        {
            get
            {
                return _recognizedBarcodePen;
            }
            set
            {
                _recognizedBarcodePen = value;
            }
        }

        Brush _recognizedBarcodeBrush = new SolidColorBrush(Color.FromArgb(48, Colors.Green.R, Colors.Green.G, Colors.Green.B));
        /// <summary>
        /// Gets or sets a brush for filling region of recognized barcode.
        /// </summary>
        public Brush RecognizedBarcodeBrush
        {
            get
            {
                return _recognizedBarcodeBrush;
            }
            set
            {
                _recognizedBarcodeBrush = value;
            }
        }


        Pen _unrecognizedBarcodePen;
        /// <summary>
        /// Gets or sets a pen for drawing region of unrecognized barcode.
        /// </summary>
        public Pen UnrecognizedBarcodePen
        {
            get
            {
                return _unrecognizedBarcodePen;
            }
            set
            {
                _unrecognizedBarcodePen = value;
            }
        }

        Brush _unrecognizedBarcodeBrush = new SolidColorBrush(Color.FromArgb(48, Colors.Red.R, Colors.Red.G, Colors.Red.B));
        /// <summary> 
        /// Gets or sets a brush for filling region of unrecognized barcode.
        /// </summary>
        public Brush UnrecognizedBarcodeBrush
        {
            get
            {
                return _unrecognizedBarcodeBrush;
            }
            set
            {
                _unrecognizedBarcodeBrush = value;
            }
        }

        Brush _fontBrush = new SolidColorBrush(Colors.Blue);
        /// <summary>
        /// Gets or sets a brush for drawing the barcode value.
        /// </summary>
        public Brush FontBrush
        {
            get
            {
                return _fontBrush;
            }
            set
            {
                _fontBrush = value;
            }
        }
        
        #endregion



        #region Methods

        #region PUBLIC

        /// <summary> 
        /// Recognizes barcodes synchronously.
        /// </summary>
        public void ReadBarcodesSync()
        {
            if (_isRecognitionStarted)
                throw new InvalidOperationException("Recognition process is executing at this moment.");

            _isRecognitionStarted = true;

            if (InvokeRequired)
                Dispatcher.Invoke(new OnRecoginitionStartedDelegate(OnRecoginitionStarted), EventArgs.Empty);
            else
                OnRecoginitionStarted(EventArgs.Empty);

            try
            {
#if !REMOVE_BARCODE_SDK
                if (ImageViewer != null)
                {
                    VintasoftImage image = ImageViewer.Image;
                    if (image != null)
                    {
                        ChangePixelFormatCommand convertCommand = null;
                        switch (image.PixelFormat)
                        {
                            case Vintasoft.Imaging.PixelFormat.Gray16:
                                convertCommand = new ChangePixelFormatCommand(Vintasoft.Imaging.PixelFormat.Gray8);
                                break;
                        }
                        if (convertCommand != null)
                            image = convertCommand.Execute(image);

                        // set settings
                        if (Rectangle.Size.IsEmpty)
                            ReaderSettings.ScanRectangle = Vintasoft.Primitives.VintasoftRectI.Empty;
                        else
                            ReaderSettings.ScanRectangle = new Vintasoft.Primitives.VintasoftRectI(
                                (int)Math.Round(Rectangle.X),
                                (int)Math.Round(Rectangle.Y),
                                (int)Math.Round(Rectangle.Width),
                                (int)Math.Round(Rectangle.Height));

                        // recognize barcodes
                        BitmapSource source = VintasoftImageConverter.ToBitmapSource(image);
                        RecognitionResults = _reader.ReadBarcodes(source);

                        if (convertCommand != null)
                            image.Dispose();
                    }
                    else
                    {
                        RecognitionResults = null;
                    }
                }
                else
                {
                    RecognitionResults = null;
                } 
#endif
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
            finally
            {
                _isRecognitionStarted = false;
                _isAsyncRecognitionStarted = false;

                if (InvokeRequired)
                    Dispatcher.Invoke(new OnRecoginitionFinishedDelegate(OnRecoginitionFinished), EventArgs.Empty);
                else
                    OnRecoginitionFinished(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Start the barcode recoginition in selected rectangle.
        /// </summary>
        public void ReadBarcodesAsync()
        {
            if (IsRecognotionStarted)
                throw new InvalidOperationException("Recognition process is executing at this moment.");
            _isAsyncRecognitionStarted = true;
            Thread thread = new Thread(ReadBarcodesSync);
            thread.IsBackground = true;
            thread.Start();
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Renders the content of <see cref="WpfBarcodeReaderTool"/>.
        /// </summary>
        /// <param name="dc">An instance of <see cref="System.Windows.Media.DrawingContext" />
        /// used to render the tool.</param>
        protected override void RenderContent(DrawingContext dc)
        {
#if !REMOVE_BARCODE_SDK
            if (RecognitionResults != null)
            {
                AffineMatrix transform = ImageViewer.ViewerState.GetTransformFromImageToVisualTool();
                dc.PushTransform(new MatrixTransform(VintasoftWpfConverter.Convert(transform)));
                for (int i = 0; i < _recognitionResults.Length; i++)
                    RenderBarcodeInfo(dc, _recognitionResults[i]);
                dc.Pop();
            } 
#endif
            base.RenderContent(dc);
        }

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Renders information about specified barcode info.
        /// </summary>
        /// <param name="dc">An instance of <see cref="System.Windows.Media.DrawingContext" />
        /// used to render the barcode info.</param>
        /// <param name="info">A barcode information.</param>
        protected virtual void RenderBarcodeInfo(DrawingContext dc, IBarcodeInfo info)
        {
            Pen pen;
            Brush brush;
            if (info.BarcodeType != BarcodeType.UnknownLinear &&
                (info.Confidence > 95 || info.Confidence == ReaderSettings.ConfidenceNotAvailable))
            {
                pen = _recognizedBarcodePen;
                brush = _recognizedBarcodeBrush;
            }
            else
            {
                pen = _unrecognizedBarcodePen;
                brush = _unrecognizedBarcodeBrush;
            }

            VintasoftPointI[] vsPoints = info.Region.GetPoints();
            Point[] points = new Point[vsPoints.Length];
            for (int i = 0; i < points.Length; i++)
                points[i] = WpfConverter.Convert(vsPoints[i]);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(CreatePolygon(points, true));
            dc.DrawGeometry(brush, pen, geometry);

            if (_fontBrush != null)
            {
                FormattedText text = new FormattedText(
                    GetBarcodeInfoAsString(info),
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                    FontSize, _fontBrush, 1);

                dc.DrawText(text, new Point(info.Region.LeftTop.X, info.Region.LeftTop.Y - FontSize * 2));
            }
        }

        /// <summary> 
        /// Returns information about barcode as text string.
        /// </summary>
        /// <param name="info">A barcode info.</param>
        protected virtual string GetBarcodeInfoAsString(IBarcodeInfo info)
        {
            info.ShowNonDataFlagsInValue = true;
            int index = Array.IndexOf(_recognitionResults, info);
            return string.Format("[{0}]: {1}", index + 1, info.Value);
        } 
#endif

        /// <summary>
        /// Resets this tool.
        /// </summary>
        protected override void Reset()
        {
#if !REMOVE_BARCODE_SDK
            _recognitionResults = null; 
#endif
            base.Reset();
        }

        /// <summary>
        /// Raises the <see cref="RecognitionStarted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that
        /// contains the event data.</param>
        protected virtual void OnRecoginitionStarted(EventArgs e)
        {
            if (RecognitionStarted != null)
                RecognitionStarted(this, e);
        }

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Raises the <see cref="RecognitionProgress"/> event.
        /// </summary>
        /// <param name="e">A <see cref="BarcodeReaderProgressEventArgs"/> that
        /// contains the event data.</param>
        protected virtual void OnRecoginitionProgress(BarcodeReaderProgressEventArgs e)
        {
            if (RecognitionProgress != null)
                RecognitionProgress(this, e);
        }
#endif

        /// <summary>
        /// Raises the <see cref="RecognitionFinished"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that
        /// contains the event data.</param>
        protected virtual void OnRecoginitionFinished(EventArgs e)
        {
            if (RecognitionFinished != null)
                RecognitionFinished(this, e);
        }

        #endregion


        #region PRIVATE

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// BarcodeReader.Progress event handler.
        /// </summary>
        private void Reader_Progress(object sender, BarcodeReaderProgressEventArgs e)
        {
            if (InvokeRequired)
                Dispatcher.Invoke(new OnRecoginitionProgressDelegate(OnRecoginitionProgress), e);
            else
                OnRecoginitionProgress(e);
        } 
#endif

        /// <summary>
        /// TransformController interaction logic.
        /// </summary>
        private void TransformController_Interaction(object sender, WpfInteractionEventArgs e)
        {
            // if interact with move area then
            if (e.Area == TransformController.MoveArea)
                if (TransformController.MoveArea.ActionMouseButton == ActionButton)
                {
                    // if action begins then 
                    if ((e.Action & InteractionAreaAction.Begin) != 0)
                    {
                        _needStartBarcodeRecognition = true;
                    }
                    // if action is mowing
                    else if ((e.Action & InteractionAreaAction.Move) != 0)
                    {
                        // change cursor to "move" cursor
                        _needStartBarcodeRecognition = false;
                        TransformController.MoveArea.Cursor = Cursors.SizeAll;
                    }
                    // if action is ending
                    else if ((e.Action & InteractionAreaAction.End) != 0)
                    {
                        if (_needStartBarcodeRecognition)
                        {
                            try
                            {
                                ReadBarcodesAsync();
                            }
                            catch (InvalidOperationException)
                            {
                            }
                            catch (Exception ex)
                            {
                                DemosTools.ShowErrorMessage(ex);
                            }
                            finally
                            {
                                _needStartBarcodeRecognition = false;
                            }
                        }
                        else
                        {
                            TransformController.MoveArea.Cursor = ActionCursor;
                        }
                    }
                    else if ((e.Action & InteractionAreaAction.Cancel) != 0)
                    {
                        TransformController.MoveArea.Cursor = ActionCursor;
                    }
                }
        }

        /// <summary>
        /// Creates polygon using specified points.
        /// </summary>
        private static PathFigure CreatePolygon(Point[] points, bool isClosed)
        {
            PathFigure figure = new PathFigure();
            figure.IsClosed = isClosed;
            figure.StartPoint = points[0];
            for (int i = 1; i < points.Length; i++)
                figure.Segments.Add(new LineSegment(points[i], true));
            return figure;
        }


        #endregion

        #endregion



        #region Events

        /// <summary>
        /// Occurs when barcode recognition process is started.
        /// </summary>
        public event EventHandler RecognitionStarted;

#if !REMOVE_BARCODE_SDK
        /// <summary>
        /// Occurs when progress of barcode reading is changed. 
        /// </summary>
        /// <remarks>
        /// Used only in <see cref="Vintasoft.Wpf.Barcode.ReaderSettings.AutomaticRecognition">Automatic recognition</see>
        /// and <see cref="Vintasoft.Wpf.Barcode.ReaderSettings.ThresholdIterations">iteration process</see>.
        /// </remarks>
        public event EventHandler<BarcodeReaderProgressEventArgs> RecognitionProgress; 
#endif

        /// <summary>
        /// Occurs when barcode recognition process is finished.
        /// </summary>
        public event EventHandler RecognitionFinished;

        #endregion



        #region Delegates

        delegate void OnRecoginitionStartedDelegate(EventArgs e);

#if !REMOVE_BARCODE_SDK
        delegate void OnRecoginitionProgressDelegate(BarcodeReaderProgressEventArgs e); 
#endif

        delegate void OnRecoginitionFinishedDelegate(EventArgs e);

        #endregion

    }
}
