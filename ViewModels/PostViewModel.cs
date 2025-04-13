using Genesyslab.Desktop.Modules.SocialMedia.LogPrint;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using System;
using System.Windows;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    [PrintOut]
    public class PostViewModel : ViewModelBase
    {
        //internal static string AutherImagePath = "pack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Images/emptyprofile.png";


        public PostViewModel()
        {
            AuthorImage = new AuthorImageViewModel();
            AuthorName = Text = Id = string.Empty;
            PostImage = new ImageViewModel();
            PostDateInixUTCStr = DateTime.Now;
        }

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public string AuthorNameText => Text == string.Empty ? string.Empty : AuthorName;

        private string _authorName;
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
            set
            {
                SetProperty(ref _authorImage, value);
            }
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
                OnPropertyChanged(nameof(AuthorNameText));
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private ImageViewModel _postImage;
        public ImageViewModel PostImage
        {
            get { return _postImage; }
            set { SetProperty(ref _postImage, value); }
        }

        private DateTime _postDateUnixUTCStr = DateTime.Now;
        public DateTime PostDateInixUTCStr
        {
            get { return _postDateUnixUTCStr; }
            set
            {
                SetProperty(ref _postDateUnixUTCStr, value);
                OnPropertyChanged(nameof(DateText));
            }
        }

        public string DateText => PostDateInixUTCStr.ToString();

        private bool _canComment = false;
        public bool CanComment
        {
            get { return _canComment; }
            set
            {
                SetProperty(ref _canComment, value);
                OnPropertyChanged(nameof(CommentButtonVisibilty));
            }
        }

        public Visibility CommentButtonVisibilty => CanComment ? Visibility.Visible : Visibility.Collapsed;

    }
}
