using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    public class YoutubeViewModel : ViewModelBase
    {
        public YoutubeViewModel()
        {

        }

        internal void CreateWorkItem(KeyValueCollection data,string replyDate)
        {
            if (data != null)
            {
                Post = data.GetPostData();

                InComment = data.GetInCommentData(Post.Id, Post.AuthorName);
                YoutubeOptions.Log.Info("CreateWorkItem AuthorName  " + Post.AuthorName);

                YoutubeOptions.Log.Info("CreateWorkItem replydate  " + YoutubeDataKey.ReplyData.Date);

                ReplyMessages = data.GetOutReplyList(YoutubeDataKey.ReplyData, Post.Id, MessageType.Reply, InComment.Id, replyDate, Post.AuthorName);

                OutComments = data.GetOutList(YoutubeDataKey.CommentData, Post.Id, MessageType.Comment, Post.Id, replyDate, Post.AuthorName);
            }
        }

        internal void CreateMainViewWorkItem(KeyValueCollection youtubeData, bool isOutboundInteraction, PropertyChangedEventHandler outMessageTextChanged)
        {
            CreateWorkItem(youtubeData,null);

            IsOutbound = isOutboundInteraction;
           // IsOutbound = false;
            IsHistory = false;

            if (IsOutbound)
            {
                OutMessageTextChanged += outMessageTextChanged;
                InitializeOutbound(youtubeData);
            }
        }

        private void InitializeOutbound(KeyValueCollection userData)
        {
            var msgType = userData.GetInteractionMethodType();
            var msgIndex = userData.GetInteractionMethodIndex();

            if (msgIndex < 0 || (msgType != MessageType.Comment && msgType != MessageType.Reply))
                return;

            if (MessageType.Reply == msgType)
            {
                SetupOutboundMessage(ReplyMessages, msgIndex);
                return;
            }

            if (MessageType.Comment == msgType)
            {
                SetupOutboundMessage(OutComments, msgIndex);
                return;
            }
        }

        private void SetupOutboundMessage(ObservableCollection<CommentViewModel> comments, int index)
        {
            if (index < comments.Count)
            {
                comments[index].CanEdit = true;
                comments[index].PropertyChanged += OutMessageText_PropertyChanged;
            }
        }

        public event PropertyChangedEventHandler OutMessageTextChanged;

        private void OutMessageText_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutMessageTextChanged != null && e.PropertyName == nameof(CommentViewModel.Text))
            {
                OutMessageTextChanged(sender, e);
            }
        }

        internal void GetContactDetailContentData(KeyValueCollection userData, string typeId, PropertyChangedEventHandler outMessageDeleted,string replydate)
        {


            CreateWorkItem(userData, replydate);

            IsHistory = true;
            IsOutbound = typeId == "Outbound";
            if (IsOutbound) InitializeHistoryDelete(userData, outMessageDeleted);
        }

        internal void InitializeHistoryDelete(KeyValueCollection userData, PropertyChangedEventHandler outMessageDeleted)
        {
            try
            {
                var msgType = userData.GetInteractionMethodType();
                var msgIndex = userData.GetInteractionMethodIndex();
                var id = userData.GetInteractionMethodID();

                if (id == string.Empty || msgIndex < 0 || (msgType != MessageType.Comment && msgType != MessageType.Reply))
                    return;

                if (MessageType.Reply == msgType && msgIndex < ReplyMessages.Count)
                {
                    ReplyMessages[msgIndex].CanDelete = true;
                    ReplyMessages[msgIndex].Id = id;
                    ReplyMessages[msgIndex].CommentDeletedEvent += outMessageDeleted;
                    return;
                }

                if (MessageType.Comment == msgType && msgIndex < OutComments.Count)
                {
                    OutComments[msgIndex].CanDelete = true;
                    OutComments[msgIndex].Id = id;
                    OutComments[msgIndex].CommentDeletedEvent += outMessageDeleted;
                    return;
                }
            }
            catch (Exception ex)
            {
               YoutubeOptions.Log.Error("Exception in InitializeHistoryDelete ", ex);
            }
        }

        private bool _isOutbound = false;
        public bool IsOutbound
        {
            get { return _isOutbound; }
            set { SetProperty(ref _isOutbound, value); }
        }

        private bool _isHistory = false;
        public bool IsHistory
        {
            get { return _isHistory; }
            set { SetProperty(ref _isHistory, value); }
        }

        #region post

        private PostViewModel _post;
        public PostViewModel Post
        {
            get { return _post; }
            set
            {
                SetProperty(ref _post, value);
            }
        }

        #endregion

        #region Comment

        private CommentViewModel _inComment;
        public CommentViewModel InComment
        {
            get { return _inComment; }
            set
            {
                SetProperty(ref _inComment, value);
            }
        }

        #endregion

        #region Replies

        private ObservableCollection<CommentViewModel> _replyMessages;
        public ObservableCollection<CommentViewModel> ReplyMessages
        {
            get { return _replyMessages; }
            set
            {
                SetProperty(ref _replyMessages, value);
            }
        }

        private CommentViewModel _replyMessage;
        public CommentViewModel ReplyMessage
        {
            get { return _replyMessage; }
            set { SetProperty(ref _replyMessage, value); }
        }

        #endregion

        #region OutComments

        private ObservableCollection<CommentViewModel> _outComments;
        public ObservableCollection<CommentViewModel> OutComments
        {
            get { return _outComments; }
            set
            {
                SetProperty(ref _outComments, value);
            }
        }

        #endregion

        #region Comment Text box

        private AddCommentViewModel _addComment;
        public AddCommentViewModel AddComment
        {
            get { return _addComment; }
            set { SetProperty(ref _addComment, value); }
        }

        #endregion
    }
}
