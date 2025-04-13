using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Enterprise.Extensions;
using Genesyslab.Enterprise.Model.Interaction;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Commands
{
    [ElementOfCommand(parameters = "CommandParameter: Genesyslab.Desktop.Modules.OpenMedia.Model.Interaction.IInteractionOpenMedia")]
    internal class SaveUnsavedUserDataCommandYoutube : InteractionOpenMediaCommand
    {
        public SaveUnsavedUserDataCommandYoutube(IUnityContainer container) : base(container)
        {
        }

        public override string Name
        {
            get { return nameof(SaveUnsavedUserDataCommandYoutube); }
        }

        public override bool Execute(IDictionary<string, object> parameters, IProgressUpdater progressUpdater)
        {
            this.log.Info((object)nameof(SaveUnsavedUserDataCommandYoutube));
            if (parameters["CommandParameter"] is InteractionYoutube)
            {
                this.log.InfoFormat("{0} InteractionYoutube", (object)nameof(SaveUnsavedUserDataCommandYoutube));
                UpdateInboundInteraction(parameters);
            }
            else if (parameters["CommandParameter"] is InteractionOutboundYoutube)
            {
                this.log.InfoFormat("{0} InteractionLiInteractionOutboundYoutubenkedIn", (object)nameof(SaveUnsavedUserDataCommandYoutube));

                InteractionOutboundYoutube parameter = parameters["CommandParameter"] as InteractionOutboundYoutube;

                parameters.Add("WorkbinId", (object)WorkbinsOptions.Default.GetDraftWorkbinId(YoutubeWorkItemModule.MediaTypeModuleMedia));
                parameters.Add("WorkbinOptionName", (object)WorkbinsOptions.Default.GetDraftWorkbinOptionName(YoutubeWorkItemModule.MediaTypeModuleMedia));

                if (!string.IsNullOrEmpty(parameter.FromWorkbinId))
                    parameters["WorkbinId"] = (object)parameter.FromWorkbinId;

                if (parameter != null && parameter.UnsavedUserData != null && parameter.UnsavedUserData.Count > 0)
                {
                    this.log.InfoFormat("UnsavedUserData found");
                    var interaction = parameter.EntrepriseInteractionCurrent as IOpenMediaInteraction;
                    var responseTime = YoutubeOptions.Default.ResponseWaitTime;

                    var setAttachedDataResult = new SocialMediaContainer(this.container).OpenMediaService
                            .SetAttachedData(interaction, parameter.UnsavedUserData, responseTime, (string)null);

                    if (setAttachedDataResult == null)
                    {
                        this.log.Warn("OpenMediaService.SetAttachedData failed");
                        return true;
                    }

                }
            }
            return false;
        }

        private static void UpdateInboundInteraction(IDictionary<string, object> parameters)
        {
            InteractionYoutube parameter = parameters["CommandParameter"] as InteractionYoutube;

            var inProgressWorkbinId = (object)WorkbinsOptions.Default.GetInProgressWorkbinId(YoutubeWorkItemModule.MediaTypeModuleMedia);
            var inProgreaaWorkbinName = (object)WorkbinsOptions.Default.GetInProgressWorkbinOptionName(YoutubeWorkItemModule.MediaTypeModuleMedia);

            parameters.Add("WorkbinId", inProgressWorkbinId);
            parameters.Add("WorkbinOptionName", inProgreaaWorkbinName);

            if (!string.IsNullOrEmpty(parameter.FromWorkbinId))
                parameters["WorkbinId"] = (object)parameter.FromWorkbinId;
        }
    }
}
