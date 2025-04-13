using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.Windows.Views.Toaster;
using Microsoft.Practices.Unity;
using System.Windows.Controls;
using System.Windows.Media;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    internal class ToasterControllerInteractionOutboundYoutubeExtension : IToasterControllerInteractionExtension
    {
        private IToasterControllerInteractionExtension ExtensionOpenMedia;

        public ToasterControllerInteractionOutboundYoutubeExtension(IUnityContainer container)
        {
            this.ExtensionOpenMedia = container.Resolve<IToasterControllerInteractionExtension>("ToasterControllerInteractionOpenMediaExtension");
        }

        public string Type
        {
            get
            {
                return "InteractionOutboundYoutube";
            }
        }

        public bool InteractionManager_InteractionCreated(IInteraction interaction)
        {
            return this.ExtensionOpenMedia.InteractionManager_InteractionCreated(interaction);
        }

        public void InteractionManager_InteractionEvent(IInteraction interaction, out bool create, out bool remove, IToasterWindow toasterWindow)
        {
            if (toasterWindow != null)
                ((Control)toasterWindow).FontFamily = new FontFamily("Arial Narrow");
            this.ExtensionOpenMedia.InteractionManager_InteractionEvent(interaction, out create, out remove, toasterWindow);
        }

        public void interactionManager_InteractionClosed(IInteraction interaction)
        {
            this.ExtensionOpenMedia.interactionManager_InteractionClosed(interaction);
        }

        public IToasterWindow AddToaster(IInteraction interaction, IViewManager viewManager)
        {
            return this.ExtensionOpenMedia.AddToaster(interaction, viewManager);
        }
    }
}
