using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    public class AddCommentViewModel : ViewModelBase
    {
        public AddCommentViewModel()
        {
            Comment = new CommentViewModel();
        }

        private CommentViewModel _comment;
        public CommentViewModel Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }
    }
}
