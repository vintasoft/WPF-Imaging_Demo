using System.Windows;

using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfImagingDemo
{
    class WpfImageProcessingPreviewInViewer
    {

        #region Properties

        bool _isPreviewStarted = false;
        WpfImageViewer _viewer;
        WpfSelectionRegionView _selectionRegionView;
        WpfVisualTool _viewerTool;
        WpfImageProcessingTool _rectangularPreview;
        Rect _rectangularSelectionToolRect;
        WpfSelectionRegionViewWithImageProcessingPreview _processedSelectionRegionView;

        #endregion



        #region Constructors

        public WpfImageProcessingPreviewInViewer(WpfImageViewer viewer)
        {
            _viewer = viewer;
        }

        #endregion



        #region Properties

        bool _useCurrentViewerZoomWhenPreviewProcessing = false;
        /// <summary> 
        /// Gets or sets a flag that indicates when 
        /// image with current viewer zoom used for preview image processing.
        /// </summary>
        public bool UseCurrentViewerZoomWhenPreviewProcessing
        {
            get
            {
                return _useCurrentViewerZoomWhenPreviewProcessing;
            }
            set
            {
                _useCurrentViewerZoomWhenPreviewProcessing = value;
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

        #endregion



        #region Methods

        /// <summary>
        /// Start preview.
        /// </summary>
        public void StartPreview()
        {
            // if current tool is CustomSelectionTool
            if (_viewer.VisualTool is WpfCustomSelectionTool)
            {
                WpfCustomSelectionTool selectionTool = (WpfCustomSelectionTool)_viewer.VisualTool;
                Rect selectionBBox = Rect.Empty;
                if (selectionTool.Selection != null)
                    selectionBBox = selectionTool.Selection.GetBoundingBox();
                if (selectionBBox.Width >= 1 && selectionBBox.Height >= 1)
                {
                    _selectionRegionView = selectionTool.Selection.View;
                    _processedSelectionRegionView = new WpfSelectionRegionViewWithImageProcessingPreview(_selectionRegionView.Selection);
                    _processedSelectionRegionView.UseViewerZoomForProcessing = UseCurrentViewerZoomWhenPreviewProcessing;
                    selectionTool.Selection.View = _processedSelectionRegionView;
                    _isPreviewStarted = true;
                    return;
                }
            }

            // set current tool to ImageProcessingToolWithRectangularSelection
            _viewerTool = _viewer.VisualTool;
            _rectangularPreview = new WpfImageProcessingTool();
            Rect imageRect = new Rect(0, 0, _viewer.Image.Width, _viewer.Image.Height);
            Rect rect = imageRect;
            if (_viewer.VisualTool is WpfRectangularSelectionTool)
            {
                WpfRectangularSelectionTool rectangularSelection = (WpfRectangularSelectionTool)_viewer.VisualTool;
                if (rectangularSelection.Rectangle.Width > 0 && rectangularSelection.Rectangle.Height > 0)
                {
                    if (_viewer.VisualTool is WpfRectangularSelectionToolWithCopyPaste)
                        rect = rectangularSelection.Rectangle;
                    _rectangularSelectionToolRect = rectangularSelection.Rectangle;
                }
            }
            _rectangularPreview.UseViewerZoomForPreviewProcessing = UseCurrentViewerZoomWhenPreviewProcessing;
            _viewer.VisualTool = _rectangularPreview;
            _rectangularPreview.Rectangle = rect;
            if (rect == imageRect)
                _rectangularPreview.InteractionController = null;
            _isPreviewStarted = true;
        }

        /// <summary>
        /// Stop preview.
        /// </summary>
        public void StopPreview()
        {
            if (_viewer.VisualTool is WpfCustomSelectionTool)
            {
                WpfCustomSelectionTool selectionTool = (WpfCustomSelectionTool)_viewer.VisualTool;
                if (selectionTool.Selection != null)
                    selectionTool.Selection.View = _selectionRegionView;
            }
            else
            {
                _viewer.VisualTool = _viewerTool;
                if (_viewer.VisualTool is WpfRectangularSelectionTool)
                    ((WpfRectangularSelectionTool)_viewer.VisualTool).Rectangle = _rectangularSelectionToolRect;
            }
            _isPreviewStarted = false;
        }

        /// <summary>
        /// Sets a current image processing command.
        /// </summary>
        public void SetCommand(ProcessingCommandBase command)
        {
            if (_isPreviewStarted)
            {
                if (command != null)
                {
                    command.ExpandSupportedPixelFormats = ExpandSupportedPixelFormats;
                }

                if (_viewer.VisualTool is WpfCustomSelectionTool)
                {
                    WpfCustomSelectionTool selectionTool = (WpfCustomSelectionTool)_viewer.VisualTool;
                    _processedSelectionRegionView.ProcessingCommand = command;
                    selectionTool.Selection.InvalidateVisual();
                }
                else
                {
                    _rectangularPreview.ProcessingCommand = command;
                }
            }
        }

        #endregion

    }
}
