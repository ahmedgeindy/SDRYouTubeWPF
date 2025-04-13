using Genesyslab.Desktop.Infrastructure;
using Genesyslab.Desktop.Infrastructure.Events;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Agents;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.SocialMedia.LogPrint;
using Genesyslab.Desktop.Modules.Windows.IWMessageBox;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Commands;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels.Interfaces;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView.Interfaces;
using Genesyslab.Enterprise.Interaction;
using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.Objects;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Genesyslab.Enterprise.Commons.Collections;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Animation;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Windows.ContentView
{
    /// <summary>
    /// Interaction logic for YoutubeWorkItemView.xaml
    /// </summary>
    public partial class YoutubeWorkItemView : UserControl, IYoutubeWorkItemView
    {
        public static readonly string NAME = typeof(YoutubeWorkItemView).FullName;
        public static readonly string PARENT_REGION = "InteractionsBundleRegion";
        private readonly Storyboard _showLoadingAnimation;
        private readonly Storyboard _hideLoadingAnimation;
        private bool _isAnimating = false;

        public YoutubeWorkItemView(IYoutubeWorkItemViewModel customWorkItemViewModel, IUnityContainer container)
        {
            Model = customWorkItemViewModel;
            InitializeComponent();
            _showLoadingAnimation = (Storyboard)MainYoutubeView.LoadingOverlay.Resources["ShowLoadingAnimation"];
            _hideLoadingAnimation = (Storyboard)MainYoutubeView.LoadingOverlay.Resources["HideLoadingAnimation"];

            _hideLoadingAnimation.Completed += (s, e) =>
            {
                MainYoutubeView.LoadingOverlay.Visibility = Visibility.Collapsed;
                _isAnimating = false;
            };
            this.Container = container;

            Width = Double.NaN;
            Height = Double.NaN;
        }

        #region property chanegd

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion


        #region IView Members

        public IUnityContainer Container { get; set; }

        public IRegionManager RegionManager { get; set; }

        public IInteractionOpenMedia ParentInteraction
        {
            get
            {
                return this.Model.Interaction;
            }
        }

        private IOpenMediaInteraction _EditedInteraction;
        public IOpenMediaInteraction EditedInteraction
        {
            get
            {
                return this._EditedInteraction;
            }
            set
            {
                this._EditedInteraction = value;
                this.NotifyPropertyChanged(nameof(EditedInteraction));
            }
        }

        public object Context { get; set; }

        private ToolbarButtonState buttonsState { get; set; }

        public void Create()
        {
            IDictionary<string, object> contextDictionary = Context as IDictionary<string, object>;
            Model.Interaction = contextDictionary.TryGetValue("Interaction") as IInteractionOpenMedia;
            if (Model.Interaction != null)
            {
                Model.CreateWorkItem();
            }

            InitializeButtonsState();

            MainYoutubeView.Create();
            MainYoutubeView.DeleteClick += MainYoutubeView_DeleteClick;
            MainYoutubeView.LikeClick += MainYoutubeView_LikeClick;

            MainYoutubeView.CreateOutboundInteraction += MainYoutubeView_CreateOutboundInteraction;

            MainYoutubeView.SendOutboundInteraction += MainYoutubeView_SendOutboundInteraction;
            MainYoutubeView.CloseClick += MainYoutubeView_CloseClick;

            // Send from outbound view
            MainYoutubeView.OutCommentSendClick += MainYoutubeView_OutCommentSendClick;
        }

        public void Destroy()
        {
            YoutubeOptions.Log.InfoFormat("Destroying {0}", NAME);

            try
            {
                if (EditedInteraction != null)
                {
                    YoutubeOptions.Log.InfoFormat("Stop Edited interaction {0} ", EditedInteraction);
                    StopEditedInteraction((UIElement)null);

                    this.EditedInteraction = (IOpenMediaInteraction)null;
                    this.Container = (IUnityContainer)null;
                }
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception in Destroying YoutubeWorkItemView, ", ex);
            }
        }
        #endregion

        #region IYoutubeWorkItemView Members

        public IYoutubeWorkItemViewModel Model
        {
            get { return this.DataContext as IYoutubeWorkItemViewModel; }
            set { this.DataContext = value; }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PrintOut.PrintUIAction(e);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            YoutubeOptions.Log.Info("UserControl_Unloaded");
            PrintOut.PrintUIAction(e);
        }

        #region toolbar buttons

        private void InitializeButtonsState()
        {
            buttonsState = new ToolbarButtonState();
            buttonsState.IsItPossibleToMarkDone = this.Model.Interaction.IsItPossibleToMarkDone;
            //buttonsState.IsItPossibleToMarkDone = true;
            buttonsState.IsItPossibleToMoveToWorkbin = this.Model.Interaction.IsItPossibleToMoveToWorkbin;
            buttonsState.IsItPossibleToOneStepTransfer = this.Model.Interaction.IsItPossibleToOneStepTransfer;
        }

        private void DisableToolbarButtons()
        {
            if (this.ParentInteraction != null)
            {
                ((Genesyslab.Desktop.Modules.Core.Model.Interactions.Interaction)this.ParentInteraction).IsItPossibleToMarkDone = false;
                ((InteractionOpenMedia)this.ParentInteraction).IsItPossibleToOneStepTransfer = false;
                ((InteractionOpenMedia)this.ParentInteraction).IsItPossibleToMoveToWorkbin = false;
            }
        }

        private void ResetButtons()
        {
            if (this.ParentInteraction != null && this.buttonsState != null)
            {
                ((Genesyslab.Desktop.Modules.Core.Model.Interactions.Interaction)this.ParentInteraction).IsItPossibleToMarkDone =
                    this.buttonsState.IsItPossibleToMarkDone;
                ((InteractionOpenMedia)this.ParentInteraction).IsItPossibleToOneStepTransfer =
                    this.buttonsState.IsItPossibleToOneStepTransfer;
                ((InteractionOpenMedia)this.ParentInteraction).IsItPossibleToMoveToWorkbin =
                    this.buttonsState.IsItPossibleToMoveToWorkbin;
            }
        }

        #endregion

        #region DeleteComment

        private bool MainYoutubeView_DeleteClick(object sender, RoutedActionEventArgs args)
        {
            YoutubeOptions.Log.Info("YoutubeWorkItem DeleteComment Started");
            try
            {
                //ResetButtons();

                OpenMediaInteraction interaction = ParentInteraction.EntrepriseInteractionCurrent as OpenMediaInteraction;

                YoutubeEspCommand.Parameters paramerters = new YoutubeEspCommand.Parameters()
                {
                    AppName = ParentInteraction.GetAttachedData(GenericAttachedDataKeys._umsInboundIxnSubmittedBy) as string,
                    Endpoint = interaction.ActiveChannel.Endpoint,
                    ChannelMonitor = ParentInteraction.GetAttachedData(GenericAttachedDataKeys._umsChannelMonitor) as string,
                    Service = ParentInteraction.GetAttachedData(GenericAttachedDataKeys._umsChannel) as string,
                    TenantId = interaction.TenantID,
                    ParentId = args.ParentID,
                    MessageId = args.ID,
                    MessageName = EspRequestDefaultData.DeleteMethodName,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    ErrorResponseHandler = new ErrorResponseHandler(DeleteCommentErrorResponseHandler),
                    ResponseHandler = new ResponseHandler(args.ResponseHandler)
                };

                YoutubeEspCommand.AsyncRun(this.Container, paramerters, null);

                return true;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem Exception in DeleteComment: ", ex);
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

        #endregion

        #region Like

        private bool MainYoutubeView_LikeClick(object sender, RoutedActionEventArgs args)
        {
            YoutubeOptions.Log.Info("YoutubeWorkItem MainYoutubeView_LikeClick Started");
            try
            {
                OpenMediaInteraction interaction = ParentInteraction.EntrepriseInteractionCurrent as OpenMediaInteraction;

                YoutubeEspCommand.Parameters paramerters = new YoutubeEspCommand.Parameters()
                {
                    AppName = ParentInteraction.GetAttachedData(GenericAttachedDataKeys._umsInboundIxnSubmittedBy) as string,
                    Endpoint = interaction.ActiveChannel.Endpoint,
                    ChannelMonitor = ParentInteraction.GetAttachedData(GenericAttachedDataKeys._umsChannelMonitor) as string,
                    Service = ParentInteraction.GetAttachedData(GenericAttachedDataKeys._umsChannel) as string,
                    TenantId = interaction.TenantID,
                    ParentId = args.ParentID,
                    MessageId = args.ID,
                    MessageName = EspRequestDefaultData.LikeMethodName,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    ErrorResponseHandler = new ErrorResponseHandler(LikeCommentErrorResponseHandler),
                    ResponseHandler = new ResponseHandler(args.ResponseHandler)
                };

                YoutubeOptions.Log.Info("Call YoutubeEspCommand");

                YoutubeEspCommand.AsyncRun(this.Container, paramerters, null);

                return true;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem Exception in LikeComment: ", ex);
                ShowErrMessage(string.Empty, LanguageDictionaryHelper.LikeError);

                return false;
            }
        }

        private void LikeCommentErrorResponseHandler(string errorMessage)
        {
            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke((Delegate)new ErrorResponseHandler(this.LikeCommentErrorResponseHandler), (object)errorMessage);
            else
            {
                YoutubeOptions.Log.Error("Couln't Like comment " + errorMessage);
                ShowErrMessage(string.Empty, LanguageDictionaryHelper.LikeError);
            }
        }

        #endregion

        #region Create oubound interaction

        private bool MainYoutubeView_CreateOutboundInteraction(object sender, RoutedActionEventArgs args)
        {
            YoutubeOptions.Log.Info("MainYoutubeView_CreateOutboundInteraction Started");

            try
            {
                var createInteractionResponseHandler = new CreateInteraction.ResponseHandler(this.CreateInteractionResponseHandler);
                var createInteractionErrorResponseHandler = new CreateInteraction.ErrorResponseHandler(this.CreateInteractionErrorResponseHandler);
                var userData = YoutubeOutboundInteractionHelper.GetCreateInteractionUserData(this.ParentInteraction);

                SocialMediaContainer container = new SocialMediaContainer(this.Container);

                CreateInteraction.Parameters parameter =
                    YoutubeOutboundInteractionHelper.GetCreateInteractionParameters(
                        this.ParentInteraction,
                        this.Container,
                        createInteractionResponseHandler,
                        createInteractionErrorResponseHandler,
                        userData);

                if (parameter != null)
                {
                    DisableToolbarButtons();

                    CreateInteraction.AsyncRun(container, parameter, this.MainYoutubeView);
                }
                //ResetButtons();

                return true;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Execption, ", ex);
                return false;
            }
        }

        private void CreateInteractionErrorResponseHandler()
        {
            if (base.Dispatcher.CheckAccess())
            {
                this.MainYoutubeView.CloseEditor();
                ResetButtons();
                this.EditedInteraction = (IOpenMediaInteraction)null;

                ShowErrMessage(string.Empty, LanguageDictionaryHelper.CreateOutboundInteractionError);
                return;
            }
            base.Dispatcher.Invoke(new CreateInteraction.ErrorResponseHandler(this.CreateInteractionErrorResponseHandler), new object[0]);
        }

        private void CreateInteractionResponseHandler(IOpenMediaInteraction response)
        {
            if (!base.Dispatcher.CheckAccess())
            {
                Dispatcher dispatcher = base.Dispatcher;
                CreateInteraction.ResponseHandler responseHandler =
                    new CreateInteraction.ResponseHandler(this.CreateInteractionResponseHandler);

                object[] objArray = new object[] { response };
                dispatcher.Invoke(responseHandler, objArray);

                return;
            }
            if (response != null)
            {
                this.EditedInteraction = response;
                if (this.ParentInteraction != null)
                {
                    ((InteractionOpenMedia)this.ParentInteraction).IsItPossibleToInsertStandardResponse = true;
                }

                // open text box and set focus
                this.MainYoutubeView.OpenEditor();
            }
        }

        #endregion

        #region Editor



        private bool MainYoutubeView_SendOutboundInteraction(object sender, SendEventArgs args)
        {
            YoutubeOptions.Log.Info("YoutubeWorkItem MainYoutubeView_SendOutboundInteraction Started");

            try
            {
                YoutubeOptions.Log.Info("YoutubeWorkItem in SendRepyCommentMsg: Reply/Comment to : {0} " + args);

                SendOutboundInteraction(args);

                return true;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem Exception in SendRepyCommentMsg: ", ex);
                return false;
            }
        }

        private void MainYoutubeView_CloseClick(object sender, RoutedEventArgs e)
        {
            if (this.EditedInteraction == null)
                return;

            StopEditedInteraction((UIElement)this.MainYoutubeView);
        }

        #endregion

        #region Reply

        #endregion

        #region Inbound Send
        #region SendInteraction
        private void ShowLoading()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            MainYoutubeView.LoadingOverlay.Opacity = 0;
            MainYoutubeView.LoadingOverlay.Visibility = Visibility.Visible;
            _showLoadingAnimation.Begin();
        }

        private void HideLoading()
        {
            if (!_isAnimating) return;
            _hideLoadingAnimation.Begin();
        }

        private async void SendOutboundInteraction(SendEventArgs args)
        {
            YoutubeOptions.Log.Info("SendOutboundInteraction Started");
            ShowLoading();

            try
            {
                // Force UI to render changes immediately
                Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                var socialMediaContainer = new SocialMediaContainer(this.Container);
                string messageIndex = string.Empty;

                var parameter = YoutubeOutboundInteractionHelper.GetSendInteractionParameters(
                    new YoutubeOutboundInteractionHelper.SendParameters
                    {
                        Container = this.Container,
                        ResponseHandler = SendInteractionResponseHandler,
                        ErrorResponseHandler = SendInteractionErrorResponseHandler,
                        UserDataParameters = new YoutubeOutboundInteractionHelper.Parameters
                        {
                            EditedInteraction = this.EditedInteraction,
                            YoutubeContainer = socialMediaContainer,
                            ParentInteraction = this.ParentInteraction,
                            ReplyCommentToSend = this.Model.GetCommentKvList(args.Comment, out messageIndex),
                            MessageText = args.Comment.Text,
                            ParentMessageId = args.Comment.ParentId,
                            MethodType = args.Comment.Type.GetMethodType(),
                            MessageIndex = messageIndex,
                        }
                    });

                // Run the interaction asynchronously
                await Task.Run(() => SendInteraction.AsyncRun(socialMediaContainer, parameter, this.MainYoutubeView));

                // Update UI after the operation
                ResetButtons();
                MainYoutubeView.AddCommentToViewAndCloseEditor();
            }
            catch (Exception ex)
            {
                // Log the error and show a user-friendly message
                YoutubeOptions.Log.Error("Exception occurred during SendOutboundInteraction.", ex);
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Hide the loading spinner
                HideLoading();
            }
        }
        private void SendOutboundInteraction1(SendEventArgs args)
        {
            YoutubeOptions.Log.Info("SendOutboundInteraction Started");
            ShowLoading();

            try
            {
                var socialMediaContainer = new SocialMediaContainer(this.Container);
                string messageIndex = string.Empty;
                var parameter = YoutubeOutboundInteractionHelper.GetSendInteractionParameters(new YoutubeOutboundInteractionHelper.SendParameters()
                {
                    Container = this.Container,
                    ResponseHandler = new SendInteraction.ResponseHandler(this.SendInteractionResponseHandler),
                    ErrorResponseHandler = new SendInteraction.ErrorResponseHandler(this.SendInteractionErrorResponseHandler),
                    UserDataParameters = new YoutubeOutboundInteractionHelper.Parameters()
                    {
                        EditedInteraction = this.EditedInteraction,
                        YoutubeContainer = socialMediaContainer,
                        ParentInteraction = this.ParentInteraction,
                        ReplyCommentToSend = this.Model.GetCommentKvList(args.Comment, out messageIndex),
                        MessageText = args.Comment.Text,
                        ParentMessageId = args.Comment.ParentId,
                        MethodType = args.Comment.Type.GetMethodType(),
                        MessageIndex = messageIndex,



                    }
                });

                SendInteraction.AsyncRun(socialMediaContainer, parameter, this.MainYoutubeView);



                Task.Run(() =>
                {
                    try
                    {
                        Thread.Sleep(6000); // Simulate work

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ResetButtons();
                            HideLoading();
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        { MessageBox.Show($"Error: {ex.Message}"); });
                    }
                });
                MainYoutubeView.AddCommentToViewAndCloseEditor();
                System.Windows.Threading.Dispatcher dispatcher = base.Dispatcher;
                SendInteraction.ResponseHandler responseHandler =
                    new SendInteraction.ResponseHandler(this.SendInteractionResponseHandler);
                object[] objArray = new object[] { this.EditedInteraction };
                dispatcher.Invoke(responseHandler, objArray);



            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception ", ex);
            }
        }
        private void SendInteractionErrorResponseHandler(string errorMessage)
        {
            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke((Delegate)new ErrorResponseHandler(this.SendInteractionErrorResponseHandler), (object)errorMessage);
            else
            {
                YoutubeOptions.Log.Error("Error in Sending outbound interaction " + errorMessage);

                ShowErrMessage(string.Empty, LanguageDictionaryHelper.SendInteractionError);
                this.EditedInteraction = (IOpenMediaInteraction)null;
                ResetButtons();
                MainYoutubeView.CloseEditor();
            }
        }

        public void SendInteractionResponseHandler(IOpenMediaInteraction response, KeyValueCollection userData)
        {
            if (base.Dispatcher.CheckAccess())
            {
                if (response != null)
                {
                    this.EditedInteraction = (IOpenMediaInteraction)null;
                    ResetButtons();
                    MainYoutubeView.AddCommentToViewAndCloseEditor();
                }
                return;
            }
            System.Windows.Threading.Dispatcher dispatcher = base.Dispatcher;
            SendInteraction.ResponseHandler responseHandler =
                new SendInteraction.ResponseHandler(this.SendInteractionResponseHandler);
            object[] objArray = new object[] { response };
            dispatcher.Invoke(responseHandler, objArray);
        }

        #endregion
        #endregion

        #region Outbound Send

        private bool MainYoutubeView_OutCommentSendClick(object sender, SendEventArgs args)
        {
            YoutubeOptions.Log.Info("YoutubeWorkItem MainYoutubeView_OutCommentSendClick Started");

            try
            {
                YoutubeOptions.Log.InfoFormat("YoutubeWorkItem in PlaceInQueue: Reply/Comment to : {0} ", args);

                PlaceInQueueItx(args);

                return true;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem Exception in PlaceInQueue: ", ex);
                return false;
            }
        }

        #region PlaceInQueue

        public void PlaceInQueueItx(SendEventArgs args)
        {
            YoutubeOptions.Log.Info("PlaceInQueueItx");

            try
            {
                var media = YoutubeOutboundInteractionHelper.GetOpenMediaService(this.Container);

                var userData = YoutubeOutboundInteractionHelper.GetOutboundItxUserData(this.Model.Interaction,
                    this.Container, args.Comment);

                PlaceInQueue.Parameters parameter1 = new PlaceInQueue.Parameters()
                {
                    CommandParameter = media as IMediaOpenMedia,
                    Interaction = this.Model.Interaction,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    UserData = userData,
                    Queue = YoutubeOptions.Default.OutboundQueue,
                    ResponseHandler = new PlaceInQueue.ResponseHandler(this.PlaceInQueueResponseHandler),
                    ErrorResponseHandler = new PlaceInQueue.ErrorResponseHandler(this.PlaceInQueueErrorResponseHandler)
                };

                SocialMediaContainer socialMediaContainer = new SocialMediaContainer(this.Container);
                PlaceInQueue.AsyncRun(socialMediaContainer, parameter1, this.MainYoutubeView);
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception PlaceInQueueItx", ex);
            }
        }

        public void PlaceInQueueResponseHandler(IOpenMediaInteraction response)
        {
            YoutubeOptions.Log.Info("PlaceInQueueResponseHandler");

            if (!base.Dispatcher.CheckAccess())
            {
                System.Windows.Threading.Dispatcher dispatcher = base.Dispatcher;
                PlaceInQueue.ResponseHandler responseHandler = new PlaceInQueue.ResponseHandler(this.PlaceInQueueResponseHandler);
                object[] objArray = new object[] { response };
                dispatcher.Invoke(responseHandler, objArray);
            }
        }

        public void PlaceInQueueErrorResponseHandler(string error)
        {
            YoutubeOptions.Log.Info("PlaceInQueueErrorResponseHandler");
            if (base.Dispatcher.CheckAccess())
            {
                // this.Model.Interaction.IsCancelEnabled = true;
                return;
            }
            System.Windows.Threading.Dispatcher dispatcher = base.Dispatcher;
            PlaceInQueue.ErrorResponseHandler errorResponseHandler = new PlaceInQueue.ErrorResponseHandler(this.PlaceInQueueErrorResponseHandler);
            object[] objArray = new object[] { error };
            dispatcher.Invoke(errorResponseHandler, objArray);
        }

        #endregion

        #endregion

        #region StopInteraction

        private void StopEditedInteraction(UIElement control)
        {
            try
            {
                YoutubeOptions.Log.Info("StopInteraction StopEditedInteraction");
                StopInteraction.AsyncRun(this.Container, new StopInteraction.Parameters()
                {
                    Interaction = this.EditedInteraction,
                    Timeout = YoutubeOptions.Default.ResponseWaitTime,
                    ResponseHandler = new StopInteraction.ResponseHandler(this.StopInteractionResponseHandler)
                }, control);
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("StopInteraction StopEditedInteraction", ex);
            }
        }

        public void StopInteractionResponseHandler(IOpenMediaInteraction response)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke((Delegate)new StopInteraction.ResponseHandler(this.StopInteractionResponseHandler), (object)response);
            }
            else
            {
                if (response == null)
                    return;

                this.EditedInteraction = (IOpenMediaInteraction)null;

                this.ResetButtons();
                MainYoutubeView.CloseEditor();
            }
        }

        #endregion

        private void ShowErrorMsgBox(string msg)
        {
            System.Windows.Window parentWindow = (System.Windows.Window)PresentationSource.FromVisual((Visual)this).RootVisual;
            parentWindow.Dispatcher.Invoke((Action)(() =>
            {
                IWMessageBoxView.Show(parentWindow, msg, IWMessageBoxButtons.Ok, MessageBoxIcon.Error);
                IWMessageBoxView.DestroyBoxResult();
            }));
        }

        private void ShowErrMessage(string errorCode, string message)
        {
            ShowErrorMessage.AsyncRun(this.Container, new ShowErrorMessage.Parameters()
            {
                SeverityType = SeverityType.Error,
                ErrorCode = errorCode,
                ErrorMessage = message,
                InteractionOpenMedia = this.ParentInteraction
            }, (UIElement)null);
        }

        private void MainYoutubeView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
