using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using Genesyslab.Desktop.Modules.Windows.IWMessageBox;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Genesyslab.Desktop.Modules.Sdr.Common.Controls;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls
{
    /// <summary>
    /// Interaction logic for AddComment.xaml
    /// </summary>
    public partial class AddComment : UserControl
    {
        public AddComment()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                    new System.Action(delegate ()
                    {
                        ReplyTextBox.Focus();
                    }));
            }
        }

        public event SendEventHandler SendClick;

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SendClick == null)
                return;

            var vm = this.DataContext as AddCommentViewModel;

            if (vm == null)
                return;

            if (string.IsNullOrWhiteSpace(vm.Comment.Text))
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

            if (vm.Comment.Text.Length > YoutubeOptions.Default.MaxMsgLenght)
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

            SendClick(sender, new SendEventArgs()
            {
                Comment = vm.Comment
            });
        }

        public event RoutedEventHandler CloseClick;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (CloseClick != null)
            {
                CloseClick(sender, e);
            }
        }


        private void EmojiPicker_SelectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var emoji = (sender as Control).DataContext as EmojiPicker;
            var vm = this.DataContext as AddCommentViewModel;
            if (emoji != null && vm != null)
            {
                var icon = emoji.SelectedIcon;
                var caretIndex = ReplyTextBox.CaretIndex;

                var iconLenght = icon.Icon.Length;
                var prevCartIndex = ReplyTextBox.CaretIndex;

                vm.Comment.Text = ReplyTextBox.Text.Insert(ReplyTextBox.CaretIndex, icon.Icon);

                ReplyTextBox.CaretIndex = prevCartIndex + iconLenght;

                return;
            }
        }

        #region ButtonText

        public static readonly DependencyProperty buttonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(AddComment),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnButtonTextPropertyChanged)));

        private static void OnButtonTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddComment mycontrol = d as AddComment;
            mycontrol.updateButtonText(e);
        }

        private void updateButtonText(DependencyPropertyChangedEventArgs e)
        {
            SendBtn.Content = (string)e.NewValue;
        }

        public string ButtonText
        {
            get { return (string)GetValue(buttonTextProperty); }
            set { SetValue(buttonTextProperty, value); }
        }

        #endregion
    }
}
