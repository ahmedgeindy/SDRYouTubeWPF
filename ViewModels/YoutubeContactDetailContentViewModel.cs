using Genesyslab.Desktop.Modules.Contacts;
using Genesyslab.Desktop.Modules.Contacts.IWInteraction;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels.Base;
using Microsoft.Practices.Unity;
using System;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using System.Windows;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    public class YoutubeContactDetailContentViewModel : ViewModelBase, IYoutubeContactDetailContentViewModel
    {
        private readonly IUnityContainer container;

        private IContactHandling contactHandling;

        private IIWInteractionContent _interactionContent;

        public IIWInteractionContent InteractionContent
        {
            get
            {
                return this._interactionContent;
            }
            set
            {
                this._interactionContent = value;
                this.OnPropertyChanged("InteractionContent");
            }
        }

        public KeyValueCollection InteractionAttachedData
        {
            get
            {
                return Enterprise.Commons.Collections.KeyValueCollectionHelper.ConvertToEnterpriseKeyValueCollection(this.InteractionContent.InteractionAttributes.AllAttributes);
            }
        }


        private YoutubeViewModel _youtubeData;
        public YoutubeViewModel YoutubeData
        {
            get { return _youtubeData; }
            set { SetProperty(ref _youtubeData, value); }
        }

        public YoutubeContactDetailContentViewModel(IUnityContainer container, IContactHandling contactHandling)
        {
            this.container = container;
            this.contactHandling = contactHandling;

            YoutubeData = new YoutubeViewModel();
        }

        public void Load()
        {
        }

        public void Unload()
        {
            this.InteractionContent = null;
        }

        public void GetInteractionData()
        {
            try
            {
                KeyValueCollection userData =
                        Enterprise.Commons.Collections.KeyValueCollectionHelper.ConvertToEnterpriseKeyValueCollection(this.InteractionContent.InteractionAttributes.AllAttributes);


                var SDRInteractionID = userData.GetAsString("SDRInteractionID");
                var _commentId = userData.GetAsString("_youtubeCommentId");
                YoutubeOptions.Log.InfoFormat("SDRInteractionID is {0} , and _youtubeCommentId is {1}", SDRInteractionID, _commentId);
                //this.InteractionContent.InteractionAttributes :contain data from ucs database from interaction table 
                YoutubeOptions.Log.Info("startDate after call ucs: " + this.InteractionContent.InteractionAttributes.StartDate);
                YoutubeData.GetContactDetailContentData(userData, this.InteractionContent.InteractionAttributes.TypeId, YoutubeData_OutMessageDeleted, this.InteractionContent.InteractionAttributes.StartDate.ToString());
            }
            catch (Exception e)
            {
                YoutubeOptions.Log.Error("GetInteractionData", e);
            }
        }

        private void YoutubeData_OutMessageDeleted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {

                YoutubeOptions.Log.InfoFormat("YoutubeData_OutMessageDeleted sender {0} , Args {1}", sender, e);

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
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception", ex);
            }
        }

        private void UpdateCommentUserData(CommentViewModel commentVM, string index, IDataKey dataKey)
        {
            YoutubeOptions.Log.Info("Comment Deleted " + commentVM.Id);

            KeyValueCollection userData = new KeyValueCollection();
            userData = this.InteractionAttachedData;

            if (userData.ContainsKey(dataKey.IsDeleted + index))
            {
                userData.Remove(dataKey.IsDeleted + index);
            }

            userData.Add(dataKey.IsDeleted + index, commentVM.IsDeleted.ToString());


            if (userData.ContainsKey(dataKey.DeletedAt + index))
            {
                userData.Remove(dataKey.DeletedAt + index);
            }

            userData.Add(dataKey.DeletedAt + index, commentVM.DeletedAtUnixStr);


            if (userData.ContainsKey(dataKey.DeletedBy + index))
            {
                userData.Remove(dataKey.DeletedBy + index);
            }

            userData.Add(dataKey.DeletedBy + index, commentVM.DeletedBy);


            this.InteractionContent.InteractionAttributes.AllAttributes =
                        Enterprise.Commons.Collections.KeyValueCollectionHelper
                        .ConvertToPSDKKeyValueCollection(userData);

            UpdateItxInUCS.AsyncRun(this.container, new UpdateItxInUCS.Parameters()
            {
                InteractionContent = this.InteractionContent
            }, (UIElement)null);
        }
    }
}
