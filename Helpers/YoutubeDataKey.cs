namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers
{
    internal interface IDataKey
    {
        string ID { get; }
        string AuthorName { get; }
        string AuthorImage { get; }
        string Text { get; }
        string Date { get; }
        // from wde
        string IsLiked { get; }
        string DeletedAt { get; }
        string DeletedBy { get; }
        string IsDeleted { get; }

        string Index { get; }
    }

    internal class PostDataKey
    {
        public string ID => "videoID";// "postID";
        public string AuthorName => "videoAuthor";//"postAuthor";
        public string AuthorImage => "videoAuthorImg";// not yet
        public string Title => "videoTitle";
        public string Text => "videoDescription";//"postText";
        public string Date => "videoDate";
        public string LikesCount => "postLikes";
        public string Image => "videoImg";// "postImg";
    }

    internal class CommentDataKey : IDataKey
    {
        public string ID => "commentID"; //"commentID";
        public string AuthorName => "commentAuthor"; // "commentAuthor";
        public string AuthorImage => "commentAuthorImg"; //"commentAuthorImage";
        public string Text => "commentText"; //"commentText";
        public string Date => "commentDate";// "commentDate";
        // send from wde
        public string IsLiked => "commentIsLiked";
        public string DeletedAt => "commentDeletedAt";
        public string DeletedBy => "commentDeletedBy";
        public string IsDeleted => "commentIsDeleted";

        public string Index => "commentIndex";
    }
    internal class ReplyDataKey : IDataKey
    {
        public string ID => "replyID";
        public string AuthorName => "replyAuthor";
        public string AuthorImage => "replyAuthorImage";
        public string Text => "replyText";
        public string Date => "replyDate";
        // send from wde
        public string IsLiked => "replyIsLiked";
        public string DeletedAt => "replyDeletedAt";
        public string DeletedBy => "replyDeletedBy";
        public string IsDeleted => "replyIsDeleted";

        public string Index => "replyIndex";
    }

    internal static class YoutubeDataKey
    {
        public static PostDataKey PostData { get; set; }
        public static CommentDataKey CommentData { get; set; }
        public static ReplyDataKey ReplyData { get; set; }

        public static string OutboundMethodType => "MethodType";

        public static string Version => "Version";
        public static string Method => "Method";
        public static string Service => "Service";
        public static string MediaType => "MediaType";
        public static string AppName => "AppName";
        public static string TenantId => "TenantId";
        public static string Parameters => "Parameters";

        public static string MessageType => "_sampleMsgType";
        public static string ParentID => "PostID";
        public static string MessageID => "ID";
        public static string MessageText => "MsgText";

        public static string MsgIndex => "MsgIndex";
        public static string OutboundMsgId => "_youtubeCommentId"; // "SDRInteractionID";

        public static string CaseUid => "IW_CaseUid";
        public static string BundleUid => "IW_BundleUid";
        public static string Contact_Id => "Contact.Id";

        static YoutubeDataKey()
        {
            PostData = new PostDataKey();
            CommentData = new CommentDataKey();
            ReplyData = new ReplyDataKey();
        }
    }
}
