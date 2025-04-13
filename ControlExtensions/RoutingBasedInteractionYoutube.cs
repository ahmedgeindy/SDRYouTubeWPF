using Genesyslab.Desktop.Modules.Core.Model.RoutingBase;
using Genesyslab.Desktop.Modules.OpenMedia.Model.RoutingBase;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Platform.Commons.Logging;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ControlExtensions
{
    internal interface IRoutingBasedInteractionYoutube : IRoutingBasedOpenMedia, IRoutingBased
    {
    }

    internal class RoutingBasedInteractionYoutube : IRoutingBasedInteractionYoutube, IRoutingBasedOpenMedia, IRoutingBased
    {
        protected readonly IUnityContainer container;
        protected IRoutingBasedManager routingBaseManager;
        protected ILogger log;

        public RoutingBasedInteractionYoutube(IUnityContainer container, IRoutingBasedManager routingBaseManager, ILogger log)
        {
            this.log = log.CreateChildLogger(nameof(RoutingBasedInteractionYoutube));
            if (log.IsDebugEnabled)
                this.log.Debug((object)"RoutingBasedInteractionYoutube()");

            this.routingBaseManager = routingBaseManager;
            this.container = container;
            this.RoutingBasedInteractionWorkItem =
                (IRoutingBasedOpenMedia)container.Resolve<IRoutingBasedInteractionWorkItem>();

            this.routingBaseManager.Subscribe(typeof(IInteractionYoutube), (IRoutingBased)this);
            this.routingBaseManager.Subscribe(typeof(IInteractionOutboundYoutube), (IRoutingBased)this);
        }

        private IRoutingBasedOpenMedia RoutingBasedInteractionWorkItem { get; set; }

        public string Name
        {
            get
            {
                return this.GetType().ToString();
            }
        }

        public bool RequestToDo(string action, RoutingBasedTarget target, IDictionary<string, object> parameters)
        {
            return this.RoutingBasedInteractionWorkItem.RequestToDo(action, target, parameters);
        }
    }
}
