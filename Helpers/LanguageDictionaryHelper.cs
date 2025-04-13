using Tomers.WPF.Localization;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers
{
    public static class LanguageDictionaryHelper
    {
        public static string SendInteractionError => LanguageDictionary.Current.Translate<string>("Youtube.Actions.SendInteraction.Error", "Text");

        public static string CreateOutboundInteractionError => LanguageDictionary.Current.Translate<string>("Youtube.Actions.CreateInteraction.Error", "Text");
        public static string DeleteError => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Delete.Error", "Text");

        public static string LikeError => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Like.Error", "Text");

        public static string EmptyCommentSendError => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Send.EmptyError", "Text");
        public static string EmptyCommentSendErrorTitle => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Send.EmptyError", "Title");

        public static string LimitExceededError => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Send.LimitExceededError", "Text");
        public static string LimitExceededErrorTitle => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Send.LimitExceededError", "Title");

        public static string CommentDeleteConfirmation => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Delete.CofirmationWarningComment", "Text");
        public static string CommentDeleteConfirmationTitle => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Delete.CofirmationWarningComment", "Title");

        public static string ReplyDeleteConfirmation => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Delete.CofirmationWarningReply", "Text");
        public static string ReplyDeleteConfirmationTitle => LanguageDictionary.Current.Translate<string>("Youtube.Actions.Delete.CofirmationWarningReply", "Title");

        public static string SendCommentText => LanguageDictionary.Current.Translate<string>("Youtube.Actions.SendComment", "Text");
        public static string SendReplyText => LanguageDictionary.Current.Translate<string>("Youtube.Actions.SendReply", "Text");

        public static string DeleteFromWorkbinKey => "Windows.Youtube.Workbin.MenuItemInteractionActionFromWorkbinDelete";
        public static string DeleteFromWorkbinImageKey => "Windows.Youtube.Workbin.MenuItemInteractionActionFromWorkbinDelete.Image";
    }
}
