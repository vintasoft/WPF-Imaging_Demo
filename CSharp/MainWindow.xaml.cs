using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Win32;

using Vintasoft.Data;
using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.ImageProcessing.Effects;
using Vintasoft.Imaging.ImageProcessing.Transforms;
using Vintasoft.Imaging.ImageProcessing.Fft.Filtering.Highpass;
using Vintasoft.Imaging.ImageProcessing.Fft.Filtering.Lowpass;
using Vintasoft.Imaging.ImageProcessing.Filters;
using Vintasoft.Imaging.Media;
using Vintasoft.Imaging.Metadata;
using Vintasoft.Imaging.Print;
using Vintasoft.Imaging.Undo;
using Vintasoft.Imaging.Codecs.Decoders;

using Vintasoft.Imaging.UI;
using Vintasoft.Imaging.UIActions;

using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.Print;
using Vintasoft.Imaging.Wpf.UI.Undo;
using Vintasoft.Imaging.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;

using WpfDemosCommonCode;
using WpfDemosCommonCode.Imaging;
using WpfDemosCommonCode.Imaging.Codecs;
#if !REMOVE_PDF_PLUGIN
using Vintasoft.Imaging.Pdf;
using WpfDemosCommonCode.Pdf;
#endif
using WpfDemosCommonCode.Twain;

using WpfDemosCommonCode.Barcode;
using Vintasoft.Imaging.Drawing.Gdi;
using Vintasoft.Imaging.Drawing;

namespace WpfImagingDemo
{
    public partial class MainWindow : Window
    {

        #region Fields

        /// <summary>
        /// Template of the application's title.
        /// </summary>
        string _titlePrefix = "VintaSoft WPF Imaging Demo v" + ImagingGlobalSettings.ProductVersion + " - {0}";

        /// <summary>
        /// Selected "View - Image scale mode" menu item.
        /// </summary>
        MenuItem _imageScaleSelectedMenuItem;

        /// <summary>
        /// Image map tool.
        /// </summary>
        WpfImageMapTool _imageMapTool;


        #region Open

        /// <summary>
        /// Name of the first image file in the image collection of the image viewer.
        /// </summary>
        string _sourceFilename;

        /// <summary>
        /// Decoder name of the first image file in the image collection of the image viewer.
        /// </summary>
        string _sourceDecoderName;

        /// <summary>
        /// Determines that file is opened in read-only mode.
        /// </summary>
        bool _isFileReadOnlyMode = false;

        /// <summary>
        /// Determines that the Open File Dialog is opened.
        /// </summary>
        bool _isFileDialogOpened = false;

        /// <summary>
        /// A value indicating whether the source image file is changing.
        /// </summary> 
        bool _isSourceChanging = false;

        /// <summary>
        /// Manages asynchronous operations of an image viewer images.
        /// </summary>
        WpfImageViewerImagesManager _imagesManager;

        #endregion


        #region Load

        /// <summary>
        /// Time when loading of image is started.
        /// </summary>
        DateTime _imageLoadingStartTime;
        /// <summary>
        /// Time of image loading.
        /// </summary>
        TimeSpan _imageLoadingTime = TimeSpan.Zero;

        #endregion


        #region Save

        /// <summary>
        /// Filename of the image file to save the image collection of the image viewer.
        /// </summary>
        string _saveFilename;

        /// <summary>
        /// Name of the encoder to save the image collection of the image viewer.
        /// </summary>
        string _encoderName;

        /// <summary>
        /// Determines that saving of image must be canceled.
        /// </summary>
        bool _cancelImageSaving = false;

        #endregion


        #region Print

        /// <summary>
        /// Print manager.
        /// </summary>
        WpfImagePrintManager _printManager;

        #endregion


        #region Scan

        /// <summary>
        /// Simple TWAIN manager.
        /// </summary>
        WpfSimpleTwainManager _simpleTwainManager;

        #endregion


        #region Camera

        /// <summary>
        /// Opened webcam preview windows.
        /// </summary>
        List<WebcamPreviewWindow> _webcamWindows = new List<WebcamPreviewWindow>();

        #endregion


        /// <summary>
        /// Image processing command executor.
        /// </summary>
        WpfImageProcessingCommandExecutor _imageProcessingCommandExecutor;


        #region Image processing undo manager

        /// <summary>
        /// Window that shows image processing history.
        /// </summary>
        WpfUndoManagerHistoryWindow _historyWindow;

        /// <summary>
        /// Determines that undo information for all images must be kept.
        /// </summary>
        /// <value>
        /// <b>true</b> - undo information for focused image is kept;
        /// <b>false</b> - undo information for all images are kept.
        /// </value>
        bool _keepUndoForCurrentImageOnly = false;

        /// <summary>
        /// The undo manager.
        /// </summary>
        UndoManager _undoManager;

        /// <summary>
        /// The undo monitor of image viewer.
        /// </summary>
        WpfImageViewerUndoMonitor _imageViewerUndoMonitor = null;

        /// <summary>
        /// The data storage of undo monitors.
        /// </summary>
        IDataStorage _dataStorage = null;

        /// <summary>
        /// The maximum number of undo levels.
        /// </summary>
        int _undoLevel = 10;

        #endregion


        #region Hot keys

        public static RoutedCommand _openCommand = new RoutedCommand();
        public static RoutedCommand _addCommand = new RoutedCommand();
        public static RoutedCommand _saveAsCommand = new RoutedCommand();
        public static RoutedCommand _closeCommand = new RoutedCommand();
        public static RoutedCommand _printCommand = new RoutedCommand();
        public static RoutedCommand _exitCommand = new RoutedCommand();
        public static RoutedCommand _undoCommand = new RoutedCommand();
        public static RoutedCommand _redoCommand = new RoutedCommand();
        public static RoutedCommand _copyImageCommand = new RoutedCommand();
        public static RoutedCommand _pasteImageCommand = new RoutedCommand();
        public static RoutedCommand _rotateClockwiseCommand = new RoutedCommand();
        public static RoutedCommand _rotateCounterclockwiseCommand = new RoutedCommand();
        public static RoutedCommand _aboutCommand = new RoutedCommand();

        #endregion


        /// <summary>
        /// Window for direct access to the image pixels.
        /// </summary>
        WpfDirectPixelAccessWindow _directPixelAccessWindow;

        /// <summary>
        /// Determines that ESC key is pressed.
        /// </summary>
        bool _isEscPressed = false;

        /// <summary>
        /// Determines that window of application is closing.
        /// </summary> 
        bool _isWindowClosing = false;

        ContextMenu _imageViewerDefaultContextMenu = null;

        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();

        bool _isVisualToolChanging = false;

        /// <summary>
        /// Manages the layout settings of DOCX document image collections.
        /// </summary>
        ImageCollectionDocxLayoutSettingsManager _imageCollectionDocxLayoutSettingsManager;

        /// <summary>
        /// Manages the layout settings of XLSX document image collections.
        /// </summary>
        ImageCollectionXlsxLayoutSettingsManager _imageCollectionXlsxLayoutSettingsManager;

        /// <summary>
        /// Location of selection context menu.
        /// </summary>
        Point _selectionContextMenuStripLocation;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            // register the evaluation license for VintaSoft Imaging .NET SDK
            Vintasoft.Imaging.ImagingGlobalSettings.Register("REG_USER", "REG_EMAIL", "EXPIRATION_DATE", "REG_CODE");

            InitializeComponent();

            Jbig2AssemblyLoader.Load();
            Jpeg2000AssemblyLoader.Load();
            RawAssemblyLoader.Load();
            DicomAssemblyLoader.Load();
            PdfAnnotationsAssemblyLoader.Load();
            DocxAssemblyLoader.Load();

            ImagingTypeEditorRegistrator.Register();

            // load XPS codec
            DemosTools.LoadXpsCodec();
            // set XPS rendering requirement
            DemosTools.SetXpsRenderingRequirement(imageViewer, 0f);

            visualToolsToolBar.ImageViewer = imageViewer;
            visualToolsToolBar.VisualToolsMenuItem = visualToolsMenuItem;

            imageViewerToolBar.ImageViewer = imageViewer;
            imageViewerToolBar.AssociatedZoomSlider = zoomSlider;
            thumbnailViewer.MasterViewer = imageViewer;
            _imageViewerDefaultContextMenu = imageViewer.ContextMenu;

            _imageMapTool = new WpfImageMapTool();
            _imageMapTool.Enabled = false;

            //
            imageViewer.Images.ImageCollectionChanged += new EventHandler<ImageCollectionChangeEventArgs>(Images_CollectionChanged);

            // init "View => Image Display Mode" menu
            singlePageMenuItem.Tag = ImageViewerDisplayMode.SinglePage;
            twoColumnsMenuItem.Tag = ImageViewerDisplayMode.TwoColumns;
            singleContinuousRowMenuItem.Tag = ImageViewerDisplayMode.SingleContinuousRow;
            singleContinuousColumnMenuItem.Tag = ImageViewerDisplayMode.SingleContinuousColumn;
            twoContinuousRowsMenuItem.Tag = ImageViewerDisplayMode.TwoContinuousRows;
            twoContinuousColumnsMenuItem.Tag = ImageViewerDisplayMode.TwoContinuousColumns;

            // init "View => Image Scale Mode" menu
            normalImageMenuItem.Tag = ImageSizeMode.Normal;
            bestFitMenuItem.Tag = ImageSizeMode.BestFit;
            fitToWidthMenuItem.Tag = ImageSizeMode.FitToWidth;
            fitToHeightMenuItem.Tag = ImageSizeMode.FitToHeight;
            pixelToPixelMenuItem.Tag = ImageSizeMode.PixelToPixel;
            scaleMenuItem.Tag = ImageSizeMode.Zoom;
            scale25MenuItem.Tag = 25;
            scale50MenuItem.Tag = 50;
            scale100MenuItem.Tag = 100;
            scale200MenuItem.Tag = 200;
            scale400MenuItem.Tag = 400;
            _imageScaleSelectedMenuItem = normalImageMenuItem;
            _imageScaleSelectedMenuItem.IsChecked = true;

            // create the print manager
            _printManager = new WpfImagePrintManager();
            _printManager.Images = thumbnailViewer.Images;
            _printManager.PrintScaleMode = PrintScaleMode.BestFit;

            // create the image processing executor
            _imageProcessingCommandExecutor = new WpfImageProcessingCommandExecutor(imageViewer);
            _imageProcessingCommandExecutor.ImageProcessingCommandStarted += new EventHandler<ImageProcessingEventArgs>(_imageProcessingCommandExecutor_ImageProcessingCommandStarted);
            _imageProcessingCommandExecutor.ImageProcessingCommandProgress += new EventHandler<ImageProcessingProgressEventArgs>(_imageProcessingCommandExecutor_ImageProcessingCommandProgress);
            _imageProcessingCommandExecutor.ImageProcessingCommandFinished += new EventHandler<ImageProcessedEventArgs>(_imageProcessingCommandExecutor_ImageProcessingCommandFinished);

            // create the image undo managers
            enableUndoRedoMenuItem.IsChecked = false;
            CreateUndoManager(_keepUndoForCurrentImageOnly);
            UpdateUndoRedoMenu(_undoManager);

            // create images manager
            _imagesManager = new WpfImageViewerImagesManager(imageViewer);
            _imagesManager.IsAsync = true;
            _imagesManager.AddStarting += new EventHandler(ImagesManager_AddStarting);
            _imagesManager.AddFinished += new EventHandler(ImagesManager_AddFinished);
            _imagesManager.ImageSourceAddStarting += new EventHandler<ImageSourceEventArgs>(ImagesManager_ImageSourceAddStarting);
            _imagesManager.ImageSourceAddFinished += new EventHandler<ImageSourceEventArgs>(ImagesManager_ImageSourceAddFinished);
            _imagesManager.ImageSourceAddException += new EventHandler<ImageSourceExceptionEventArgs>(ImagesManager_ImageSourceAddException);

            thumbnailViewer.ThumbnailRenderingThreadCount = Math.Max(1, Environment.ProcessorCount / 2);

            imageViewer.CatchVisualToolExceptions = true;
            imageViewer.VisualToolException += new EventHandler<Vintasoft.Imaging.ExceptionEventArgs>(imageViewer_VisualToolException);

            imageViewer.InputGestureInsert = null;
            imageViewer.InputGestureDelete = null;
            imageViewer.InputGestureCut = null;
            imageViewer.InputGestureCopy = null;

            //
            CodecsFileFilters.SetFilters(openFileDialog1);
            DemosTools.SetTestFilesFolder(openFileDialog1);

            DocumentPasswordWindow.EnableAuthentication(imageViewer);

            // set CustomFontProgramsController for all opened PDF documents
            CustomFontProgramsController.SetDefaultFontProgramsController();

            SelectionVisualToolActionFactory.CreateActions(visualToolsToolBar);
            MeasurementVisualToolActionFactory.CreateActions(visualToolsToolBar);
            ZoomVisualToolActionFactory.CreateActions(visualToolsToolBar);
            ImageProcessingVisualToolActionFactory.CreateActions(visualToolsToolBar);
            CustomVisualToolActionFactory.CreateActions(visualToolsToolBar);
            BarcodeReaderToolActionFactory.CreateActions(visualToolsToolBar);
            BarcodeWriterToolActionFactory.CreateActions(visualToolsToolBar);

            // set default rendering settings
#if REMOVE_PDF_PLUGIN && REMOVE_OFFICE_PLUGIN
            imageViewer.ImageRenderingSettings = RenderingSettings.Empty;
#elif REMOVE_OFFICE_PLUGIN
            imageViewer.ImageRenderingSettings = new PdfRenderingSettings();
#elif REMOVE_PDF_PLUGIN
            imageViewer.ImageRenderingSettings = new CompositeRenderingSettings(
                new DocxRenderingSettings(),
                new XlsxRenderingSettings());
#else
            imageViewer.ImageRenderingSettings = new CompositeRenderingSettings(
                new PdfRenderingSettings(),
                new DocxRenderingSettings(),
                new XlsxRenderingSettings());
#endif

#if !REMOVE_OFFICE_PLUGIN
            // specify that image collection of image viewer must handle layout settings requests
            _imageCollectionDocxLayoutSettingsManager = new ImageCollectionDocxLayoutSettingsManager(imageViewer.Images);
            _imageCollectionXlsxLayoutSettingsManager = new ImageCollectionXlsxLayoutSettingsManager(imageViewer.Images);
#else
            documentLayoutSettingsMenuItem.Visibility = Visibility.Collapsed;
#endif

            // initialize color management
            ColorManagementHelper.EnableColorManagement(imageViewer);

            // update the UI
            UpdateUI();
        }

        #endregion



        #region Properties

        bool _isImageLoaded = false;
        internal bool IsImageLoaded
        {
            get
            {
                return _isImageLoaded;
            }
            set
            {
                _isImageLoaded = value;
                UpdateUI();
            }
        }

        bool _isImageProcessing = false;
        internal bool IsImageProcessing
        {
            get
            {
                return _isImageProcessing;
            }
            set
            {
                _isImageProcessing = value;
                UpdateUI();
            }
        }

        bool _isFileOpening = false;
        /// <summary>
        /// Gets or sets a value indicating whether file is opening.
        /// </summary>
        internal bool IsFileOpening
        {
            get
            {
                return _isFileOpening;
            }
            set
            {
                _isFileOpening = value;

                if (_isFileOpening)
                    Cursor = Cursors.AppStarting;
                else
                    Cursor = Cursors.Arrow;

                UpdateUI();
            }
        }

        bool _isImageSaving = false;
        internal bool IsImageSaving
        {
            get
            {
                return _isImageSaving;
            }
            set
            {
                _isImageSaving = value;
                InvokeUpdateUI();
            }
        }

        internal Rect SelectionRectangle
        {
            get
            {
                Rect selectionRect = Rect.Empty;
                WpfRectangularSelectionToolWithCopyPaste selection = imageViewer.VisualTool as WpfRectangularSelectionToolWithCopyPaste;
                if (selection != null)
                    selectionRect = selection.Rectangle;
                return selectionRect;
            }
        }

        #endregion



        #region Methods

        #region Main window

        /// <summary>
        /// Handles the Loaded event of Window object.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // process command line of the application
            string[] appArgs = Environment.GetCommandLineArgs();
            if (appArgs.Length > 1)
            {
                if (appArgs.Length == 2)
                {
                    try
                    {
                        // add image file to the image viewer
                        OpenFile(appArgs[1]);
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
                else
                {
                    // get filenames from application arguments
                    string[] filenames = new string[appArgs.Length - 1];
                    Array.Copy(appArgs, 1, filenames, 0, filenames.Length);

                    try
                    {
                        // add image file(s) to the image collection of the image viewer
                        AddFiles(filenames);
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
            }

            thumbnailViewer.Focus();
        }

        /// <summary>
        /// Main window, key up.
        /// </summary>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            _isEscPressed = e.Key == Key.Escape;
        }

        /// <summary>
        /// Main window is closing.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _isWindowClosing = true;


            foreach (WebcamPreviewWindow window in _webcamWindows)
                if (window.IsVisible)
                    window.Close();

            // do not close the application window while image loading is not canceled/finished
            if (_isFileDialogOpened)
                e.Cancel = true;
            else
                _printManager.Dispose();
        }

        /// <summary>
        /// Handles the Closed event of Window object.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            CloseCurrentFile();

            ClearHistory();

            if (_dataStorage != null)
                _dataStorage.Dispose();

            _imagesManager.Dispose();
        }

