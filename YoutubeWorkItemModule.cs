using System;
using System.Collections.Generic;
using Genesyslab.Desktop.Infrastructure;
using Microsoft.Practices.Unity;
using Genesyslab.Platform.Commons.Logging;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView;
using Genesyslab.Desktop.Modules.Windows.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Infrastructure.Configuration;
using Genesyslab.Desktop.Modules.Windows.Event;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Microsoft.Practices.Composite.Events;
using System.Reflection;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions;
using Genesyslab.Desktop.Modules.Windows.Views.Toaster;
using Genesyslab.Desktop.Modules.OpenMedia.Windows.Toaster;
using Genesyslab.Desktop.Modules.Contacts.ContactDetail;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Toolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Toolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Models;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces.Toolbar;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem
{
    public class YoutubeWorkItemModule : IModule
	{
        public static string MediaTypeModuleMedia = "youtube";

        private readonly IUnityContainer container;
        private readonly ILogger log;
        private readonly IConfigManager configManager;
        private readonly IViewManager viewManager;
        private readonly IViewEventManager viewEventManager;
        private readonly IInteractionManager interactionManager;
        private readonly ICommandManager commandManager;
        private readonly IAgent agent;

        public YoutubeWorkItemModule(IUnityContainer container, ILogger log, IConfigManager configManager,
            IViewManager viewManager, IViewEventManager viewEventManager, IInteractionManager interactionManager,
            ICommandManager commandManager, IAgent agent, IEventAggregator eventAggregator)
        {
            try
            {
                this.container = container;
                this.log = log.CreateChildLogger(nameof(YoutubeWorkItemModule));
                this.log.Debug((object)"YoutubeWorkItemModule()");
                YoutubeOptions.CreateInstance(configManager, log);

                this.configManager = configManager;
                this.viewManager = viewManager;
                this.viewEventManager = viewEventManager;
                this.interactionManager = interactionManager;
                this.commandManager = commandManager;
                this.agent = agent;

                YoutubeModuleInit.Agent = agent;

                Assembly workspaceAssembly = Assembly.GetEntryAssembly();
                AssemblyName workspaceAssemblyName = workspaceAssembly.GetName();
                string workspaceVersion = workspaceAssemblyName.Version.ToString();
                string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                YoutubeOptions.Log.Info("Constructor(): Version = " + workspaceVersion + ", Customization Version = " + pluginVersion);
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.FatalError("Excpetion in YoutubeWorkItemModule ctor(), ", ex);
            }
        }

        public void Initialize()
        {
            YoutubeOptions.Log.Debug((object)"Initialize()");
            try
            {
                this.RegisterViewsAndServices();
                this.container.Resolve<IRoutingBasedInteractionYoutube>();
                this.RegisterViewsWithRegions();
                this.RegisterCommands();
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.FatalError("Error during the module initialization: ", ex);
                throw (ex);
            }
        }

        protected void RegisterViewsAndServices()
        {
            YoutubeOptions.Log.Info((object)"RegisterViewsAndServices()");

            this.container.RegisterType<IInteractionManagerInteractionOpenMediaExtension,
             InteractionManagerInteractionOpenMediaExtensionYoutube>("InteractionYoutube",
             (LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            // inbound
            this.container.RegisterType<IInteractionYoutube, InteractionYoutube>(new InjectionMember[0]);
            this.container.RegisterType<IInteractionControlExtension, InteractionYoutubeControlExtension>
                ("InteractionYoutube", (LifetimeManager)new ContainerControlledLifetimeManager(),
                new InjectionMember[0]);

            //outbound 
            this.container.RegisterType<IInteractionOutboundYoutube, InteractionOutboundYoutube>
                (new InjectionMember[0]);
            this.container.RegisterType<IInteractionControlExtension, InteractionOutboundYoutubeControlExtension>
                ("InteractionOutboundYoutube", (LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            this.container.RegisterType<IYoutubeWorkItemViewModel, YoutubeWorkItemViewModel>();
            this.container.RegisterType<IYoutubeWorkItemView, YoutubeWorkItemView>();


            // Toaster inbound
            this.container.RegisterType<IToasterControllerInteractionExtension, ToasterControllerInteractionYoutubeExtension>
                ("ToasterControllerInteractionYoutubeExtension", (LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            this.container.RegisterType<IWorkItemToasterExtension, YoutubeToasterExtension>
                ("InteractionYoutube", (LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            // Toaster outbound
            this.container.RegisterType<IToasterControllerInteractionExtension, ToasterControllerInteractionOutboundYoutubeExtension>
                ("ToasterControllerInteractionOutboundYoutubeExtension", (LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            this.container.RegisterType<IWorkItemToasterExtension, YoutubeToasterExtension>
                ("InteractionOutboundYoutube", (LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);


            this.container.RegisterType<IRoutingBasedInteractionYoutube, RoutingBasedInteractionYoutube>((LifetimeManager)new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            this.container.RegisterType<IContactDetailControlExtension, YoutubeContactDetailControlExtension>
                    (YoutubeContactDetailControlExtension.NAME, new ContainerControlledLifetimeManager(), new InjectionMember[0]);

            this.container.RegisterType<IYoutubeContactDetailContentView, YoutubeContactDetailContentView>(new InjectionMember[0]);
            this.container.RegisterType<IYoutubeContactDetailContentViewModel, YoutubeContactDetailContentViewModel>(new InjectionMember[0]);

            // toolbar inbound
            this.container.RegisterType<IYoutubeInboundToolbarView, YoutubeInboundToolbarView>(new InjectionMember[0]);
            this.container.RegisterType<IYoutubeInboundToolbarViewModel, YoutubeInboundToolbarViewModel>(new InjectionMember[0]);

            // toolbar outbound
            this.container.RegisterType<IYoutubeOutboundToolbarView, YoutubeOutboundToolbarView>(new InjectionMember[0]);
            this.container.RegisterType<IYoutubeOutboundToolbarViewModel, YoutubeOutboundToolbarViewModel>(new InjectionMember[0]);
        }

        void RegisterViewsWithRegions()
        {
            YoutubeOptions.Log.Debug((object)"RegisterViewsWithRegions()");

            IViewManager viewManager = this.container.Resolve<IViewManager>();

            viewManager.AddViewsToRegion(YoutubeWorkItemView.PARENT_REGION, new List<ViewActivator>()
            {
                new ViewActivator()
                {
                    ViewType = typeof(IYoutubeWorkItemView),
                    ActivateView = true,
                    ViewName = YoutubeWorkItemView.NAME,
                    DynamicOnly = true,
                    CreateNewRegionManager = true
                }
            });

            viewManager.AddViewsToRegion(YoutubeContactDetailContentView.PARENT_REGION, new List<ViewActivator>()
            {
                new ViewActivator()
                {
                    ViewType = typeof(IYoutubeContactDetailContentView),
                    ActivateView = true,
                    ViewName = YoutubeContactDetailContentView.NAME,
                    DynamicOnly = true,
                    CreateNewRegionManager = true
                }
            });

            viewManager.AddViewsToRegion(YoutubeInboundToolbarView.PARENT_REGION, (IList<ViewActivator>)new List<ViewActivator>()
            {
              new ViewActivator()
              {
                ViewType = typeof (IYoutubeInboundToolbarView),
                ActivateView = true,
                ViewName = YoutubeInboundToolbarView.NAME,
                DynamicOnly = true,
                CreateNewRegionManager = true
              }
            });

            viewManager.AddViewsToRegion(YoutubeOutboundToolbarView.PARENT_REGION, (IList<ViewActivator>)new List<ViewActivator>()
            {
              new ViewActivator()
              {
                ViewType = typeof (IYoutubeOutboundToolbarView),
                ActivateView = true,
                ViewName = YoutubeOutboundToolbarView.NAME,
                DynamicOnly = true,
                CreateNewRegionManager = true
              }
            });
        }

        private void RegisterCommands()
        {
            ICommandManager commandManager = this.container.Resolve<ICommandManager>();

            //commandManager.AddCommandToChainOfCommand("InteractionYoutubeMoveToWorkbin",
            //    this.commandManager.CommandsByName["InteractionWorkItemMoveToWorkbin"]);

            //this.commandManager.InsertCommandToChainOfCommandBefore("InteractionYoutubeMoveToWorkbin",
            //    "IsWorkbinDestinationDefined", 
            //    (IList<CommandActivator>)new List<CommandActivator>()
            //    {
            //      new CommandActivator()
            //      {
            //        CommandType = typeof (SaveUnsavedUserDataCommandYoutube),
            //        Name = "SetPramsForIsWorkbinDestinationDefined"
            //      }
            //    });

            //this.commandManager.InsertCommandToChainOfCommandBefore("ApplicationClose",
            //    "ChannelsLogOff",
            //    (IList<CommandActivator>)new List<CommandActivator>()
            //{
            //  new CommandActivator()
            //  {
            //    CommandType = typeof (BeforeChannelsLogOff),
            //    Name = "BeforeChannelsLogOff"
            //  }
            //});
        }
    }
}
