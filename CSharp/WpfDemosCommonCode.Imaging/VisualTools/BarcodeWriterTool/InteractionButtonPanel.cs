using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;

namespace WpfDemosCommonCode.Barcode
{
    /// <summary>
    /// Represents a panel with interaction buttons.
    /// </summary>
    internal class InteractionButtonsPanel : WpfInteractionControllerBase<IWpfInteractiveObject>
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionButtonsPanel"/> class. 
        /// </summary>
        internal InteractionButtonsPanel(IWpfInteractiveObject interactiveObject, params InteractionButton[] buttons)
            : base(interactiveObject)
        {
            InteractionAreaList.AddRange(buttons);
        }

        #endregion



        #region Methods

        /// <summary>
        /// Updates the interaction areas of this controller.
        /// </summary>
        public override void UpdateInteractionAreas(WpfImageViewer viewer)
        {
            float x = 0;
            foreach (InteractionButton button in InteractionAreaList)
            {
                button.X = x;
                if (button.Image != null)
                    x += button.Image.Width + 1;
            }
        }

        /// <summary>
        /// Interaction logic.
        /// </summary>
        protected override void PerformInteraction(WpfInteractionEventArgs args)
        {
        }

        #endregion

    }
}
