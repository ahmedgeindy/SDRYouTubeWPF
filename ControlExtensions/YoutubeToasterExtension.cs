using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia.Windows.Toaster;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    internal class YoutubeToasterExtension : IWorkItemToasterExtension
    {
        public bool IsAutoAnswer(IInteraction interaction)
        {
            OpenMediaOptions openMediaOptions = OpenMediaOptions.CreateNewInstance(interaction.ContextualConfigManager) ?? OpenMediaOptions.Default;
            IInteractionOpenMedia interactionOpenMedia = interaction as IInteractionOpenMedia;
            if (interactionOpenMedia != null)
            {
                return openMediaOptions.WorkItemAutoAnswer(interactionOpenMedia.MediaType);
            }

            return false;
        }

        public string AcceptNameCommand
        {
            get
            {
                return "InteractionWorkItemAccept";
            }
        }

        public string DeclineNameCommand
        {
            get
            {
                return "InteractionWorkItemDecline";
            }
        }

        public bool CanUseDeclineCommand
        {
            get
            {
                return OpenMediaOptions.Default.Task[YoutubeTask.CanDecline];
            }
        }
    }
}
