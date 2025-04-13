using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using Microsoft.Practices.Unity;
using System;
using Genesyslab.Desktop.Modules.OpenMedia.View.WorkbinsView;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    public class YoutubeWorkItemViewModel : ViewModelBase, IYoutubeWorkItemViewModel
    {
        private readonly IUnityContainer _container;

        public YoutubeWorkItemViewModel(IUnityContainer container)
        {
            YoutubeData = new YoutubeViewModel();
            _container = container;
        }

        private void InComment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(YoutubeData.InComment.IsLiked))
            {
                Interaction.SetAttachedData(YoutubeDataKey.CommentData.IsLiked, YoutubeData.InComment.IsLiked.ToString());
            }
            else if (e.PropertyName == nameof(YoutubeData.InComment.IsDeleted))
            {
                Interaction.SetAttachedData(YoutubeDataKey.CommentData.IsDeleted, YoutubeData.InComment.IsDeleted.ToString());

                // update attached data for out comments:
                for (int i = 0; i < YoutubeData.ReplyMessages.Count; i++)
                {
                    YoutubeData.ReplyMessages[i].IsDeleted = YoutubeData.InComment.IsDeleted;

                    Interaction.SetAttachedData(YoutubeDataKey.ReplyData.IsDeleted + YoutubeData.ReplyMessages[i].Index,
                        YoutubeData.InComment.IsDeleted.ToString());
                }
            }
            else if (e.PropertyName == nameof(YoutubeData.InComment.DeletedBy))
            {
                Interaction.SetAttachedData(YoutubeDataKey.CommentData.DeletedBy, YoutubeData.InComment.DeletedBy);

                // update attached data for out comments:
                for (int i = 0; i < YoutubeData.ReplyMessages.Count; i++)
                {
                    YoutubeData.ReplyMessages[i].DeletedBy = YoutubeData.InComment.DeletedBy;

                    Interaction.SetAttachedData(YoutubeDataKey.ReplyData.DeletedBy + YoutubeData.ReplyMessages[i].Index,
                        YoutubeData.InComment.DeletedBy);
                }
            }
            else if (e.PropertyName == nameof(YoutubeData.InComment.DeletedAtUnixStr))
            {
                Interaction.SetAttachedData(YoutubeDataKey.CommentData.DeletedAt, YoutubeData.InComment.DeletedAtUnixStr);
                
                // update attached data for out comments:
                for (int i = 0; i < YoutubeData.ReplyMessages.Count; i++)
                {
                    YoutubeData.ReplyMessages[i].DeletedAtUnixStr = YoutubeData.InComment.DeletedAtUnixStr;

                    Interaction.SetAttachedData(YoutubeDataKey.ReplyData.DeletedAt + YoutubeData.ReplyMessages[i].Index,
                        YoutubeData.InComment.DeletedAtUnixStr);
                }
            }
        }

        #region moved from view

        public void CreateWorkItem()
        {
            if (Interaction != null)
            {
                GetInteractionType();

                KeyValueCollection youtubeData = Interaction.ExtractAttachedData();

               YoutubeData.CreateMainViewWorkItem(youtubeData, IsOutboundInteraction, YoutubeData_OutMessageTextChanged);

                YoutubeData.InComment.PropertyChanged += InComment_PropertyChanged;

                InitButtonsVisibilty();
            }
        }

        private void YoutubeData_OutMessageTextChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(CommentViewModel.Text))
                {
                    YoutubeOptions.Log.InfoFormat("YoutubeData_OutMessageTextChanged sender {0} , Args {1}", sender, e);

                    var commentVM = sender as CommentViewModel;
                    if (commentVM == null)
                        return;

                    var index = commentVM.Index.ToString();
                    IDataKey dataKey = YoutubeDataKey.ReplyData;

                    switch (commentVM.Type)
                    {
                        case MessageType.Comment:
                            dataKey = YoutubeDataKey.CommentData;
                            break;
                        case MessageType.Reply:
                            dataKey = YoutubeDataKey.ReplyData;
                            break;
                    }

                    UpdateCommentUserData(commentVM, index, dataKey);
                }
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception", ex);
            }
        }

        private void UpdateCommentUserData(CommentViewModel commentVM, string index, IDataKey dataKey)
        {
            YoutubeOptions.Log.Info("Updating comment data Text " + commentVM.Text);

            if (this.OutboundInteraction.UnsavedUserData.ContainsKey(dataKey.Text + index))
            {
                this.OutboundInteraction.UnsavedUserData[dataKey.Text + index] = commentVM.Text;
            }
            else
            {
                this.OutboundInteraction.UnsavedUserData.Add(dataKey.Text + index, commentVM.Text);
            }
        }

        private void InitButtonsVisibilty()
        {
            YoutubeData.Post.CanComment = !IsOutboundInteraction;

            YoutubeData.InComment.CanDelete = !IsOutboundInteraction;
            YoutubeData.InComment.CanLike = !IsOutboundInteraction;
            YoutubeData.InComment.CanReply = !IsOutboundInteraction;

            YoutubeData.InComment.CanEdit = false;
        }

        private void GetInteractionType()
        {
            IInteractionOpenMedia interactionOpenMedia = Interaction as IInteractionOpenMedia;

            if (interactionOpenMedia != null &&
                interactionOpenMedia.EntrepriseOpenMediaInteractionCurrent != null)
            {
                var entrepriseOpenMediaInteractionCurrent =
                    interactionOpenMedia.EntrepriseOpenMediaInteractionCurrent as Enterprise.Interaction.OpenMediaInteraction;

                if (entrepriseOpenMediaInteractionCurrent != null)
                {
                    IsOutboundInteraction = entrepriseOpenMediaInteractionCurrent.InteractionType == "Outbound";
                    UpdateOutboundInteractionIsCancelEnabled(true);
                }
            }
        }

        public void UpdateOutboundInteractionIsCancelEnabled(bool value)
        {
            if (OutboundInteraction != null)
            {
                OutboundInteraction.IsCancelEnabled = value;
            }
        }



        #endregion


        #region IYoutubeWorkItemViewModel Members

        private IInteractionOpenMedia interaction;
        public IInteractionOpenMedia Interaction
        {
            get { return interaction; }
            set
            {
                if (interaction != value)
                {
                    SetProperty(ref interaction, value);
                }
            }
        }

        public IInteractionOutboundYoutube OutboundInteraction
        {
            get
            {
                if (Interaction != null && IsOutboundInteraction)
                {
                    return Interaction as IInteractionOutboundYoutube;
                }
                return (IInteractionOutboundYoutube)null;
            }
        }

        private bool _isOutboundInteraction;
        public bool IsOutboundInteraction
        {
            get { return _isOutboundInteraction; }
            set { SetProperty(ref _isOutboundInteraction, value); }
        }

        private YoutubeViewModel _youtubeData;
        public YoutubeViewModel YoutubeData
        {
            get { return _youtubeData; }
            set { SetProperty(ref _youtubeData, value); }
        }


        #endregion

        public KeyValueCollection GetCommentKvList(CommentViewModel comment, out string messageIndex)
        {
            IDataKey dataKey = YoutubeDataKey.ReplyData;
            
            switch (comment.Type)
            {
                case MessageType.Comment:
                    comment.Index = YoutubeData.OutComments.Count;
                    dataKey = YoutubeDataKey.CommentData;
                    break;
                case MessageType.Reply:
                    comment.Index = YoutubeData.ReplyMessages.Count;
                    dataKey = YoutubeDataKey.ReplyData;
                    break;
            }

            messageIndex = comment.Index.ToString();
            var kv = comment.GetCommentKvList(dataKey);
            UpdateInteractionAttachedData(kv);

            return kv;
        }

        private void UpdateInteractionAttachedData(KeyValueCollection kv)
        {
            foreach (var item in kv.AllKeys)
            {
                SetInteractionAttachedData(item, kv.Get(item));
            }
        }

        private void SetInteractionAttachedData(string key, object value)
        {
            try
            {
                Interaction.SetAttachedData(key, value);
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("SetInteractionAttachedData , ", ex);
            }
        }

    }
}
