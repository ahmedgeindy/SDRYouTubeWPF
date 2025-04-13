using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Modules.Windows.Views.Interactions.InteractionToolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar;
using Genesyslab.Desktop.WPFCommon.Controls;
using Microsoft.Practices.Unity;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Toolbar
{
    public class YoutubeInboundToolbarViewModel : ViewModelBase, IYoutubeInboundToolbarViewModel, IButtonStyle
    {
        private readonly IUnityContainer container;
        private readonly ICommandManager commandManager;
        private IInteractionYoutube interaction;
        private ButtonStyle buttonStyle;

        public YoutubeInboundToolbarViewModel(
          IUnityContainer container,
          ICommandManager commandManager)
        {
            this.container = container;
            this.commandManager = commandManager;
        }

        public IInteractionYoutube Interaction
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
            this.Interaction = (IInteractionYoutube)null;
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
