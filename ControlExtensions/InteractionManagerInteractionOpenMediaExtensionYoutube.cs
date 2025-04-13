using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Modules.Core.Utils.Context;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Enterprise.Model.Envelope;
using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Enterprise.Model.Protocol;
using Microsoft.Practices.Unity;
using System.Windows;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    public class InteractionManagerInteractionOpenMediaExtensionYoutube : IInteractionManagerInteractionOpenMediaExtension
    {
        public static readonly string NAME = typeof(InteractionManagerInteractionOpenMediaExtensionYoutube).FullName;
        private readonly IObjectContainer container;
        private readonly IContextManager contextManager;
        private readonly ICommandManager commandManager;
        private readonly IUnityContainer unityContainer;

        public InteractionManagerInteractionOpenMediaExtensionYoutube(
          IUnityContainer unityContainer,
          IObjectContainer container,
          IContextManager contextManager,
          ICommandManager commandManager)
        {
            this.container = container;
            this.contextManager = contextManager;
            this.commandManager = commandManager;
            this.unityContainer = unityContainer;
        }

        public IInteractionOpenMedia CreateInteraction(
          IInteractionType interactionType)
        {
            if (interactionType.MediaType == MediaContentType.Multimedia
             && interactionType.SubMediaType == YoutubeWorkItemModule.MediaTypeModuleMedia)
            {
                if (interactionType.Direction == MediaDirectionType.In)
                {
                    return (IInteractionOpenMedia)this.container.Resolve<IInteractionYoutube>();
                }
                else
                {
                    return (IInteractionOpenMedia)this.container.Resolve<IInteractionOutboundYoutube>();
                }
            }
            return (IInteractionOpenMedia)null;
        }

        public IInteractionOpenMedia CreateInteraction(
          IInteractionType interactionType,
          object context)
        {
            if (interactionType.MediaType == MediaContentType.Multimedia
                      && interactionType.SubMediaType == YoutubeWorkItemModule.MediaTypeModuleMedia)
            {
                if (interactionType.Direction == MediaDirectionType.In)
                {
                    return (IInteractionOpenMedia)this.container.Resolve<IInteractionYoutube>();
                }
                else
                {
                    return (IInteractionOpenMedia)this.container.Resolve<IInteractionOutboundYoutube>();
                }
            }
            return (IInteractionOpenMedia)null;
        }

        public bool SilentWorkflowEvent(IEnvelope<IInteraction> tsp)
        {
            if (tsp.Body.IdType.Direction == MediaDirectionType.Out
                && tsp.Body.IdType.SubMediaType == YoutubeWorkItemModule.MediaTypeModuleMedia)
            {
                if (tsp.Header != null && tsp.Header.CorrelatorData != null && tsp.Header.CorrelatorData == "Silent")
                    return true;
                if (tsp.Header != null && tsp.Header.CorrelatorData != null && tsp.Header.CorrelatorData == "SilentDelete")
                {
                    YoutubeOptions.Log.Info("SilentDelete");
                    DeleteInteractionSilentCommand.AsyncRun(this.unityContainer, new DeleteInteractionSilentCommand.Parameters()
                    {
                        Interaction = tsp.Body as IOpenMediaInteraction,
                        Timeout = YoutubeOptions.Default.ResponseWaitTime
                    }, (UIElement)null);
                    return true;
                }
            }
            return false;
        }
    }
}