        #endregion


        #region UI state

        /// <summary>
        /// Updates the user interface of this window.
        /// </summary>
        private void UpdateUI()
        {
            // if application is closing
            if (_isWindowClosing)
                // exit
                return;


            // get the current status of application

            int imageCount = imageViewer.Images.Count;
            VintasoftImage currentImage = imageViewer.Image;
            bool isImageLoaded = currentImage != null;
            bool isImageProcessing = this.IsImageProcessing;
            bool isImageSaving = this.IsImageSaving;
            bool isFileOpening = this.IsFileOpening;

            bool clipboardContainsImage = Clipboard.ContainsImage();

            bool canSaveToTheSameSource = _sourceFilename != null && !_isFileReadOnlyMode;
            if (canSaveToTheSameSource)
            {
                // if format of source does not support multiple images (BMP, PNG, ...)
                if (_sourceDecoderName != null &&
                    AvailableEncoders.CreateMultipageEncoderByName(_sourceDecoderName) == null)
                    // saving of image to the source (save changes) is possible only
                    // if one image is loaded in the viewer
                    canSaveToTheSameSource = imageCount == 1;
            }


            // window title
            if (!isFileOpening)
            {
                string str;
                if (_sourceFilename != null)
                    str = Path.GetFileName(_sourceFilename);
                else
                    str = "(Untitled)";

                if (_isFileReadOnlyMode)
                    str += " [Read Only]";

                Title = string.Format(_titlePrefix, str);
            }
            if (!isImageLoaded)
                CloseHistoryWindow();


            // "File" menu
            newToolStripMenuItem.IsEnabled = !isFileOpening;
            openMenuItem.IsEnabled = !isFileOpening;
            documentLayoutSettingsMenuItem.IsEnabled = !isFileOpening;
            addFromClipboardMenuItem.IsEnabled = !isFileOpening && clipboardContainsImage;
            acquireFromScannerMenuItem.IsEnabled = !isFileOpening;
            captureFromCameraMenuItem.IsEnabled = !isFileOpening;

            saveChangesMenuItem.IsEnabled = !isFileOpening && isImageLoaded && canSaveToTheSameSource && !isImageProcessing && !isImageSaving;
            saveAsMenuItem.IsEnabled = !isFileOpening && isImageLoaded && !isImageProcessing && !isImageSaving;
            saveToMenuItem.IsEnabled = !isFileOpening && isImageLoaded && !isImageProcessing && !isImageSaving;
            saveCurrentImageMenuItem.IsEnabled = !isFileOpening && isImageLoaded && !isImageProcessing && !isImageSaving;
            printMenuItem.IsEnabled = !isFileOpening && isImageLoaded && !isImageProcessing && !isImageSaving;
            closeMenuItem.IsEnabled = imageCount > 0 || isFileOpening;

            // "Edit" menu
            //
            UpdateEditMenu();
            documentMetadataMenuItem.IsEnabled = imageCount > 0;
            editImagePaletteMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && currentImage.BitsPerPixel <= 8 && currentImage.PixelFormat != Vintasoft.Imaging.PixelFormat.Undefined;
            editImageMetadataMenuItem.IsEnabled = isImageLoaded;
            enableUndoRedoMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            keepUndoForCurrentImageOnlyMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            undoMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            redoMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            undoRedoSettingsMenuItem.IsEnabled = isImageLoaded && enableUndoRedoMenuItem.IsChecked;
            historyDialogMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving && enableUndoRedoMenuItem.IsChecked;

            // "View" menu
            //
            // update "View => Image Display Mode" menu
            singlePageMenuItem.IsChecked = false;
            twoColumnsMenuItem.IsChecked = false;
            singleContinuousRowMenuItem.IsChecked = false;
            singleContinuousColumnMenuItem.IsChecked = false;
            twoContinuousRowsMenuItem.IsChecked = false;
            twoContinuousColumnsMenuItem.IsChecked = false;
            switch (imageViewer.DisplayMode)
            {
                case ImageViewerDisplayMode.SinglePage:
                    singlePageMenuItem.IsChecked = true;
                    break;

                case ImageViewerDisplayMode.TwoColumns:
                    twoColumnsMenuItem.IsChecked = true;
                    break;

                case ImageViewerDisplayMode.SingleContinuousRow:
                    singleContinuousRowMenuItem.IsChecked = true;
                    break;

                case ImageViewerDisplayMode.SingleContinuousColumn:
                    singleContinuousColumnMenuItem.IsChecked = true;
                    break;

                case ImageViewerDisplayMode.TwoContinuousRows:
                    twoContinuousRowsMenuItem.IsChecked = true;
                    break;

                case ImageViewerDisplayMode.TwoContinuousColumns:
                    twoContinuousColumnsMenuItem.IsChecked = true;
                    break;
            }

            // "Image processing" menu
            //
            imageProcessingMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;

            // "Tools" menu
            //
            animationMenuItem.IsEnabled = imageViewer.Images.Count > 1 && !isImageProcessing && !isImageSaving;

            // thumbnail context menu
            //
            thumbnailViewer_addImageFromClipboardMenuItem.IsEnabled = addFromClipboardMenuItem.IsEnabled;

            // image viewer context menu
            imageViewer_setImageFromClipboardMenuItem.IsEnabled = pasteImageMenuItem.IsEnabled;


            // viewer tool strip
            imageViewerToolBar.OpenButtonEnabled = openMenuItem.IsEnabled;
            imageViewerToolBar.SaveButtonEnabled = saveAsMenuItem.IsEnabled;
            imageViewerToolBar.ScanButtonEnabled = acquireFromScannerMenuItem.IsEnabled;
            imageViewerToolBar.CaptureFromCameraButtonEnabled = captureFromCameraMenuItem.IsEnabled;
            imageViewerToolBar.PrintButtonEnabled = printMenuItem.IsEnabled;
            imageViewerToolBar.PageCount = imageViewer.Images.Count;

            // image processing history
            if (_historyWindow != null)
                _historyWindow.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;

            UpdateUndoRedoMenu(_undoManager);

            // update information about the focused image
            UpdateImageInfo();
        }

