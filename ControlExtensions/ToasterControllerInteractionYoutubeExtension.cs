using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.Windows.Views.Toaster;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Microsoft.Practices.Unity;
using System.Windows.Controls;
using System.Windows.Media;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    public class ToasterControllerInteractionYoutubeExtension : IToasterControllerInteractionExtension
    {
        private IToasterControllerInteractionExtension ExtensionOpenMedia;
        private IUnityContainer Container;

        public ToasterControllerInteractionYoutubeExtension(IUnityContainer container)
        {
            this.ExtensionOpenMedia = container.Resolve<IToasterControllerInteractionExtension>("ToasterControllerInteractionOpenMediaExtension");
            this.Container = container;
        }

        public string Type
        {
            get
            {
                return nameof(InteractionYoutube);
            }
        }

        public bool InteractionManager_InteractionCreated(IInteraction interaction)
        {
            var iscreated = this.ExtensionOpenMedia.InteractionManager_InteractionCreated(interaction);

            return iscreated;
        }

        public void InteractionManager_InteractionEvent(
          IInteraction interaction,
          out bool create,
          out bool remove,
          IToasterWindow toasterWindow)
        {
            if (toasterWindow != null)
                ((Control)toasterWindow).FontFamily = new FontFamily("Arial Narrow");

            YoutubeOptions.Log.Info("InteractionYoutube InteractionManager_InteractionEvent ");
            create = false;
            remove = false;
            this.ExtensionOpenMedia.InteractionManager_InteractionEvent(interaction, out create, out remove, toasterWindow);

        }

        public void interactionManager_InteractionClosed(IInteraction interaction)
        {
            // interaction
            this.ExtensionOpenMedia.interactionManager_InteractionClosed(interaction);
        }

        public IToasterWindow AddToaster(IInteraction interaction, IViewManager viewManager)
        {
            return this.ExtensionOpenMedia.AddToaster(interaction, viewManager);
        }
    }
}
