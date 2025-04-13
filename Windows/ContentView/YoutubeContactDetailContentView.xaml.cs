using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Infrastructure.ViewManager;
using Genesyslab.Desktop.Modules.Contacts.ContactDetail;
using Genesyslab.Desktop.Modules.Contacts.IWInteraction;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Agents;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Commands;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView
{
    /// <summary>
    /// Interaction logic for YoutubeContactDetailContentView.xaml
    /// </summary>
    public partial class YoutubeContactDetailContentView : UserControl, IYoutubeContactDetailContentView, IContactDetailContentView, IView, INotifyPropertyChanged
    {
        public readonly static string NAME;

        public readonly static string PARENT_REGION;

        private readonly IViewManager viewManager;

        private IUnityContainer _Container;

        static YoutubeContactDetailContentView()
        {
            YoutubeContactDetailContentView.NAME = typeof(YoutubeContactDetailContentView).FullName;
            YoutubeContactDetailContentView.PARENT_REGION = "ContactDetailRegion";
        }

        public YoutubeContactDetailContentView(IUnityContainer container, IYoutubeContactDetailContentViewModel viewModel, IViewManager viewManager)
        {
            this.Container = container;

            this.Model = viewModel;
            this.viewManager = viewManager;
            this.InitializeComponent();
            base.Width = double.NaN;
            base.Height = double.NaN;
        }

        public object Context { get; set; }

        public IUnityContainer Container
        {
            get
            {
                return this._Container;
            }
            set
            {
                this._Container = value;
                this.NotifyPropertyChanged("Container");
            }
        }

        public IYoutubeContactDetailContentViewModel Model
        {
            get
            {
                return base.DataContext as IYoutubeContactDetailContentViewModel;
            }
            set
            {
                base.DataContext = value;
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Create()
        {
            this.Model.Load();
        }

        public void CreateDetailDocument(IIWInteractionContent interactionContent)
        {
            try
            {
                this.Model.InteractionContent = interactionContent;
                this.Model.GetInteractionData();

                MainYoutubeView.DeleteClick += MainYoutubeView_DeleteClick;
            }
            catch (Exception e)
            {
                YoutubeOptions.Log.Error("ContactDetail Exception in getting data : ", e);
            }

        }

        private bool MainYoutubeView_DeleteClick(object sender, RoutedActionEventArgs args)
        {
            YoutubeOptions.Log.Info("ContactDetail DeleteComment Started");
            try
            {
                IMediaOpenMedia imedia = this.GetIMedia();
                var interactionAttr = this.Model.InteractionContent.InteractionAttributes.AllAttributes;

                YoutubeEspCommand.Parameters paramerters = new YoutubeEspCommand.Parameters()
                {
                    AppName = (interactionAttr).Get(GenericAttachedDataKeys._umsInboundIxnSubmittedBy) as string,
                    Endpoint = imedia.Channel.Endpoint,
                    ChannelMonitor = interactionAttr.Get(GenericAttachedDataKeys._umsChannelMonitor) as string,
                    Service = interactionAttr.Get(GenericAttachedDataKeys._umsChannel) as string,
                    TenantId = this.Model.InteractionContent.InteractionAttributes.TenantId.Value,
                    ParentId = args.ParentID,
                    MessageId = args.ID,
                    MessageName = EspRequestDefaultData.DeleteMethodName,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    ErrorResponseHandler = new ErrorResponseHandler(DeleteCommentErrorResponseHandler),
                    ResponseHandler = new ResponseHandler(args.ResponseHandler)
                };

                YoutubeOptions.Log.InfoFormat("ContactDetail Delete parameters {0}", paramerters);

                YoutubeEspCommand.AsyncRun(this.Container, paramerters, null);

                return true;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("ContactDetail Exception in DeleteComment: ", ex);
            }

            ShowErrMessage(string.Empty, LanguageDictionaryHelper.DeleteError);

            return false;
        }

        private void DeleteCommentErrorResponseHandler(string errorMessage)
        {
            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke((Delegate)new ErrorResponseHandler(this.DeleteCommentErrorResponseHandler), (object)errorMessage);
            else
            {
                YoutubeOptions.Log.Error("Error in Deleting Comment " + errorMessage);
                ShowErrMessage(string.Empty, LanguageDictionaryHelper.DeleteError);
            }
        }

        public IMediaOpenMedia GetIMedia()
        {
            try
            {
                IList<IMedia> availableMedia = new SocialMediaContainer(this.Container).Agent.GetAvailableMedia(YoutubeWorkItemModule.MediaTypeModuleMedia);
                if (availableMedia.Count > 0)
                    return availableMedia[0] as IMediaOpenMedia;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception", ex);
            }
            return (IMediaOpenMedia)null;
        }

        public void Destroy()
        {
            this.Model.Unload();
            base.Content = null;
        }

        private void ShowErrMessage(string errorCode, string message)
        {
            ShowErrorMessage.AsyncRun(this.Container, new ShowErrorMessage.Parameters()
            {
                SeverityType = SeverityType.Error,
                ErrorCode = errorCode,
                ErrorMessage = message,
                InteractionOpenMedia = null
            }, (UIElement)null);
        }

        private void MainYoutubeView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

