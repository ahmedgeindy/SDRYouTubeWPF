using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.Windows.Interactions;
using Genesyslab.Desktop.Modules.Windows.Views.Interactions.BundleView;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Toolbar;
using Genesyslab.Desktop.WPFCommon;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Platform.Commons.Logging;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using interactionCore = Genesyslab.Desktop.Modules.Core.Model.Interactions;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    internal class InteractionOutboundYoutubeControlExtension : InteractionControlExtension, IInteractionControlExtension
    {
        public static readonly string NAME = typeof(InteractionOutboundYoutubeControlExtension).FullName;
        private readonly IViewManager viewManager;

        public InteractionOutboundYoutubeControlExtension(IUnityContainer container, ILogger log, IViewManager viewManager)
          : base(container)
        {
            this.viewManager = viewManager;
            this.InteractionWorkItemControlExtension = container.Resolve<IInteractionControlExtension>("InteractionWorkItem");
        }

        private IInteractionControlExtension InteractionWorkItemControlExtension { get; set; }

        public interactionCore.IInteraction GetInteractionWorkItem(interactionCore.IInteraction interaction)
        {
            if (interaction.GetType().IsAssignableFrom(typeof(InteractionYoutube)))
                return (interactionCore.IInteraction)((IInteractionYoutube)interaction).InteractionWorkItem;
            if (interaction.GetType().IsAssignableFrom(typeof(InteractionOutboundYoutube)))
                return (interactionCore.IInteraction)((IInteractionOutboundYoutube)interaction).InteractionWorkItem;

            return (interactionCore.IInteraction)null;
        }

        public override string GetImageDefinitionKey(interactionCore.IInteraction interaction)
        {
            return this.InteractionWorkItemControlExtension.GetImageDefinitionKey(GetInteractionWorkItem(interaction));
        }

        public bool CanCreateInteractionControlNow(interactionCore.IInteraction interaction)
        {
            if (interaction is InteractionOutboundYoutube && !interaction.IsPending)
                return interaction.State != InteractionStateType.Ended;
            return false;
        }

        public bool CreateInteractionControl(interactionCore.IInteraction interaction, object context)
        {
            if (interaction != null && interaction is InteractionOutboundYoutube)
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

                        object view1 =
                            this.viewManager.InstantiateDynamicViewInRegion((object)bundleView,
                            YoutubeWorkItemView.PARENT_REGION,
                            YoutubeWorkItemView.NAME, interaction.InteractionId, bundleDictionary).View;


                        interaction.UserData["InteractionView"] = view1;

                        object view2 = this.viewManager.InstantiateDynamicViewInRegion
                           ((object)bundleView, 
                           YoutubeOutboundToolbarView.PARENT_REGION, 
                           YoutubeOutboundToolbarView.NAME,
                           interaction.InteractionId, bundleDictionary).View;

                        this.OnEventInteractionViewCreated(new InteractionViewEventArgs(interaction, view1, view2, bundleView,
                         bundleDictionary));

                        return true;
                    }
                    catch (Exception ex)
                    {
                        YoutubeOptions.Log.Error((object)("CreateInteractionControl " + (object)interaction + ",  Exception"), ex);
                    }
                }

                IView view3 = contextDictionary.TryGetValue<string, object>("InteractionBarInteractionView") as IView;
                if (view3 != null)
                {
                    try
                    {
                        var dc = (object)new Dictionary<string, object>(context as IDictionary<string, object>);

                        object view1 = this.viewManager.InstantiateDynamicViewInRegion((object)view3,
                            YoutubeOutboundToolbarView.PARENT_REGION, 
                            YoutubeOutboundToolbarView.NAME,
                            interaction.InteractionId, dc).View;

                        return true;
                    }
                    catch (Exception ex)
                    {
                        YoutubeOptions.Log.Error("CreateInteraction, YoutubeOutboundToolBarView " + (object)view3 + ", Exception", ex);
                    }
                }
            }
            return false;
        }

        public override bool CreateInteractionToolTipViewInRegion(interactionCore.IInteraction interaction, object context)
        {
            YoutubeOptions.Log.Info("CreateInteractionToolTipViewInRegion, InteractionWorkItem "
                + interaction.InteractionId + " , Type: " + interaction.Type);

            if (interaction.Type == "InteractionOutboundYoutube")
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

        public void RemoveInteractionControl(interactionCore.IInteraction interaction, object context)
        {
            if (!(interaction is InteractionOutboundYoutube))
                return;

            IDictionary<string, object> dictionary = context as IDictionary<string, object>;
            IBundleView bundleView = dictionary.TryGetValue<string, object>("BundleView") as IBundleView;
            if (bundleView != null)
            {
                this.viewManager.RemoveViewInRegion((object)bundleView, "BundleToolbarContainerRegion", interaction.InteractionId);
                this.viewManager.RemoveViewInRegion((object)bundleView, "InteractionsBundleRegion", interaction.InteractionId);
            }
            else
            {
                IView view = dictionary.TryGetValue<string, object>("InteractionBarInteractionView") as IView;
                if (view == null)
                    return;
                this.viewManager.RemoveViewInRegion((object)view, "BundleToolbarContainerRegion", interaction.InteractionId);
            }
        }

        public ImageSource GetInteractionMediaIcon(interactionCore.IInteraction interaction, InteractionMediaIconSource source)
        {
            return this.InteractionWorkItemControlExtension.GetInteractionMediaIcon(GetInteractionWorkItem(interaction), source);
        }

        public ImageSource GetInteractionSilentMonitoringMediaIcon(interactionCore.IInteraction interaction)
        {
            return (ImageSource)null;
        }

        public Icon GetInteractionMediaDrawingIcon(interactionCore.IInteraction interaction)
        {
            return this.InteractionWorkItemControlExtension.GetInteractionMediaDrawingIcon(GetInteractionWorkItem(interaction));
        }

        public string GetInteractionStateLabel(interactionCore.IInteraction interaction, BundleParty bundleParty)
        {
            return this.InteractionWorkItemControlExtension.GetInteractionStateLabel(GetInteractionWorkItem(interaction), bundleParty);
        }

        public IList<RequestedAction> RequestActions(string capacity, ActionTarget target, object context)
        {
            IList<RequestedAction> requestedActionList = this.InteractionWorkItemControlExtension.RequestActions(capacity, target, context);
            Dictionary<string, object> contextDictionary = context as Dictionary<string, object>;

            switch (target)
            {
                case ActionTarget.InteractionActionFromWorkbin:
                    if (!this.container.Resolve<IAgent>().HasMediaCapacity(YoutubeWorkItemModule.MediaTypeModuleMedia))
                        return (IList<RequestedAction>)new List<RequestedAction>();
                    string str = contextDictionary.TryGetValue<string, object>("mediaDirectionType") as string;
                    if (string.IsNullOrEmpty(str) || str != "Outbound")
                        return (IList<RequestedAction>)new List<RequestedAction>();

                    requestedActionList = GetRequestedActionList(requestedActionList, contextDictionary);
                    break;
            }
            return requestedActionList;
        }

        private IList<RequestedAction> GetRequestedActionList(IList<RequestedAction> requestedActionList, Dictionary<string, object> contextDictionary)
        {
            try
            {
                string interactionId = contextDictionary.TryGetValue<string, object>("interactionId") as string;
                if (string.IsNullOrEmpty(interactionId))
                    return (IList<RequestedAction>)new List<RequestedAction>();
                string workbinId = contextDictionary.TryGetValue<string, object>("workbinId") as string;
                contextDictionary.TryGetValue<string, object>("parentId");

                RequestedAction requestedAction = new RequestedAction();
                requestedAction.Type = YoutubeWorkItemModule.MediaTypeModuleMedia;
                requestedAction.NameId = LanguageDictionaryHelper.DeleteFromWorkbinKey;
                requestedAction.TooltipId = LanguageDictionaryHelper.DeleteFromWorkbinKey;
                requestedAction.ImageId = LanguageDictionaryHelper.DeleteFromWorkbinImageKey;
                requestedAction.Order = 61;
                requestedAction.IsPossible = true;
                requestedAction.IsDefault = false;

                requestedAction.Action = (System.Action)(() => MoveToWorkbinAction(interactionId, workbinId));

                IList<RequestedAction> actionList = (IList<RequestedAction>)new List<RequestedAction>();
                foreach (RequestedAction requestAction in (IEnumerable<RequestedAction>)requestedActionList)
                {
                    if (requestAction.NameId != "Workbins.Workitem.MenuItemInteractionActionFromWorkbinMarkDone")
                        actionList.Add(requestAction);
                    else
                        requestedAction.Order = requestAction.Order;
                }
                actionList.Add(requestedAction);
                return actionList;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception in getting rquested actionlist", ex);
                return requestedActionList;
            }
        }

        private void MoveToWorkbinAction(string interactionId, string workbinId)
        {
            DeleteInteractionFromWorkbin.AsyncRun(this.container, new DeleteInteractionFromWorkbin.Parameters()
            {
                MediaType = YoutubeWorkItemModule.MediaTypeModuleMedia,
                InteractionId = interactionId,
                WorkbinId = workbinId,
                Timeout = YoutubeOptions.Default.ResponseWaitTime,
                IgnoreConfirmationWindow = false,
                ErrorResponseHandler = MoveToWorkbinErrorResponseHandler
            }, (UIElement)null);
        }

        private void MoveToWorkbinErrorResponseHandler(string error)
        {
            YoutubeOptions.Log.Error("Remove from workbin : " + error);
        }

        private void AddCommandParameterMedia(IDictionary<string, object> parameters)
        {
            IList<IMedia> availableMedia = this.container.Resolve<IAgent>().GetAvailableMedia(YoutubeWorkItemModule.MediaTypeModuleMedia);
            if (availableMedia.Count <= 0)
                return;
            parameters["CommandParameter"] = (object)availableMedia.First<IMedia>();
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

        public void UpdateCaseViewContextBeforeCreateCaseView(object context, interactionCore.IInteraction interaction)
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

        public override ImageSource GetInteractionMediaIconRecorded(interactionCore.IInteraction interaction,
          InteractionMediaIconSource source)
        {
            return (ImageSource)null;
        }

        public override ImageSource GetInteractionSilentMonitoringMediaIconRecorded(interactionCore.IInteraction interaction)
        {
            return (ImageSource)null;
        }

        private void MakeActionForParty(string routingBaseAction, IDictionary<string, object> contextDictionary)
        {
            contextDictionary.TryGetValue<string, object>("BundleParty");
            IInteractionsBundle interactionsBundle = contextDictionary.TryGetValue<string, object>("Bundle") as IInteractionsBundle;
            ICase @case = contextDictionary.TryGetValue<string, object>("Case") as ICase;
            IDictionary<string, object> parameters = (IDictionary<string, object>)new Dictionary<string, object>();
            this.ShareInformation(contextDictionary);
            KeyValueCollection keyValueCollection1 = contextDictionary.TryGetValue<string, object>("UserData") as KeyValueCollection;
            if (interactionsBundle.Parties.Count == 1)
            {
                if (keyValueCollection1.ContainsKey("IW_CallType"))
                    keyValueCollection1.Remove("IW_CallType");
                KeyValueCollection keyValueCollection2 = keyValueCollection1.GetAsKeyValueCollection("SharedInformation");
                if (keyValueCollection2 != null)
                {
                    KeyValueCollection keyValueCollection3 = keyValueCollection2.GetAsKeyValueCollection("UserData");
                    keyValueCollection1.Add(keyValueCollection3);
                    keyValueCollection1.Remove("SharedInformation");
                }
                keyValueCollection1["IW_BundleUid"] = (object)interactionsBundle.BundleId;
            }
            keyValueCollection1["IW_CaseUid"] = (object)interactionsBundle.CaseId;
            if (@case != null)
            {
                IList<interactionCore.IInteraction> list = (IList<interactionCore.IInteraction>)@case.Interactions.ToList<interactionCore.IInteraction>();
                if (list != null)
                {
                    foreach (interactionCore.IInteraction interaction in (IEnumerable<interactionCore.IInteraction>)list)
                    {
                        IInteractionOpenMedia interactionOpenMedia = (IInteractionOpenMedia)(interaction as InteractionOpenMedia);
                        if (interactionOpenMedia != null && interactionOpenMedia.MediaType == YoutubeWorkItemModule.MediaTypeModuleMedia)
                        {
                            if (keyValueCollection1.ContainsKey("IW_CaseUid"))
                                keyValueCollection1.Remove("IW_CaseUid");
                            if (keyValueCollection1.ContainsKey("IW_BundleUid"))
                                keyValueCollection1.Remove("IW_BundleUid");
                        }
                    }
                }
            }
            if (interactionsBundle.MainInteraction.UserData.ContainsKey("Contact.InteractionUCSId"))
            {
                string str = interactionsBundle.MainInteraction.UserData["Contact.InteractionUCSId"] as string;
                if (!string.IsNullOrEmpty(str))
                {
                    keyValueCollection1["IW_ParentInteractionsUCS"] = (object)str;
                    parameters.Add("ParentInteractionId", (object)str);
                }
            }
            keyValueCollection1[GenericAttachedDataKeys.desktop_OutboundItxType] = (object)"UnsolicitedTargeted";
            parameters.Add("UserData", (object)keyValueCollection1);
            parameters.Add("AlertSection", (object)interactionsBundle.CaseId);
            this.AddCommandParameterMedia(parameters);
            IChainOfCommand chainOfCommandByName = this.container.Resolve<ICommandManager>().GetChainOfCommandByName(routingBaseAction);
            if (chainOfCommandByName == null)
                return;
            Utils.ExecuteAsynchronousCommand(chainOfCommandByName, parameters, (UIElement)null);
        }
    }
}
