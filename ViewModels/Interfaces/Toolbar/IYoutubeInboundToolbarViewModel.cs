﻿using Genesyslab.Desktop.Modules.Windows.Views.Interactions.InteractionToolbar;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar
{
    public interface IYoutubeInboundToolbarViewModel : IButtonStyle
    {
        IInteractionYoutube Interaction { get; set; }

        void Initialize();

        void Release();
    }
}
