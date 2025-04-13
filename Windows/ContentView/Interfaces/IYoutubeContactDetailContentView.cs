using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Modules.Contacts.ContactDetail;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces
{
    public interface IYoutubeContactDetailContentView : IContactDetailContentView, IView
    {
        IYoutubeContactDetailContentViewModel Model
        {
            get;
            set;
        }
    }
}
