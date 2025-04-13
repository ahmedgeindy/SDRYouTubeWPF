﻿using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces
{
    public interface IYoutubeWorkItemView : IView
    {
        IYoutubeWorkItemViewModel Model { get; set; }
    }
}
