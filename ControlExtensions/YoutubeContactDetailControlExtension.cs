using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Contacts.ContactDetail;
using Genesyslab.Desktop.Modules.Contacts.IWInteraction;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView;
using System.Collections.Generic;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    internal class YoutubeContactDetailControlExtension : IContactDetailControlExtension
    {
        public readonly static string NAME;

        private readonly IViewManager viewManager;

        static YoutubeContactDetailControlExtension()
        {
            YoutubeContactDetailControlExtension.NAME = typeof(YoutubeContactDetailControlExtension).FullName;
        }

        public YoutubeContactDetailControlExtension(IViewManager viewManager)
        {
            this.viewManager = viewManager;
        }

        public bool CreateDetailControl(IIWInteractionContent interactionContent, object context)
        {

            if (interactionContent.InteractionAttributes.MediaTypeId
                == YoutubeWorkItemModule.MediaTypeModuleMedia)
            {
                IContactDetailView contactDetailView =
                    Extensions.TryGetValue<string, object>(context as IDictionary<string, object>, "ContactDetailView") as IContactDetailView;

                if (contactDetailView != null)
                {
                    var dynamiccontext = new Dictionary<string, object>(context as IDictionary<string, object>);

                    if (dynamiccontext != null)
                    {
                        var instantiatedView = this.viewManager.InstantiateDynamicViewInRegion(
                            contactDetailView,
                            YoutubeContactDetailContentView.PARENT_REGION,
                            YoutubeContactDetailContentView.NAME,
                            YoutubeContactDetailContentView.NAME,
                            dynamiccontext);

                        if (instantiatedView != null)
                        {
                            IContactDetailContentView contactDetailContentView = instantiatedView.View as IContactDetailContentView;

                            if (contactDetailContentView != null)
                            {
                                contactDetailContentView.CreateDetailDocument(interactionContent);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool RemoveDetailControl(IIWInteractionContent interactionContent, object context)
        {
            if (interactionContent.InteractionAttributes.MediaTypeId
                == YoutubeWorkItemModule.MediaTypeModuleMedia)
            {
                IContactDetailView contactDetailView =
                    Extensions.TryGetValue<string, object>(context as IDictionary<string, object>, "ContactDetailView") as IContactDetailView;

                if (contactDetailView != null)
                {
                    this.viewManager.RemoveViewInRegion(contactDetailView,
                       YoutubeContactDetailContentView.PARENT_REGION,
                       YoutubeContactDetailContentView.NAME);
                    return true;

                }
            }
            return false;
        }
    }
}