        /// <summary>
        /// Update UI safely.
        /// </summary>
        private void InvokeUpdateUI()
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateUI();
            else
                Dispatcher.Invoke(new UpdateUIDelegate(UpdateUI));
        }

        /// <summary>
        /// Updates the "Edit" menu.
        /// </summary>
        private void UpdateEditMenu()
        {
            VintasoftImage currentImage = imageViewer.Image;
            bool isImageLoaded = currentImage != null;
            bool isEntireImageLoaded = imageViewer.IsEntireImageLoaded;
            bool isImageProcessing = this.IsImageProcessing;
            bool isImageSaving = this.IsImageSaving;
            bool clipboardContainsImage = Clipboard.ContainsImage();
            bool isFileOpening = IsFileOpening;

            //
            copyImageMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            //
            pasteImageMenuItem.IsEnabled = isImageLoaded && clipboardContainsImage && !isImageProcessing && !isImageSaving;
            setImageFromFileMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            //
            insertImageFromClipboardMenuItem.IsEnabled = !isFileOpening && isImageLoaded && clipboardContainsImage && !isImageProcessing && !isImageSaving;
            insertImageFromFileMenuItem.IsEnabled = !isFileOpening && isImageLoaded && !isImageProcessing && !isImageSaving;
            //
            deleteImageMenuItem.IsEnabled = isImageLoaded && !isImageProcessing && !isImageSaving;
            //
            editImagePixelsMenuItem.IsEnabled = isEntireImageLoaded && !isImageProcessing && !isImageSaving;
        }

        /// <summary>
        /// Updates the "Edit" menu when it is opening.
        /// </summary>
        private void editMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            UpdateEditMenu();
            UpdateEditUIActionMenuItems();
        }

        #endregion


        #region 'File' menu

        /// <summary>
        /// Create new image and add image to the image collection of the image viewer.
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateNewImageWindow dlg = new CreateNewImageWindow();
            if (dlg.ShowDialog().Value)
            {
                VintasoftImage image = dlg.CreateImage();
                imageViewer.Images.Add(image);
            }
        }

        /// <summary>
        /// Clears image collection of the image viewer and adds image(s) to the image collection
        /// of the image viewer.
        /// </summary>
        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isFileDialogOpened)
                return;

            _isFileDialogOpened = true;

            // select image file
            if (openFileDialog1.ShowDialog().Value)
            {
                try
                {
                    // add image file to image viewer as a source
                    OpenFile(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }

            _isFileDialogOpened = false;
        }

        /// <summary>
        /// Clears image collection of the image viewer and adds image(s) to the image collection
        /// of the image viewer.
        /// </summary>
        private void imageViewerToolBar_OpenFile(object sender, EventArgs e)
        {
            openMenuItem_Click(imageViewerToolBar, null);
        }

        /// <summary>
        /// Adds image(s) to the image collection of the image viewer.
        /// </summary>
        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isFileDialogOpened)
                return;

            _isFileDialogOpened = true;
            openFileDialog1.Multiselect = true;

            // select image file(s)
            if (openFileDialog1.ShowDialog().Value)
            {
                try
                {
                    // add image file(s) to image collection of the image viewer
                    AddFiles(openFileDialog1.FileNames);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }

            openFileDialog1.Multiselect = false;
            _isFileDialogOpened = false;
        }

        /// <summary>
        /// Handles the Click event of DocxLayoutSettingsMenuItem object.
        /// </summary>
        private void docxLayoutSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageCollectionDocxLayoutSettingsManager.EditLayoutSettingsUseDialog(this);
        }

        /// <summary>
        /// Handles the Click event of XlsxLayoutSettingsMenuItem object.
        /// </summary>
        private void xlsxLayoutSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageCollectionXlsxLayoutSettingsManager.EditLayoutSettingsUseDialog(this);
        }

        /// <summary>
        /// Adds image from clipboard to the image collection of the image viewer.
        /// </summary>
        private void addFromClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                imageViewer.Images.Add(VintasoftImageConverter.FromBitmapSource(Clipboard.GetImage()));

                // update the UI
                UpdateUI();
            }
        }

        /// <summary>
        /// Acquires image(s) from scanner.
        /// </summary>
        private void acquireFromScannerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool scanMenuEnabled = acquireFromScannerMenuItem.IsEnabled;
            acquireFromScannerMenuItem.IsEnabled = false;
            bool viewerToolstripCanScan = imageViewerToolBar.CanScan;
            imageViewerToolBar.ScanButtonEnabled = false;
            try
            {
                if (_simpleTwainManager == null)
                    _simpleTwainManager = new WpfSimpleTwainManager(this, imageViewer.Images);

                _simpleTwainManager.SelectDeviceAndAcquireImage();
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
            finally
            {
                acquireFromScannerMenuItem.IsEnabled = scanMenuEnabled;
                imageViewerToolBar.ScanButtonEnabled = viewerToolstripCanScan;
            }
        }

        /// <summary>
        /// Captures image(s) from camera (webcam).
        /// </summary>
        private void captureFromCameraToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ImageCaptureDevice device = WebcamSelectionWindow.SelectWebcam();
                if (device != null)
                {
                    WebcamPreviewWindow webcamWindow = new WebcamPreviewWindow(device);
                    webcamWindow.SnapshotViewer = imageViewer;
                    _webcamWindows.Add(webcamWindow);
                    webcamWindow.Show();

                }
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Saves changes in image collection to the source file.
        /// </summary>
        private void saveChangesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EncoderBase encoder = null;
            try
            {
                if (PluginsEncoderFactory.Default.GetEncoderByName(_sourceDecoderName, out encoder))
                    SaveImageCollection(imageViewer.Images, _sourceFilename, encoder, true);
                else
                    DemosTools.ShowErrorMessage("Image is not saved.");
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Saves image collection to new source and switches to the new source.
        /// </summary>
        private void saveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isFileDialogOpened)
                return;
            _isFileDialogOpened = true;

            bool saveSingleImage = imageViewer.Images.Count == 1;

            //
            CodecsFileFilters.SetFilters(saveFileDialog1, !saveSingleImage);
            if (saveFileDialog1.ShowDialog().Value)
            {
                string filename = saveFileDialog1.FileName;

                try
                {
                    PluginsEncoderFactory encoderFactory = new PluginsEncoderFactory();
                    encoderFactory.CanAddImagesToExistingFile = false;

                    EncoderBase encoder = null;
                    if (encoderFactory.GetEncoder(filename, out encoder))
                        SaveImageCollection(imageViewer.Images, filename, encoder, true);
                    else
                        DemosTools.ShowErrorMessage("Images are not saved.");
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }

            _isFileDialogOpened = false;
        }

        /// <summary>
        /// Saves image collection to new source and switches to the new source.
        /// </summary>
        private void imageViewerToolBar_SaveFile(object sender, EventArgs e)
        {
            saveAsMenuItem_Click(imageViewerToolBar, null);
        }

        /// <summary>
        /// Saves image collection to new source and do NOT switch to the new source.
        /// </summary>
        private void saveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isFileDialogOpened)
                return;
            _isFileDialogOpened = true;

            bool saveSingleImage = imageViewer.Images.Count == 1;

            //
            CodecsFileFilters.SetFilters(saveFileDialog1, !saveSingleImage);
            if (saveFileDialog1.ShowDialog().Value)
            {
                string filename = Path.GetFullPath(saveFileDialog1.FileName);
                bool isFileExist = File.Exists(filename);

                try
                {
                    PluginsEncoderFactory encoderFactory = new PluginsEncoderFactory();
                    encoderFactory.CanAddImagesToExistingFile = isFileExist;

                    EncoderBase encoder = null;
                    if (encoderFactory.GetEncoder(filename, out encoder))
                        SaveImageCollection(imageViewer.Images, filename, encoder, false);
                    else
                        DemosTools.ShowErrorMessage("Image is not saved.");
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }

            _isFileDialogOpened = false;
        }

        /// <summary>
        /// Saves current image to a file.
        /// </summary>
        private void saveCurrentImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isFileDialogOpened)
                return;
            _isFileDialogOpened = true;

            try
            {
                CodecsFileFilters.SetFilters(saveFileDialog1, false);
                if (saveFileDialog1.ShowDialog().Value)
                {
                    string filename = saveFileDialog1.FileName;
                    bool isFileExist = File.Exists(filename);

                    PluginsEncoderFactory encoderFactory = new PluginsEncoderFactory();
                    encoderFactory.CanAddImagesToExistingFile = isFileExist;

                    EncoderBase encoder = null;
                    if (encoderFactory.GetEncoder(filename, out encoder))
                    {
                        VintasoftImage image = imageViewer.Images[imageViewer.FocusedIndex];
                        // save the image
                        SaveSingleImage(image, filename, encoder, false);
                    }
                    else
                        DemosTools.ShowErrorMessage("Image is not saved.");
                }
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }

            _isFileDialogOpened = false;
        }

        /// <summary>
        /// Closes the current image file.
        /// </summary>
        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CloseCurrentFile();

            UpdateUI();
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion


        #region 'Edit' menu

        #region Copy, paste and delete image

        /// <summary>
        /// Copies an image to the clipboard.
        /// </summary>
        private void copyImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CopyImageToClipboard();
        }


        /// <summary>
        /// Pastes an image from clipboard.
        /// </summary>
        private void pasteImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PasteImageFromClipboard();
        }

        /// <summary>
        /// Sets an image from a file.
        /// </summary>
        private void setImageFromFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog1.ShowDialog().Value)
            {
                try
                {
                    VintasoftImage image = new VintasoftImage(openFileDialog1.FileName);
                    imageViewer.Image.SetImage(image);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }


        /// <summary>
        /// Inserts an image from clipboard to the image viewer.
        /// </summary>
        private void insertImageFromClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                try
                {
                    VintasoftImage image = VintasoftImageConverter.FromBitmapSource(Clipboard.GetImage());
                    imageViewer.Images.Insert(imageViewer.FocusedIndex, image);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Inserts an image from file to the image viewer.
        /// </summary>
        private void insertImageFromFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog1.ShowDialog().Value)
            {
                try
                {
                    imageViewer.Images.Insert(imageViewer.FocusedIndex, openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }


        /// <summary>
        /// Deletes an image from image viewer.
        /// </summary>
        private void deleteImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VintasoftImage image = imageViewer.Images[imageViewer.FocusedIndex];
            imageViewer.Images.RemoveAt(imageViewer.FocusedIndex);
            image.Dispose();
        }

        #endregion


        #region Copy, paste and delete measurement object

        /// <summary>
        /// Copies the selected measurement into "internal" buffer.
        /// </summary>
        private void copyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get the copy UI action for current visual tool
            CopyItemUIAction copyUIAction = DemosTools.GetUIAction<CopyItemUIAction>(imageViewer.VisualTool);
            // if UI action exists
            if (copyUIAction != null)
                // execute the UI action
                copyUIAction.Execute();

            // update the UI
            UpdateUI();
        }

        /// <summary>
        /// Cuts the selected measurement into "internal" buffer.
        /// </summary>
        private void cutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get the cut UI action for current visual tool
            CutItemUIAction cutUIAction = DemosTools.GetUIAction<CutItemUIAction>(imageViewer.VisualTool);
            // if UI action exists
            if (cutUIAction != null)
                // execute the UI action
                cutUIAction.Execute();

            // update the UI
            UpdateUI();
        }

        /// <summary>
        /// Pastes measurement from "internal" buffer and makes it active.
        /// </summary>
        private void pasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get the paste UI action for current visual tool
            PasteItemWithOffsetUIAction pasteUIAction = DemosTools.GetUIAction<PasteItemWithOffsetUIAction>(imageViewer.VisualTool);
            // if UI action exists AND UI action is enabled
            if (pasteUIAction != null && pasteUIAction.IsEnabled)
            {
                pasteUIAction.OffsetX = 20;
                pasteUIAction.OffsetY = 20;
                // execute the UI action
                pasteUIAction.Execute();
            }

            // update the UI
            UpdateUI();
        }

        /// <summary>
        /// Removes the selected measurement.
        /// </summary>
        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get the delete UI action for current visual tool
            UIAction deleteUIAction = DemosTools.GetUIAction<DeleteItemUIAction>(imageViewer.VisualTool);
            // if UI action exists AND UI action is enabled
            if (deleteUIAction != null && deleteUIAction.IsEnabled)
                // execute the UI action
                deleteUIAction.Execute();

            // update the UI
            UpdateUI();
        }

        /// <summary>
        /// Removes all measurements.
        /// </summary>
        private void deleteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get the delete all UI action for current visual tool
            UIAction deleteUIAction = DemosTools.GetUIAction<DeleteAllItemsUIAction>(imageViewer.VisualTool);
            // if UI action exists AND UI action is enabled
            if (deleteUIAction != null && deleteUIAction.IsEnabled)
                // execute the UI action
                deleteUIAction.Execute();

            // update the UI
            UpdateUI();
        }

        #endregion


        /// <summary>
        /// Shows document metadata window.
        /// </summary>
        private void documentMetadataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DocumentMetadata metadata = imageViewer.Image.SourceInfo.Decoder.GetDocumentMetadata();

            if (metadata != null)
            {
                PropertyGridWindow propertyWindow = new PropertyGridWindow(metadata, "Document Metadata");
                propertyWindow.Owner = this;
                propertyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                propertyWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("File does not contain metadata.", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Shows a window for editing image metadata.
        /// </summary>
        private void editImageMetadataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VintasoftImage image = imageViewer.Image;

            Window window;
#if !REMOVE_DICOM_PLUGIN
            if (image.Metadata.MetadataTree is DicomFrameMetadata)
            {
                DicomMetadataEditorWindow editorWindow = new DicomMetadataEditorWindow();
                editorWindow.Image = image;
                window = editorWindow;
            }
            else
#endif
            {
                MetadataEditorWindow editorWindow = new MetadataEditorWindow();
                editorWindow.Image = image;
                window = editorWindow;
            }

            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        /// <summary>
        /// Shows a window for editing image palette.
        /// </summary>
        private void editImagePaletteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PaletteWindow paletteDialog = new PaletteWindow();
            paletteDialog.WindowStartupLocation = WindowStartupLocation.Manual;
            paletteDialog.Left = this.Left + 16;
            paletteDialog.Top = this.Top + (this.ActualHeight - paletteDialog.Height) / 2;
            Palette bakupPalette = imageViewer.Image.Palette.Clone();
            paletteDialog.PaletteViewer.Palette = imageViewer.Image.Palette;
            paletteDialog.Owner = this;
            paletteDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (paletteDialog.ShowDialog() != true &&
                paletteDialog.PaletteViewer.IsPaletteChanged)
                imageViewer.Image.Palette.SetColors(bakupPalette.GetAsArray());
        }

        /// <summary>
        /// Shows a window for editing image pixels.
        /// </summary>
        private void editImagePixelsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (editImagePixelsMenuItem.IsChecked == true)
            {
                OpenDirectPixelAccessWindow();
            }
            else
            {
                CloseDirectPixelAccessWindow();
            }
        }


        #region #region Undo/redo changes in images

        /// <summary>
        /// Enables or disables the image processing history.
        /// </summary>
        private void enableUndoRedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool isEnabled = _undoManager.IsEnabled ^ true;

            enableUndoRedoMenuItem.IsChecked = isEnabled;

            if (!isEnabled)
                // clear the image processing history
                _undoManager.Clear();

            _undoManager.IsEnabled = isEnabled;

            // close the image processing history form
            CloseHistoryWindow();

            // initialize the "Undo/Redo" menu
            UpdateUndoRedoMenu(_undoManager);
            // update the UI
            UpdateUI();
        }

        /// <summary>
        /// Enables/disables the image processing history for current image only.
        /// </summary>
        private void keepUndoForCurrentImageOnlyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            keepUndoForCurrentImageOnlyMenuItem.IsChecked ^= true;

            _keepUndoForCurrentImageOnly = keepUndoForCurrentImageOnlyMenuItem.IsChecked;

            CreateUndoManager(_keepUndoForCurrentImageOnly);
        }

        /// <summary>
        /// Undoes changes in image.
        /// </summary>
        private void undoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _undoManager.Undo(1);

            UpdateUndoRedoMenu(_undoManager);
        }

        /// <summary>
        /// Redoes changes in image.
        /// </summary>
        private void redoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _undoManager.Redo(1);

            UpdateUndoRedoMenu(_undoManager);
        }

        /// <summary>
        /// Enables/disables showing history for the displayed images.
        /// </summary>
        private void showHistoryForDisplayedImagesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showHistoryForDisplayedImagesMenuItem.IsChecked ^= true;

            _imageViewerUndoMonitor.ShowHistoryForDisplayedImages =
                showHistoryForDisplayedImagesMenuItem.IsChecked;
        }

        /// <summary>
        /// Edits the undo manager settings.
        /// </summary>
        private void undoRedoSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfUndoManagerSettingsWindow dlg = new WpfUndoManagerSettingsWindow(_undoManager, _dataStorage);
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = this;

            if (dlg.ShowDialog() == true)
            {
                _undoLevel = _undoManager.UndoLevel;

                if (dlg.DataStorage != _dataStorage)
                {
                    IDataStorage prevDataStorage = _dataStorage;

                    _dataStorage = dlg.DataStorage;

                    _undoManager.Clear();
                    _undoManager.DataStorage = _dataStorage;

                    if (prevDataStorage != null)
                        prevDataStorage.Dispose();

                    if (_imageViewerUndoMonitor != null)
                        _imageViewerUndoMonitor.DataStorage = _dataStorage;
                }

                UpdateUndoRedoMenu(_undoManager);
            }
        }

        /// <summary>
        /// Shows/hides the image processing history window.
        /// </summary>
        private void historyDialogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            historyDialogMenuItem.IsChecked ^= true;

            if (historyDialogMenuItem.IsChecked == true)
                // show the image processing history window
                ShowHistoryWindow();
            else
                // close the image processing history window
                CloseHistoryWindow();
        }

        #endregion

        #endregion


        #region 'View' menu

        /// <summary>
        /// Shows the thumbnail viewer settings. 
        /// </summary>
        private void thumbnailViewerSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ThumbnailViewerSettingsWindow viewerSettingsDialog = new ThumbnailViewerSettingsWindow(thumbnailViewer, (Style)Resources["ThumbnailItemStyle"]);
            viewerSettingsDialog.Owner = this;
            viewerSettingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            viewerSettingsDialog.ShowDialog();
        }


        /// <summary>
        /// Changes image display mode of image viewer.
        /// </summary>
        private void ImageDisplayMode_Click(object sender, RoutedEventArgs e)
        {
            MenuItem imageDisplayModeMenuItem = (MenuItem)sender;
            imageViewer.DisplayMode = (ImageViewerDisplayMode)imageDisplayModeMenuItem.Tag;
            UpdateUI();
        }

        /// <summary>
        /// Changes image scale mode of image viewer.
        /// </summary>
        private void ImageScale_Click(object sender, RoutedEventArgs e)
        {
            _imageScaleSelectedMenuItem.IsChecked = false;
            _imageScaleSelectedMenuItem = (MenuItem)sender;

            // if menu item sets ImageSizeMode
            if (_imageScaleSelectedMenuItem.Tag is ImageSizeMode)
            {
                // set size mode
                imageViewer.SizeMode = (ImageSizeMode)_imageScaleSelectedMenuItem.Tag;
                _imageScaleSelectedMenuItem.IsChecked = true;
            }
            // if menu item sets zoom
            else
            {
                // get zoom value
                int zoomValue = (int)_imageScaleSelectedMenuItem.Tag;
                // set ImageSizeMode as Zoom
                imageViewer.SizeMode = ImageSizeMode.Zoom;
                // set zoom value
                imageViewer.Zoom = zoomValue;
            }
        }

        /// <summary>
        /// Enables/disables centering of image in image viewer.
        /// </summary>
        private void centerImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (centerImageMenuItem.IsChecked)
            {
                imageViewer.FocusPointAnchor = AnchorType.None;
                imageViewer.IsFocusPointFixed = true;
                imageViewer.ScrollToCenter();
            }
            else
            {
                imageViewer.FocusPointAnchor = AnchorType.Left | AnchorType.Top;
                imageViewer.IsFocusPointFixed = true;
            }
        }

        /// <summary>
        /// Rotates images in both image viewer and thumbnail viewer by 90 degrees clockwise.
        /// </summary>
        private void rotateClockwiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotateViewClockwise();
        }

        /// <summary>
        /// Rotates images in both image viewer and thumbnail viewer by 90 degrees counterclockwise.
        /// </summary>
        private void rotateCounterclockwiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotateViewCounterClockwise();
        }

        /// <summary>
        /// Rotates images in both image viewer and thumbnail viewer by 90 degrees clockwise.
        /// </summary>
        private void RotateViewClockwise()
        {
            if (imageViewer.ImageRotationAngle != 270)
            {
                imageViewer.ImageRotationAngle += 90;
                thumbnailViewer.ImageRotationAngle += 90;
            }
            else
            {
                imageViewer.ImageRotationAngle = 0;
                thumbnailViewer.ImageRotationAngle = 0;
            }
        }

        /// <summary>
        /// Rotates images in both image viewer and thumbnail viewer by 90 degrees counterclockwise.
        /// </summary>
        private void RotateViewCounterClockwise()
        {
            if (imageViewer.ImageRotationAngle != 0)
            {
                imageViewer.ImageRotationAngle -= 90;
                thumbnailViewer.ImageRotationAngle -= 90;
            }
            else
            {
                imageViewer.ImageRotationAngle = 270;
                thumbnailViewer.ImageRotationAngle = 270;
            }
        }

        /// <summary>
        /// Shows the image viewer settings.
        /// </summary>
        private void imageViewerSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageViewerSettingsWindow viewerSettingsDialog = new ImageViewerSettingsWindow(imageViewer);
            viewerSettingsDialog.Owner = this;
            viewerSettingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            viewerSettingsDialog.ShowDialog();
            UpdateUI();
        }

        /// <summary>
        /// Shows the image viewer rendering settings.
        /// </summary>
        private void viewerRenderingSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CompositeRenderingSettingsWindow viewerRenderingSettingsDialog = new CompositeRenderingSettingsWindow(imageViewer.ImageRenderingSettings);
            viewerRenderingSettingsDialog.Owner = this;
            viewerRenderingSettingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            viewerRenderingSettingsDialog.ShowDialog();
            UpdateUI();
        }

        /// <summary>
        /// Shows the image map settings.
        /// </summary>
        private void imageMapSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageViewerMapSettingsWindow mapSettingsDialog = new ImageViewerMapSettingsWindow(_imageMapTool);
            mapSettingsDialog.Owner = this;
            mapSettingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapSettingsDialog.ShowDialog();

            _isVisualToolChanging = true;

            if (_imageMapTool.Enabled)
            {
                if (imageViewer.VisualTool == null)
                    imageViewer.VisualTool = _imageMapTool;
                else
                {
                    if (imageViewer.VisualTool is WpfCompositeVisualTool)
                    {
                        WpfCompositeVisualTool compositeVisualTool = (WpfCompositeVisualTool)imageViewer.VisualTool;
                        foreach (WpfVisualTool visualTool in compositeVisualTool)
                        {
                            if (visualTool == _imageMapTool)
                            {
                                _isVisualToolChanging = false;
                                return;
                            }
                        }

                        imageViewer.VisualTool = new WpfCompositeVisualTool(_imageMapTool, compositeVisualTool);
                    }
                    else if (imageViewer.VisualTool != _imageMapTool)
                    {
                        imageViewer.VisualTool = new WpfCompositeVisualTool(_imageMapTool, imageViewer.VisualTool);
                    }
                }
            }
            else
            {
                if (imageViewer.VisualTool != null)
                {
                    if (imageViewer.VisualTool is WpfCompositeVisualTool)
                    {
                        WpfCompositeVisualTool compositeVisualTool = (WpfCompositeVisualTool)imageViewer.VisualTool;
                        List<WpfVisualTool> visualTools = new List<WpfVisualTool>();
                        foreach (WpfVisualTool visualTool in compositeVisualTool)
                        {
                            if (visualTool == _imageMapTool)
                                continue;

                            visualTools.Add(visualTool);
                        }

                        if (visualTools.Count == 0)
                            imageViewer.VisualTool = null;
                        else if (visualTools.Count == 1)
                            imageViewer.VisualTool = visualTools[0];
                        else
                            imageViewer.VisualTool = new WpfCompositeVisualTool(visualTools.ToArray());
                    }
                    else if (imageViewer.VisualTool == _imageMapTool)
                        imageViewer.VisualTool = null;
                }
            }

            _isVisualToolChanging = false;
        }

        /// <summary>
        /// Shows the magnifier settings.
        /// </summary>
        private void magnifierSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MagnifierToolAction magnifierToolAction = visualToolsToolBar.FindAction<MagnifierToolAction>();

            if (magnifierToolAction != null)
                magnifierToolAction.ShowVisualToolSettings();
        }


        /// <summary>
        /// Shows the decoding settings of current image.
        /// </summary>
        private void currentImageDecodingSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DecodingSettings settings = imageViewer.Image.DecodingSettings;
            if (settings == null)
            {
                DemosTools.ShowInfoMessage("Current image does not have decoding settings.");
            }
            else
            {
                PropertyGridWindow dlg = new PropertyGridWindow(settings, settings.GetType().Name, false);
                dlg.ShowDialog();
                if (dlg.PropertyValueChanged)
                    imageViewer.Image.Reload(true);
            }
        }


        /// <summary>
        /// Color management settings.
        /// </summary>
        private void colorManamgementMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColorManagementSettingsWindow.EditColorManagement(imageViewer);
        }


        #endregion


        #region 'Image processing' menu

        #region Base

        #region Change pixel format

        /// <summary>
        /// Changes pixel format of image to BlackWhite, threshold value is specified by user.
        /// </summary>
        private void convertToBlackWhiteThresholdModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer.Image.PixelFormat != Vintasoft.Imaging.PixelFormat.BlackWhite)
            {
                WpfBinarizeWindow dlg = new WpfBinarizeWindow(imageViewer, true);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
                if (dlg.ShowProcessingDialog())
                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
            }
        }

        /// <summary>
        /// Change pixel format of image to BlackWhite, global threshold is detected automatically.
        /// </summary>
        private void convertToBlackWhiteGlobalModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatToBlackWhiteCommand(BinarizationMode.Global));

        }

        /// <summary>
        /// Changes pixel format of image to BlackWhite, adaptive threshold is detected automatically.
        /// </summary>
        private void convertToBlackWhiteAdaptiveModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer.Image.PixelFormat != Vintasoft.Imaging.PixelFormat.BlackWhite)
            {
                WpfAdaptiveBinarizeWindow dlg = new WpfAdaptiveBinarizeWindow(imageViewer, true);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
                if (dlg.ShowProcessingDialog())
                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
            }
        }

        /// <summary>
        /// Changes pixel format of image to BlackWhite using Halftone binarization.
        /// </summary>
        private void convertToBlackWhiteHalftoneModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatToBlackWhiteCommand(BinarizationMode.Halftone));
        }

        /// <summary>
        /// Changes pixel format of image to Palette1.
        /// </summary>
        private void convertToPalette1MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatCommand(Vintasoft.Imaging.PixelFormat.Indexed1));
        }

        /// <summary>
        /// Changes pixel format of image to Gray8.
        /// </summary>
        private void convertToGray8MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatCommand(Vintasoft.Imaging.PixelFormat.Gray8));
        }

        /// <summary>
        /// Changes pixel format of image to Indexed8.
        /// </summary>
        private void convertToPalette8MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangePixelFormatToPaletteCommand command =
                new ChangePixelFormatToPaletteCommand(Vintasoft.Imaging.PixelFormat.Indexed8);
            // consider transparency
            command.Transparency = true;
            PropertyGridWindow propertyGridWindow =
                new PropertyGridWindow(command, "Change Pixel Format to Indexed8 Command Properties ", true);
            propertyGridWindow.Owner = this;
            propertyGridWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (propertyGridWindow.ShowDialog() == true)
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(command);
        }

        /// <summary>
        /// Changes pixel format of image to Bgr24.
        /// </summary>
        private void convertToBgr24MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatCommand(Vintasoft.Imaging.PixelFormat.Bgr24));
        }

        /// <summary>
        /// Changes pixel format of image to Bgra32.
        /// </summary>
        private void convertToBgra32MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatCommand(Vintasoft.Imaging.PixelFormat.Bgra32));
        }

        /// <summary>
        /// Changes pixel format of image to the custom format.
        /// </summary>
        private void convertToCustomFormatMenuItem1_Click(object sender, RoutedEventArgs e)
        {
            WpfChangePixelFormatWindow dlg = new WpfChangePixelFormatWindow();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dlg.ShowDialog().Value)
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatCommand(dlg.PixelFormat));

        }

        #endregion


        /// <summary>
        /// Crops an image.
        /// </summary>
        private void cropMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (HasRectangularSelection() ||
                HasCustomSelection())
            {
                try
                {
                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(new CropCommand());
                }
                catch (ImageProcessingException ex)
                {
                    MessageBox.Show(ex.Message, "Image processing exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        /// <summary>
        /// Resizes an image.
        /// </summary>
        private void resizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get image
            VintasoftImage image = imageViewer.Image;
            // get resize command
            ResizeCommand command = new ResizeCommand();

            int width;
            int height;
            // get selection tool
            WpfRectangularSelectionTool selectionTool = imageViewer.VisualTool as WpfRectangularSelectionTool;
            // if selection tool is not empty AND selection is not empty
            if (selectionTool != null && selectionTool.Rectangle.Width != 0 && selectionTool.Rectangle.Height != 0)
            {
                // get selection size
                width = (int)selectionTool.Rectangle.Width;
                height = (int)selectionTool.Rectangle.Height;
            }
            else
            {
                // get image size
                width = image.Width;
                height = image.Height;
            }

            WpfResizeWindow dlg = new WpfResizeWindow(width, height, command.InterpolationMode);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dlg.ShowDialog().Value)
            {
                command.Width = dlg.ImageWidth;
                command.Height = dlg.ImageHeight;
                command.InterpolationMode = dlg.InterpolationMode;
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(command);
            }
        }

        /// <summary>
        /// Adds canvas and resizes an image.
        /// </summary>
        private void resizeCanvasMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VintasoftImage image = imageViewer.Image;
            WpfResizeCanvasWindow dlg = new WpfResizeCanvasWindow(image.Width, image.Height);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dlg.ShowDialog().Value)
            {
                ResizeCanvasCommand command = ImageProcessingCommandFactory.CreateCommand<ResizeCanvasCommand>(image);
                command.Width = dlg.CanvasWidth;
                command.Height = dlg.CanvasHeight;
                command.ImagePosition = dlg.ImagePosition;
                command.CanvasColor = dlg.CanvasColor;
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(command);
            }
        }

        /// <summary>
        /// Resamples an image.
        /// </summary>
        private void resampleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VintasoftImage image = imageViewer.Image;
            ResampleCommand command = new ResampleCommand();
            WpfResampleWindow dlg = new WpfResampleWindow((float)image.Resolution.Horizontal, (float)image.Resolution.Vertical, command.InterpolationMode, "Resample", true);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dlg.ShowDialog().Value)
            {
                command.HorizontalResolution = dlg.HorizontalResolution;
                command.VerticalResolution = dlg.VerticalResolution;
                command.InterpolationMode = dlg.InterpolationMode;
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(command);
            }
        }

        /// <summary>
        /// Changes resolution of image.
        /// </summary>
        private void changeResolutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VintasoftImage image = imageViewer.Image;
            WpfResampleWindow dlg = new WpfResampleWindow((float)image.Resolution.Horizontal, (float)image.Resolution.Vertical,
                ImageInterpolationMode.HighQualityBicubic, "Change resolution", false);
            dlg.ShowInterpolationComboBox = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dlg.ShowDialog().Value)
            {
                try
                {
                    if (!image.IsChanged && image.SourceInfo.Decoder.IsVectorDecoder)
                        DemosTools.ShowErrorMessage("Resolution of vector image", "Cannot change resolution for vector image. Change rendering resolution using RenderingSettings.Resolution property: View -> Image Viewer Settings... -> Image Rendering Settings.");
                    else
                        image.Resolution = new Vintasoft.Imaging.Resolution(dlg.HorizontalResolution, dlg.VerticalResolution);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Fills an image.
        /// </summary>
        private void fillImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfFillImageWindow dlg = new WpfFillImageWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Fills rectangle on an image.
        /// </summary>
        private void fillRectangleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteFillRectangleCommand();
        }

        /// <summary>
        /// Overlays an image.
        /// </summary>
        private void overlayImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteOverlayCommand();
        }

        /// <summary>
        /// Overlays binary image.
        /// </summary>
        private void overlayBinaryImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteOverlayBinaryCommand();
        }

        /// <summary>
        /// Overlays with color blending.
        /// </summary>
        private void overlayWithBlendingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteOverlayWithBlendingCommand();
        }

        /// <summary>
        /// Overlays with alpha mask.
        /// </summary>
        private void overlayWithMaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteOverlayMaskedCommand();
        }

        /// <summary>
        /// Compares two images.
        /// </summary>
        private void imageCompareMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteComparisonCommand();
        }

        #endregion


        #region Info

        /// <summary>
        /// Shows histogram of image.
        /// </summary>
        private void histogramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Drawing.Rectangle selectionRectangle = System.Drawing.Rectangle.Empty;

                // if current tool contains WpfRectangularSelectionToolWithCopyPaste with selection
                if (HasRectangularSelection())
                {
                    WpfRectangularSelectionToolWithCopyPaste selection = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(imageViewer.VisualTool);

                    selectionRectangle = new System.Drawing.Rectangle(
                            (int)Math.Round(selection.Rectangle.X),
                            (int)Math.Round(selection.Rectangle.Y),
                            (int)Math.Round(selection.Rectangle.Width),
                            (int)Math.Round(selection.Rectangle.Height));
                }

                WpfGetHistogramWindow dlg = new WpfGetHistogramWindow(
                    imageViewer.Image,
                    selectionRectangle,
                    _imageProcessingCommandExecutor.ExpandSupportedPixelFormats);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Determines that image is black-white.
        /// </summary>
        private void isImageBlackWhiteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteIsImageBlackWhiteCommand();
        }

        /// <summary>
        /// Determines that image is grayscale.
        /// </summary>
        private void isImageGrayscaleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteIsImageGrayscaleCommand();
        }

        /// <summary>
        /// Gets the number of colors in image.
        /// </summary>
        private void colorCountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteColorCountCommand();
        }

        /// <summary>
        /// Gets the real color depth of image.
        /// </summary>
        private void getImageColorDepthMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteGetImageColorDepthCommand();
        }

        /// <summary>
        /// Gets a border color of image.
        /// </summary>
        private void borderColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteGetBorderColorCommand();
        }

        /// <summary>
        /// Gets a backgorund color of image.
        /// </summary>
        private void getBackgroundColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteGetBackgroundColorCommand();
        }

        /// <summary>
        /// Gets the optimal binarization threshold of image.
        /// </summary>
        private void detectThresholdMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteGetThresholdCommand();
        }

        /// <summary>
        /// Determines that image is blank.
        /// </summary>
        private void isImageBlankMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteIsBlankCommand();
        }

        /// <summary>
        /// Determines that image contains certain color.
        /// </summary>
        private void hasCertainColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteHasCertainColorCommand();
        }

        #endregion


        #region Channels

        /// <summary>
        /// Extracts the alpha channel of image.
        /// </summary>
        private void extractAlphaChannelMenuItem1_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new GetAlphaChannelMaskCommand());
        }

        /// <summary>
        /// Inverts the red channel of image.
        /// </summary>
        private void invertRChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            InvertColorChannel(System.Drawing.Color.FromArgb(255, 0, 0));
        }

        /// <summary>
        /// Inverts the green channel of image.
        /// </summary>
        private void invertGChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            InvertColorChannel(System.Drawing.Color.FromArgb(0, 255, 0));
        }

        /// <summary>
        /// Inverts the blue channel of image.
        /// </summary>
        private void invertBChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            InvertColorChannel(System.Drawing.Color.FromArgb(0, 0, 255));
        }

        /// <summary>
        /// Sets a value of alpha channel for all pixels of image to the specified value.
        /// </summary>
        private void setAlphaChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteSetAlphaChannelValueCommand();
        }

        /// <summary>
        /// Changes the alpha channel of image from the specified image-mask.
        /// </summary>
        private void setAlphaChannelFromMaskMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteSetAlphaChannelCommand();
        }

        /// <summary>
        /// Remove the red channel of image.
        /// </summary>
        private void removeRChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExtractColorChannels(0, 255, 255);
        }

        /// <summary>
        /// Remove the green channel of image.
        /// </summary>
        private void removeGChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExtractColorChannels(255, 0, 255);
        }

        /// <summary>
        /// Remove the blue channel of image.
        /// </summary>
        private void removeBChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExtractColorChannels(255, 255, 0);
        }

        #endregion


        #region Color

        /// <summary>
        /// Converts an image to black-white image, threshold value is specified by user.
        /// </summary>
        private void convertToBlackWhiteThresholdMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer.Image.PixelFormat != Vintasoft.Imaging.PixelFormat.BlackWhite)
            {
                WpfBinarizeWindow dlg = new WpfBinarizeWindow(imageViewer, false);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
                if (dlg.ShowProcessingDialog())
                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
            }
        }

        /// <summary>
        /// Converts an image to black-white image, global threshold is detected automatically.
        /// </summary>
        private void convertToBlackWhiteGlobalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new BinarizeCommand(BinarizationMode.Global));
        }

        /// <summary>
        /// Converts an image to black-white image, adaptive threshold is detected automatically.
        /// </summary>
        private void convertToBlackWhiteAdaptiveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer.Image.PixelFormat != Vintasoft.Imaging.PixelFormat.BlackWhite)
            {
                WpfAdaptiveBinarizeWindow dlg = new WpfAdaptiveBinarizeWindow(imageViewer, false);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
                if (dlg.ShowProcessingDialog())
                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
            }
        }

        /// <summary>
        /// Convert colors of an image to black-white, use color gradient binarization.
        /// </summary>
        private void colorGradientBinarizationMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfColorGradientBinarizationWindow dlg = new WpfColorGradientBinarizationWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Convert an image to black-white image, use color gradient binarization.
        /// </summary>
        private void convertToBlackWhiteColorGradientMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfColorGradientBinarizationWindow dlg = new WpfColorGradientBinarizationWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.ChangePixelFormatToBlackWhite = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Desaturates an image.
        /// </summary>
        private void desaturateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new DesaturateCommand());
        }

        /// <summary>
        /// Converts an image to a halftone image.
        /// </summary>
        private void halftoneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new HalftoneCommand());
        }

        /// <summary>
        /// Posterizes an image.
        /// </summary>
        private void posterizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer.Image.PixelFormat != Vintasoft.Imaging.PixelFormat.BlackWhite)
            {
                WpfPosterizeWindow dlg = new WpfPosterizeWindow(imageViewer);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
                if (dlg.ShowProcessingDialog())
                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
            }
        }

        /// <summary>
        /// Changes the brightness and/or contrast of image.
        /// </summary>
        private void brightnessContrastMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfBrightnessContrastWindow dlg = new WpfBrightnessContrastWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Changes HSL of image.
        /// </summary>
        private void hueSaturationLuminanceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfHueSaturationLuminanceWindow dlg = new WpfHueSaturationLuminanceWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Changes gamma of image.
        /// </summary>
        private void gammaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfGammaWindow dlg = new WpfGammaWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Changes levels of image.
        /// </summary>
        private void levelsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfLevelsWindow dlg = new WpfLevelsWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Inverts an image.
        /// </summary>
        private void invertColorsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new InvertCommand());
        }

        /// <summary>
        /// Replaces a color in image.
        /// </summary>
        private void replaceColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfReplaceColorWindow dlg = new WpfReplaceColorWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Replaces a color gradient in image.
        /// </summary>
        private void replaceColorGradientMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfReplaceColorGradientWindow dlg = new WpfReplaceColorGradientWindow(imageViewer);
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Blends colors of image.
        /// </summary>
        private void colorBlendMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteColorBlendCommand(this);
        }

        /// <summary>
        /// Applies a color transform to an image.
        /// </summary>
        private void colorTransformMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColorTransformWindow dlg = new ColorTransformWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand(), true);
        }

        #endregion


        #region Transforms

        /// <summary>
        /// Flips an image.
        /// </summary>
        private void flipMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageRotateFlipType flipType = ImageRotateFlipType.RotateNoneFlipX;
            if (sender == xMenuItem1)
            {
                flipType = ImageRotateFlipType.RotateNoneFlipX;
            }
            else if (sender == yMenuItem1)
            {
                flipType = ImageRotateFlipType.RotateNoneFlipY;
            }
            else if (sender == xYMenuItem1)
            {
                flipType = ImageRotateFlipType.RotateNoneFlipXY;
            }

            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new FlipCommand(flipType));
        }

        /// <summary>
        /// Rotates an image.
        /// </summary>
        private void rotate_Click(object sender, RoutedEventArgs e)
        {
            if (sender == customMenuItem)
            {
                WpfRotateWindow dlg = new WpfRotateWindow(imageViewer.Image.PixelFormat);
                dlg.Owner = this;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (dlg.ShowDialog().Value)
                {
                    // if pixel formats are not equal
                    if (dlg.SourceImagePixelFormat != imageViewer.Image.PixelFormat)
                    {
                        // change pixel format
                        _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ChangePixelFormatCommand(dlg.SourceImagePixelFormat), false);
                    }

                    RotateCommand rotateCommand = new RotateCommand((float)dlg.Angle, dlg.BorderColorType);
                    rotateCommand.IsAntialiasingEnabled = dlg.IsAntialiasingEnabled;

                    _imageProcessingCommandExecutor.ExecuteProcessingCommand(rotateCommand);
                }
            }
            else
            {
                float angle = 0f;
                if (sender == rotate90)
                {
                    angle = 90f;
                }
                else if (sender == rotate180)
                {
                    angle = 180f;
                }
                else if (sender == rotate270)
                {
                    angle = 270f;
                }

                _imageProcessingCommandExecutor.ExecuteProcessingCommand(new RotateCommand(angle, System.Drawing.Color.Black));
            }
        }

        /// <summary>
        /// Scales an image.
        /// </summary>
        private void imageScalingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfImageScalingWindow dlg = new WpfImageScalingWindow();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Command = ImageProcessingCommandFactory.CreateCommand<ImageScalingCommand>(imageViewer.Image);
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand(), false);
            imageViewer.CenterImage = true;
        }

        /// <summary>
        /// Skews an image.
        /// </summary>
        private void skewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new SkewCommand());
            dlg.IsPreviewAvailable = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies quadrilateral warp transformation to an image.
        /// </summary>
        private void quadrilateralWarpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new QuadrilateralWarpCommand());
            dlg.IsPreviewAvailable = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        #endregion


        #region Filters

        #region Arithmetic filters

        /// <summary>
        /// Applies an arithmetic minimum filter to an image.
        /// </summary>
        private void minimumMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMinimumWindow dlg = new WpfMinimumWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies an arithmetic maximum filter to an image.
        /// </summary>
        private void maximumMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMaximumWindow dlg = new WpfMaximumWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies an arithmetic midpoint filter to an image.
        /// </summary>
        private void midPointMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMidpointWindow dlg = new WpfMidpointWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies an arithmetic mean filter to an image.
        /// </summary>
        private void meanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMeanWindow dlg = new WpfMeanWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies an arithmetic median filter to an image.
        /// </summary>
        private void medianMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMedianWindow dlg = new WpfMedianWindow(imageViewer);
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        #endregion


        #region Morphological filters

        /// <summary>
        /// Applies the morphological dilate filter to an image.
        /// </summary>
        private void dilateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfDilateWindow dlg = new WpfDilateWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(new DilateCommand(dlg.WindowSize));
        }

        /// <summary>
        /// Applies the morphological erode filter to an image.
        /// </summary>
        private void erodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfErodeWindow dlg = new WpfErodeWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(new ErodeCommand(dlg.WindowSize));
        }

        #endregion


        /// <summary>
        /// Applies the Blur filter to an image.
        /// </summary>
        private void blurMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfBlurWindow dlg = new WpfBlurWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Gaussian blur filter to an image.
        /// </summary>
        private void gaussianBlurMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfGaussianBlurWindow dlg = new WpfGaussianBlurWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Sharpen filter to an image.
        /// </summary>
        private void sharpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfSharpenWindow dlg = new WpfSharpenWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Edge Detection filter to an image.
        /// </summary>
        private void edgesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new EdgeDetectionCommand());
        }

        /// <summary>
        /// Applies the Emboss filter to an image.
        /// </summary>
        private void embossMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfEmbossWindow dlg = new WpfEmbossWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Add noise filter to an image.
        /// </summary>
        private void addNoiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new AddNoiseCommand());
            dlg.IsPreviewAvailable = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Canny edge detector filter to an image.
        /// </summary>
        private void cannyEdgeDetectorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfCannyEdgeDetectorWindow dlg = new WpfCannyEdgeDetectorWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        #endregion


        #region Document Cleanup

        /// <summary>
        /// Determines that image is document image.
        /// </summary>
        private void isDocumentImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteIsDocumentImageCommand();
        }

        /// <summary>
        /// Returns the rotation angle of document image.
        /// </summary>
        private void getDocumentImageRotationAngleMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            _imageProcessingCommandExecutor.ExecuteGetDocumentImageRotationAngleCommand();
