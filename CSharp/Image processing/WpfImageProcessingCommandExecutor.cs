using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows;

using Microsoft.Win32;

using Vintasoft.Data;
using Vintasoft.Imaging;
using Vintasoft.Imaging.Drawing;
using Vintasoft.Imaging.Drawing.Gdi;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.ImageProcessing.Document;
using Vintasoft.Imaging.ImageProcessing.Info;
using Vintasoft.Imaging.ImageProcessing.Transforms;
using Vintasoft.Imaging.Undo;

using Vintasoft.Imaging.Wpf;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools;

using WpfDemosCommonCode;
using WpfDemosCommonCode.Imaging;
using WpfDemosCommonCode.Imaging.Codecs;


namespace WpfImagingDemo
{
    internal class WpfImageProcessingCommandExecutor
    {

        #region Nested classes

        /// <summary>
        /// Image processing command task.
        /// </summary>
        class ProcessingCommandTask
        {

            #region Fields

            ProcessingCommandBase _command;
            VintasoftImage _image;

            #endregion


            #region Constructor

            public ProcessingCommandTask(ProcessingCommandBase command, VintasoftImage image)
            {
                _command = command;
                _image = image;
            }

            #endregion


            #region Methods

            /// <summary>
            /// Executes image processing.
            /// </summary>
            public void Execute()
            {
                try
                {
                    // execute image processing command
                    _command.ExecuteInPlace(_image);
                    // previous image will be disposed automatically
                }
                catch (Exception ex)
                {
                    if (ImageProcessingExceptionOccurs != null)
                        ImageProcessingExceptionOccurs(this, EventArgs.Empty);

                    DemosTools.ShowErrorMessage("Image processing exception", ex);
                }
            }

            #endregion


            #region Events

            /// <summary>
            /// Occurs if exception occurs during image processing.
            /// </summary>
            public event EventHandler ImageProcessingExceptionOccurs;

            #endregion

        }

        #endregion



        #region Fields

        /// <summary>
        /// Image viewer that shows the image.
        /// </summary>
        WpfImageViewer _viewer;

        /// <summary>
        /// Open file dialog for SetAlphaChannelMaskCommand.
        /// </summary>
        OpenFileDialog _openFileDialog;

        /// <summary>
        /// Blending mode for ColorBlendCommand.
        /// </summary>
        BlendingMode _blendMode = BlendingMode.Multiply;

        /// <summary>
        /// Blending color for ColorBlendCommand.
        /// </summary>
        System.Drawing.Color _blendColor = System.Drawing.Color.LightGreen;

        /// <summary>
        /// Overlay image for OverlayCommand, OverlayWithBlendingCommand and OverlayMaskedCommand.
        /// </summary>
        VintasoftImage _overlayImage = null;
        /// <summary>
        /// Mask image for OverlayCommand, OverlayWithBlendingCommand and OverlayMaskedCommand.
        /// </summary>
        VintasoftImage _maskImage = null;

        /// <summary>
        /// Comparison image for ImageComparisonCommand.
        /// </summary>
        VintasoftImage _comparisonImage = null;

        /// <summary>
        /// The image processing command undo monitor.
        /// </summary>
        ImageProcessingUndoMonitor _imageProcessingUndoMonitor = null;

        #endregion



        #region Constructors

        public WpfImageProcessingCommandExecutor(WpfImageViewer viewer)
        {
            _viewer = viewer;

            _openFileDialog = new OpenFileDialog();
            CodecsFileFilters.SetFilters(_openFileDialog);
        }

        #endregion



        #region Properties

        bool _isImageProcessingWorking = false;
        /// <summary>
        /// Gets the value indicating whether the image is processing.
        /// </summary>
        public bool IsImageProcessingWorking
        {
            get
            {
                return _isImageProcessingWorking;
            }
        }

        bool _executeMultithread = false;
        /// <summary>
        /// Gets the value indicating whether the processing command must be executed
        /// in multiple threads.
        /// </summary>
        public bool ExecuteMultithread
        {
            get
            {
                return _executeMultithread;
            }
            set
            {
                _executeMultithread = value;
            }
        }

