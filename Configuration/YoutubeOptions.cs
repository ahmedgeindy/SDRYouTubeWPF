using Genesyslab.Desktop.Infrastructure.Configuration;
using Genesyslab.Desktop.Modules.Core.Model.Interactions;
using Genesyslab.Platform.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration
{
    internal class YoutubeOptions : Options
    {
        public static ILogger Log { get; private set; }

        public YoutubeOptions()
        {
        }

        public static YoutubeOptions Default { get; private set; }

        public string DefaultQueue
        {
            get
            {
                return this.GetValueAsString("youtube.default-queue", (string)null);
            }
        }

        public string OutboundQueue
        {
            get
            {
                return this.GetValueAsString("youtube.outbound-queue", (string)null);
            }
        }

        public string OutboundQueue1
        {
            get
            {
                return this.GetValueAsString("youtube.outbound1-queue", (string)null);
            }
        }

        public int ResponseWaitTime
        {
            get
            {
                return 10000;
            }
        }

        public string OutboundReplySubType
        {
            get
            {
                return "OutboundReply";
            }
        }

        public string Encoding
        {
            get
            {
                return this.GetValueAsString("general.non-unicode-connection-encoding", (string)null);
            }
        }

        public int MaxMsgLenght
        {
            get
            {
                return 10000;
            }
        }

        private YoutubeOptions(IConfigManager configManager, ILogger oLog) : base(configManager, oLog)
        {
            if (YoutubeOptions.Log != null)
                return;
            YoutubeOptions.Log = oLog.CreateChildLogger("Youtube Plug-In");
        }

        public static void CreateInstance(IConfigManager configManager, ILogger oLog)
        {
            YoutubeOptions.Default = new YoutubeOptions(configManager, oLog);
        }

        public static YoutubeOptions CreateNewInstance(
          IConfigManager configManager,
          ILogger oLog)
        {
            return new YoutubeOptions(configManager, oLog);
        }

        public static YoutubeOptions GetContextualInstance(
          IInteraction interaction,
          ILogger oLog)
        {
            YoutubeOptions newInstance = YoutubeOptions.Default;
            if (interaction != null)
                newInstance = YoutubeOptions.CreateNewInstance(interaction.ContextualConfigManager, oLog);
            return newInstance;
        }

        protected string GetValueAsString(string key, string defaultValue, string[] values)
        {
            try
            {
                string str = this.configManager.GetValue(key) as string;
                if (!string.IsNullOrEmpty(str))
                {
                    if (((IEnumerable<string>)values).Contains<string>(str))
                        return str;
                }
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.ErrorFormat("Can't parse option '{0}' to string. will use default value '{1}'", (object)key, (object)defaultValue);
                YoutubeOptions.Log.Error("Exception, ", ex);
            }
            YoutubeOptions.Log.ErrorFormat("Option '{0}' has incorrect value. will use default value '{1}'", (object)key, (object)defaultValue);
            return defaultValue;
        }

        protected string GetValueAsString(string key, string defaultValue)
        {
            try
            {
                string str = this.configManager.GetValue(key) as string;
                if (!string.IsNullOrEmpty(str))
                    return str;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.ErrorFormat("Can't parse option '{0}' to string. will use default value '{1}'", (object)key, (object)defaultValue);
                YoutubeOptions.Log.Error("Exception, ", ex);
            }
            return defaultValue;
        }

        protected int GetValueAsInt(string key, int defaultValue, int[] values)
        {
            try
            {
                string str = this.configManager.GetValue(key) as string;
                if (string.IsNullOrEmpty(str))
                    return defaultValue;
                int int32 = Convert.ToInt32(str);
                foreach (int num in values)
                {
                    if (num == int32)
                        return int32;
                }
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.ErrorFormat("Can't parse option '{0}' to int. will use default value '{1}'", (object)key, (object)defaultValue);
                YoutubeOptions.Log.Error("Exception, ", ex);
                return defaultValue;
            }
            YoutubeOptions.Log.ErrorFormat("Option '{0}' has incorrect value. will use default value '{1}'", (object)key, (object)defaultValue);
            return defaultValue;
        }

        protected int GetValueAsInt(string key, int defaultValue, int max, int min)
        {
            try
            {
                string str = this.configManager.GetValue(key) as string;
                if (string.IsNullOrEmpty(str))
                    return defaultValue;
                int int32 = Convert.ToInt32(str);
                if (int32 > max)
                {
                    YoutubeOptions.Log.ErrorFormat("Value of option '{0}' is larger than maximum value '{1}' . will use default value '{2}'", (object)key, (object)max, (object)defaultValue);
                    return defaultValue;
                }
                if (int32 >= min)
                    return int32;
                YoutubeOptions.Log.ErrorFormat("Value of option '{0}' is lower than minimum value '{1}' . will use default value '{2}'", (object)key, (object)min, (object)defaultValue);
                return defaultValue;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.ErrorFormat("Can't parse option '{0}' to int. will use default value '{1}'", (object)key, (object)defaultValue);
                YoutubeOptions.Log.Error("Exception, ", ex);
            }
            return defaultValue;
        }

        protected bool GetValueAsBool(string key, bool defaultValue)
        {
            try
            {
                string str = this.configManager.GetValue(key) as string;
                if (str == null)
                    return defaultValue;
                return Convert.ToBoolean(str);
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.ErrorFormat("Can't parse option '{0}' to int. will use default value '{1}'", (object)key, (object)defaultValue);
                YoutubeOptions.Log.Error("Exception, ", ex);
            }
            return defaultValue;
        }
    }
}
