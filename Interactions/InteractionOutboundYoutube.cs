using Genesyslab.Desktop.Infrastructure.Configuration;
using Genesyslab.Desktop.Infrastructure.ObjectFormat;
using Genesyslab.Desktop.Modules.Core.Configurations;
using Genesyslab.Desktop.Modules.Core.Model.Agents;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions;
using Genesyslab.Desktop.Modules.OpenMedia.Model.Interactions.WorkItem;
using Genesyslab.Desktop.Modules.SocialMedia;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Enterprise.Model.Envelope;
using Genesyslab.Enterprise.Model.Interaction;
using Genesyslab.Enterprise.Model.ServiceModel;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;

using interactions = Genesyslab.Desktop.Modules.Core.Model.Interactions;


namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Interactions
{
    public interface IInteractionOutboundYoutube : IInteractionOpenMedia, interactions.IInteraction, INotifyPropertyChanged
    {
        IInteractionWorkItem InteractionWorkItem { get; }

        bool IsCancelEnabled { get; set; }

        KeyValueCollection UnsavedUserData { get; }
    }

    internal class InteractionOutboundYoutube : InteractionOpenMedia, IInteractionOutboundYoutube, IInteractionOpenMedia, interactions.IInteraction, INotifyPropertyChanged
    {
        private static readonly IList<string> NoUpdateProperties = (IList<string>)new List<string>()
        {
          nameof (InteractionId),
          nameof (BundleId),
          nameof (CaseId),
          "Duration",
        };

        private static readonly IList<string> TraceProperties = (IList<string>)new List<string>()
        {
          "IsItPossibleToOneStepTransfer",
          "IsItPossibleToMoveToWorkbin",
          "IsItPossibleToMarkDone",
          "IsItPossibleToConsult",
          "IsItPossibleToInsertStandardResponse"
        };

        protected KeyValueCollection unsavedUserData = new KeyValueCollection();
        protected bool isCancelEnabled;

        public InteractionOutboundYoutube(IUnityContainer container, IInteractionManager interactionManager,
            IEnterpriseServiceProvider entrepriseService, IAgent agent, IConfigManager configManager,
            IConfigAttachedDataManager configAttachedDataManager)
            : base(container, interactionManager, entrepriseService, agent, configManager, configAttachedDataManager)
        {
            this.log = YoutubeOptions.Log;
            this.log.Info((object)"InteractionOutboundYoutube()");

            this.InteractionParties = (IList<IInteractionParty>)new ObservableCollection<IInteractionParty>();
            this.InteractionWorkItem = container.Resolve<IInteractionWorkItem>();

            this.InteractionWorkItem.PropertyChanged +=
                new PropertyChangedEventHandler(this.InteractionWorkItem_PropertyChanged);

            this.InteractionWorkItem.MediaType = YoutubeWorkItemModule.MediaTypeModuleMedia;

            ((INotifyCollectionChanged)this.UserData).CollectionChanged +=
                new NotifyCollectionChangedEventHandler(this.UserData_CollectionChanged);

            (this.InteractionWorkItem.InteractionParties as INotifyCollectionChanged).CollectionChanged +=
                new NotifyCollectionChangedEventHandler(this.InteractionOutboundInstagram_CollectionChanged);
        }

        private void InteractionWorkItem_UserDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!NotifyCollectionChangedAction.Add.Equals((object)e.Action) && !NotifyCollectionChangedAction.Replace.Equals((object)e.Action))
                return;
            foreach (KeyValuePair<string, object> newItem in (IEnumerable)e.NewItems)
            {
                if (newItem.Key == "NotepadViewModel" && this.InteractionWorkItem != null && this.InteractionWorkItem.UserData != null)
                    this.InteractionWorkItem.UserData["NotepadViewModel"] = newItem.Value;
                if (newItem.Key == "Contact.Id" && this.InteractionWorkItem != null && this.InteractionWorkItem.UserData != null)
                    this.InteractionWorkItem.UserData["Contact.Id"] = newItem.Value;
            }
        }

        public IInteractionWorkItem InteractionWorkItem { get; private set; }

        public override string MediaType { get; set; }

        public override bool OpenedToSetMandatoryDispositionCode
        {
            get
            {
                return this.InteractionWorkItem.OpenedToSetMandatoryDispositionCode;
            }
        }

        public override IFormattedObject EvaluateParty(IParty party)
        {
            return (IFormattedObject)null;
        }

        public override IFormattedObject FormattedObject
        {
            get
            {
                return this.InteractionWorkItem.FormattedObject;
            }
            protected set
            {
            }
        }

        public override IList<IInteractionParty> InteractionParties { get; protected set; }

        public virtual bool IsCancelEnabled
        {
            get
            {
                return this.isCancelEnabled;
            }
            set
            {
                this.isCancelEnabled = value;
                this.OnPropertyChanged(nameof(IsCancelEnabled));
            }
        }

        public override bool SetAttachedData(KeyValueCollection attacheddata)
        {
            return this.InteractionWorkItem.SetAttachedData(attacheddata);
        }

        public override bool SetAttachedData(string key, object value)
        {
            return this.InteractionWorkItem.SetAttachedData(key, value);
        }

        public override object GetAttachedData(string key)
        {
            return this.InteractionWorkItem.GetAttachedData(key);
        }

        public override KeyValueCollection GetAllAttachedData()
        {
            return this.InteractionWorkItem.GetAllAttachedData();
        }

        public override bool RemoveAttachedData(string key)
        {
            return this.InteractionWorkItem.RemoveAttachedData(key);
        }

        public override string Type
        {
            get
            {
                return nameof(InteractionOutboundYoutube);
            }
        }

        public override Genesyslab.Enterprise.Model.Interaction.IInteraction EntrepriseInteractionCurrent
        {
            get
            {
                return this.InteractionWorkItem.EntrepriseInteractionCurrent;
            }
            set
            {
                this.InteractionWorkItem.EntrepriseInteractionCurrent = value;
            }
        }

        public override IList<Enterprise.Model.Interaction.IInteraction> EntrepriseInteractions
        {
            get
            {
                return this.InteractionWorkItem.EntrepriseInteractions;
            }
        }

        public override void Update(IEnvelope<Genesyslab.Enterprise.Model.Interaction.IInteraction> tsp)
        {
            this.InteractionWorkItem.Update(tsp);
        }

        public override bool CreateDispositionCode()
        {
            return base.CreateDispositionCode();
        }

        public override string InteractionId
        {
            get
            {
                return base.InteractionId;
            }
            set
            {
                base.InteractionId = value;
                this.InteractionWorkItem.InteractionId = value;
            }
        }

        public override string BundleId
        {
            get
            {
                return base.BundleId;
            }
            set
            {
                base.BundleId = value;
                this.InteractionWorkItem.BundleId = value;
            }
        }

        public override string CaseId
        {
            get
            {
                return base.CaseId;
            }
            set
            {
                base.CaseId = value;
                this.InteractionWorkItem.CaseId = value;
                // this.InteractionWorkItem.BundleId = value;
            }
        }

        public virtual KeyValueCollection UnsavedUserData
        {
            get
            {
                return this.unsavedUserData;
            }
            set
            {
                this.unsavedUserData = value;
                this.OnPropertyChanged(nameof(UnsavedUserData));
            }
        }

        public override void Release()
        {
            (this.InteractionWorkItem.InteractionParties as INotifyCollectionChanged).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.InteractionOutboundInstagram_CollectionChanged);

            this.InteractionWorkItem.PropertyChanged -= new PropertyChangedEventHandler(this.InteractionWorkItem_PropertyChanged);

            this.InteractionWorkItem.Release();

            if (this.UserData != null)
                ((INotifyCollectionChanged)this.UserData).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.UserData_CollectionChanged);
        }

        private void InteractionWorkItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (InteractionOutboundYoutube.NoUpdateProperties.Contains(e.PropertyName))
                    return;
                PropertyInfo property1 = this.GetType().GetProperty(e.PropertyName);
                if (property1 != (PropertyInfo)null)
                {
                    PropertyInfo property2 = this.InteractionWorkItem.GetType().GetProperty(e.PropertyName);
                    property1.SetValue((object)this, property2.GetValue((object)this.InteractionWorkItem, (object[])null), (object[])null);
                }
                if (this.log.IsDebugEnabled && InteractionOutboundYoutube.TraceProperties.Contains(e.PropertyName))
                {
                    StringBuilder possibleStringBuilder = new StringBuilder();
                    foreach (string traceProperty in (IEnumerable<string>)InteractionOutboundYoutube.TraceProperties)
                        this.AddIsPossibleToString(traceProperty, possibleStringBuilder);
                    this.log.Debug((object)("Computed possibles,  Interaction:'" + (object)this + "': " + possibleStringBuilder.ToString()));
                }
                var IsItPossibleToReply = this.IsItPossibleToMoveToWorkbin;
            }
            catch (Exception ex)
            {
                SocialMediaOptions.Log.Error((object)"Update property, Exception", ex);
            }
        }

        private void InteractionOutboundInstagram_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            foreach (IInteractionParty newItem in (IEnumerable)e.NewItems)
            {
                this.InteractionParties.Add((IInteractionParty)new YoutubeParty(newItem, (Genesyslab.Desktop.Modules.Core.Model.Interactions.IInteraction)this));
            }
        }

        private void UserData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!NotifyCollectionChangedAction.Add.Equals((object)e.Action) && !NotifyCollectionChangedAction.Replace.Equals((object)e.Action))
                return;
            foreach (KeyValuePair<string, object> newItem in (IEnumerable)e.NewItems)
            {
                if (newItem.Key == "NotepadViewModel" && this.InteractionWorkItem != null && this.InteractionWorkItem.UserData != null)
                    this.InteractionWorkItem.UserData["NotepadViewModel"] = newItem.Value;
                if (newItem.Key == "Contact.Id" && this.InteractionWorkItem != null && this.InteractionWorkItem.UserData != null)
                    this.InteractionWorkItem.UserData["Contact.Id"] = newItem.Value;
            }
        }
    }
}
