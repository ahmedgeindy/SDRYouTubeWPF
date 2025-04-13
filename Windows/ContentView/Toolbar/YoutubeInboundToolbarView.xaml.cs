using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.LogPrint;
using Genesyslab.Desktop.Modules.Windows.Interactions;
using Genesyslab.Desktop.Modules.Windows.Views.Interactions.InteractionToolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces.Toolbar;
using Genesyslab.Desktop.WPFCommon;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Toolbar
{
    /// <summary>
    /// Interaction logic for YoutubeInboundToolbarView.xaml
    /// </summary>
    public partial class YoutubeInboundToolbarView : ToolBar, IYoutubeInboundToolbarView
    {
        public static readonly string NAME = typeof(YoutubeInboundToolbarView).FullName;
        public static readonly string PARENT_REGION = "BundleToolbarContainerRegion";
        private IUnityContainer container;
        private IViewManager viewManager;

        public YoutubeInboundToolbarView(IYoutubeInboundToolbarViewModel interactionWorkItemToolbarViewModel,
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
            this.Model.Interaction = context.TryGetValue<string, object>("Interaction") as IInteractionYoutube;
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
            if (this.Model.Interaction.IsItPossibleToOneStepTransfer || this.Model.Interaction.IsItPossibleToMoveToWorkbin)
                this.Visibility = Visibility.Visible;
               // this.Visibility = Visibility.Collapsed;
            else
                this.Visibility = Visibility.Collapsed;
        }

        public IYoutubeInboundToolbarViewModel Model
        {
            get
            {
                return this.DataContext as IYoutubeInboundToolbarViewModel;
            }
            set
            {
                this.DataContext = (object)value;
            }
        }

        public void Destroy()
        {
            YoutubeOptions.Log.Info(" YoutubeInboundToolbarView Destroy -> start");
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
                { "CommandParameter", (object) this.Model.Interaction } });
        }

        private void buttonMoveToWorkbin_Click(object sender, RoutedEventArgs e)
        {
            PrintOut.PrintUIAction(e);
            Utils.ExecuteAsynchronousCommand(new SocialMediaContainer(this.container).CommandManager.GetChainOfCommandByName("InteractionWorkItemMoveToWorkbin"), (IDictionary<string, object>)new Dictionary<string, object>()
            {
              {
                "CommandParameter",
                (object) this.Model.Interaction
              }
            }, (UIElement)this.buttonMoveToWorkbin);
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ToolBar_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
