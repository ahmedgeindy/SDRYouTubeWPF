using Genesyslab.Desktop.Modules.Contacts.IWInteraction;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces
{
    public interface IYoutubeContactDetailContentViewModel
    {
        IIWInteractionContent InteractionContent
        {
            get;
            set;
        }

        void Load();

        void Unload();

        void GetInteractionData();
    }
}