#endif
        }

        /// <summary>
        /// Returns the rotation angle of image.
        /// </summary>
        private void rotationAngleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteGetRotationAngleCommand();
        }

        /// <summary>
        /// Determines orientation of the text.
        /// </summary>
        private void getTextOrientationMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            _imageProcessingCommandExecutor.ExecuteGetTextOrientationCommand();
#endif
        }

        /// <summary>
        /// Removes noise in a document image.
        /// </summary>
        private void despeckleMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new DespeckleCommand());
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;
            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Automatically corrects the orientation of document image.
        /// </summary>
        private void deskewDocumentImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN

            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new DeskewDocumentImageCommand());

            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.IsPreviewAvailable = false;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());

#endif
        }

        /// <summary>
        /// Automatically detects correct position of document image.
        /// </summary>
        private void deskewMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new DeskewCommand());

            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.IsPreviewAvailable = false;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Automatically detects and corrects orientation of document image.
        /// </summary>
        private void autoOrientationMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new AutoTextOrientationCommand());

            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.IsPreviewAvailable = false;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Inverts text blocks in a document image.
        /// </summary>
        private void textBlockInvertMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfAutoTextInvertWindow dlg = new WpfAutoTextInvertWindow(imageViewer);
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Automatically inverts a document image.
        /// </summary>
        private void automaticInvertMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new AutoInvertCommand());
