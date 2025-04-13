using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces.Toolbar;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces.Toolbar
{
    public interface IYoutubeInboundToolbarView : IView
    {
        IYoutubeInboundToolbarViewModel Model { get; set; }
    }
}
