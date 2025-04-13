using Genesyslab.Desktop.Modules.Windows.IWMessageBox;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Models;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using Genesyslab.Desktop.Modules.Sdr.Common.Controls;
//using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls
{
    /// <summary>
    /// Interaction logic for Comment.xaml
    /// </summary>
    public partial class Comment : UserControl
    {
        public Comment()
        {
            InitializeComponent();
        }

        #region ClickEvents

        #region Delete

        public event ReturnBoolEventHandler DeleteClick;

        private void DeleteBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (DeleteClick != null)
            {
                var vm = this.DataContext as CommentViewModel;
                if (vm != null)
                {
                    DeleteClick(this, new RoutedActionEventArgs()
                    {
                        ID = vm.Id,
                        ParentID = vm.PostId,
                        ResponseHandler = new ResponseHandler(CommentDeleted),
                        ErrorResponseHandler = new ErrorResponseHandler(CommentDeletedError)
                    });
                }
            }
        }

        private void CommentDeletedError(string errorMsg)
        {
            if (!base.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke((Delegate)new ErrorResponseHandler(this.CommentDeletedError));
            }
            else
            {
                YoutubeOptions.Log.Error("Delete Comment Error " + errorMsg);
            }
        }

        private void CommentDeleted()
        {
            try
            {
                if (!base.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.Invoke((Delegate)new ResponseHandler(this.CommentDeleted));
                }
                else
                {
                    var vm = this.DataContext as CommentViewModel;
                    if (vm != null)
                    {
                        vm.DeletedAtUnixStr = DateTime.UtcNow.ToUTCTimeStampStrcom();
                        vm.DeletedBy = YoutubeModuleInit.Agent.ConfPerson.EmployeeID;
                        vm.IsDeleted = true;
                        vm.OnCommentDeleted();

                        //DeleteInfoVisibility = Visibility.Visible;
                        DeleteButtonVisibility = Visibility.Collapsed;
                        SendButtonVisibility = Visibility.Collapsed;
                        ReplyButtonVisibility = Visibility.Collapsed;
                        LikeButtonVisibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception exception)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem Error while handling CommentDeleted reponse handler", exception);
            }
        }

        #endregion

        #region Like

        public event ReturnBoolEventHandler LikeClick;

        private void LikeBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (LikeClick != null)
            {
                var vm = this.DataContext as CommentViewModel;
                if (vm != null)
                {
                    if (!vm.IsLiked)
                    {
                        LikeClick(this, new RoutedActionEventArgs()
                        {
                            ID = vm.Id,
                            ParentID = vm.PostId,
                            ResponseHandler = new ResponseHandler(CommentLiked),
                            // ErrorResponseHandler = new ErrorResponseHandler(CommentLikedError)
                        });
                    }
                }
            }
        }

        private void CommentLikedError(string errorMsg)
        {
            if (!base.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke((Delegate)new ErrorResponseHandler(this.CommentLikedError));
            }
            else
            {
                YoutubeOptions.Log.Error("Like Comment Error " + errorMsg);
            }
        }

        private void CommentLiked()
        {
            try
            {
                if (!base.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.Invoke((Delegate)new ResponseHandler(this.CommentLiked));
                }
                else
                {
                    var vm = this.DataContext as CommentViewModel;
                    if (vm != null)
                    {
                        vm.IsLiked = true;
                    }
                }
            }
            catch (Exception exception)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem Error while handling LikeCommentResponseHandler", exception);
            }
        }
        #endregion


        public event ReturnBoolEventHandler ReplyClick;

        private void ReplyBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (ReplyClick != null)
            {
                var vm = this.DataContext as CommentViewModel;
                YoutubeOptions.Log.Info("vm reply date" + vm.CommentDateUnixUtc);
               // vm.CommentDateUnixUtc= DateTime.Now;
                YoutubeOptions.Log.Info("vm reply date after change" + vm.CommentDateUnixUtc);


                //vm.CommentDateUnixUtc = DateTime.UtcNow;
                if (vm != null)
                {
                    ReplyClick(this, new RoutedActionEventArgs()
                    {
                        ID = vm.Id,
                        ParentID = vm.PostId
                    });
                }
            }
        }


        public event SendEventHandler SendClick;

        private void SendBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (SendClick == null)
                return;

            var vm = this.DataContext as CommentViewModel;
            vm.CommentDateUnixUtc = DateTime.UtcNow;
           
            YoutubeOptions.Log.Info("vm reply date"+vm.CommentDateUnixUtc);

            if (vm == null)
                return;

            if (string.IsNullOrWhiteSpace(vm.Text))
            {
                Window parentWindow = (Window)PresentationSource.FromVisual((Visual)this).RootVisual;

                var msg = LanguageDictionaryHelper.EmptyCommentSendError;

                parentWindow.Dispatcher.Invoke((Action)(() =>
                {
                    IWMessageBoxView.Show(parentWindow, msg, IWMessageBoxButtons.Ok, MessageBoxIcon.Error);
                    IWMessageBoxView.DestroyBoxResult();
                }));
                return;
            }

            if (vm.Text.Length > YoutubeOptions.Default.MaxMsgLenght)
            {
                Window parentWindow = (Window)PresentationSource.FromVisual((Visual)this).RootVisual;

                var msg = LanguageDictionaryHelper.LimitExceededError;

                parentWindow.Dispatcher.Invoke((Action)(() =>
                {
                    IWMessageBoxView.Show(parentWindow, msg, IWMessageBoxButtons.Ok, MessageBoxIcon.Error);
                    IWMessageBoxView.DestroyBoxResult();
                }));
                return;
            }

            YoutubeOptions.Log.InfoFormat("SendClick {0}", vm);
            SendClick(sender, new SendEventArgs() { Comment = vm });
        }

        private void EmojiPicker_SelectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var emoji = (sender as Control).DataContext as EmojiPicker;
            var vm = this.DataContext as CommentViewModel;
            if (emoji != null && vm != null)
            {
                var icon = emoji.SelectedIcon;
                var caretIndex = CommentTextBox.CaretIndex;

                var iconLenght = icon.Icon.Length;
                var prevCartIndex = CommentTextBox.CaretIndex;

                vm.Text = CommentTextBox.Text.Insert(CommentTextBox.CaretIndex, icon.Icon);

                CommentTextBox.CaretIndex = prevCartIndex + iconLenght;

                return;
            }
        }


        #endregion

        #region DeleteInfo

        public static readonly DependencyProperty deleteInfoVisibilityProperty =
