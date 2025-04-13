using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Agents;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.SocialMedia.LogPrint;
using Genesyslab.Desktop.Modules.SocialMedia.Objects;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers
{
    public static class YoutubeOutboundInteractionHelper
    {
        public class Parameters : PrintOut
        {
            public IInteractionOpenMedia ParentInteraction { get; set; }

            public IOpenMediaInteraction EditedInteraction { get; set; }

            public SocialMediaContainer YoutubeContainer { get; set; }

            public KeyValueCollection ReplyCommentToSend { get; set; }

            public string MethodType { get; set; }

            public string ParentMessageId { get; set; }

            public string MessageText { get; set; }

            public string MessageIndex { get; set; }

        }

        public class SendParameters : PrintOut
        {
            public IUnityContainer Container { get; set; }

            public Parameters UserDataParameters { get; set; }

            public SendInteraction.ResponseHandler ResponseHandler { get; set; }

            public SendInteraction.ErrorResponseHandler ErrorResponseHandler { get; set; }
        }

        public static KeyValueCollection GetUserData(Parameters parameters)
        {
            YoutubeOptions.Log.Info("GetUserData Start");

            if (parameters.ParentInteraction == null)
                return GetUserDataWhenParentInteractionIsNull();

            KeyValueCollection userData = new KeyValueCollection();

            KeyValueCollection AllAttachedData = parameters.ParentInteraction.GetAllAttachedData();

            KeyValueCollection AttachedData = parameters.ParentInteraction.ExtractAttachedData();

            userData.Add(AttachedData);

            KeyValueCollection desktop_ParentItxData = Getdesktop_ParentItxData(AllAttachedData);
            KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.desktop_ParentItxData, (object)desktop_ParentItxData);

            var youtubeContainer = parameters.YoutubeContainer;

            if (youtubeContainer.Agent.ConfPerson.EmployeeID != null)
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.desktop_AgentEmployeeId,
                   (object)youtubeContainer.Agent.ConfPerson.EmployeeID);
                YoutubeOptions.Log.Info(GenericAttachedDataKeys.desktop_AgentEmployeeId + " : " + youtubeContainer.Agent.ConfPerson.EmployeeID);
            }

            if (parameters.ParentInteraction.UserData.ContainsKey(YoutubeDataKey.Contact_Id))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.ContactId,
                    parameters.ParentInteraction.UserData[YoutubeDataKey.Contact_Id]);
            }

            if (AllAttachedData.ContainsKey(GenericAttachedDataKeys.FromAddress))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.To,
                    AllAttachedData.Get(GenericAttachedDataKeys.FromAddress));
            }

            if (AllAttachedData.ContainsKey(YoutubeDataKey.PostData.AuthorName))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.FromAddress,
                    AllAttachedData.Get(YoutubeDataKey.PostData.AuthorName));
            }

            if (AllAttachedData.ContainsKey(GenericAttachedDataKeys._umsChannel))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys._umsChannel,
                    AllAttachedData.Get(GenericAttachedDataKeys._umsChannel));
            }

            if (AllAttachedData.ContainsKey(GenericAttachedDataKeys._umsInboundIxnSubmittedBy))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys._umsInboundIxnSubmittedBy,
                     AllAttachedData.Get(GenericAttachedDataKeys._umsInboundIxnSubmittedBy));
            }

            if (AllAttachedData.ContainsKey(GenericAttachedDataKeys.Subject))
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.Subject, (object)("Reply: " +
                    AllAttachedData.Get(GenericAttachedDataKeys.Subject)));
            else
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.Subject, (object)"Reply:");

            KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.desktop_OutboundItxType, (object)"Solicited");

            var lastReplyCommentInfo = parameters.ReplyCommentToSend;
            if (lastReplyCommentInfo != null)
            {
                foreach (var item in lastReplyCommentInfo.AllKeys)
                {
                    if (!userData.AllKeys.Contains(item))
                    {
                        var key = item;
                        var value = lastReplyCommentInfo.Get(item);

                        KVListHelper.AddKVPair(userData, key, (object)value);
                    }
                }
            }

            userData.Add(YoutubeDataKey.MessageType, EspRequestDefaultData.MessageType);
            userData.Add(YoutubeDataKey.OutboundMethodType, parameters.MethodType);
            userData.Add(YoutubeDataKey.MessageID, parameters.ParentMessageId); // parent id
                                                                                 // System.Uri.EscapeDataString(
            userData.Add(YoutubeDataKey.MessageText, System.Uri.EscapeDataString(parameters.MessageText));
            userData.Add(YoutubeDataKey.MsgIndex, parameters.MessageIndex);

            return userData;
        }

        private static KeyValueCollection Getdesktop_ParentItxData(KeyValueCollection ParentInteractionAttachedData)
        {
            KeyValueCollection desktop_ParentItxData = new KeyValueCollection();
            foreach (FieldInfo field in typeof(GenericAttachedDataKeys).GetFields())
            {
                string key = field.GetValue((object)null) as string;
                if (key != null && ParentInteractionAttachedData.ContainsKey(key))
                    KVListHelper.AddKVPair(desktop_ParentItxData, key, ParentInteractionAttachedData.Get(key));
            }
            return desktop_ParentItxData;
        }

        public static KeyValueCollection GetUserDataWhenParentInteractionIsNull()
        {
            KeyValueCollection userData = new KeyValueCollection();

            KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.Subject, "Reply:");

            return userData;
        }

        public static SendInteraction.Parameters GetSendInteractionParameters(SendParameters parameters)
        {
            IMedia OpenMediaService = GetOpenMediaService(parameters.Container);
            if (OpenMediaService == null)
            {
                YoutubeOptions.Log.Error("Current OpenMediaService is null");
                return null;
            }

            if (parameters.UserDataParameters.EditedInteraction == null)
            {
                YoutubeOptions.Log.Error("Current EditedInteraction is null");
                return null;
            }

            string _outboundQueue = GetOutboundQueue(parameters.UserDataParameters.EditedInteraction.OutputQueues);

            KeyValueCollection userData = GetUserData(parameters.UserDataParameters);

            SendInteraction.Parameters parameter = new SendInteraction.Parameters()
            {
                MediaType = YoutubeWorkItemModule.MediaTypeModuleMedia,
                CommandParameter = OpenMediaService as IMediaOpenMedia,
                Interaction = parameters.UserDataParameters.EditedInteraction,
                Timeout = YoutubeOptions.Default.ResponseWaitTime,
                UserData = userData,
                Queue = _outboundQueue,
                ResponseHandler = parameters.ResponseHandler,
                ErrorResponseHandler = parameters.ErrorResponseHandler
            };

            return parameter;
        }

        private static string GetOutboundQueue(ICollection<string> outputQueues)
        {
            var queue = YoutubeOptions.Default.OutboundQueue;
            YoutubeOptions.Log.Info("Option youtube.outbound-queue " + queue);

            if (outputQueues != null && outputQueues.Count > 0)
            {
                queue = outputQueues.First<string>();
                YoutubeOptions.Log.Info("Outbound queue from ParentInteraction : " + queue);
            }

            YoutubeOptions.Log.Info("OutboundQueue: " + queue);
            return queue;
        }

        public static IMedia GetOpenMediaService(IUnityContainer container)
        {
            IAgent myagent = container.Resolve<IAgent>();
            var medias = myagent.GetAvailableMedia(YoutubeWorkItemModule.MediaTypeModuleMedia);
            if (medias == null)
            {
                YoutubeOptions.Log.Error("medias is null");
                return null;
            }

            IMedia OpenMediaService = medias.FirstOrDefault();
            if (OpenMediaService == null)
            {
                YoutubeOptions.Log.Error("OpenMediaService is null");
                return null;
            }

            return OpenMediaService;
        }

        public static CreateInteraction.Parameters GetCreateInteractionParameters(
            IInteractionOpenMedia ParentInteraction,
            IUnityContainer Container,
            CreateInteraction.ResponseHandler responseHandler,
            CreateInteraction.ErrorResponseHandler errorResponseHandler,
           KeyValueCollection userData)
        {
            if (ParentInteraction == null)
            {
                YoutubeOptions.Log.Error("ParentInteraction is null");
                return null;
            }

            IMedia OpenMediaService = GetOpenMediaService(Container);
            if (OpenMediaService == null)
                return null;

            YoutubeOptions.Log.Info("Option youtube.default-queue " + YoutubeOptions.Default.DefaultQueue);

            return new CreateInteraction.Parameters()
            {
                MediaType = YoutubeWorkItemModule.MediaTypeModuleMedia,
                CommandParameter = OpenMediaService as IMediaOpenMedia,
                ParentInteractionId = ParentInteraction.EntrepriseInteractionCurrent.Id,
                InputQueues = ParentInteraction.EntrepriseOpenMediaInteractionCurrent.InputQueues,
                OutputQueues = ParentInteraction.EntrepriseOpenMediaInteractionCurrent.OutputQueues,
                UserData = userData,
                InteractionSubtype = YoutubeOptions.Default.OutboundReplySubType,
                Timeout = YoutubeOptions.Default.ResponseWaitTime,
                DefaultQueue = YoutubeOptions.Default.DefaultQueue,
                ResponseHandler = responseHandler,
                ErrorResponseHandler = errorResponseHandler,
                CreateInUCS = false
            };
        }

        public static KeyValueCollection GetCreateInteractionUserData(IInteractionOpenMedia ParentInteraction)
        {
            KeyValueCollection userData = new KeyValueCollection();

            if (ParentInteraction.UserData.ContainsKey(GenericAttachedDataKeys.ContactId))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.ContactId, ParentInteraction.UserData[GenericAttachedDataKeys.ContactId]);
            }

            if (ParentInteraction.UserData.ContainsKey(GenericAttachedDataKeys.FromAddress))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.FromAddress, (object)ParentInteraction.UserData[GenericAttachedDataKeys.FromAddress]);
            }

            return userData;
        }

        public static KeyValueCollection GetOutboundItxUserData(
            IInteractionOpenMedia Interaction, IUnityContainer Container, CommentViewModel comment)
        {
            YoutubeOptions.Log.Info("GetOutboundItxUserData Started");

            KeyValueCollection userData = new KeyValueCollection();
            KeyValueCollection InteractionAttachedData = Interaction.ExtractAttachedData();
            userData.Add(InteractionAttachedData);

            YoutubeOptions.Log.InfoFormat("UserData start: {0}", userData);

            KeyValueCollection parentItxData = new KeyValueCollection();
            KeyValueCollection InteractionAllData = Interaction.GetAllAttachedData();
            foreach (FieldInfo field in typeof(GenericAttachedDataKeys).GetFields())
            {
                string key = field.GetValue((object)null) as string;
                if (key != null && InteractionAllData.ContainsKey(key))
                    KVListHelper.AddKVPair(parentItxData, key, InteractionAllData.Get(key));
            }

            KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.desktop_ParentItxData, (object)parentItxData);

            var youtubeContainer = new SocialMediaContainer(Container);
            if (youtubeContainer.Agent.ConfPerson.EmployeeID != null)
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.desktop_AgentEmployeeId,
                   (object)youtubeContainer.Agent.ConfPerson.EmployeeID);

                YoutubeOptions.Log.Info("YoutubeOptions Key " + GenericAttachedDataKeys.desktop_AgentEmployeeId + " Value: " + youtubeContainer.Agent.ConfPerson.EmployeeID);
            }

            if (InteractionAttachedData.ContainsKey(YoutubeDataKey.Contact_Id))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.ContactId, InteractionAttachedData[YoutubeDataKey.Contact_Id]);
            }

            if (InteractionAttachedData.ContainsKey(GenericAttachedDataKeys._umsChannel))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys._umsChannel,
                    InteractionAttachedData.Get(GenericAttachedDataKeys._umsChannel));
            }

            if (InteractionAttachedData.ContainsKey(GenericAttachedDataKeys._umsInboundIxnSubmittedBy))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys._umsInboundIxnSubmittedBy,
                     InteractionAttachedData.Get(GenericAttachedDataKeys._umsInboundIxnSubmittedBy));

                YoutubeOptions.Log.InfoFormat("YoutubeOptions Key {0} , value : {0} ", GenericAttachedDataKeys._umsInboundIxnSubmittedBy, InteractionAttachedData.Get(GenericAttachedDataKeys._umsInboundIxnSubmittedBy));
            }

            if (InteractionAttachedData.ContainsKey(GenericAttachedDataKeys.Subject))
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.Subject, (object)(
                   InteractionAttachedData.Get(GenericAttachedDataKeys.Subject)));
            }
            else
            {
                KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.Subject, (object)"Reply:");
            }

            KVListHelper.AddKVPair(userData, GenericAttachedDataKeys.desktop_OutboundItxType, (object)"Solicited");

            if (InteractionAttachedData.ContainsKey(YoutubeDataKey.MessageType))
            {
                KVListHelper.AddKVPair(userData, YoutubeDataKey.MessageType, InteractionAttachedData.Get(YoutubeDataKey.MessageType));
            }

            if (InteractionAttachedData.ContainsKey(YoutubeDataKey.OutboundMethodType))
            {
                string methodType = InteractionAttachedData.GetAsString(YoutubeDataKey.OutboundMethodType);
                KVListHelper.AddKVPair(userData, YoutubeDataKey.OutboundMethodType, methodType);
            }

            if (comment.Type == MessageType.Comment)
            {
                KVListHelper.AddKVPair(userData, YoutubeDataKey.CommentData.Text + comment.Index, comment.Text);
            }

            if (comment.Type == MessageType.Reply)
            {
                KVListHelper.AddKVPair(userData, YoutubeDataKey.ReplyData.Text + comment.Index, System.Uri.EscapeDataString(comment.Text));
            }
            KVListHelper.AddKVPair(userData, YoutubeDataKey.MessageID, InteractionAttachedData.Get(YoutubeDataKey.MessageID));

            //  System.Uri.EscapeDataString(
            KVListHelper.AddKVPair(userData, YoutubeDataKey.MessageText, System.Uri.EscapeDataString(comment.Text));

            return userData;
        }
    }
}
