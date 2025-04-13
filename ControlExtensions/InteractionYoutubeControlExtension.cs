using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.Windows.Interactions;
using Genesyslab.Desktop.Modules.Windows.Views.Interactions.BundleView;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Toolbar;
using Genesyslab.Enterprise.Interaction;
using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Platform.Commons.Logging;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using CoreModelInteractions = Genesyslab.Desktop.Modules.Core.Model.Interactions;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    public class InteractionYoutubeControlExtension : InteractionControlExtension, IInteractionControlExtension
    {
        public static readonly string NAME = typeof(InteractionYoutubeControlExtension).FullName;
        private readonly IViewManager viewManager;

        public InteractionYoutubeControlExtension(IUnityContainer container, ILogger log, IViewManager viewManager) 
            : base(container)
        {
            this.viewManager = viewManager;

            this.InteractionWorkItemControlExtension =
                container.Resolve<IInteractionControlExtension>("InteractionWorkItem");
        }

        private IInteractionControlExtension InteractionWorkItemControlExtension { get; set; }

        public CoreModelInteractions.IInteraction GetInteractionWorkItem(CoreModelInteractions.IInteraction interaction)
        {
            if (interaction.GetType().IsAssignableFrom(typeof(InteractionYoutube)))
                return (CoreModelInteractions.IInteraction)((IInteractionYoutube)interaction).InteractionWorkItem;
            if (interaction.GetType().IsAssignableFrom(typeof(InteractionOutboundYoutube)))
                return (CoreModelInteractions.IInteraction)((IInteractionOutboundYoutube)interaction).InteractionWorkItem;

            return (CoreModelInteractions.IInteraction)null;
        }

        public override string GetImageDefinitionKey(CoreModelInteractions.IInteraction interaction)
        {
            return this.InteractionWorkItemControlExtension.GetImageDefinitionKey(GetInteractionWorkItem(interaction));
        }

        public bool CanCreateInteractionControlNow(CoreModelInteractions.IInteraction interaction)
        {
            try
            {
                if (interaction != null && interaction is InteractionYoutube)
                {
                    IInteractionOpenMedia interactionOpenMedia = interaction as IInteractionOpenMedia;

                    if (interactionOpenMedia != null &&
                        interactionOpenMedia.EntrepriseOpenMediaInteractionCurrent != null)
                    {
                        var entrepriseOpenMediaInteractionCurrent = interactionOpenMedia.EntrepriseOpenMediaInteractionCurrent as OpenMediaInteraction;

                        if (entrepriseOpenMediaInteractionCurrent.InteractionType == "Inbound")
                        {
                            if (!interaction.IsPending)
                            {
                                return interaction.State != InteractionStateType.Ended;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Info("YoutubeWorkItemModule in CanCreateInteractionControlNow : ex : " + ex);
            }
            return false;
        }

        public bool CreateInteractionControl(CoreModelInteractions.IInteraction interaction, object context)
        {
            if (interaction != null && interaction is InteractionYoutube)
            {
                IDictionary<string, object> contextDictionary = context as IDictionary<string, object>;
                IBundleView bundleView = contextDictionary.TryGetValue<string, object>("BundleView") as IBundleView;
                if (bundleView != null)
                {
                    try
                    {
                        IDictionary<string, object> bundleDictionary = (IDictionary<string, object>)new Dictionary<string, object>(bundleView.Context as IDictionary<string, object>);
                        if (bundleDictionary != null)
                            bundleDictionary["Interaction"] = (object)interaction;

                        object view1 = this.viewManager.InstantiateDynamicViewInRegion
                            ((object)bundleView,
                            YoutubeWorkItemView.PARENT_REGION,
                            YoutubeWorkItemView.NAME,
                            interaction.InteractionId, bundleDictionary).View;

                        interaction.UserData["InteractionView"] = view1;

                        object view2 = this.viewManager.InstantiateDynamicViewInRegion
                            ((object)bundleView, YoutubeInboundToolbarView.PARENT_REGION, YoutubeInboundToolbarView.NAME,
                            interaction.InteractionId, bundleDictionary).View;

                        this.OnEventInteractionViewCreated(
                            new InteractionViewEventArgs
                            (interaction, view1, view2, bundleView, (object)bundleDictionary));

                        return true;
                    }
                    catch (Exception ex)
                    {
                        YoutubeOptions.Log.Error((object)("YoutubeWorkItemModule - CreateInteractionControl : " + (object)interaction
                            + ",  Exception"), ex);
                    }
                }

                IView view3 = contextDictionary.TryGetValue<string, object>("InteractionBarInteractionView") as IView;
                if (view3 != null)
                {
                    try
                    {
                        var dc = (object)new Dictionary<string, object>(context as IDictionary<string, object>);

                        object view1 = this.viewManager.InstantiateDynamicViewInRegion((object)view3,
                            YoutubeInboundToolbarView.PARENT_REGION, 
                            YoutubeInboundToolbarView.NAME,
                            interaction.InteractionId, dc).View;

                        return true;
                    }
                    catch (Exception ex)
                    {
                        YoutubeOptions.Log.Error("AddInteraction, InteractionBarBundleView " + (object)view3 + ", Exception", ex);
                    }
                }
            }
            return false;
        }

        public override bool CreateInteractionToolTipViewInRegion(CoreModelInteractions.IInteraction interaction, object context)
        {
            YoutubeOptions.Log.Info("CreateInteractionToolTipViewInRegion, InteractionWorkItem "
                + interaction.InteractionId + " , Type: " + interaction.Type);

            if (interaction.Type == nameof(InteractionYoutube))
            {
                IDictionary<string, object> dictionary = context as IDictionary<string, object>;
                try
                {
                    IView view1 = dictionary.TryGetValue<string, object>("View") as IView;
                    string regionName = dictionary.TryGetValue<string, object>("Region") as string;
                    if (view1 != null)
                    {
                        if (!string.IsNullOrEmpty(regionName))
                        {
                            object view2 = this.viewManager.InstantiateDynamicViewInRegion((object)view1, regionName, "InteractionWorkItemMouseOverDetailView", "InteractionWorkItemMouseOverDetailView" + interaction.InteractionId, (object)new Dictionary<string, object>(context as IDictionary<string, object>)).View;
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    YoutubeOptions.Log.Error((object)("CreateInteractionToolTipViewInRegion, InteractionWorkItem " + interaction.InteractionId + ", Exception"), ex);
                }
            }
            return false;
        }

        public void RemoveInteractionControl(CoreModelInteractions.IInteraction interaction, object context)
        {
            if (!(interaction is InteractionYoutube))
                return;

            IDictionary<string, object> dictionary = context as IDictionary<string, object>;
            IBundleView bundleView = dictionary.TryGetValue<string, object>("BundleView") as IBundleView;
            if (bundleView != null)
            {
                this.viewManager.RemoveViewInRegion((object)bundleView, "BundleToolbarContainerRegion",
                    interaction.InteractionId);
                this.viewManager.RemoveViewInRegion((object)bundleView, "InteractionsBundleRegion",
                    interaction.InteractionId);
            }
            else
            {
                IView view = dictionary.TryGetValue<string, object>("InteractionBarInteractionView") as IView;
                if (view != null)
                {
                    this.viewManager.RemoveViewInRegion((object)view, "BundleToolbarContainerRegion",
                        interaction.InteractionId);
                }
            }
        }

        public ImageSource GetInteractionMediaIcon(CoreModelInteractions.IInteraction interaction, InteractionMediaIconSource source)
        {
            return this.InteractionWorkItemControlExtension.GetInteractionMediaIcon(GetInteractionWorkItem(interaction), source);
        }

        public ImageSource GetInteractionSilentMonitoringMediaIcon(CoreModelInteractions.IInteraction interaction)
        {
            return (ImageSource)null;
        }

        public Icon GetInteractionMediaDrawingIcon(CoreModelInteractions.IInteraction interaction)
        {
            var icon = this.InteractionWorkItemControlExtension.GetInteractionMediaDrawingIcon(GetInteractionWorkItem(interaction));
            return icon;
        }

        public string GetInteractionStateLabel(CoreModelInteractions.IInteraction interaction, BundleParty bundleParty)
        {
            return this.InteractionWorkItemControlExtension.GetInteractionStateLabel(GetInteractionWorkItem(interaction), bundleParty);
        }

        public IList<RequestedAction> RequestActions(string capacity, ActionTarget target, object context)
        {
            IList<RequestedAction> requestedActionList = this.InteractionWorkItemControlExtension.RequestActions(capacity, target, context);
            switch (target)
            {
                case ActionTarget.InteractionActionFromWorkbin:
                    if (!this.container.Resolve<IAgent>().HasMediaCapacity(YoutubeWorkItemModule.MediaTypeModuleMedia))
                        return (IList<RequestedAction>)new List<RequestedAction>();
                    string str = (context as Dictionary<string, object>).TryGetValue<string, object>("mediaDirectionType") as string;
                    if (string.IsNullOrEmpty(str) || str != "Inbound")
                        return (IList<RequestedAction>)new List<RequestedAction>();
                    break;
            }
            return requestedActionList;
        }

        public string BeforeCloseCommand
        {
            get
            {
                return this.InteractionWorkItemControlExtension.BeforeCloseCommand;
            }
        }

        public string CloseInteractionCommand
        {
            get
            {
                return "InteractionWorkItemMoveToWorkbin";
            }
        }

        public string PossibleToCloseInteractionCommand
        {
            get
            {
                return (string)null;
            }
        }

        public void UpdateCaseViewContextBeforeCreateCaseView(object context, CoreModelInteractions.IInteraction interaction)
        {
            this.InteractionWorkItemControlExtension.UpdateCaseViewContextBeforeCreateCaseView(context, GetInteractionWorkItem(interaction));
        }

        public IList<string> SupportedMedia
        {
            get
            {
                return (IList<string>)new List<string>() { YoutubeWorkItemModule.MediaTypeModuleMedia };
            }
        }

        public override ImageSource GetInteractionMediaIconRecorded(
          CoreModelInteractions.IInteraction interaction,
          InteractionMediaIconSource source)
        {
            return (ImageSource)null;
        }

        public override ImageSource GetInteractionSilentMonitoringMediaIconRecorded(
          CoreModelInteractions.IInteraction interaction)
        {
            return (ImageSource)null;
        }
    }
}
