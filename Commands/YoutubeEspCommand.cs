using Genesyslab.Desktop.Infrastructure.Commands;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.SocialMedia.Commands;
using Genesyslab.Desktop.Modules.SocialMedia.LogPrint;
using Genesyslab.Desktop.Modules.SocialMedia.Objects;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Events;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.ExternalService.Request;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Commands
{
    internal class YoutubeEspCommand : BaseCommand, IElementOfCommand
    {
        public static readonly string NAME = typeof(YoutubeEspCommand).FullName;

        public static void AsyncRun(
          SocialMediaContainer container,
          YoutubeEspCommand.Parameters prms,
          UIElement control)
        {
            BaseCommand.AsyncRun(typeof(YoutubeEspCommand), container, (object)prms, control);
        }

        public static void AsyncRun(
          IUnityContainer container,
          YoutubeEspCommand.Parameters prms,
          UIElement control)
        {
            YoutubeEspCommand.AsyncRun(new SocialMediaContainer(container), prms, control);
        }

        public YoutubeEspCommand(IUnityContainer container)
        {
            this.Container = new SocialMediaContainer(container);
        }

        public override bool Execute(IDictionary<string, object> parameters)
        {
            YoutubeEspCommand.Parameters parameter = parameters[YoutubeEspCommand.NAME] as YoutubeEspCommand.Parameters;
            ExternalServiceProtocol espServerProtocol = this.CreateEspServerProtocol(parameter.Endpoint, YoutubeOptions.Default.Encoding);
            YoutubeOptions.Log.Info("Encoding " + YoutubeOptions.Default.Encoding);

            try
            {
                PrintOut.PrintCommandAction(SocialMediaOptions.Log, YoutubeEspCommand.NAME + " Start", (object)parameter);
                Request3rdServer request3rdServer = Request3rdServer.Create();

                var messageName = parameter.MessageName;

                KeyValueCollection kv_parameters = new KeyValueCollection();
                KVListHelper.AddKVPair(kv_parameters, GenericAttachedDataKeys._umsChannel, parameter.Service);

                KeyValueCollection requestData = new KeyValueCollection();
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.Version, EspRequestDefaultData.Version);
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.Method, EspRequestDefaultData.EspMethod);
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.MediaType, YoutubeWorkItemModule.MediaTypeModuleMedia);
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.Service, EspRequestDefaultData.EspService);
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.AppName, parameter.AppName);
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.TenantId, parameter.TenantId);

                KVListHelper.AddKVPair(requestData, YoutubeDataKey.MessageID, parameter.MessageId);
                KVListHelper.AddKVPair(requestData, GenericAttachedDataKeys._umsChannelMonitor, parameter.ChannelMonitor);
                KVListHelper.AddKVPair(requestData, GenericAttachedDataKeys._umsChannel, parameter.Service);
                KVListHelper.AddKVPair(requestData, YoutubeDataKey.Parameters, kv_parameters);

                request3rdServer.Request = requestData;

                request3rdServer.UserData = new KeyValueCollection();

                request3rdServer.UserData.Add(YoutubeDataKey.MessageType, EspRequestDefaultData.MessageType);
                request3rdServer.UserData.Add(YoutubeDataKey.OutboundMethodType, messageName);
                request3rdServer.UserData.Add(YoutubeDataKey.ParentID, parameter.ParentId);
                request3rdServer.UserData.Add(YoutubeDataKey.MessageID, parameter.MessageId);
                request3rdServer.UserData.Add(YoutubeDataKey.MessageText, string.Empty);

                espServerProtocol.Open();

                YoutubeOptions.Log.Info("Preparing to send ESP Request and CommentID is " + parameter.MessageId);
                YoutubeOptions.Log.InfoFormat("Preparing to send ESP Request : {0}", (object)request3rdServer);

                IMessage message = espServerProtocol.Request((IMessage)request3rdServer);

                if (message == null)
                {
                    YoutubeOptions.Log.Error("Received response to ESP Request : null");
                    YoutubeOptions.Log.ErrorFormat("Can't  " + messageName + " comment");
                    parameter.ErrorResponseHandler("Can't " + messageName + " comment");
                    return true;
                }
                if (message.Id == 502)
                {
                    YoutubeOptions.Log.Error("Can't " + messageName + " comment");
                    YoutubeOptions.Log.ErrorFormat("Received response to ESP Request : {0}", (object)message);

                    parameter.ErrorResponseHandler("Can't " + messageName + " comment");
                    return true;
                }

                YoutubeOptions.Log.InfoFormat("Received response to ESP Request : {0}", (object)message);
            }
            finally
            {
                if (espServerProtocol.State == ChannelState.Opened)
                {
                    try
                    {
                        espServerProtocol.Close();
                    }
                    catch (Exception ex)
                    {
                        YoutubeOptions.Log.DebugFormat("Can't close connection to Interaction Server : {0}", (object)ex);
                    }
                }
                PrintOut.PrintCommandAction(YoutubeOptions.Log, YoutubeEspCommand.NAME + " End");
            }
            parameter.ResponseHandler();
            return false;
        }

        public virtual string Name { get; set; }

        public class Parameters : PrintOut
        {
            [PrintOut]
            public string Service { get; set; }

            [PrintOut]
            public string AppName { get; set; }

            [PrintOut]
            public int TenantId { get; set; }

            [PrintOut]
            public string MessageId { get; set; }

            [PrintOut]
            public string ParentId { get; set; }

            [PrintOut]
            public string MessageName { get; set; }

            [PrintOut]
            public string ChannelMonitor { get; set; }

            [PrintOut]
            public Uri Endpoint { get; set; }

            [PrintOut]
            public int Timeout { get; set; }

            [PrintOut]
            public ResponseHandler ResponseHandler { get; set; }

            [PrintOut]
            public ErrorResponseHandler ErrorResponseHandler { get; set; }
        }
    }
}
