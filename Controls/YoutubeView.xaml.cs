using Genesyslab.Desktop.Modules.Windows.IWMessageBox;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls
{
    /// <summary>
    /// Interaction logic for YoutubeView.xaml
    /// </summary>
    public partial class YoutubeView : UserControl
    {
        private CommentViewModel _comment = new CommentViewModel();
        public YoutubeView()
        {
            InitializeComponent();

        }

        public void Create()
        {
            PostControl.CommentClick += PostControl_CommentClick;

            InComment.DeleteClick += InComment_DeleteClick;
            InComment.LikeClick += InComment_LikeClick;
            InComment.ReplyClick += InComment_ReplyClick;

            AddCommentControl.SendClick += AddCommentControl_SendClick;
            AddCommentControl.CloseClick += AddCommentControl_CloseClick;
        }

        public event ReturnBoolEventHandler CreateOutboundInteraction;

        private bool PostControl_CommentClick(object sender, RoutedActionEventArgs args)
        {
            return CallCreateOutboundInteraction(sender, args, MessageType.Comment);
        }

        private bool InComment_ReplyClick(object sender, RoutedActionEventArgs args)
        {
            return CallCreateOutboundInteraction(sender, args, MessageType.Reply);
        }

        private bool CallCreateOutboundInteraction(object sender, RoutedActionEventArgs args, MessageType msgType)
        {
            // create outbound interaction
            YoutubeOptions.Log.Info("create interaction, start send interactions");

            if (CreateOutboundInteraction == null)
            {
                YoutubeOptions.Log.Error("Cannot create interaction, reaseon: No Create outbound interaction event subscribed");

                return false;
            }
            var vm = this.DataContext as YoutubeViewModel;
            if (vm == null)
            {
                YoutubeOptions.Log.Error("Cannot create interaction, reaseon: No YoutubeViewModel binded");
                return false;
            }

            _comment = new CommentViewModel()
            {
                AuthorImage = vm.Post.AuthorImage,
                AuthorName = vm.Post.AuthorName,
                PostId = vm.Post.Id,
                ParentId = args.ID,
                Type = msgType
            };
            YoutubeOptions.Log.Info("_comment interaction" + _comment.AuthorName + "," + _comment.ParentId + "," + _comment.Type);

            CreateOutboundInteraction(sender, args);
            YoutubeOptions.Log.Info("after call CreateOutboundInteraction");

            return true;
        }

        public void OpenEditor()
        {
            if (AddCommentControl.Visibility == Visibility.Collapsed)
            {
                AddCommentControl.Visibility = Visibility.Visible;
            }

            AddCommentControl.SendBtn.ButtonText =
                _comment.Type == MessageType.Reply ? LanguageDictionaryHelper.SendReplyText : LanguageDictionaryHelper.SendCommentText;

            AddCommentControl.DataContext = new AddCommentViewModel()
            {
                Comment = _comment,
            };
        }

        public void CloseEditor()
        {
            AddCommentControl.Visibility = Visibility.Collapsed;
            AddCommentControl.DataContext = new AddCommentViewModel();
        }

        public void AddCommentToViewAndCloseEditor()
        {
            var vm = DataContext as YoutubeViewModel;
            if (vm == null || _comment == null || _comment.Type == MessageType.None)
                return;

            if (_comment.Type == MessageType.Reply)
            {
                _comment.CommentDateUnixUtc = DateTime.Now;
                vm.ReplyMessages.Add(_comment);

            }
            else if (_comment.Type == MessageType.Comment)
            {
                _comment.CommentDateUnixUtc = DateTime.Now;
                vm.OutComments.Add(_comment);
            }

            CloseEditor();
        }

        #region AddComment Editor functions

        // send created interaction
        public event SendEventHandler SendOutboundInteraction;
  
        private bool AddCommentControl_SendClick(object sender, SendEventArgs args)
        {
            if (SendOutboundInteraction != null)
            {
                _comment = args.Comment;
                return SendOutboundInteraction(sender, args);
            }
            return false;
        }
        //private bool AddCommentControl_SendClick(object sender, SendEventArgs args)
        //{

        //    try
        //    {
        //        ShowLoading(); // Show spinner when starting

        //        if (SendOutboundInteraction != null)
        //        {

        //            Task.Run(() =>
        //            {
        //                try
        //                {
        //                    Thread.Sleep(5000); // Simulate work

        //                    Application.Current.Dispatcher.Invoke(() =>
        //                    {
        //                        _comment = args.Comment;

        //                       // return SendOutboundInteraction(sender, args);


        //                    });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Application.Current.Dispatcher.Invoke(() =>
        //                        { MessageBox.Show($"Error: {ex.Message}"); });
        //                }
        //            });
        //        }
        //        return false;
        //    }
        //    finally
        //    {
        //        HideLoading(); // Always hide spinner when done
        //    }
        //}

        public event RoutedEventHandler CloseClick;

        private void AddCommentControl_CloseClick(object sender, RoutedEventArgs e)
        {
            if (CloseClick != null)
            {
                CloseClick(sender, e);
            }
        }

        #endregion

        #region Delete

        public event ReturnBoolEventHandler DeleteClick;

        private bool InComment_DeleteClick(object sender, RoutedActionEventArgs args)
        {
            if (DeleteClick != null)
            {
                Window ConfirmParentWindow = (Window)PresentationSource.FromVisual((Visual)this).RootVisual;
                IWMessageBoxResult confirmResult = IWMessageBoxResult.None;

                var confirmDeleteMsg = LanguageDictionaryHelper.CommentDeleteConfirmation;

                ConfirmParentWindow.Dispatcher.Invoke((Action)(() =>
                {
                    confirmResult = IWMessageBoxView.Show(ConfirmParentWindow, confirmDeleteMsg, IWMessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    IWMessageBoxView.DestroyBoxResult();
                }));

                if (confirmResult == IWMessageBoxResult.Yes)
                {
                    var commentDeleted = DeleteClick(this, args);
                    return commentDeleted;
                }
            }
            return false;
        }

        #endregion

        #region Like

        public event ReturnBoolEventHandler LikeClick;

        private bool InComment_LikeClick(object sender, RoutedActionEventArgs args)
        {
            if (LikeClick != null)
            {
                var isLiked = LikeClick(this, args);

                return isLiked;
            }
            return false;
        }

        #endregion

        public event SendEventHandler OutCommentSendClick;

        private bool OutComment_SendClick(object sender, SendEventArgs args)
        {
            if (OutCommentSendClick != null)
            {
                return OutCommentSendClick(this, args);
            }
            return false;
        }

        private void InComment_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PostControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
