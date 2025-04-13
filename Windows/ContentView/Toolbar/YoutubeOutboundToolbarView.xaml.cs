using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.SocialMedia.LogPrint;
using Genesyslab.Desktop.Modules.Windows.Interactions;
using Genesyslab.Desktop.Modules.Windows.IWMessageBox;
using Genesyslab.Desktop.Modules.Windows.Views.Interactions.InteractionToolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces.Toolbar;
using Genesyslab.Desktop.WPFCommon;
using Genesyslab.Enterprise.Model.Interaction;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Tomers.WPF.Localization;


namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Toolbar
{
    /// <summary>
    /// Interaction logic for YoutubeOutboundToolbarView.xaml
    /// </summary>
    public partial class YoutubeOutboundToolbarView : ToolBar, IYoutubeOutboundToolbarView
    {
        public static readonly string NAME = typeof(YoutubeOutboundToolbarView).FullName;
        public static readonly string PARENT_REGION = "BundleToolbarContainerRegion";
        private IUnityContainer container;
        private IViewManager viewManager;

        public YoutubeOutboundToolbarView(
            IYoutubeOutboundToolbarViewModel interactionWorkItemToolbarViewModel,
            IUnityContainer container,
            IViewManager viewManager)
        {
            this.container = container;
            this.viewManager = viewManager;
            this.Model = interactionWorkItemToolbarViewModel;
            this.InitializeComponent();
            this.Width = double.NaN;
            this.Height = double.NaN;
        }

        public object Context { get; set; }

        public void Create()
        {
            IDictionary<string, object> context = this.Context as IDictionary<string, object>;
            this.Model.Initialize();
            this.Model.Interaction = context.TryGetValue<string, object>("Interaction") as IInteractionOutboundYoutube;
            this.Model.Interaction.PropertyChanged += new PropertyChangedEventHandler(this.Interaction_PropertyChanged);
            this.ChangeToolbarVisibility();
            HelperToolbarFramework.SetButtonStyle(context, (IButtonStyle)this.Model);
        }

        private void Interaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(DispatcherPriority.Send, (Delegate)new Action<object, PropertyChangedEventArgs>(this.Interaction_PropertyChanged), sender, (object)e);
            }
            else
            {
                switch (e.PropertyName)
                {
                    case "IsCancelEnabled":
                    case "IsTransferring":
                    case "IsItPossibleToOneStepTransfer":
                    case "IsItPossibleToMoveToWorkbin":
                        this.ChangeToolbarVisibility();
                        break;
                }
            }
        }

        private void ChangeToolbarVisibility()
        {
            if (this.Model == null || this.Model.Interaction == null)
                return;
            if (this.Model.Interaction.IsCancelEnabled || this.Model.Interaction.IsItPossibleToOneStepTransfer || this.Model.Interaction.IsItPossibleToMoveToWorkbin)
                this.Visibility = Visibility.Visible;

            else
                this.Visibility = Visibility.Collapsed;
        }

        public IYoutubeOutboundToolbarViewModel Model
        {
            get
            {
                return this.DataContext as IYoutubeOutboundToolbarViewModel;
            }
            set
            {
                this.DataContext = (object)value;
            }
        }

        public void Destroy()
        {
            YoutubeOptions.Log.Info(" YoutubeoutboundToolbarView Destroy -> start");

            this.Model.Interaction.PropertyChanged -= new PropertyChangedEventHandler(this.Interaction_PropertyChanged);
            this.Model.Release();
        }

        public int SortIndex
        {
            get
            {
                return 30;
            }
        }

        private IAgent Agent
        {
            get
            {
                return this.container.Resolve<IAgent>();
            }
        }

        private void buttonTransfer_Click(object sender, RoutedEventArgs e)
        {
            PrintOut.PrintUIAction(e);
            if (this.Model.Interaction.UnsavedUserData.Count > 0)
                UpdateUserData.AsyncRun(this.container, new UpdateUserData.Parameters()
                {
                    itx = this.Model.Interaction.EntrepriseInteractionCurrent,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    UserData = this.Model.Interaction.UnsavedUserData,
                    ResponseHandler = new UpdateUserData.ResponseHandler(this.UpdateUserDataResponseTransferHandler)
                }, (UIElement)this.buttonTransfer);
            else
                this.UpdateUserDataResponseTransferHandler((IOpenMediaInteraction)null);
        }

        public void UpdateUserDataResponseTransferHandler(IOpenMediaInteraction response)
        {
            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke((Delegate)new UpdateUserData.ResponseHandler(this.UpdateUserDataResponseTransferHandler), (object)response);
            else
            {
                YoutubeOptions.Log.InfoFormat("UpdateUserDataResponseTransferHandler response {0}", response);

                this.container.Resolve<ICommandManager>().GetChainOfCommandByName("TeamCommunicatorOpenInteraction")?.Execute((IDictionary<string, object>)new Dictionary<string, object>(this.Context as IDictionary<string, object>)
                {
                    {"Button",(object) this.buttonTransfer},
                    {"MediaType",(object) this.Model.Interaction.MediaType},
                    { "ActionTarget", (object) ActionTarget.OneStepTransferDialer},
                    { "UserData",
                        (object) new Dictionary<string, object>()
                        {
                            { "IW_CaseUid", (object) this.Model.Interaction.ExternalCaseId },
                            { "IW_BundleUid", (object) null }
                        }
                    },
                    { "CaseUID", (object) this.Model.Interaction.CaseId },
                    { "CommandParameter", (object) this.Model.Interaction }
                });
            }
        }

        private void buttonMoveToWorkbin_Click(object sender, RoutedEventArgs e)
        {
            PrintOut.PrintUIAction(e);

            if (this.Model.Interaction.UnsavedUserData.Count > 0)
                UpdateUserData.AsyncRun(this.container, new UpdateUserData.Parameters()
                {
                    itx = this.Model.Interaction.EntrepriseInteractionCurrent,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    UserData = this.Model.Interaction.UnsavedUserData,
                    ResponseHandler = new UpdateUserData.ResponseHandler(this.UpdateUserDataResponseWorkbinHandler)
                }, (UIElement)null);
            else
                this.UpdateUserDataResponseWorkbinHandler((IOpenMediaInteraction)null);
        }

        public void UpdateUserDataResponseWorkbinHandler(IOpenMediaInteraction response)
        {
            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke((Delegate)new UpdateUserData.ResponseHandler(this.UpdateUserDataResponseWorkbinHandler), (object)response);
            else
            {
                YoutubeOptions.Log.InfoFormat("UpdateUserDataResponseWorkbinHandler response {0}", response);

                Utils.ExecuteAsynchronousCommand(new SocialMediaContainer(this.container).CommandManager.GetChainOfCommandByName("InteractionWorkItemMoveToWorkbin"),
                (IDictionary<string, object>)new Dictionary<string, object>()
                {
                  {
                    "CommandParameter",
                    (object) this.Model.Interaction
                  }
                }, (UIElement)this.buttonMoveToWorkbin);
            }
        }


        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ToolBar_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            PrintOut.PrintUIAction(e);
            this.Model.Interaction.IsCancelEnabled = false;
            Window ConfirmParentWindow = (Window)PresentationSource.FromVisual((Visual)this).RootVisual;
            string message = LanguageDictionary.Current.Translate<string>("SocialMedia.Confirmation.DeleteInteraction", "Message");
            message = string.Format(message, (object)YoutubeWorkItemModule.MediaTypeModuleMedia);
            IWMessageBoxResult confirmResult = IWMessageBoxResult.None;
            ConfirmParentWindow.Dispatcher.Invoke((Action)(() =>
            {
                confirmResult = IWMessageBoxView.Show(ConfirmParentWindow, message, IWMessageBoxButtons.YesNo, MessageBoxIcon.Question);
                IWMessageBoxView.DestroyBoxResult();
            }));
            if (confirmResult == IWMessageBoxResult.Yes)
            {
                IWMessageBoxView.DestroyBoxResult();
                DeleteInteraction.AsyncRun(this.container, new DeleteInteraction.Parameters()
                {
                    Interaction = (IInteractionOpenMedia)this.Model.Interaction,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    ErrorResponseHandler = new DeleteInteraction.ErrorResponseHandler(this.DeleteInteractionErrorResponseHandler)
                }, (UIElement)this.buttonCancel);
            }
            else
                this.Model.Interaction.IsCancelEnabled = true;
        }

        public void DeleteInteractionErrorResponseHandler(string errorMessage)
        {
            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke((Delegate)new DeleteInteraction.ErrorResponseHandler(this.DeleteInteractionErrorResponseHandler), (object)errorMessage);
            else
                this.Model.Interaction.IsCancelEnabled = true;
        }
    }

}
