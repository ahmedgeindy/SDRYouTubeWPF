using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Events
{
    public class SendEventArgs : YoutubeActionEventArgs
    {
        public CommentViewModel Comment { get; set; }
    }
}
