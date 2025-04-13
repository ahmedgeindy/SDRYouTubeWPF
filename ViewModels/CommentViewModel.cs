using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
//using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Enterprise.Commons.Collections;
//using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    public class CommentViewModel : ViewModelBase
    {
        public CommentViewModel()
        {
            AuthorImage = new AuthorImageViewModel();
        }
        
        private string _ReplyAuthorName = string.Empty;
        public string ReplyAuthorName
        {
            get { return _ReplyAuthorName; }
            set { SetProperty(ref _ReplyAuthorName, value); }
        }
        private int _index = -1;
        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        private string _postId = string.Empty;
        public string PostId
        {
            get { return _postId; }
            set { SetProperty(ref _postId, value); }
        }

        private string _parentId = string.Empty; // parent post or comment
        public string ParentId
        {
            get { return _parentId; }
            set { SetProperty(ref _parentId, value); }
        }

        private MessageType _type = MessageType.None;
        public MessageType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string AuthorNameText => Text == string.Empty ? string.Empty : AuthorName;

        private string _authorName = string.Empty;
        public string AuthorName
        {
            get { return _authorName; }
            set
            {
                SetProperty(ref _authorName, value);
                OnPropertyChanged(nameof(AuthorNameText));
            }
        }

        private AuthorImageViewModel _authorImage;
        public AuthorImageViewModel AuthorImage
        {
            get { return _authorImage; }
            set { SetProperty(ref _authorImage, value); }
        }

        private string _text = string.Empty;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value);
            }
        }

        private DateTime _commentDateUnixUtc;
        public DateTime CommentDateUnixUtc
        {
            get { return _commentDateUnixUtc; }
            set
            {
                SetProperty(ref _commentDateUnixUtc, value);
                OnPropertyChanged(nameof(DateText));
            }
        }

        public string DateText => CommentDateUnixUtc.ToString();

        #region Delete

        private bool AgendCanRemoveComments
        {
            get
            {
                try
                {
                    var canRemove = YoutubeOptions.Default.Task[YoutubeTask.CanDelete];
                    return canRemove;
                }
                catch (Exception ex)
                {
                    YoutubeOptions.Log.Error("Cannot find option CanDelete", ex);
                    return true;
                }
            }
        }

        private bool _canDelete = false;
        public bool CanDelete
        {
            get
            {
                return _canDelete;
            }
            set
            {
                SetProperty(ref _canDelete, value);
                OnPropertyChanged(nameof(DeleteButtonVisibilty));
            }
        }

        public Visibility DeleteButtonVisibilty => !IsDeleted && CanDelete ? Visibility.Visible : Visibility.Collapsed;

        private bool _canReply = false;
        public bool CanReply
        {
            get { return _canReply; }
            set
            {
                SetProperty(ref _canReply, value);
                OnPropertyChanged(nameof(ReplyButtonVisibilty));
            }
        }

        public Visibility ReplyButtonVisibilty => !IsDeleted && CanReply ? Visibility.Visible : Visibility.Collapsed;

        private bool _isDeleted;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                SetProperty(ref _isDeleted, value);
                OnPropertyChanged(nameof(DeleteDetailsVisibilty));
                OnPropertyChanged(nameof(DeletedAtText));
                OnPropertyChanged(nameof(DeletedBy));
                OnPropertyChanged(nameof(IsDeleted));
                OnPropertyChanged(nameof(DeleteButtonVisibilty));
                OnPropertyChanged(nameof(ReplyButtonVisibilty));
            }
        }

        public Visibility DeleteDetailsVisibilty => IsDeleted ? Visibility.Visible : Visibility.Collapsed;

        public string DeletedAtText => Helpers.DateTimeHelper.GetLocalDateFromUnix(DeletedAtUnixStr).ToString();

        private string _deletedAtUnixStr;// = DateTime.UtcNow.ToUTCTimeStampStr(); // utc time stamp
        public string DeletedAtUnixStr
        {
            get { return _deletedAtUnixStr; }
            set
            {
                SetProperty(ref _deletedAtUnixStr, value);
                OnPropertyChanged(nameof(DeletedAtText));
            }
        }

        private string _deletedBy = string.Empty;
        public string DeletedBy
        {
            get { return _deletedBy; }
            set
            {
                SetProperty(ref _deletedBy, value);
            }
        }

        #endregion

        #region Like

        private bool _canLike = false;
        public bool CanLike
        {
            get { return _canLike; }
            set
            {
                SetProperty(ref _canLike, value);
                OnPropertyChanged(nameof(LikeButtonVisibilty));
            }
        }

        public Visibility LikeButtonVisibilty => CanLike || IsLiked ? Visibility.Visible : Visibility.Collapsed;

        private bool _isLiked;
        public bool IsLiked
        {
            get { return _isLiked; }
            set
            {
                SetProperty(ref _isLiked, value);
                OnPropertyChanged(nameof(LikeButtonFill));
                OnPropertyChanged(nameof(LikeButtonVisibilty));
            }
        }

        public Brush LikeButtonFill => IsLiked ?
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0077b5")) :
            new SolidColorBrush(Colors.Gray);

        #endregion

        #region Edit Outboundview

        private bool _canEdit = false;
        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }
            set
            {
                SetProperty(ref _canEdit, value);
                OnPropertyChanged(nameof(IsReplyTextReadOnly));
                OnPropertyChanged(nameof(SendButtonVisibility));
                OnPropertyChanged(nameof(ReplyCurser));
            }
        }

        public bool IsReplyTextReadOnly => !CanEdit;
        public Visibility SendButtonVisibility => !IsDeleted && CanEdit ? Visibility.Visible : Visibility.Collapsed;
        public Cursor ReplyCurser => IsReplyTextReadOnly ? Cursors.Arrow : Cursors.Hand;

        #endregion
        //old (IDataKey dataKey)
        internal KeyValueCollection GetCommentKvList(IDataKey dataKey)
        {
            KeyValueCollection kv = new KeyValueCollection();
            YoutubeOptions.Log.Info("youtubeReplyDate datakvlist" + CommentDateUnixUtc);

            kv.AddItem(dataKey.Index + Index, Index);

            kv.AddItem(dataKey.AuthorName + Index, AuthorName);
            //old kv.AddItem(dataKey.Date + Index, CommentDateUnixUtc);
            kv.AddItem(dataKey.Date + Index, CommentDateUnixUtc.ToUTCTimeStampStr());

            kv.AddItem(dataKey.Text + Index, Text);

            kv.AddItem(dataKey.AuthorImage + Index, AuthorImage.Image.ImageByte);

            kv.AddItem(dataKey.ID + Index, Id);

            kv.AddItem(dataKey.IsLiked + Index, IsLiked);

            // delete info.
            kv.AddItem(dataKey.DeletedAt + Index, DeletedAtUnixStr);
            kv.AddItem(dataKey.DeletedBy + Index, DeletedBy);
            kv.AddItem(dataKey.IsDeleted + Index, IsDeleted);

            return kv;
        }

        public event PropertyChangedEventHandler CommentDeletedEvent;
        public void OnCommentDeleted()
        {
            if (CommentDeletedEvent != null)
            {
                CommentDeletedEvent(this, new PropertyChangedEventArgs("CommentDeleted"));
            }
        }
    }
}