#endif
        }

        /// <summary>
        /// Clears the border of document image.
        /// </summary>
        private void borderClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new BorderClearCommand());
#endif
        }

        /// <summary>
        /// Removes (crops) border of a document image.
        /// </summary>
        private void borderRemovalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfBorderRemovalWindow dlg = new WpfBorderRemovalWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Removes halftone in a document image.
        /// </summary>
        private void halftoneRemovalMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfHalftoneRemovalWindow dlg = new WpfHalftoneRemovalWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Smoothes a document image.
        /// </summary>
        private void smoothingMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfSmoothingWindow dlg = new WpfSmoothingWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Fills not filled hole punches in a document image.
        /// </summary>
        private void holePunchFillingMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            HolePunchFillingCommand holePunchFillingCommand = new HolePunchFillingCommand();
            holePunchFillingCommand.HolePunchLocation =
                HolePunchLocation.Left |
                HolePunchLocation.Right |
                HolePunchLocation.Top |
                HolePunchLocation.Bottom;
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, holePunchFillingCommand);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Removes filled hole punches in a document image.
        /// </summary>
        private void holePunchRemovalMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            HolePunchRemovalCommand holePunchRemovalCommand = new HolePunchRemovalCommand();
            holePunchRemovalCommand.HolePunchLocation =
                HolePunchLocation.Left |
                HolePunchLocation.Right |
                HolePunchLocation.Top |
                HolePunchLocation.Bottom;
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, holePunchRemovalCommand);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Removes lines in a document image.
        /// </summary>
        private void lineRemovalMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new LineRemovalCommand());
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Removes shapes in a document image.
        /// </summary>
        private void shapeRemovalMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new ShapeRemovalCommand());
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Removes a color noise in a document image.
        /// </summary>
        private void colorNoiseClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            ColorNoiseClearWindow dlg = new ColorNoiseClearWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
