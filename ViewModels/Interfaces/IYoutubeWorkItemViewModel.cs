using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Enterprise.Commons.Collections;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces
{
    public interface IYoutubeWorkItemViewModel
    {
        IInteractionOpenMedia Interaction { get; set; }

        void CreateWorkItem();
        KeyValueCollection GetCommentKvList(CommentViewModel comment, out string messageIndex);
    }
}
