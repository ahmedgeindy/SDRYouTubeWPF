using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.Windows.Views.Interactions.InteractionToolbar;
using Genesyslab.Desktop.WPFCommon.Controls;
using Microsoft.Practices.Unity;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Toolbar
{
    public class YoutubeOutboundToolbarViewModel : ViewModelBase, IYoutubeOutboundToolbarViewModel, IButtonStyle
    {
        private readonly IUnityContainer container;
        private readonly ICommandManager commandManager;
        private IInteractionOutboundYoutube interaction;
        private ButtonStyle buttonStyle;

        public YoutubeOutboundToolbarViewModel(
          IUnityContainer container,
          ICommandManager commandManager)
        {
            this.container = container;
            this.commandManager = commandManager;
        }

        public IInteractionOutboundYoutube Interaction
        {
            get
            {
                return this.interaction;
            }
            set
            {
                if (this.interaction == value)
                    return;
                this.interaction = value;
                this.OnPropertyChanged(nameof(Interaction));
            }
        }

        public void Initialize()
        {
        }

        public void Release()
        {
            this.Interaction = (IInteractionOutboundYoutube)null;
        }

        public ButtonStyle ButtonStyle
        {
            get
            {
                return this.buttonStyle;
            }
            set
            {
                if (this.buttonStyle == value)
                    return;
                this.buttonStyle = value;
                this.OnPropertyChanged(nameof(ButtonStyle));
            }
        }
    }
}