#endif
        }

        /// <summary>
        /// Replaces colors in a document image.
        /// </summary>
        private void advancedReplaceColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteAdvancedReplaceColorCommand();
        }

        /// <summary>
        /// Corrects the perspective of image.
        /// </summary>
        private void documentPerspectiveCorrectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteDocumentPerspectiveCorrectionCommand();
        }

        #endregion


        #region Photo Effects

        /// <summary>
        /// Applies the Auto Levels effect to an image.
        /// </summary>
        private void autoLevelsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new AutoLevelsCommand());
        }

        /// <summary>
        /// Applies the Auto Colors effect to an image.
        /// </summary>
        private void autoColorsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new AutoColorsCommand());
        }

        /// <summary>
        /// Applies the Auto Contrast effect to an image.
        /// </summary>
        private void autoContrastMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new AutoContrastCommand());
        }

        /// <summary>
        /// Applies the Bevel Edge effect to an image.
        /// </summary>
        private void bevelEdgeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfBevelEdgeWindow dlg = new WpfBevelEdgeWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Drop Shadow effect to an image.
        /// </summary>
        private void dropShadowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new DropShadowCommand());
            dlg.IsPreviewAvailable = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Motion Blur effect to an image.
        /// </summary>
        private void motionBlurMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMotionBlurWindow dlg = new WpfMotionBlurWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Mozaic effect to an image.
        /// </summary>
        private void mozaicMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfMosaicWindow dlg = new WpfMosaicWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Oil Painting effect to an image.
        /// </summary>
        private void oilPaintingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfOilPaintingWindow dlg = new WpfOilPaintingWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Pixelate effect to an image.
        /// </summary>
        private void pixelateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPixelateWindow dlg = new WpfPixelateWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Red Eye Removal effect to an image.
        /// </summary>
        private void redEyeRemovalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfRedEyeRemovalWindow dlg = new WpfRedEyeRemovalWindow(imageViewer);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the Sepia effect to an image.
        /// </summary>
        private void sepiaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new SepiaCommand());
        }

        /// <summary>
        /// Applies the Solarize effect to an image.
        /// </summary>
        private void solarizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(new SolarizeCommand());
        }

        /// <summary>
        /// Applies the Tile Reflection effect to an image.
        /// </summary>
        private void tileReflectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfTileReflectionWindow dlg = new WpfTileReflectionWindow(imageViewer);
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        #endregion


        #region FFT

        #region Filtering

        /// <summary>
        /// Applies Ideal lowpass filter to an image.
        /// </summary>
        private void idealLowpassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new IdealLowpassCommand());
            dlg.Owner = this;
            dlg.IsPreviewAvailable = true;
            dlg.IsPreviewEnabled = false;
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());

        }

        /// <summary>
        /// Applies Butterworth lowpass filter to an image.
        /// </summary>
        private void butterworthLowpassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new ButterworthLowpassCommand());
            dlg.IsPreviewAvailable = true;
            dlg.IsPreviewEnabled = false;
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies Gaussian lowpass filter to an image.
        /// </summary>
        private void gaussianLowpassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new GaussianLowpassCommand());
            dlg.IsPreviewAvailable = true;
            dlg.IsPreviewEnabled = false;
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies Ideal highpass filter to an image.
        /// </summary>
        private void idealHighpassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new IdealHighpassCommand());
            dlg.IsPreviewAvailable = true;
            dlg.IsPreviewEnabled = false;
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies Butterworth highpass filter to an image.
        /// </summary>
        private void butterworthHighpassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new ButterworthHighpassCommand());
            dlg.IsPreviewAvailable = true;
            dlg.IsPreviewEnabled = false;
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies Gaussian highpass filter to an image.
        /// </summary>
        private void gaussianHighpassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfPropertyGridConfigWindow dlg = new WpfPropertyGridConfigWindow(imageViewer, new GaussianHighpassCommand());
            dlg.IsPreviewAvailable = true;
            dlg.IsPreviewEnabled = false;
            dlg.UseCurrentViewerZoomWhenPreviewProcessing = true;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        #endregion


        /// <summary>
        /// Applies the image smoothing filter to an image.
        /// </summary>
        private void imageSmoothingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfImageSmoothingWindow dlg = new WpfImageSmoothingWindow(imageViewer);
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Applies the image sharpening filter to an image.
        /// </summary>
        private void imageSharpeningMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfImageSharpeningWindow dlg = new WpfImageSharpeningWindow(imageViewer);
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        /// <summary>
        /// Visualizes image frequency spectrum.
        /// </summary>
        private void frequencySpecrtumVisualizerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfFrequencySpectrumVisualizerWindow dlg = new WpfFrequencySpectrumVisualizerWindow(imageViewer);
            dlg.IsPreviewEnabled = false;
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ExpandSupportedPixelFormats = _imageProcessingCommandExecutor.ExpandSupportedPixelFormats;

            if (dlg.ShowProcessingDialog())
                _imageProcessingCommandExecutor.ExecuteProcessingCommand(dlg.GetProcessingCommand());
        }

        #endregion


        /// <summary>
        /// Enables/disables the image processing in multiple threads.
        /// </summary>
        private void useMultithreadingMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            _imageProcessingCommandExecutor.ExecuteMultithread = useMultithreadingMenuItem.IsChecked;
        }

        /// <summary>
        /// Enables/disables the expanding of image pixel format during image processing.
        /// </summary>
        private void expandPixelFormatMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (_imageProcessingCommandExecutor != null)
                _imageProcessingCommandExecutor.ExpandSupportedPixelFormats = expandPixelFormatMenuItem.IsChecked;
        }

        /// <summary>
        /// Loads clipping paths from metadata of current image.
        /// </summary>
        private void loadPathsFromMetadataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.imageViewer.Image != null)
            {
                MetadataNode metadata = this.imageViewer.Image.Metadata.MetadataTree;

                PhotoshopResourcesMetadata photoshopMetadata = metadata.FindChildNode<PhotoshopResourcesMetadata>();

                bool pathsAreLoaded = false;
                if (photoshopMetadata != null)
                {
                    int width = this.imageViewer.Image.Width;
                    int height = this.imageViewer.Image.Height;

                    GdiGraphicsPath paths = new GdiGraphicsPath();
                    foreach (PhotoshopResource resource in photoshopMetadata.Resources)
                    {
                        if (resource is PhotoshopImagePathResource)
                        {
                            IGraphicsPath path = ((PhotoshopImagePathResource)resource).GetPath(width, height);
                            if (path.PointCount > 0)
                                paths.AddPath(path);
                        }
                    }

                    if (paths.PointCount > 0)
                    {
                        WpfPathSelectionRegion selection = new WpfPathSelectionRegion(WpfObjectConverter.CreateWindowsPathGeometry(paths.Source));
                        selection.InteractionController = selection.TransformInteractionController;
                        WpfCustomSelectionTool tool = new WpfCustomSelectionTool();
                        tool.Selection = selection;

                        this.imageViewer.VisualTool = tool;
                        pathsAreLoaded = true;
                    }
                }

                if (!pathsAreLoaded)
                    DemosTools.ShowInfoMessage("No clipping paths found in metadata.");
            }
        }

        #endregion


        #region 'Tools' menu

        /// <summary>
        /// Shows a window with images animation.
        /// </summary>
        private void animationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfShowAnimationWindow window = new WpfShowAnimationWindow(imageViewer.Images);
            window.Owner = this;
            window.ShowDialog();
        }

        #endregion


        #region 'Help' menu

        /// <summary>
        /// Shows an "About" dialog.
        /// </summary>
        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder description = new StringBuilder();

            description.AppendLine("This project demonstrates the following SDK capabilities:");
            description.AppendLine();
            description.AppendLine("- Load image from file, acquire from scanner, capture from webcam.");
            description.AppendLine();
            description.AppendLine("- Display, print and save all supported image and document formats.");
            description.AppendLine();
            description.AppendLine("- Navigate images: first, previous, next, last.");
            description.AppendLine();
            description.AppendLine("- Copy image to/from clipboard.");
            description.AppendLine();
            description.AppendLine("- Access image pixels directly.");
            description.AppendLine();
            description.AppendLine("- Set image and thumbnail preview settings.");
            description.AppendLine();
            description.AppendLine("- Process images using  90+ image processing functions.");
            description.AppendLine();
            description.AppendLine("- Undo/redo changes in processed images.");
            description.AppendLine();
            description.AppendLine("- View image slide show.");
            description.AppendLine();
            description.AppendLine("- View and edit image palette.");
            description.AppendLine();
            description.AppendLine("- Process images interactively: select, magnify, crop, drag-n-drop, overlay, zoom, pan, scroll.");
            description.AppendLine();
            description.AppendLine("- Supported image formats: BMP, CUR, DICOM, DOC, DOCX, XLS, XLSX, EMF, GIF, ICO, JBIG2, JPEG, JPEG2000, JPEG-LS, PCX, PDF, PNG, TIFF, BigTIFF, WMF, RAW (NEF, NRW, CR2, CRW, DNG).");
            description.AppendLine();
            description.AppendLine();
            description.AppendLine("The project is available in C# and VB.NET for Visual Studio .NET.");

            WpfAboutBoxBaseWindow dlg = new WpfAboutBoxBaseWindow("vsimaging-dotnet");
            dlg.Description = description.ToString();
            dlg.Owner = this;
            dlg.ShowDialog();
        }


        #endregion


        #region SelectionTool's context menu

        /// <summary>
        /// Mouse up in image viewer.
        /// </summary>
        private void imageViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // if clicked right button then
            if (e.ChangedButton == MouseButton.Right && imageViewer.Image != null)
            {
                // if current tool has CustomSelectionTool
                // and selection tool has selection
                if (HasCustomSelection())
                {
                    WpfCustomSelectionTool customSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);

                    // if clicks on selection then
                    Point mousePositiong = e.GetPosition(imageViewer);
                    Point point = imageViewer.PointFromControlToImage(mousePositiong);
                    if (customSelectionTool.Selection.IsPointOnObject(point, 10.0))
                    {
                        // show selection context menu
                        _selectionContextMenuStripLocation = point;
                        imageViewer.ContextMenu = CreateCustomSelectionToolContextMenu();
                        return;
                    }
                }
                else
                {
                    // if current tool has RectangularSelectionTool
                    // and selection tool has selection
                    if (HasRectangularSelection())
                    {
                        WpfRectangularSelectionTool rectangularSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(imageViewer.VisualTool);

                        // if clicks on selection then
                        Point mousePositiong = e.GetPosition(imageViewer);
                        Point point = imageViewer.PointFromControlToImage(mousePositiong);
                        if (rectangularSelectionTool.Rectangle.IntersectsWith(new Rect(point.X, point.Y, 10.0, 10.0)))
                        {
                            // show selection context menu
                            _selectionContextMenuStripLocation = point;
                            imageViewer.ContextMenu = CreateCustomSelectionToolContextMenu();
                            return;
                        }
                    }
                }
                // show image viewer context menu                
                imageViewer.ContextMenu = _imageViewerDefaultContextMenu;
            }
            else
            {
                imageViewer.ContextMenu = null;
            }
        }

        private ContextMenu CreateCustomSelectionToolContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            WpfCustomSelectionTool customSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);
            WpfRectangularSelectionTool rectangularSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(imageViewer.VisualTool);

            // if current tool has CustomSelectionTool or RectangularSelectionTool
            if (customSelectionTool != null || rectangularSelectionTool != null)
            {
                // builds the selection context menu
                MenuItem copyPasteItem = new MenuItem();
                copyPasteItem.Header = "Copy";
                copyPasteItem.Click += new RoutedEventHandler(copyImageMenuItem_Click);
                contextMenu.Items.Add(copyPasteItem);

                copyPasteItem = new MenuItem();
                copyPasteItem.Header = "Paste";
                copyPasteItem.Click += new RoutedEventHandler(pasteImageMenuItem_Click);
                contextMenu.Items.Add(copyPasteItem);

                // if current tool has CustomSelectionTool then
                if (customSelectionTool != null)
                {
                    MenuItem transformersItem = new MenuItem();
                    transformersItem.Header = "Transformers";
                    contextMenu.Items.Add(transformersItem);

                    // add "None" item to context menu - use selection without transformation
                    MenuItem noneMenuItem = new MenuItem();
                    noneMenuItem.Header = "None";
                    transformersItem.Items.Add(noneMenuItem);

                    // add separator
                    transformersItem.Items.Add(new Separator());
                    // add building interaction controller of current selection to context menu
                    AddItemToSelectionContextMenu(transformersItem.Items, "Building", customSelectionTool.Selection.BuildingInteractionController);

                    // add separator
                    transformersItem.Items.Add(new Separator());
                    // foreach available transform interactions of current selection
                    foreach (string name in customSelectionTool.Selection.AvailableTransformInteractionControllers.Keys)
                        // add transform interaction controller to context menu
                        AddItemToSelectionContextMenu(transformersItem.Items, name, customSelectionTool.Selection.AvailableTransformInteractionControllers[name]);

                    // if current interaction controller is PointBasedObjectPointTransformer then
                    if (customSelectionTool.Selection.InteractionController is WpfPointBasedObjectPointTransformer)
                    {
                        // add separator
                        transformersItem.Items.Add(new Separator());

                        // add "Remove selected points" context menu item
                        MenuItem item = new MenuItem();
                        item.Header = "Remove selected points";
                        item.Click += new RoutedEventHandler(removeSelectedPoints_Click);
                        transformersItem.Items.Add(item);

                        // add "Add point" context menu item
                        item = new MenuItem();
                        item.Header = "Add point";
                        item.Click += new RoutedEventHandler(addPoint_Click);
                        transformersItem.Items.Add(item);
                    }
                }
            }

            return contextMenu;
        }

        /// <summary>
        /// Adds new item to the context menu of CustomSelectionTool.
        /// </summary>
        /// <param name="contextMenuItems">Context menu item collection.</param>
        /// <param name="name">Item name.</param>
        /// <param name="interactionController">Interaction controller.</param>
        private void AddItemToSelectionContextMenu(
            ItemCollection contextMenuItems,
            string name,
            IWpfInteractionController interactionController)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = interactionController;
            item.Click += new RoutedEventHandler(selectionContextMenuStrip_ChangeInteractionController);
            contextMenuItems.Add(item);
        }

        /// <summary>
        /// Changes current interaction controller of selection area.
        /// </summary>
        private void selectionContextMenuStrip_ChangeInteractionController(object sender, RoutedEventArgs e)
        {
            // if current tool contains a Custom Selection with selection
            if (HasCustomSelection())
            {
                WpfCustomSelectionTool selectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);

                // gets an interaction controller of this context menu item
                IWpfInteractionController interactionController = ((IWpfInteractionController)((MenuItem)sender).Tag);
                // if interaction controller is BuildingInteractionController
                if (interactionController == selectionTool.Selection.BuildingInteractionController)
                {
                    // start or continue building of current selection
                    selectionTool.BeginBuilding();
                }
                else
                {
                    // change transform interaction controller of current selection
                    selectionTool.Selection.TransformInteractionController = interactionController;
                    if (selectionTool.Selection.InteractionController != selectionTool.Selection.TransformInteractionController)
                        selectionTool.Selection.InteractionController = selectionTool.Selection.TransformInteractionController;
                }
            }
        }

        /// <summary>
        /// Event handler for "Remove selected points" item from the context menu of CustomSelectionTool.
        /// </summary>
        private void removeSelectedPoints_Click(object sender, RoutedEventArgs e)
        {
            if (HasCustomSelection())
            {
                WpfCustomSelectionTool selectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);

                // gets the PointBasedObjectPointTransformer of current selection
                WpfPointBasedObjectPointTransformer controller = (WpfPointBasedObjectPointTransformer)selectionTool.Selection.InteractionController;
                // remove selected points
                controller.RemovePoints(controller.SelectedPointIndexes);
            }
        }

        /// <summary>
        /// Event handler for "Add point" item from the context menu of CustomSelectionTool.
        /// </summary>
        private void addPoint_Click(object sender, RoutedEventArgs e)
        {
            if (HasCustomSelection())
            {
                WpfCustomSelectionTool selectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);
                // gets the PointBasedObjectPointTransformer of current selection
                WpfPointBasedObjectPointTransformer controller = (WpfPointBasedObjectPointTransformer)selectionTool.Selection.InteractionController;
                // add point to current selection
                controller.InsertPoint(_selectionContextMenuStripLocation);
            }
        }

        #endregion


        #region Image viewer

        /// <summary>
        /// Image is loading in image viewer.
        /// </summary>
        private void imageViewer_ImageLoading(object sender, ImageLoadingEventArgs e)
        {
            _imageLoadingStartTime = DateTime.Now;
            _imageLoadingTime = TimeSpan.Zero;

            imageLoadingStatusLabel.Visibility = Visibility.Visible;
            imageLoadingProgressBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Loading of image in image viewer is in progress.
        /// </summary>
        private void imageViewer_ImageLoadingProgress(object sender, ProgressEventArgs e)
        {
            if (_isWindowClosing)
            {
                e.Cancel = true;
                return;
            }

            imageLoadingProgressBar.Value = e.Progress;
        }

        /// <summary>
        /// Image is loaded in image viewer.
        /// </summary>
        private void imageViewer_ImageLoaded(object sender, ImageLoadedEventArgs e)
        {
            if (_isWindowClosing)
                _isFileDialogOpened = false;
            else
                _imageLoadingTime = DateTime.Now.Subtract(_imageLoadingStartTime);

            imageLoadingStatusLabel.Visibility = Visibility.Collapsed;
            imageLoadingProgressBar.Visibility = Visibility.Collapsed;

            this.IsImageLoaded = true;

            if (editImagePixelsMenuItem.IsChecked && !imageViewer.IsEntireImageLoaded)
                editImagePixelsMenuItem.IsChecked = false;
        }

        /// <summary>
        /// Image is not loaded because of error.
        /// </summary>
        private void imageViewer_ImageLoadingException(object sender, Vintasoft.Imaging.ExceptionEventArgs e)
        {
            imageLoadingStatusLabel.Visibility = Visibility.Collapsed;
            imageLoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Image is changed in image viewer.
        /// </summary>
        private void imageViewer_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            this.IsImageLoaded = true;
        }

        /// <summary>
        /// Image is reloaded in image viewer.
        /// </summary>
        private void imageViewer_ImageReloaded(object sender, ImageReloadEventArgs e)
        {
            this.IsImageLoaded = true;
        }

        /// <summary>
        /// Index of focused image in viewer is changing.
        /// </summary>
        private void imageViewer_FocusedIndexChanging(object sender, PropertyChangedEventArgs<int> e)
        {
            if (_isWindowClosing)
                return;
        }

        /// <summary>
        /// Index of focused image in viewer is changed.
        /// </summary>
        private void imageViewer_FocusedIndexChanged(object sender, PropertyChangedEventArgs<int> e)
        {
            if (_isWindowClosing)
                return;

            imageViewerToolBar.SelectedPageIndex = e.NewValue;

            if (_directPixelAccessWindow != null)
                _directPixelAccessWindow.SelectPixel(-1, -1);
        }

        /// <summary>
        /// Insert key is pressed in image viewer.
        /// </summary>
        private void imageViewer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
                InsertKeyPressed();
        }

        /// <summary>
        /// Visual tool of image viewer is changed.
        /// </summary>
        private void imageViewer_VisualToolChanged(object sender, PropertyChangedEventArgs<WpfVisualTool> e)
        {
            if (_isVisualToolChanging)
                return;

            if (_imageMapTool.Enabled)
            {
                _isVisualToolChanging = true;
                if (e.NewValue != null)
                {
                    if (e.NewValue != _imageMapTool)
                        imageViewer.VisualTool = new WpfCompositeVisualTool(_imageMapTool, e.NewValue);
                }
                else
                    imageViewer.VisualTool = _imageMapTool;
                _isVisualToolChanging = false;
            }
        }

        /// <summary>
        /// Changes the zoom factor in image viewer.
        /// </summary>
        private void imageViewer_ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            _imageScaleSelectedMenuItem.IsChecked = false;
            switch (imageViewer.SizeMode)
            {
                case ImageSizeMode.BestFit:
                    _imageScaleSelectedMenuItem = bestFitMenuItem;
                    break;
                case ImageSizeMode.FitToHeight:
                    _imageScaleSelectedMenuItem = fitToHeightMenuItem;
                    break;
                case ImageSizeMode.FitToWidth:
                    _imageScaleSelectedMenuItem = fitToWidthMenuItem;
                    break;
                case ImageSizeMode.Normal:
                    _imageScaleSelectedMenuItem = normalImageMenuItem;
                    break;
                case ImageSizeMode.PixelToPixel:
                    _imageScaleSelectedMenuItem = pixelToPixelMenuItem;
                    break;
                case ImageSizeMode.Zoom:
                    _imageScaleSelectedMenuItem = scaleMenuItem;
                    break;
            }
            _imageScaleSelectedMenuItem.IsChecked = true;
        }

        #endregion


        #region Visual tool

        /// <summary>
        /// Occurs when visual tool throws an exception.
        /// </summary>
        void imageViewer_VisualToolException(object sender, Vintasoft.Imaging.ExceptionEventArgs e)
        {
            DemosTools.ShowErrorMessage(e.Exception);
        }

        #endregion


        #region Thumbnail viewer

        /// <summary>
        /// Loading of thumbnails is in progress.
        /// </summary>
        private void thumbnailViewer_ThumbnailsLoadingProgress(object sender, ProgressEventArgs e)
        {
            Visibility isProgressVisibility = Visibility.Collapsed;
            if (e.Progress != 100)
                isProgressVisibility = Visibility.Visible;

            addingThumbnailsProgressBar.Value = e.Progress;
            addingThumbnailsStatusLabel.Visibility = isProgressVisibility;
            addingThumbnailsProgressBar.Visibility = isProgressVisibility;
        }

        /// <summary>
        /// Insert key is pressed in thumbnail viewer.
        /// </summary>
        private void thumbnailViewer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
                InsertKeyPressed();
        }

        /// <summary>
        /// Handles the ThumbnailAdded event of the thumbnailViewer control.
        /// </summary>
        private void thumbnailViewer_ThumbnailAdded(object sender, ThumbnailImageItemEventArgs e)
        {
            e.Thumbnail.MouseEnter += Thumbnail_MouseEnter;
        }

        /// <summary>
        /// Handles the MouseEnter event of the ThumbnailImageItem control.
        /// </summary>
        private void Thumbnail_MouseEnter(object sender, MouseEventArgs e)
        {
            ThumbnailImageItem thumbnail = (ThumbnailImageItem)sender;
            if (thumbnail.ContextMenu == null)
            {
                ContextMenu contextMenu = new ContextMenu();

                //Save Image
                MenuItem saveImageMenuItem = new MenuItem();
                saveImageMenuItem.Header = "Save Image";

                MenuItem toFileSaveImageMenuItem = new MenuItem();
                toFileSaveImageMenuItem.Header = "To file";
                toFileSaveImageMenuItem.Click += new RoutedEventHandler(saveCurrentImageMenuItem_Click);
                MenuItem toClipboardSaveImageMenuItem = new MenuItem();
                toClipboardSaveImageMenuItem.Header = "To clipboard";
                toClipboardSaveImageMenuItem.Click += new RoutedEventHandler(copyImageMenuItem_Click);

                saveImageMenuItem.Items.Add(toFileSaveImageMenuItem);
                saveImageMenuItem.Items.Add(toClipboardSaveImageMenuItem);
                contextMenu.Items.Add(saveImageMenuItem);

                //Set Image
                MenuItem setImageMenuItem = new MenuItem();
                setImageMenuItem.Header = "Set Image";

                MenuItem fromFileSetImageMenuItem = new MenuItem();
                fromFileSetImageMenuItem.Header = "From file";
                fromFileSetImageMenuItem.Click += new RoutedEventHandler(setImageFromFileMenuItem_Click);

                MenuItem fromClipboardSetImageMenuItem = new MenuItem();
                fromClipboardSetImageMenuItem.Header = "From clipboard";
                fromClipboardSetImageMenuItem.Click += new RoutedEventHandler(pasteImageMenuItem_Click);

                setImageMenuItem.Items.Add(fromFileSetImageMenuItem);
                setImageMenuItem.Items.Add(fromClipboardSetImageMenuItem);
                contextMenu.Items.Add(setImageMenuItem);

                //Insert Image
                MenuItem inserImageMenuItem = new MenuItem();
                inserImageMenuItem.Header = "Insert Image";

                MenuItem fromFileInserImageMenuItem = new MenuItem();
                fromFileInserImageMenuItem.Header = "From file";
                fromFileInserImageMenuItem.Click += new RoutedEventHandler(insertImageFromFileMenuItem_Click);

                MenuItem fromClipboardInserImageMenuItem = new MenuItem();
                fromClipboardInserImageMenuItem.Header = "From clipboard";
                fromClipboardInserImageMenuItem.Click += new RoutedEventHandler(insertImageFromClipboardMenuItem_Click);

                inserImageMenuItem.Items.Add(fromFileInserImageMenuItem);
                inserImageMenuItem.Items.Add(fromClipboardInserImageMenuItem);
                contextMenu.Items.Add(inserImageMenuItem);

                //Delete Image
                MenuItem deleteImage = new MenuItem();
                deleteImage.Header = "Delete Image";
                deleteImage.Click += new RoutedEventHandler(deleteImageMenuItem_Click);

                contextMenu.Items.Add(deleteImage);

                thumbnail.ContextMenu = contextMenu;
            }
        }

        /// <summary>
        /// Sets the ToolTip of hovered thumbnail.
        /// </summary>
        private void thumbnailViewer_HoveredThumbnailChanged(object sender, RoutedPropertyChangedEventArgs<ThumbnailImageItem> e)
        {
            ThumbnailImageItem thumbnailImage = (ThumbnailImageItem)e.NewValue;
            if (thumbnailImage != null)
            {
                try
                {
                    // get information about hovered image in thumbnail viewer
                    ImageSourceInfo imageSourceInfo = thumbnailImage.Source.SourceInfo;
                    string filename = null;

                    // if image loaded from file
                    if (imageSourceInfo.SourceType == ImageSourceType.File)
                    {
                        // get image file name
                        filename = Path.GetFileName(imageSourceInfo.Filename);
                    }
                    // if image loaded from stream
                    else if (imageSourceInfo.SourceType == ImageSourceType.Stream)
                    {
                        // if stream is file stream
                        if (imageSourceInfo.Stream is FileStream)
                            // get image file name
                            filename = Path.GetFileName(((FileStream)imageSourceInfo.Stream).Name);
                    }
                    // if image is new image
                    else
                    {
                        filename = "Bitmap";
                    }

                    // if image is multipage image
                    if (imageSourceInfo.PageCount > 1)
                        thumbnailImage.ToolTip = string.Format("{0}, page {1}", filename, imageSourceInfo.PageIndex + 1);
                    else
                        thumbnailImage.ToolTip = filename;
                }
                catch
                {
                    thumbnailImage.ToolTip = "";
                }
            }
        }

        #endregion


        #region File manipulation


        /// <summary>
        /// Opens a file stream and adds stream to the image collection of image viewer.
        /// </summary>
        /// <param name="filename">Opening file name.</param>
        private void OpenFile(string filename)
        {
            // close the previosly opened file
            CloseCurrentFile();

            // file, that is being opened, will be a new source
            _isSourceChanging = true;

            // save the source filename
            _sourceFilename = Path.GetFullPath(filename);

            // check the source file for read-write access
            CheckSourceFileForReadWriteAccess();

            // add the source file to the viewer
            _imagesManager.Add(filename, _isFileReadOnlyMode);
        }

        /// <summary>
        /// Adds files to the image collection of annotation viewer.
        /// </summary>
        /// <param name="filenames">Opening files names.</param>
        private void AddFiles(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                // add file to the viewer
                _imagesManager.Add(filename);
            }
        }

        /// <summary>
        /// Closes current image file.
        /// </summary>
        private void CloseCurrentFile()
        {
            WaitUntilSavingAndProcessingIsFinished();

            _imagesManager.Cancel();

            _isFileReadOnlyMode = false;
            _sourceFilename = null;
            _sourceDecoderName = null;

            imageViewer.Images.ClearAndDisposeItems();
        }

        /// <summary>
        /// Checks the source file for read-write access.
        /// </summary>
        private void CheckSourceFileForReadWriteAccess()
        {
            _isFileReadOnlyMode = false;
            Stream stream = null;
            try
            {
                stream = new FileStream(_sourceFilename, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            if (stream == null)
            {
                _isFileReadOnlyMode = true;
            }
            else
            {
                stream.Close();
                stream.Dispose();
            }
        }

        /// <summary>
        /// Waits while image saving and/or processing will be finished.
        /// </summary>
        void WaitUntilSavingAndProcessingIsFinished()
        {
            // if image collection is saving at the moment
            if (this.IsImageSaving)
            {
                // send signal that saving must be canceled
                _cancelImageSaving = true;
                // wait while saving is canceled/finished
                while (this.IsImageSaving)
                {
                    Thread.Sleep(5);
                    DoEvents();
                }
            }

            // if image is processing at the moment
            while (_imageProcessingCommandExecutor.IsImageProcessingWorking)
            {
                // wait
                DoEvents();
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Handler of the ImageViewerImagesManager.AddStarting event.
        /// </summary>
        private void ImagesManager_AddStarting(object sender, EventArgs e)
        {
            IsFileOpening = true;
        }

        /// <summary>
        /// Handler of the ImageViewerImagesManager.ImageSourceAddStarting event.
        /// </summary>
        private void ImagesManager_ImageSourceAddStarting(object sender, ImageSourceEventArgs e)
        {
            // update window title
            string fileState = string.Format("Opening {0}...", Path.GetFileName(e.SourceFilename));
            Title = string.Format(_titlePrefix, fileState);
        }

        /// <summary>
        /// Handler of the ImageViewerImagesManager.ImageSourceAddFinished event.
        /// </summary>
        private void ImagesManager_ImageSourceAddFinished(object sender, ImageSourceEventArgs e)
        {
            // if source is changed
            if (_isSourceChanging)
            {
                if (imageViewer.Images.Count > 0)
                {
                    // set new source decoder name
                    _sourceDecoderName = imageViewer.Images[0].SourceInfo.DecoderName;
                }
                _isSourceChanging = false;
            }
        }

        /// <summary>
        /// Handler of the ImageViewerImagesManager.AddFinished event.
        /// </summary>
        private void ImagesManager_AddFinished(object sender, EventArgs e)
        {
            IsFileOpening = false;
            _isSourceChanging = false;
        }

        /// <summary>
        /// Handler of the ImageViewerImagesManager.ImageSourceAddException event.
        /// </summary>
        private void ImagesManager_ImageSourceAddException(object sender, ImageSourceExceptionEventArgs e)
        {
            // show error message
            string message = string.Format("Cannot open {0} : {1}", Path.GetFileName(e.SourceFilename), e.Exception.Message);
            DemosTools.ShowErrorMessage(message);

            // if new source failed to set, close file
            if (_isSourceChanging)
                CloseCurrentFile();
        }

        private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        #endregion


        #region Image collection

        /// <summary>
        /// Image collection of image viewer is changed.
        /// </summary>
        private void Images_CollectionChanged(object sender, ImageCollectionChangeEventArgs e)
        {
            if (imageViewer.Images.Count == 0)
                _isImageLoaded = false;

            // update the UI
            InvokeUpdateUI();
        }

        #endregion


        #region Image info

        /// <summary>
        /// Updates information about focused image.
        /// </summary>
        private void UpdateImageInfo()
        {
            try
            {
                if (imageViewer.FocusedIndex == -1)
                {
                    imageInfoLabel.Text = "";
                    return;
                }

                VintasoftImage image = imageViewer.Image;

                // show message if image is changed
                string sChanged = "";
                if (image.IsChanged)
                    sChanged = "[Changed] ";

                // show loading time
                string sImageLoadingTime = "";
                if (_imageLoadingTime != TimeSpan.Zero)
                    sImageLoadingTime = string.Format("[Loading time: {0}ms] ", _imageLoadingTime.TotalMilliseconds);

                // show error message if not critical error occurs during image loading
                string sImageLoadingError = "";
                if (image.LoadingError)
                    sImageLoadingError = string.Format("[{0}] ", image.LoadingErrorString);

                // image size (megapixels or gigapixels)
                string sizeInfo;
                float mpx = (float)image.Width * image.Height / (1000f * 1000f);
                if (mpx < 0.01)
                    sizeInfo = (image.Width * image.Height).ToString() + "Px";
                else if (mpx < 10)
                    sizeInfo = mpx.ToString("F2", CultureInfo.InvariantCulture) + "MPx";
                else if (mpx < 1000)
                    sizeInfo = mpx.ToString("F1", CultureInfo.InvariantCulture) + "MPx";
                else
                    sizeInfo = (mpx / 1000f).ToString("F2", CultureInfo.InvariantCulture) + "GPx";

                // show information about the current image
                imageInfoLabel.Text = string.Format("{0}{1}{2} Codec={8}; Width={3}, Height={4} ({5}); PixelFormat={6}; Resolution={7}",
                    sChanged, sImageLoadingTime, sImageLoadingError, image.Width, image.Height,
                    sizeInfo,
                    image.PixelFormat, image.Resolution, GetImageCompression(image));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Returns name of image compression.
        /// </summary>
        /// <param name="image">An image.</param>
        /// <returns>A name of image compression.</returns>
        public static string GetImageCompression(VintasoftImage image)
        {
            string compression = null;
            switch (image.SourceInfo.DecoderName)
            {
                case "Bmp":
                    BmpMetadata bmpMetadata = image.Metadata.MetadataTree as BmpMetadata;
                    if (bmpMetadata != null)
                        compression = bmpMetadata.Compression.ToString();
                    break;

                case "Tiff":
                    TiffPageMetadata tiffMetadata = image.Metadata.MetadataTree as TiffPageMetadata;
                    if (tiffMetadata != null)
                        compression = tiffMetadata.Compression.ToString();
                    break;

#if !REMOVE_PDF_PLUGIN
                case "Pdf":
                    Vintasoft.Imaging.Pdf.Tree.PdfPage page = PdfDocumentController.GetPageAssociatedWithImage(image);
                    if (page.IsImageOnly)
                        compression = page.BackgroundImage.Compression.ToString();
                    break;
#endif
#if !REMOVE_RAW_PLUGIN
                case "Raw":
                    DigitalCameraRawMetadata rawMetadata = image.Metadata.MetadataTree as DigitalCameraRawMetadata;
                    if (rawMetadata != null)
                        compression = rawMetadata.FileFormat.ToString();
                    break;
#endif
                case "Jpeg":
                    JpegMetadata jpegMetadata = image.Metadata.MetadataTree as JpegMetadata;
                    if (jpegMetadata != null)
                        compression = string.Format("Quality {0}", jpegMetadata.Quality.ToString());
                    break;
            }

            if (compression != null)
                return string.Format("{0} ({1})", image.SourceInfo.DecoderName, compression);
            return image.SourceInfo.DecoderName;
        }

        #endregion


        #region Scan image(s)

        /// <summary>
        /// Scans image(s) from scanner.
        /// </summary>
        private void imageViewerToolBar_Scan(object sender, EventArgs e)
        {
            acquireFromScannerMenuItem_Click(imageViewerToolBar, null);
        }

        #endregion


        #region Capture image(s)

        /// <summary>
        /// Captures image(s) from camera.
        /// </summary>
        private void imageViewerToolBar_CaptureFromCamera(object sender, EventArgs e)
        {
            captureFromCameraToolStripMenuItem_Click(imageViewerToolBar, null);
        }

        #endregion


        #region Print image(s)

        /// <summary>
        /// Prints the image(s).
        /// </summary>
        private void printMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = _printManager.PrintDialog;
            printDialog.MinPage = 1;
            printDialog.MaxPage = (uint)_printManager.Images.Count;
            printDialog.UserPageRangeEnabled = true;

            // show print dialog and
            // start print if dialog results is OK
            if (printDialog.ShowDialog().Value)
            {
                try
                {
                    _printManager.Print(this.Title);
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Prints the image(s).
        /// </summary>
        private void imageViewerToolBar_Print(object sender, EventArgs e)
        {
            printMenuItem_Click(imageViewerToolBar, null);
        }

        #endregion


        #region Save image(s)

        /// <summary>
        /// Saves a single image.
        /// </summary>
        private bool SaveSingleImage(
            VintasoftImage image,
            string filename,
            EncoderBase encoder,
            bool saveAndSwitchSource)
        {
            filename = Path.GetFullPath(filename);

            bool result = true;

            //
            this.IsImageSaving = true;

            // save image to file and do not switch source
            encoder.SaveAndSwitchSource = saveAndSwitchSource;

            try
            {
                // save image synchronously
                image.Save(filename, encoder, Images_ImageSavingProgress);
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);

                result = false;
            }

            // if we need to switch source
            if (result && saveAndSwitchSource)
            {
                if (_sourceFilename.ToUpperInvariant() != filename.ToUpperInvariant())
                {
                    _sourceFilename = filename;
                    _isFileReadOnlyMode = false;
                }
            }

            //
            this.IsImageSaving = false;

            return result;
        }

        /// <summary>
        /// Saves an image collection.
        /// </summary>
        private bool SaveImageCollection(
            ImageCollection images,
            string filename,
            EncoderBase encoder,
            bool saveAndSwitchSource)
        {
            bool result = true;

            //
            this.IsImageSaving = true;

            //
            RenderingSettingsWindow.SetRenderingSettingsIfNeed(images, encoder, imageViewer.ImageRenderingSettings);

            // subscribe to the events
            images.ImageCollectionSavingProgress += new EventHandler<ProgressEventArgs>(Images_ImageCollectionSavingProgress);
            images.ImageSavingProgress += new EventHandler<ProgressEventArgs>(Images_ImageSavingProgress);
            images.ImageSavingException += new EventHandler<Vintasoft.Imaging.ExceptionEventArgs>(Images_ImageSavingException);
            images.ImageCollectionSavingFinished += new EventHandler(images_ImageCollectionSavingFinished);

            filename = Path.GetFullPath(filename);

            //
            if (saveAndSwitchSource)
            {
                _saveFilename = filename;
                _encoderName = encoder.Name;
            }
            else
            {
                _saveFilename = null;
                _encoderName = null;
            }

            // save images to file and switch source
            encoder.SaveAndSwitchSource = saveAndSwitchSource;

            try
            {
                // save image collection asynchronously
                images.SaveAsync(filename, encoder);
            }
            catch (Exception ex)
            {
                _saveFilename = null;

                result = false;

                DemosTools.ShowErrorMessage(ex);

                this.IsImageSaving = false;
            }

            return result;
        }

        /// <summary>
        /// Image from image collection is saved.
        /// </summary>
        private void Images_ImageCollectionSavingProgress(object sender, ProgressEventArgs e)
        {
            // if saving of image must be canceled (application is closing OR new image is opening)
            if (_cancelImageSaving)
            {
                // if saving of image can be canceled (decoder can cancel saving of image)
                if (e.CanCancel)
                {
                    // send a request to cancel saving of image
                    e.Cancel = true;
                    return;
                }
            }

            //
            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateImageCollectionSavingProgress(e);
            else
                Dispatcher.Invoke(new UpdateImageCollectionSavingProgressMethod(UpdateImageCollectionSavingProgress), e);
        }

        /// <summary>
        /// Image saving is in progress.
        /// </summary>
        private void Images_ImageSavingProgress(object sender, ProgressEventArgs e)
        {
            // if saving of image must be canceled (application is closing OR new image is opening)
            if (_cancelImageSaving)
            {
                // if saving of image can be canceled (decoder can cancel saving of image)
                if (e.CanCancel)
                    // send a request to cancel saving of image
                    e.Cancel = _cancelImageSaving;
            }

            //
            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateImageSavingProgress(e);
            else
                Dispatcher.Invoke(new UpdateImageSavingProgressMethod(UpdateImageSavingProgress), e);
        }

        /// <summary>
        /// Image collection is NOT saved because of error.
        /// </summary>
        private void Images_ImageSavingException(object sender, Vintasoft.Imaging.ExceptionEventArgs e)
        {
            // do not show error message if application is closing
            if (!_isWindowClosing)
                DemosTools.ShowErrorMessage("Image saving error", e.Exception);

            _saveFilename = null;
            //
            this.IsImageSaving = false;
        }

        /// <summary>
        /// Image saving is in-progress.
        /// </summary>
        private void images_ImageCollectionSavingFinished(object sender, EventArgs e)
        {
            ImageCollection images = (ImageCollection)sender;

            // unsubscribe from the events
            images.ImageCollectionSavingProgress -= new EventHandler<ProgressEventArgs>(Images_ImageCollectionSavingProgress);
            images.ImageSavingProgress -= new EventHandler<ProgressEventArgs>(Images_ImageSavingProgress);
            images.ImageSavingException -= new EventHandler<Vintasoft.Imaging.ExceptionEventArgs>(Images_ImageSavingException);
            images.ImageCollectionSavingFinished -= new EventHandler(images_ImageCollectionSavingFinished);

            // if image collection saved to the source OR
            // image collection must be switched to new source
            if (_saveFilename != null)
            {
                _sourceFilename = _saveFilename;
                _sourceDecoderName = _encoderName;
                _isFileReadOnlyMode = false;

                _saveFilename = null;
                _encoderName = null;
            }

            // saving of images is finished successfully
            this.IsImageSaving = false;
        }

        /// <summary>
        /// Updates status about image collection saving. Thread safe.
        /// </summary>
        private void UpdateImageCollectionSavingProgress(ProgressEventArgs e)
        {
            imageCollectionSavingProgressBar.Value = e.Progress;

            Visibility progressVisibility = Visibility.Collapsed;
            if (e.Progress != 100)
                progressVisibility = Visibility.Visible;

            imageCollectionSavingStatusLabel.Visibility = progressVisibility;
            imageCollectionSavingProgressBar.Visibility = progressVisibility;
        }

        /// <summary>
        /// Updates status of image saving. Thread safe.
        /// </summary>
        private void UpdateImageSavingProgress(ProgressEventArgs e)
        {
            imageSavingProgressBar.Value = e.Progress;

            Visibility progressVisibility = Visibility.Collapsed;
            if (e.Progress != 100)
                progressVisibility = Visibility.Visible;

            imageSavingStatusLabel.Visibility = progressVisibility;
            imageSavingProgressBar.Visibility = progressVisibility;
        }

        #endregion


        #region Image processing

        /// <summary>
        /// Inverts the color channel of the image.
        /// </summary>
        private void InvertColorChannel(System.Drawing.Color color)
        {
            ColorBlendCommand command = new ColorBlendCommand(BlendingMode.Difference, color);
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(command);
        }

        /// <summary>
        /// Extracts color channels of the image.
        /// </summary>
        private void ExtractColorChannels(int rMask, int gMask, int bMask)
        {
            System.Drawing.Color blendingColor = System.Drawing.Color.FromArgb(rMask, gMask, bMask);
            ColorBlendCommand command = new ColorBlendCommand(BlendingMode.Darken, blendingColor);
            _imageProcessingCommandExecutor.ExecuteProcessingCommand(command);
        }

        /// <summary>
        /// Image processing is started.
        /// </summary>
        void _imageProcessingCommandExecutor_ImageProcessingCommandStarted(object sender, ImageProcessingEventArgs e)
        {

            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                this.Cursor = Cursors.Wait;
                this.IsImageProcessing = true;
            }
            else
                Dispatcher.Invoke(new ImageProcessingEventHandlerDelegate(_imageProcessingCommandExecutor_ImageProcessingCommandStarted), sender, e);
        }

        /// <summary>
        /// Image processing is in progress.
        /// </summary>
        void _imageProcessingCommandExecutor_ImageProcessingCommandProgress(object sender, ImageProcessingProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                _isEscPressed = false;
            }
            else if (e.CanCancel)
            {
                if (_isEscPressed || _isWindowClosing)
                {
                    e.Cancel = true;
                    _isEscPressed = false;
                }
            }

            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateImageProcessingProgress(sender, e);
            else
                Dispatcher.Invoke(new UpdateImageProcessingProgressMethod(UpdateImageProcessingProgress), sender, e);

        }

        /// <summary>
        /// Image processing is finished.
        /// </summary>
        void _imageProcessingCommandExecutor_ImageProcessingCommandFinished(object sender, ImageProcessedEventArgs e)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
            {
                this.Cursor = Cursors.Arrow;
                this.IsImageProcessing = false;
            }
            else
                Dispatcher.Invoke(new ImageProcessedEventHandlerDelegate(_imageProcessingCommandExecutor_ImageProcessingCommandFinished), sender, e);
        }

        /// <summary>
        /// Updates the "Image processing" progress info. Thread safe.
        /// </summary>
        private void UpdateImageProcessingProgress(object sender, ImageProcessingProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                imageProcessingProgressBar.Visibility = Visibility.Collapsed;

                TimeSpan imageProcessingTime = DateTime.Now - _imageProcessingCommandExecutor.ProcessingCommandStartTime;
                imageProcessingStatusLabel.Content = string.Format("{0}: {1} ms", imageProcessingStatusLabel.Content, imageProcessingTime.TotalMilliseconds);
                imageProcessingStatusLabel.Visibility = Visibility.Visible;
            }
            else
            {
                ProcessingCommandBase processingCommand = sender as ProcessingCommandBase;
                if (processingCommand is ParallelizingProcessingCommand)
                {
                    ParallelizingProcessingCommand parallelizingCommand = (ParallelizingProcessingCommand)processingCommand;
                    imageProcessingStatusLabel.Content = parallelizingCommand.ProcessingCommand.Name + " (parallelized)";
                }
                else
                    imageProcessingStatusLabel.Content = processingCommand.Name;

                imageProcessingProgressBar.Value = e.Progress;
                imageProcessingProgressBar.Visibility = Visibility.Visible;
            }
        }

        #endregion


        #region Copy, paste image

        /// <summary>
        /// Returns a value, which determines that image viewer has custom selection.
        /// </summary>
        /// <returns>
        /// <b>true</b> - if image viewer has custom selection; otherwise - <b>false</b>.
        /// </returns>
        private bool HasCustomSelection()
        {
            WpfCustomSelectionTool customSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);
            if (customSelectionTool != null)
            {
                if (customSelectionTool.Selection != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a value, which determines that image viewer has rectangular selection.
        /// </summary>
        /// <returns>
        /// <b>true</b> - if image viewer has rectangular selection; otherwise - <b>false</b>.
        /// </returns>
        private bool HasRectangularSelection()
        {
            WpfRectangularSelectionTool rectangularSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(imageViewer.VisualTool);
            if (rectangularSelectionTool != null)
            {
                if (rectangularSelectionTool.Rectangle != Rect.Empty)
                    return true;
            }
            return false;
        }


        #region CopyMethods

        /// <summary>
        /// Copies an image from image viewer to the clipboard.
        /// </summary>
        private void CopyImageFromImageViewer()
        {
            VintasoftImage image = null;
            try
            {
                // set rendering settings
                RenderingSettingsWindow.SetRenderingSettingsIfNeed(imageViewer.Image, null, imageViewer.ImageRenderingSettings);

                // get image
                image = imageViewer.Image;
                // if image pixel format is Gray16
                if (image.PixelFormat == Vintasoft.Imaging.PixelFormat.Gray16)
                {
                    // change pixel format to Gray8
                    ChangePixelFormatToGrayscaleCommand command = new ChangePixelFormatToGrayscaleCommand(Vintasoft.Imaging.PixelFormat.Gray8);
                    image = command.Execute(image);
                }

                // copy image to the clipboard
                Clipboard.SetImage(VintasoftImageConverter.ToBitmapSource(image));

                // update user interface
                UpdateUI();
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
            finally
            {
                // if image is NOT empty AND image is not equal to the image in viewer
                if (image != null && image != imageViewer.Image)
                {
                    // dispose image
                    image.Dispose();
                }
            }
        }

        /// <summary>
        /// Copies an image from custom selection to the clipboard.
        /// </summary>
        private void CopyImageFromCustomSelection()
        {
            // if current tool contains custom selection tool with selection
            if (HasCustomSelection())
            {
                try
                {
                    // get custom selection tool
                    WpfCustomSelectionTool customSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);

                    // get selection
                    WpfSelectionRegionBase selection = customSelectionTool.Selection;

                    VintasoftImage clipboardImage;

                    // get selection as graphics path
                    using (System.Drawing.Drawing2D.GraphicsPath path = WpfObjectConverter.CreateDrawingGraphicsPath(selection.GetAsPathGeometry()))
                    {
                        // get bounding box
                        System.Drawing.RectangleF bounds = path.GetBounds();
                        if (bounds.Width <= 0 && bounds.Height <= 0)
                            return;

                        // get image viewer rectangle
                        System.Drawing.RectangleF viewerImageRect =
                            new System.Drawing.RectangleF(0, 0, imageViewer.Image.Width, imageViewer.Image.Height);
                        // get copy rectangle
                        System.Drawing.RectangleF viewerCopyRect = System.Drawing.RectangleF.Intersect(bounds, viewerImageRect);
                        if (viewerCopyRect.Width <= 0 || viewerCopyRect.Height <= 0)
                            return;

                        // get image from rectangle
                        using (VintasoftImage image = imageViewer.GetFocusedImageRect(WpfObjectConverter.CreateWindowsRect(viewerCopyRect)))
                        {
                            if (image == null)
                                return;

                            // create a composite command, which will clear image, overlay image with path and crop the image
                            CompositeCommand compositeCommand = new CompositeCommand(
                                new ClearImageCommand(System.Drawing.Color.Transparent),
                                new ProcessPathCommand(new OverlayCommand(image), new GdiGraphicsPath(path, false)),
                                new CropCommand(System.Drawing.Rectangle.Round(viewerCopyRect)));
                            // apply the composite command to the iamge and get the result image
                            clipboardImage = compositeCommand.Execute(imageViewer.Image);
                        }
                    }

                    // copy to the clipboard
                    Clipboard.SetImage(VintasoftImageConverter.ToBitmapSource(clipboardImage));
                    clipboardImage.Dispose();
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
            // update user interface
            UpdateUI();
        }

        /// <summary>
        /// Copies an image from rectangular selection to the clipboard.
        /// </summary>
        private void CopyImageFromRectangularSelection()
        {
            // get rectangular selection tool
            WpfRectangularSelectionToolWithCopyPaste rectSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(imageViewer.VisualTool);

            // if rectangular selection tool exists
            if (rectSelectionTool != null)
            {
                try
                {
                    // copy selection to the clipboard
                    rectSelectionTool.CopyToClipboard();
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Copies an image from thumbnail to the clipboard.
        /// </summary>
        private void CopyImageFromThumbnailViewer()
        {
            thumbnailViewer.DoCopy();
        }

        /// <summary>
        /// Copies an image to the clipboard.
        /// </summary>
        private void CopyImageToClipboard()
        {
            if (thumbnailViewer.IsFocused)
            {
                CopyImageFromThumbnailViewer();
            }
            else if (HasCustomSelection())
            {
                CopyImageFromCustomSelection();
            }
            else if (HasRectangularSelection())
            {
                CopyImageFromRectangularSelection();
            }
            else
            {
                CopyImageFromImageViewer();
            }
        }

        #endregion


        #region PasteMethods

        /// <summary>
        /// Pastes an image from clipboard to the image viewer.
        /// </summary>
        private void PasteImageToImageViewer()
        {
            // if clipboard does NOT contain an image
            if (!Clipboard.ContainsImage())
                return;

            try
            {
                VintasoftImage image = VintasoftImageConverter.FromBitmapSource(Clipboard.GetImage());
                // if image viewer has focused image
                if (imageViewer.FocusedIndex != -1)
                {
                    // change the focused image to the image from clipboard
                    imageViewer.Image.SetImage(image);
                }
                // if image viewer does NOT have focused image
                else
                {
                    // add image from clipboard as new image
                    imageViewer.Images.Add(image);
                }
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Pastes an image from clipboard to the custom selection.
        /// </summary>
        private void PasteImageToCustomSelection()
        {
            // if clipboard does not contain image
            if (!Clipboard.ContainsImage())
                return;

            // if current tool contains custom selection tool
            // with selection
            if (HasCustomSelection())
            {
                try
                {
                    // get custom selection tool
                    WpfCustomSelectionTool customSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(imageViewer.VisualTool);

                    // get image
                    VintasoftImage source = imageViewer.Images[imageViewer.FocusedIndex];
                    if (source == null)
                        return;

                    // get selection
                    WpfSelectionRegionBase selection = customSelectionTool.Selection;
                    if (selection == null)
                        return;

                    // get region to paste
                    Rect rect = selection.GetBoundingBox();
                    int left = (int)Math.Floor(rect.Left);
                    int top = (int)Math.Floor(rect.Top);
                    int width = (int)Math.Ceiling(rect.Width);
                    int height = (int)Math.Ceiling(rect.Height);
                    if (width <= 0 || height <= 0)
                        return;

                    if (left >= source.Width || top >= source.Height || left + width <= 0 || top + height <= 0)
                        return;

                    if (left < 0)
                    {
                        width += left;
                        left = 0;
                    }
                    if (top < 0)
                    {
                        height += top;
                        top = 0;
                    }
                    if (left + width > source.Width)
                        width = source.Width - left;
                    if (top + height > source.Height)
                        height = source.Height - top;
                    System.Drawing.Drawing2D.GraphicsPath path =
                        WpfObjectConverter.CreateDrawingGraphicsPath(customSelectionTool.Selection.GetAsPathGeometry());
                    System.Drawing.RectangleF pathBounds = path.GetBounds();
                    if (pathBounds.Width <= 0 && pathBounds.Height <= 0)
                        return;

                    // get image from clipboard
                    using (VintasoftImage imageFromClipboard = VintasoftImageConverter.FromBitmapSource(Clipboard.GetImage()))
                    {
                        if (imageFromClipboard.Width != width || imageFromClipboard.Height != height)
                        {
                            // resize image
                            ResizeCommand resizeCommand = new ResizeCommand(width, height);
                            resizeCommand.ExecuteInPlace(imageFromClipboard);
                        }

                        // paste image from clipboard

                        OverlayCommand overlayCommand = new OverlayCommand(imageFromClipboard);
                        // overlay with path command
                        ProcessPathCommand overlayWithPath = new ProcessPathCommand(overlayCommand, new GdiGraphicsPath(path, false));
                        overlayWithPath.ExecuteInPlace(source);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Pastes an image from clipboard to the rectangular selection.
        /// </summary>
        private void PasteImageToRectangularSelection()
        {
            // if clipboard does not contain image
            if (!Clipboard.ContainsImage())
                return;

            // get rectangular selection tool
            WpfRectangularSelectionToolWithCopyPaste rectSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(imageViewer.VisualTool);

            // if rectangular selection tool is not empty
            if (rectSelectionTool != null)
            {
                try
                {
                    // paste image from clipboard
                    rectSelectionTool.PasteFromClipboard();
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Pastes an image from clipboard to the thumbnail viewer.
        /// </summary>
        private void PasteImageToThumbnailViewer()
        {
            // if clipboard does NOT contain an image
            if (!Clipboard.ContainsImage())
                return;

            try
            {
                VintasoftImage image = VintasoftImageConverter.FromBitmapSource(Clipboard.GetImage());
                // if thumbnail viewer has focused image
                if (thumbnailViewer.FocusedIndex != -1)
                {
                    // change the focused image to the image from clipboard
                    thumbnailViewer.Images[thumbnailViewer.FocusedIndex].SetImage(image);
                }
                // if thumbnail viewer does NOT have focused image
                else
                {
                    // add image from clipboard as new image
                    thumbnailViewer.Images.Add(image);
                }
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Pastes an image from clipboard.
        /// </summary>
        private void PasteImageFromClipboard()
        {
            if (thumbnailViewer.IsFocused)
            {
                PasteImageToThumbnailViewer();
            }
            if (HasCustomSelection())
            {
                PasteImageToCustomSelection();
            }
            else if (HasRectangularSelection())
            {
                PasteImageToRectangularSelection();
            }
            else
            {
                PasteImageToImageViewer();
            }
        }

        /// <summary>
        /// Inserts image from clipboard.
        /// </summary>
        private void InsertKeyPressed()
        {
            // if clipboard contains the image
            if (Clipboard.ContainsImage())
            {
                PasteImageFromClipboard();
            }
        }

        #endregion

        #endregion


        #region Measurement UI actions

        /// <summary>
        /// Updates the UI action items in "Edit" menu.
        /// </summary>
        private void UpdateEditUIActionMenuItems()
        {
            UpdateEditUIActionMenuItem(cutMenuItem, DemosTools.GetUIAction<CutItemUIAction>(imageViewer.VisualTool), "Cut Measurement");
            UpdateEditUIActionMenuItem(copyMenuItem, DemosTools.GetUIAction<CopyItemUIAction>(imageViewer.VisualTool), "Copy Measurement");
            UpdateEditUIActionMenuItem(pasteMenuItem, DemosTools.GetUIAction<PasteItemUIAction>(imageViewer.VisualTool), "Paste Measurement");
            UpdateEditUIActionMenuItem(deleteMenuItem, DemosTools.GetUIAction<DeleteItemUIAction>(imageViewer.VisualTool), "Delete Measurement");
            UpdateEditUIActionMenuItem(deleteAllMenuItem, DemosTools.GetUIAction<DeleteAllItemsUIAction>(imageViewer.VisualTool), "Delete All Measurements");
        }

        /// <summary>
        /// Updates the UI action item in "Edit" menu.
        /// </summary>
        /// <param name="menuItem">The "Edit" menu item.</param>
        /// <param name="uiAction">The UI action, which is associated with the "Edit" menu item.</param>
        /// <param name="defaultText">The default text for the "Edit" menu item.</param>
        private void UpdateEditUIActionMenuItem(MenuItem editMenuItem, UIAction uiAction, string defaultText)
        {
            // if UI action is specified AND UI action is enabled
            if (uiAction != null && uiAction.IsEnabled)
            {
                // enable the menu item
                editMenuItem.IsEnabled = true;
                // set text to the menu item
                editMenuItem.Header = uiAction.Name;
            }
            else
            {
                // disable the menu item
                editMenuItem.IsEnabled = false;
                // set the default text to the menu item
                editMenuItem.Header = defaultText;
            }
        }

        #endregion


        #region Image processing history (undo/redo)

        /// <summary>
        /// Clears the image processing history.
        /// </summary>
        private void ClearHistory()
        {
            _undoManager.Clear();
        }

        /// <summary>
        /// Creates the undo manager.
        /// </summary>
        /// <param name="keepUndoForCurrentImageOnly">Determines that undo information for all images must be kept.</param>
        private void CreateUndoManager(bool keepUndoForCurrentImageOnly)
        {
            DisposeUndoManager();

            if (keepUndoForCurrentImageOnly)
                _undoManager = new UndoManager();
            else
                _undoManager = new CompositeUndoManager();
            _undoManager.UndoLevel = _undoLevel;
            _undoManager.DataStorage = _dataStorage;
            _undoManager.IsEnabled = enableUndoRedoMenuItem.IsChecked;

            _undoManager.Changed += new EventHandler<UndoManagerChangedEventArgs>(undoManager_Changed);
            _undoManager.Navigated += new EventHandler<UndoManagerNavigatedEventArgs>(undoManager_Navigated);

            _imageProcessingCommandExecutor.UndoManager = _undoManager;

            _imageViewerUndoMonitor = new WpfImageViewerUndoMonitor(_undoManager, imageViewer);
            _imageViewerUndoMonitor.ShowHistoryForDisplayedImages =
                showHistoryForDisplayedImagesMenuItem.IsChecked;

            if (_historyWindow != null)
                _historyWindow.UndoManager = _undoManager;
        }

        /// <summary>
        /// Disposes the undo manager.
        /// </summary>
        private void DisposeUndoManager()
        {
            if (_undoManager == null)
                return;

            _imageProcessingCommandExecutor.UndoManager = null;

            if (_imageViewerUndoMonitor != null)
                _imageViewerUndoMonitor.Dispose();

            _undoManager.Changed -= undoManager_Changed;
            _undoManager.Navigated -= undoManager_Navigated;
            _undoManager.Dispose();
            _undoManager = null;
        }

        /// <summary>
        /// Current history manager is changed.
        /// </summary>
        private void undoManager_Changed(object sender, UndoManagerChangedEventArgs e)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateUndoRedoMenu((UndoManager)sender);
            else
                Dispatcher.Invoke(new UpdateUndoRedoMenuDelegate(UpdateUndoRedoMenu), sender);
        }

        /// <summary>
        /// Current action in history of current history manager is changed.
        /// </summary>
        private void undoManager_Navigated(object sender, UndoManagerNavigatedEventArgs e)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateUndoRedoMenu((UndoManager)sender);
            else
                Dispatcher.Invoke(new UpdateUndoRedoMenuDelegate(UpdateUndoRedoMenu), sender);
        }

        /// <summary>
        /// Updates the "Undo/Redo" menu.
        /// </summary>
        private void UpdateUndoRedoMenu(UndoManager undoManager)
        {
            if (undoManager == null)
            {
                undoMenuItem.IsEnabled = false;
                undoMenuItem.Header = "Undo";
                redoMenuItem.IsEnabled = false;
                redoMenuItem.Header = "Redo";
            }
            else
            {
                undoMenuItem.IsEnabled = undoManager.UndoCount > 0;
                undoMenuItem.Header = string.Format("Undo {0}", undoManager.UndoDescription).Trim();

                redoMenuItem.IsEnabled = undoManager.RedoCount > 0;
                redoMenuItem.Header = string.Format("Redo {0}", undoManager.RedoDescription).Trim();
            }

            keepUndoForCurrentImageOnlyMenuItem.IsEnabled = enableUndoRedoMenuItem.IsEnabled;
            keepUndoForCurrentImageOnlyMenuItem.IsChecked = _keepUndoForCurrentImageOnly;
            undoRedoSettingsMenuItem.IsEnabled = enableUndoRedoMenuItem.IsEnabled;

            if (IsImageProcessing || IsImageSaving || !IsImageLoaded)
            {
                undoMenuItem.IsEnabled = false;
                redoMenuItem.IsEnabled = false;
                undoRedoSettingsMenuItem.IsEnabled = false;
            }
        }

        /// <summary>
        /// Shows the window of image processing history.
        /// </summary>
        private void ShowHistoryWindow()
        {
            if (imageViewer.Image == null)
                return;

            _historyWindow = new WpfUndoManagerHistoryWindow(this, _undoManager);
            _historyWindow.Closed += new EventHandler(UndoManagerHistoryWindow_Closed);
            _historyWindow.Show();
        }

        /// <summary>
        /// Closes the window of image processing history.
        /// </summary>
        private void CloseHistoryWindow()
        {
            if (_historyWindow != null)
            {
                _historyWindow.Close();
                _historyWindow = null;
            }
        }

        /// <summary>
        /// Image processing history window is closed.
        /// </summary>
        private void UndoManagerHistoryWindow_Closed(object sender, EventArgs e)
        {
            historyDialogMenuItem.IsChecked = false;
        }

        #endregion


        #region Access to image pixels

        /// <summary>
        /// Opens a window for direct access to the image pixels.
        /// </summary>
        private void OpenDirectPixelAccessWindow()
        {
            if (_directPixelAccessWindow == null)
            {
                _directPixelAccessWindow = new WpfDirectPixelAccessWindow(imageViewer);
                _directPixelAccessWindow.Owner = this;
                _directPixelAccessWindow.Closed += new EventHandler(_editImagePixelsWindow_Closed);
            }

            _directPixelAccessWindow.Show();
        }

        /// <summary>
        /// Closes a window for direct access to the image pixels.
        /// </summary>
        private void CloseDirectPixelAccessWindow()
        {
            if (_directPixelAccessWindow != null)
            {
                _directPixelAccessWindow.Closed -= new EventHandler(_editImagePixelsWindow_Closed);
                _directPixelAccessWindow.Close();
                _directPixelAccessWindow = null;
            }
        }

        /// <summary>
        /// A direct pixel access window is closed.
        /// </summary>
        private void _editImagePixelsWindow_Closed(object sender, EventArgs e)
        {
            editImagePixelsMenuItem.IsChecked = false;
        }

        #endregion


        #region Hot keys

        /// <summary>
        /// Handles the CanExecute event of OpenCommandBinding object.
        /// </summary>
        private void openCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = openMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of AddCommandBinding object.
        /// </summary>
        private void addCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = addMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of SaveAsCommandBinding object.
        /// </summary>
        private void saveAsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = saveAsMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of CloseCommandBinding object.
        /// </summary>
        private void closeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = closeMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of PrintCommandBinding object.
        /// </summary>
        private void printCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = printMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of ExitCommandBinding object.
        /// </summary>
        private void exitCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = exitMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of RotateClockwiseCommandBinding object.
        /// </summary>
        private void rotateClockwiseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rotateClockwiseMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of RotateCounterclockwiseCommandBinding object.
        /// </summary>
        private void rotateCounterclockwiseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rotateCounterclockwiseMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of UndoCommandBinding object.
        /// </summary>
        private void undoCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of RedoCommandBinding object.
        /// </summary>
        private void redoCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = redoMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of CopyImageCommandBinding object.
        /// </summary>
        private void copyImageCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = copyImageMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of PasteImageCommandBinding object.
        /// </summary>
        private void pasteImageCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = pasteImageMenuItem.IsEnabled;
        }

        #endregion

        #endregion



        #region Delegates

        private delegate void UpdateUIDelegate();

        private delegate void SetIsFileOpeningDelegate(bool isFileOpening);

        private delegate void SetAddingFilenameDelegate(string filename);

        private delegate void CloseCurrentFileDelegate();

        //
        private delegate void UpdateImageCollectionSavingProgressMethod(ProgressEventArgs e);
        //
        private delegate void UpdateImageSavingProgressMethod(ProgressEventArgs e);

        //
        private delegate void UpdateImageProcessingProgressMethod(object sender, ImageProcessingProgressEventArgs e);

        //
        private delegate void ImageProcessingEventHandlerDelegate(object sender, ImageProcessingEventArgs e);
        private delegate void ImageProcessedEventHandlerDelegate(object sender, ImageProcessedEventArgs e);

        delegate void UpdateUndoRedoMenuDelegate(UndoManager undoManager);

        #endregion

    }
}