DependencyProperty.Register(nameof(DeleteInfoVisibility), typeof(System.Windows.Visibility), typeof(Comment),
new PropertyMetadata(Visibility.Hidden, new PropertyChangedCallback(OnDeleteInfoVisibityChanged)));

        private static void OnDeleteInfoVisibityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Comment mycontrol = d as Comment;
            mycontrol.updateDeleteInfoVisibility(e);
        }

        private void updateDeleteInfoVisibility(DependencyPropertyChangedEventArgs e)
        {
            CommentDeleteInfo.Visibility = (Visibility)e.NewValue;
        }

        public System.Windows.Visibility DeleteInfoVisibility
        {
            get
            {
                return (System.Windows.Visibility)GetValue(deleteInfoVisibilityProperty);
            }
            set
            {
                SetValue(deleteInfoVisibilityProperty, value);
            }
        }
        #endregion

        #region ReplyButton

        public static readonly DependencyProperty replyButtonVisibilityProperty =
        DependencyProperty.Register(nameof(ReplyButtonVisibility), typeof(System.Windows.Visibility), typeof(Comment),
            new PropertyMetadata(Visibility.Hidden, new PropertyChangedCallback(OnReplyVisibityChanged)));

        private static void OnReplyVisibityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Comment mycontrol = d as Comment;
            mycontrol.updateReplyButtonVisibility(e);
        }

        private void updateReplyButtonVisibility(DependencyPropertyChangedEventArgs e)
        {
            ReplyBtn.Visibility = (Visibility)e.NewValue;
        }

        public System.Windows.Visibility ReplyButtonVisibility
        {
            get
            {
                return (System.Windows.Visibility)GetValue(replyButtonVisibilityProperty);
            }
            set
            {
                SetValue(replyButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region DeleteButton

        public static readonly DependencyProperty deleteButtonVisibilityProperty =
DependencyProperty.Register(nameof(DeleteButtonVisibility), typeof(System.Windows.Visibility), typeof(Comment),
    new PropertyMetadata(Visibility.Hidden, new PropertyChangedCallback(OnDeleteVisibityChanged)));

        private static void OnDeleteVisibityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Comment mycontrol = d as Comment;
            mycontrol.updateDeleteButtonVisibility(e);
        }

        private void updateDeleteButtonVisibility(DependencyPropertyChangedEventArgs e)
        {
            DeleteBtn.Visibility = (Visibility)e.NewValue;
        }

        public System.Windows.Visibility DeleteButtonVisibility
        {
            get
            {
                return (System.Windows.Visibility)GetValue(deleteButtonVisibilityProperty);
            }
            set
            {
                SetValue(deleteButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region LikeButton

        public static readonly DependencyProperty likeButtonVisibilityProperty =
            DependencyProperty.Register(nameof(LikeButtonVisibility), typeof(System.Windows.Visibility), typeof(Comment),
                new PropertyMetadata(Visibility.Hidden, new PropertyChangedCallback(OnLikeVisibityChanged)));

        private static void OnLikeVisibityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Comment mycontrol = d as Comment;
            mycontrol.updateLikeButtonVisibility(e);
        }

        private void updateLikeButtonVisibility(DependencyPropertyChangedEventArgs e)
        {
            LikeBtn.Visibility = (Visibility)e.NewValue;
        }
        public System.Windows.Visibility LikeButtonVisibility
        {
            get
            {
                return (System.Windows.Visibility)GetValue(likeButtonVisibilityProperty);
            }
            set
            {
                SetValue(likeButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region SendButton

        public static readonly DependencyProperty sendButtonVisibilityProperty =
DependencyProperty.Register(nameof(SendButtonVisibility), typeof(System.Windows.Visibility), typeof(Comment),
    new PropertyMetadata(Visibility.Hidden, new PropertyChangedCallback(OnSendVisibityChanged)));

        private static void OnSendVisibityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Comment mycontrol = d as Comment;
            mycontrol.updateSendButtonVisibility(e);
        }

        private void updateSendButtonVisibility(DependencyPropertyChangedEventArgs e)
        {
            SendBtn.Visibility = (Visibility)e.NewValue;
        }

        public System.Windows.Visibility SendButtonVisibility
        {
            get
            {
                return (System.Windows.Visibility)GetValue(sendButtonVisibilityProperty);
            }
            set
            {
                SetValue(sendButtonVisibilityProperty, value);
            }
        }

        #endregion
    }
}