        DateTime _processingCommandStartTime;
        /// <summary>
        /// Gets the time when image processing is started.
        /// </summary>
        public DateTime ProcessingCommandStartTime
        {
            get
            {
                return _processingCommandStartTime;
            }
        }

        UndoManager _undoManager;
        /// <summary>
        /// Gets or sets the undo manager associated with processing image.
        /// </summary>
        public UndoManager UndoManager
        {
            get
            {
                return _undoManager;
            }
            set
            {
                _undoManager = value;
            }
        }

        bool _expandSupportedPixelFormats = true;
        /// <summary>
        /// Gets or sets a value indicating whether the processing command need to
        /// convert the processing image to the nearest pixel format without color loss
        /// if processing command does not support pixel format
        /// of the processing image.
        /// </summary>
        public bool ExpandSupportedPixelFormats
        {
            get
            {
                return _expandSupportedPixelFormats;
            }
            set
            {
                _expandSupportedPixelFormats = value;
            }
        }


        /// <summary>
        /// Gets the selection rectangle of viewer.
        /// </summary>
        Rect ViewerSelectionRectangle
        {
            get
            {
                Rect selectionRect = Rect.Empty;
                WpfRectangularSelectionToolWithCopyPaste selection = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(_viewer.VisualTool);
                if (selection != null)
                    selectionRect = selection.Rectangle;
                return selectionRect;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Crops focuse image of image viewer by specified path.
        /// </summary>
        /// <param name="viewer">The image viewer.</param>
        /// <param name="path">The path to crop.</param>
        /// <returns>Cropped image.</returns>
        public static VintasoftImage CropFocusedImage(WpfImageViewer viewer, IGraphicsPath path)
        {
            // get bounding box
            Rectangle bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 && bounds.Height <= 0)
                return null;

            // get image viewer rectangle
            Rectangle viewerImageRect = new Rectangle(0, 0, viewer.Image.Width, viewer.Image.Height);
            // get copy rectangle
            Rectangle viewerCopyRect = Rectangle.Intersect(bounds, viewerImageRect);

            if (viewerCopyRect.Width <= 0 || viewerCopyRect.Height <= 0)
                return null;

            // get image rect
            using (VintasoftImage image = viewer.GetFocusedImageRect(new Rect(viewerCopyRect.X, viewerCopyRect.Y, viewerCopyRect.Width, viewerCopyRect.Height)))
            {
                if (image == null)
                    return null;

                // create result image
                VintasoftImage cropImage = new VintasoftImage(image.Width, image.Height, PixelFormat.Bgra32);
                cropImage.Clear(Color.Transparent);
                cropImage.Resolution = viewer.Image.Resolution;

                // overlay uses path
                ProcessPathCommand processPathCommand = new ProcessPathCommand(new OverlayCommand(image), path);
                processPathCommand.PathTransform = AffineMatrix.CreateTranslation(-viewerCopyRect.X, -viewerCopyRect.Y);
                processPathCommand.ExecuteInPlace(cropImage);

                return cropImage;
            }
        }

        /// <summary>
        /// Executes image processing command asynchronously.
        /// </summary>
        public void ExecuteProcessingCommand(ProcessingCommandBase command)
        {
            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes image processing command synchronously or asynchronously.
        /// </summary>
        public bool ExecuteProcessingCommand(ProcessingCommandBase command, bool async)
        {
            WpfRectangularSelectionToolWithCopyPaste rectSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfRectangularSelectionToolWithCopyPaste>(_viewer.VisualTool);
            WpfCustomSelectionTool customSelectionTool = WpfCompositeVisualTool.FindVisualTool<WpfCustomSelectionTool>(_viewer.VisualTool);

            if (rectSelectionTool != null)
            {
                // set the region of interest
                Rect selectionRectangle = ViewerSelectionRectangle;
                ProcessingCommandWithRegion commandWorkWithRegion = command as ProcessingCommandWithRegion;
                if (!selectionRectangle.IsEmpty && commandWorkWithRegion != null)
                {
                    commandWorkWithRegion.RegionOfInterest = new RegionOfInterest((int)selectionRectangle.Left,
                        (int)selectionRectangle.Top, (int)selectionRectangle.Width, (int)selectionRectangle.Height);
                }
            }
            else if (customSelectionTool != null)
            {
                // process custom selection
                Rect selectionBBox = Rect.Empty;
                if (customSelectionTool.Selection != null)
                    selectionBBox = customSelectionTool.Selection.GetBoundingBox();
                if (selectionBBox.Width >= 1 && selectionBBox.Height >= 1)
                {
                    if (!(command is ChangePixelFormatCommand) &&
                        !(command is ChangePixelFormatToBgrCommand) &&
                        !(command is ChangePixelFormatToBlackWhiteCommand) &&
                        !(command is ChangePixelFormatToGrayscaleCommand) &&
                        !(command is ChangePixelFormatToPaletteCommand) &&
                        !(command is RotateCommand) &&
                        !(command is ResampleCommand) &&
                        !(command is ResizeCommand) &&
                        command.CanModifyImage)
                    {
                        GraphicsPath path = WpfObjectConverter.CreateDrawingGraphicsPath(customSelectionTool.Selection.GetAsPathGeometry());
                        RectangleF pathBounds = path.GetBounds();
                        if (pathBounds.Width > 0 && pathBounds.Height > 0)
                        {
                            if (command is CropCommand)
                            {
                                // clear selection
                                customSelectionTool.Selection = null;
                                // crop to custom selection
                                VintasoftImage croppedImage = CropFocusedImage(_viewer, new Vintasoft.Imaging.Drawing.Gdi.GdiGraphicsPath(path, false));
                                if (croppedImage == null)
                                    return false;
                                _viewer.Image.SetImage(croppedImage);
                                return true;
                            }
                         
                            // process path
                            command = new ProcessPathCommand(command, new GdiGraphicsPath(path, false));
                        }
                        else
                        {
                            MessageBox.Show("Selected path is empty.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return false;
                        }
                    }
                }
            }

            // get a reference to the image for processing
            VintasoftImage imageToProcess = _viewer.Image;
            ProcessingCommandBase executeCommand = command;

            if (_executeMultithread)
            {
                ParallelizingProcessingCommand parallelizingCommand = new ParallelizingProcessingCommand(command);
                if (command is ProcessingCommandWithRegion)
                    parallelizingCommand.RegionOfInterest = ((ProcessingCommandWithRegion)command).RegionOfInterest;
                executeCommand = parallelizingCommand;
            }

            if (UndoManager != null)
                _imageProcessingUndoMonitor = new ImageProcessingUndoMonitor(UndoManager, executeCommand);

            // subscribe to the events of the image processing command
            executeCommand.Started += new EventHandler<ImageProcessingEventArgs>(command_Started);
            executeCommand.Progress += new EventHandler<ImageProcessingProgressEventArgs>(command_Progress);
            executeCommand.Finished += new EventHandler<ImageProcessedEventArgs>(command_Finished);

            executeCommand.ExpandSupportedPixelFormats = ExpandSupportedPixelFormats;
            executeCommand.RestoreSourcePixelFormat = false;

            // specify that image processing command is working (several commands cannot work together)
            _isImageProcessingWorking = true;
            // get the start time of the image processing command
            _processingCommandStartTime = DateTime.Now;

            // if image processing command should be executed asynchronously
            if (async)
            {
                // start the image processing command asynchronously
                ProcessingCommandTask executor = new ProcessingCommandTask(executeCommand, imageToProcess);
                executor.ImageProcessingExceptionOccurs += new EventHandler(executor_ImageProcessingExceptionOccurs);
                Thread thread = new Thread(executor.Execute);
                thread.IsBackground = true;
                thread.Start();
            }
            // if image processing command should be executed synchronously
            else
            {
                try
                {
                    // execute the image processing command synchronously
                    executeCommand.ExecuteInPlace(imageToProcess);
                }
                catch (ImageProcessingException ex)
                {
                    DemosTools.ShowErrorMessage("Image processing exception", ex);
                    return false;
                }
            }

            return true;
        }


        #region Commands

        /// <summary>
        /// Executes the ImageComparisonCommand command.
        /// </summary>
        public void ExecuteComparisonCommand()
        {
            // create the processing command
            ImageComparisonCommand command = new ImageComparisonCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Image Comparison Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            // save a reference to the comparison image, image will be disposed when command is finished
            _comparisonImage = command.Image;

            // execute the command
            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes the OverlayCommand command.
        /// </summary>
        public void ExecuteOverlayCommand()
        {
            // create the processing command
            OverlayCommand command = new OverlayCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Overlay Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            // save a reference to the overlay image, image will be disposed when command is finished
            _overlayImage = command.OverlayImage;

            // execute the command
            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes the OverlayBinaryCommand command.
        /// </summary>
        public void ExecuteOverlayBinaryCommand()
        {
            // create the processing command
            OverlayBinaryCommand command = new OverlayBinaryCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Overlay Binary Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            // save a reference to the overlay image, image will be disposed when command is finished
            _overlayImage = command.OverlayImage;

            // execute the command
            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes the OverlayCommand command.
        /// </summary>
        public void ExecuteOverlayWithBlendingCommand()
        {
            // create the processing command
            OverlayWithBlendingCommand command = new OverlayWithBlendingCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Overlay with Blending Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            // save a reference to the overlay image, image will be disposed when command is finished
            _overlayImage = command.OverlayImage;

            // execute the command
            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes the OverlayCommand command.
        /// </summary>
        public void ExecuteOverlayMaskedCommand()
        {
            // create the processing command
            OverlayMaskedCommand command = new OverlayMaskedCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Overlay Masked Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            // save a reference to the overlay image, image will be disposed when command is finished
            _overlayImage = command.OverlayImage;
            // save a reference to the mask image, image will be disposed when command is finished
            _maskImage = command.MaskImage;

            // execute the command
            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes the FillRectangleCommand command.
        /// </summary>
        public void ExecuteFillRectangleCommand()
        {
            // create the processing command
            FillRectangleCommand command = new FillRectangleCommand();

            WpfRectangularSelectionTool visualTool = null;
            if (_viewer.VisualTool != null && _viewer.VisualTool is WpfRectangularSelectionTool)
            {
                visualTool = (WpfRectangularSelectionTool)_viewer.VisualTool;
                if (!visualTool.Rectangle.IsEmpty)
                {
                    Rect selectionRect = visualTool.Rectangle;
                    int x = (int)Math.Round(selectionRect.X);
                    int y = (int)Math.Round(selectionRect.Y);
                    int width = (int)Math.Round(selectionRect.Width);
                    int height = (int)Math.Round(selectionRect.Height);
                    Rectangle rect = new Rectangle(x, y, width, height);
                    command.Rectangles = new Rectangle[] { rect };
                }
            }

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Fill Rectangle Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            if (visualTool != null)
                visualTool.Rectangle = Rect.Empty;

            ExecuteProcessingCommand(command, true);
        }

        /// <summary>
        /// Executes the IsImageBlackWhiteCommand command.
        /// </summary>
        public void ExecuteIsImageBlackWhiteCommand()
        {
            if (!CommandCanProcessImage(new IsImageBlackWhiteCommand(), _viewer.Image))
                return;
            WpfIsImageBlackWhiteWindow dlg = new WpfIsImageBlackWhiteWindow(_viewer);
            if (dlg.ShowProcessingDialog())
            {
                IsImageBlackWhiteCommand command = (IsImageBlackWhiteCommand)dlg.GetProcessingCommand();
                if (ExecuteProcessingCommand(command, false))
                {
                    if (command.Result.IsImageBlackWhite)
                        MessageBox.Show("Image is black-white");
                    else
                        MessageBox.Show("Image is not black-white");
                }
            }
        }

        /// <summary>
        /// Executes the IsImageGrayscaleCommand command.
        /// </summary>
        public void ExecuteIsImageGrayscaleCommand()
        {
            if (!CommandCanProcessImage(new IsImageGrayscaleCommand(), _viewer.Image))
                return;
            WpfIsImageGrayscaleWindow dlg = new WpfIsImageGrayscaleWindow(_viewer);
            if (dlg.ShowProcessingDialog())
            {
                IsImageGrayscaleCommand command = (IsImageGrayscaleCommand)dlg.GetProcessingCommand();
                if (ExecuteProcessingCommand(command, false))
                {
                    if (command.Result.IsImageGrayscale)
                        MessageBox.Show("Image is grayscale");
                    else
                        MessageBox.Show("Image is not grayscale");
                }
            }
        }

        /// <summary>
        /// Executes the GetColorCountCommand command.
        /// </summary>
        public void ExecuteColorCountCommand()
        {
            GetColorCountCommand command = new GetColorCountCommand();
            if (!CommandCanProcessImage(command, _viewer.Image))
                return;
            if (ExecuteProcessingCommand(command, false))
                MessageBox.Show(String.Format("This image has {0} unique colors.", command.ColorCount));
        }

        /// <summary>
        /// Executes the GetImageColorDepthCommand command.
        /// </summary>
        public void ExecuteGetImageColorDepthCommand()
        {
            if (!CommandCanProcessImage(new GetImageColorDepthCommand(), _viewer.Image))
                return;
            WpfGetImageColorDepthWindow dlg = new WpfGetImageColorDepthWindow(_viewer);
            if (dlg.ShowProcessingDialog())
            {
                GetImageColorDepthCommand command = (GetImageColorDepthCommand)dlg.GetProcessingCommand();
                if (ExecuteProcessingCommand(command, false))
                {
                    if (command.Result.PixelFormat == PixelFormat.Undefined)
                        MessageBox.Show("Image color depth can not be reduced.");
                    else
                        MessageBox.Show(string.Format("Detected image color depth is {0}.", command.Result.PixelFormat));
                }
            }
        }

        /// <summary>
        /// Executes the GetBorderColorCommand command.
        /// </summary>
        public void ExecuteGetBorderColorCommand()
        {
            GetBorderColorCommand command = new GetBorderColorCommand();

            if (ExecuteProcessingCommand(command, false))
            {
                if (command.IsBorderColorFound)
                    MessageBox.Show(String.Format("Most probably border color is {0}.", command.BorderColor.ToString()));
                else
                    MessageBox.Show("Border color is unknown.");
            }
        }

        /// <summary>
        /// Executes the GetBackgroundColorCommand command.
        /// </summary>
        public void ExecuteGetBackgroundColorCommand()
        {
            GetBackgroundColorCommand command = new GetBackgroundColorCommand();

            if (ExecuteProcessingCommand(command, false))
            {
                if (command.IsBackgroundColorFound)
                    MessageBox.Show(String.Format("Most probably background color is {0}.", command.BackgroundColor.ToString()));
                else
                    MessageBox.Show("Background color is unknown.");
            }
        }

        /// <summary>
        /// Executes the GetRotationAngleCommand command.
        /// </summary>
        public void ExecuteGetRotationAngleCommand()
        {
            GetRotationAngleCommand command = new GetRotationAngleCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Get Rotation Angle Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            if (ExecuteProcessingCommand(command, false))
                MessageBox.Show(String.Format("Most probably rotation angle is {0} degree.", command.Angle.ToString("f2")));
        }

        /// <summary>
        /// Executes the GetThresholdCommand command.
        /// </summary>
        public void ExecuteGetThresholdCommand()
        {
            GetThresholdCommand command = new GetThresholdCommand();

            if (ExecuteProcessingCommand(command, false))
                MessageBox.Show(String.Format("Optimal binarization threshold in the range 0..765 is {0}.", command.Threshold.ToString()));
        }

        /// <summary>
        /// Executes the IsImageBlankCommand command.
        /// </summary>
        public void ExecuteIsBlankCommand()
        {
            IsImageBlankCommand command = new IsImageBlankCommand(0.01f);

            if (ExecuteProcessingCommand(command, false))
            {
                if (command.Result)
                    MessageBox.Show(string.Format("Image is blank, noise level is {0}%", command.NoiseLevel * 100));
                else
                    MessageBox.Show(string.Format("Image is not blank, noise level is {0}%", command.NoiseLevel * 100));
            }
        }

        /// <summary>
        /// Executes the IsDocumentImageCommand command.
        /// </summary>
        public void ExecuteIsDocumentImageCommand()
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            IsDocumentImageCommand command = new IsDocumentImageCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Is Document Image Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            if (ExecuteProcessingCommand(command, false))
            {
                if (command.IsDocumentImage)
                {
                    if (command.IsInvertedDocumentImage)
                        MessageBox.Show("Is inverted document image.");
                    else
                        MessageBox.Show("Is document image.");
                }
                else
                {
                    MessageBox.Show("Image is not document image.");
                }
            }
#endif
        }

        /// <summary>
        /// Executes the HasCertainColorCommand command.
        /// </summary>
        public void ExecuteHasCertainColorCommand()
        {
            // create the processing command
            HasCertainColorCommand command = new HasCertainColorCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Has Certain Color Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            if (ExecuteProcessingCommand(command, false))
            {
                if (command.Result)
                    MessageBox.Show(String.Format("Image has {0} color.", command.Color));
                else
                    MessageBox.Show(String.Format("Image does not have {0} color.", command.Color));
            }
        }

        /// <summary>
        /// Executes the GetTextOrientationCommand command.
        /// </summary>
        public void ExecuteGetTextOrientationCommand()
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            GetTextOrientationCommand command = new GetTextOrientationCommand();
            command.Results = new ProcessingCommandResults();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Get Text Orientation Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            if (ExecuteProcessingCommand(command, false))
            {
                GetTextOrientationCommandResult result = (GetTextOrientationCommandResult)command.Results[0];
                ImageOrthogonalOrientation documentOrientation = result.Orientation;

                switch (documentOrientation)
                {
                    case ImageOrthogonalOrientation.Undefined:
                        MessageBox.Show(String.Format("Document orientation is not defined."));
                        break;
                    case ImageOrthogonalOrientation.Rotated0:
                        MessageBox.Show(String.Format("Document has the right orientation."));
                        break;
                    case ImageOrthogonalOrientation.Rotated90:
                        MessageBox.Show(String.Format("Document is rotated by 90 degrees clockwise."));
                        break;
                    case ImageOrthogonalOrientation.Rotated180:
                        MessageBox.Show(String.Format("Document is rotated by 180 degrees clockwise."));
                        break;
                    case ImageOrthogonalOrientation.Rotated270:
                        MessageBox.Show(String.Format("Document is rotated by 270 degrees clockwise."));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
#endif
        }

        /// <summary>
        /// Executes the GetDocumentImageRotationAngleCommand command.
        /// </summary>
        public void ExecuteGetDocumentImageRotationAngleCommand()
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            GetDocumentImageRotationAngleCommand command = new GetDocumentImageRotationAngleCommand();

            // set properties of command
            PropertyGridWindow propertyGridForm = new PropertyGridWindow(command, "Get Document ImageRotation Angle Command Properties", true);
            if (propertyGridForm.ShowDialog() != true)
                return;

            if (ExecuteProcessingCommand(command, false))
                MessageBox.Show(String.Format("Most probably rotation angle is {0} degree.", command.Angle.ToString("f2")));
#endif
        }

        /// <summary>
        /// Executes the BinarizeCommand command.
        /// </summary>
        public void ExecuteBinarizeCommand()
        {
            if (_viewer.Image.PixelFormat != Vintasoft.Imaging.PixelFormat.BlackWhite)
            {
                WpfBinarizeWindow dlg = new WpfBinarizeWindow(_viewer, false);
                if (dlg.ShowProcessingDialog())
                {
                    BinarizeCommand command = new BinarizeCommand(dlg.Threshold);
                    ExecuteProcessingCommand(command);
                }
            }
        }

        /// <summary>
        /// Executes the SetAlphaChannelValueCommand command.
        /// </summary>
        public void ExecuteSetAlphaChannelValueCommand()
        {
            WpfAlphaChannelWindow dlg = new WpfAlphaChannelWindow(_viewer);
            if (dlg.ShowProcessingDialog())
            {
                SetAlphaChannelValueCommand command = new SetAlphaChannelValueCommand(dlg.Alpha);
                ExecuteProcessingCommand(command);
            }
        }

        /// <summary>
        /// Executes the SetAlphaChannelCommand command.
        /// </summary>
        public void ExecuteSetAlphaChannelCommand()
        {
            if (_openFileDialog.ShowDialog() != true)
                return;

            VintasoftImage maskImage;
            try
            {
                maskImage = new VintasoftImage(_openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage("SetAlphaChannelMaskCommand", ex);
                return;
            }

            SetAlphaChannelMaskCommand command = new SetAlphaChannelMaskCommand(maskImage);
            ExecuteProcessingCommand(command, false);

            maskImage.Dispose();
        }

        /// <summary>
        /// Executes the ColorBlendCommand command.
        /// </summary>
        public void ExecuteColorBlendCommand(Window ownerWindow)
        {
            WpfColorBlendWindow dlg;
            try
            {
                dlg = new WpfColorBlendWindow(_viewer, _blendColor, _blendMode);
            }
            catch (ImageProcessingException ex)
            {
                MessageBox.Show(ex.Message, "Image processing exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (dlg.ShowProcessingDialog(ownerWindow))
            {
                _blendMode = dlg.BlendMode;
                _blendColor = dlg.BlendColor;
                ColorBlendCommand command = new ColorBlendCommand(_blendMode, _blendColor);
                ExecuteProcessingCommand(command);
            }
        }

        /// <summary>
        /// Executes the AdvancedReplaceColorCommand command.
        /// </summary>
        public void ExecuteAdvancedReplaceColorCommand()
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            // create the processing command
            AdvancedReplaceColorCommand command = new AdvancedReplaceColorCommand();

            // set properties of command
            PropertyGridWindow propertyGridWindow = new PropertyGridWindow(command, "Advanced Replace Color Command Properties", true);
            if (propertyGridWindow.ShowDialog() != true)
                return;

            ExecuteProcessingCommand(command, true);
#endif
        }

        /// <summary>
        /// Executes the DocumentPerspectiveCorrection command.
        /// </summary>
        public void ExecuteDocumentPerspectiveCorrectionCommand()
        {
#if !REMOVE_DOCCLEANUP_PLUGIN
            // create the processing command
            DocumentPerspectiveCorrectionCommand command = new DocumentPerspectiveCorrectionCommand();

            ExecuteProcessingCommand(command);
#endif
        }

        #endregion

        #endregion


        #region PRIVATE

        /// <summary>
        /// Determines whether specified processing command can process the specified image.
        /// </summary>
        /// <param name="command">Image processing command.</param>
        /// <param name="image">Image to process.</param>
        /// <returns>
        /// <b>true</b> - processing command can process image;
        /// otherwise, <b>false</b>.
        /// </returns>
        private bool CommandCanProcessImage(ProcessingCommandBase command, VintasoftImage image)
        {
            bool expandSupportedPixelFormats = command.ExpandSupportedPixelFormats;
            command.ExpandSupportedPixelFormats = ExpandSupportedPixelFormats;
            PixelFormat outputPixelFormat = command.GetOutputPixelFormat(image);
            command.ExpandSupportedPixelFormats = expandSupportedPixelFormats;

            if (outputPixelFormat != PixelFormat.Undefined)
                return true;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            ReadOnlyCollection<PixelFormat> formats = command.SupportedPixelFormats;
            for (int i = 0; i < formats.Count; i++)
            {
                sb.Append(" -");
                sb.Append(formats[i].ToString());
                sb.AppendLine(";");
            }
            string supportedPixelFormatNames = sb.ToString();

            string message = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}: unsupported pixel format - {1}.\n\nProcessing command supports only the following pixel formats:\n{2}\nPlease convert the image to supported pixel format first.",
                command.Name,
                image.PixelFormat,
                supportedPixelFormatNames);

            DemosTools.ShowErrorMessage("Image processing exception", message);
            return false;
        }

        /// <summary>
        /// Create "Crop to custom selection" composite command. 
        /// </summary>
        ProcessingCommandBase GetCropToPathCommand(
            GraphicsPath path,
            RectangleF pathBounds,
            CropCommand crop)
        {
            Rectangle viewerImageRect = new Rectangle(0, 0, _viewer.Image.Width, _viewer.Image.Height);
            crop.RegionOfInterest = new RegionOfInterest(GetBoundingRect(RectangleF.Intersect(pathBounds, viewerImageRect)));

            // overlay command
            _overlayImage = crop.Execute(_viewer.Image);
            OverlayCommand overlay = new OverlayCommand(_overlayImage);

            // overlay with path command
            ProcessPathCommand overlayWithPath = new ProcessPathCommand(overlay, new GdiGraphicsPath(path, false));

            // clear image command
            ClearImageCommand clearImage = new ClearImageCommand(System.Drawing.Color.Transparent);

            // create composite command: clear, overlay with path, crop
            return new CompositeCommand(clearImage, overlayWithPath, crop);
        }

        /// <summary>
        /// Returns a bounding rectangle of specified <see cref="RectangleF"/>.
        /// </summary>
        static Rectangle GetBoundingRect(RectangleF rect)
        {
            float dx = rect.X - (int)rect.X;
            float dy = rect.Y - (int)rect.Y;
            return new Rectangle((int)rect.X, (int)rect.Y, (int)(rect.Width + 1 + dx), (int)(rect.Height + 1 + dy));
        }


        /// <summary>
        /// Handler of the ProcessingCommandTask.ImageProcessingException event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void executor_ImageProcessingExceptionOccurs(object sender, EventArgs e)
        {
            _isImageProcessingWorking = false;
        }


        /// <summary>
        /// Image processing is started.
        /// </summary>
        void command_Started(object sender, ImageProcessingEventArgs e)
        {
            ProcessingCommandBase command = (ProcessingCommandBase)sender;
            command.Started -= new EventHandler<ImageProcessingEventArgs>(command_Started);

            //
            OnImageProcessingCommandStarted((ProcessingCommandBase)sender, e);
        }

        /// <summary>
        /// Image processing is in progress.
        /// </summary>
        void command_Progress(object sender, ImageProcessingProgressEventArgs e)
        {
            OnImageProcessingCommandProgress((ProcessingCommandBase)sender, e);
        }

        /// <summary>
        /// Image processing is finished.
        /// </summary>
        void command_Finished(object sender, ImageProcessedEventArgs e)
        {
            ProcessingCommandBase command = (ProcessingCommandBase)sender;
            command.Finished -= new EventHandler<ImageProcessedEventArgs>(command_Finished);

            if (command is ParallelizingProcessingCommand)
                command = ((ParallelizingProcessingCommand)command).ProcessingCommand;

            //
            if (command is OverlayCommand ||
                command is OverlayWithBlendingCommand ||
                command is OverlayMaskedCommand)
            {
                if (_overlayImage != null)
                {
                    // dispose the temporary overlay image because the processing command is finished
                    _overlayImage.Dispose();
                    _overlayImage = null;
                }
                if (_maskImage != null)
                {
                    // dispose the temporary mask image because the processing command is finished
                    _maskImage.Dispose();
                    _maskImage = null;
                }
            }

            _isImageProcessingWorking = false;

            if (_imageProcessingUndoMonitor != null)
            {
                _imageProcessingUndoMonitor.Dispose();
                _imageProcessingUndoMonitor = null;
            }

            //
            OnImageProcessingCommandFinished((ProcessingCommandBase)sender, e);
        }


        /// <summary>
        /// Raises the ImageProcessingCommandStarted event.
        /// </summary>
        void OnImageProcessingCommandStarted(ProcessingCommandBase command, ImageProcessingEventArgs e)
        {
            if (ImageProcessingCommandStarted != null)
                ImageProcessingCommandStarted(command, e);
        }

        /// <summary>
        /// Raises the ImageProcessingCommandProgress event.
        /// </summary>
        void OnImageProcessingCommandProgress(ProcessingCommandBase command, ImageProcessingProgressEventArgs e)
        {
            if (ImageProcessingCommandProgress != null)
                ImageProcessingCommandProgress(command, e);
        }

        /// <summary>
        /// Raises the ImageProcessingCommandFinished event.
        /// </summary>
        void OnImageProcessingCommandFinished(ProcessingCommandBase command, ImageProcessedEventArgs e)
        {
            if (ImageProcessingCommandFinished != null)
                ImageProcessingCommandFinished(command, e);
        }

        #endregion

        #endregion



        #region Events

        public event EventHandler<ImageProcessingEventArgs> ImageProcessingCommandStarted;

        public event EventHandler<ImageProcessingProgressEventArgs> ImageProcessingCommandProgress;

        public event EventHandler<ImageProcessedEventArgs> ImageProcessingCommandFinished;

        #endregion

    }
}
