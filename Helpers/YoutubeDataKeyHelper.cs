using Genesyslab.Desktop.Modules.YoutubeWorkItem.Configuration;
using Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels;
using Genesyslab.Enterprise.Commons.Collections;
using Genesyslab.Desktop.Modules.Sdr.Common.Helpers;
using Genesyslab.Desktop.Modules.Sdr.Common.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.Helpers
{
    internal static class YoutubeDataKeyHelper
    {
        internal static MessageType GetInteractionMethodType(this KeyValueCollection YoutubeData)
        {
            if (YoutubeData.ContainsKey(YoutubeDataKey.OutboundMethodType))
            {
                var method = YoutubeData.GetAsString(YoutubeDataKey.OutboundMethodType);
                return method != null ? method.ToMessageType() : MessageType.None;
            }
            return MessageType.None;
        }

        internal static int GetInteractionMethodIndex(this KeyValueCollection YoutubeData)
        {
            if (YoutubeData.ContainsKey(YoutubeDataKey.MsgIndex))
            {
                var indexStr = YoutubeData.GetAsString(YoutubeDataKey.MsgIndex);
                int index = -1;
                int.TryParse(indexStr, out index);

                return index;
            }
            return -1;
        }

        internal static string GetInteractionMethodID(this KeyValueCollection YoutubeData)
        {
            if (YoutubeData.ContainsKey(YoutubeDataKey.OutboundMsgId))
            {
                var msgID = YoutubeData.GetAsString(YoutubeDataKey.OutboundMsgId);

                return string.IsNullOrWhiteSpace(msgID) ? string.Empty : msgID;
            }
            return string.Empty;
        }

        internal static PostViewModel GetPostData(this KeyValueCollection YoutubeData)
        {
            try
            {
                PostViewModel post = new PostViewModel()
                {
                    Id = YoutubeData.GetAsString(YoutubeDataKey.PostData.ID),
                    AuthorName = YoutubeData.GetAsString(YoutubeDataKey.PostData.AuthorName),
                    Text = YoutubeData.GetAsString(YoutubeDataKey.PostData.Text),
                    Title = YoutubeData.GetAsString(YoutubeDataKey.PostData.Title),
                   // PostDateInixUTCStr = DateTime.Parse(YoutubeData.GetAsString(YoutubeDataKey.PostData.Date)),
                    AuthorImage = YoutubeData.GetAuthorImage(YoutubeDataKey.PostData.AuthorImage),
                    PostImage = YoutubeData.GetImage(YoutubeDataKey.PostData.Image)
                };
                var postDate = YoutubeData[YoutubeDataKey.PostData.Date];
                if (postDate != null)
                {
                    var timestampStr = postDate.ToString();
                    double doubleTimeStamp;
                    if (double.TryParse(timestampStr, out doubleTimeStamp))
                    {
                        post.PostDateInixUTCStr = Helper.GetDateTimeFromUnixTimeStamp(doubleTimeStamp);
                    }
                }

                YoutubeOptions.Log.InfoFormat("YoutubeWorkItem LoadPostData: {0} ", post);

                return post;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem LoadPostData Exception: ", ex);

                return null;
            }
        }

        internal static CommentViewModel GetInCommentData(this KeyValueCollection YoutubeData, string postID,string postAuthorName)
        {
            try
            {
                CommentViewModel comment = new CommentViewModel()
                {
                    PostId = postID,
                    //add new for display post profile name in reply
                    ReplyAuthorName = postAuthorName,
                    ParentId = postID,
                    Type = MessageType.Comment,
                    Id = YoutubeData.GetAsString(YoutubeDataKey.CommentData.ID),
                    AuthorName = YoutubeData.GetAsString(YoutubeDataKey.CommentData.AuthorName),
                    Text = YoutubeData.GetAsString(YoutubeDataKey.CommentData.Text),
                    //CommentDateUnixUtc = YoutubeData.GetAsString(YoutubeDataKey.CommentData.Date),
                    AuthorImage = YoutubeData.GetAuthorImage(YoutubeDataKey.CommentData.AuthorImage),
                    DeletedAtUnixStr = YoutubeData.GetAsString(YoutubeDataKey.CommentData.DeletedAt),
                    DeletedBy = YoutubeData.GetAsString(YoutubeDataKey.CommentData.DeletedBy),
                    IsDeleted = AttachedDataHelper.GetBoolFromAttachedData(YoutubeData, YoutubeDataKey.CommentData.IsDeleted, false),
                    IsLiked = AttachedDataHelper.GetBoolFromAttachedData(YoutubeData, YoutubeDataKey.CommentData.IsLiked, false)
                };
                var commentDate = YoutubeData[YoutubeDataKey.CommentData.Date];
                if (commentDate != null)
                {
                    var timestampStr = commentDate.ToString();
                    double doubleTimeStamp;
                    if (double.TryParse(timestampStr, out doubleTimeStamp))
                    {
                        comment.CommentDateUnixUtc = Helper.GetDateTimeFromUnixTimeStamp(doubleTimeStamp);
                    }
                    
                }

                comment.IsLiked = comment.IsDeleted ? false : comment.IsLiked;

                YoutubeOptions.Log.InfoFormat("YoutubeWorkItem LoadCommentData : {0}", (object)comment);
                return comment;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem", ex);

                return null;
            }
        }

        /// <summary>
        /// Gets the out list for comments/ replies depending on the data key sent
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataKey"> for comments send YoutubeDataKey.CommentDataKey and replies send YoutubeDataKey.ReplyDataKey </param>
        /// <returns></returns>
        internal static ObservableCollection<CommentViewModel> GetOutList(this KeyValueCollection data, IDataKey dataKey, string postID, MessageType messageType, string parentId, string dateReply,
            string postAuotherName)
        {
            try
            {
                var comments = new ObservableCollection<CommentViewModel>();

                string CommentText = data.GetAsString(dataKey.Text + "0");
                YoutubeOptions.Log.Info("youtubeReplyDatatext " + CommentText);
                YoutubeOptions.Log.Info("youtubeReplyDate data0"+ data.GetAsString(dataKey.Date + "0"));
                YoutubeOptions.Log.Info("replydate before convert " + data.GetAsString(dataKey.Date));

                DateTime replydate = DateTimeHelper.GetLocalDate(dateReply);
                YoutubeOptions.Log.Info("replydate after convert from ucs 1 " + replydate);
                int i = 0;
                while (CommentText != null)
                {
                    var comment = new CommentViewModel()
                    {
                        Index = data.GetIndex(dataKey.Index, i),
                        PostId = postID,
                        ParentId = parentId,
                        Type = messageType,
                        AuthorImage = data.GetAuthorImage(dataKey.AuthorImage + i),
                        AuthorName = data.GetAsString(dataKey.AuthorName + i),
                        CommentDateUnixUtc= replydate,
                        //old  CommentDateUnixUtc = Convert.ToDateTime(data.GetAsString(dataKey.Date + i)),
                        // CommentDateUnixUtc = DateTimeHelper.GetLocalDate(data.GetAsString(dataKey.Date)),
                        // CommentDateUnixUtc = DateTime.Parse(data.GetAsString(dataKey.Date)),
                        Text = CommentText,
                        IsDeleted = AttachedDataHelper.GetBoolFromAttachedData(data, dataKey.IsDeleted + i, false),
                        DeletedAtUnixStr = data.GetAsString(dataKey.DeletedAt + i),
                        DeletedBy = data.GetAsString(dataKey.DeletedBy + i)
                    };
                    comment.ReplyAuthorName = postAuotherName;


                    YoutubeOptions.Log.InfoFormat("YoutubeWorkItem GetOutListreply ", comment.CommentDateUnixUtc);

                    YoutubeOptions.Log.InfoFormat("YoutubeWorkItem GetOutList : {0}", (object)comment.CommentDateUnixUtc);

                    comments.Add(comment);
                    i++;
                    CommentText = data.GetAsString(dataKey.Text + i);
                }
                return comments;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem GetOutList Exception :", ex);
                return null;
            }
        }
        internal static ObservableCollection<CommentViewModel> GetOutReplyList(this KeyValueCollection data, IDataKey dataKey, string postID, MessageType messageType, string parentId,
            string dateReply,string postAuotherName)
        {
            try
            {
                var comments = new ObservableCollection<CommentViewModel>();

                string CommentText = data.GetAsString(dataKey.Text + "0");
                YoutubeOptions.Log.Info("youtubeReplyDatatext " + CommentText);
                YoutubeOptions.Log.Info("youtubeReplyDate data0" + data.GetAsString(dataKey.Date + "0"));
                YoutubeOptions.Log.Info("replydate before convert " + data.GetAsString(dataKey.Date));
                YoutubeOptions.Log.Info("replydate after convert from ucs2 " + dateReply);

                DateTime replydate = DateTimeHelper.GetLocalDate(dateReply);
                YoutubeOptions.Log.Info("replydate after convert from ucs " + replydate);
                int i = 0;
                while (CommentText != null)
                {
                    var comment = new CommentViewModel()
                    {
                        Index = data.GetIndex(dataKey.Index, i),
                        PostId = postID,
                        ParentId = parentId,
                        Type = messageType,
                        //to take defaut image of reply send null
                        AuthorImage = data.GetAuthorImage(null),
                        AuthorName = data.GetAsString(dataKey.AuthorName + i),
                        //old  CommentDateUnixUtc = Convert.ToDateTime(data.GetAsString(dataKey.Date + i)),
                        CommentDateUnixUtc = replydate,
                        //CommentDateUnixUtc = DateTime.Parse(dateReply),
                        // CommentDateUnixUtc = DateTimeHelper.GetLocalDate(data.GetAsString(dataKey.Date)),
                        // CommentDateUnixUtc = DateTime.Parse(data.GetAsString(dataKey.Date)),
                        Text = CommentText,
                        IsDeleted = AttachedDataHelper.GetBoolFromAttachedData(data, dataKey.IsDeleted + i, false),
                        DeletedAtUnixStr = data.GetAsString(dataKey.DeletedAt + i),
                        DeletedBy = data.GetAsString(dataKey.DeletedBy + i)
                    };
                    comment.ReplyAuthorName = postAuotherName;


                    YoutubeOptions.Log.InfoFormat("YoutubeWorkItem GetOutListreply ", comment.CommentDateUnixUtc);

                    YoutubeOptions.Log.InfoFormat("YoutubeWorkItem GetOutList : {0}", (object)comment.CommentDateUnixUtc);

                    comments.Add(comment);
                    i++;
                    CommentText = data.GetAsString(dataKey.Text + i);
                }
                return comments;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem GetOutList Exception :", ex);
                return null;
            }
        }
        public static int GetIndex(this KeyValueCollection data, string indexKey, int defIndex)
        {
            try
            {
                if (data.ContainsKey(indexKey))
                {
                    var index = data.GetAsInt(indexKey);
                    if (index > -1)
                        return index;
                }
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("Exception in GetIndex", ex);
            }
            return defIndex;
        }

        private static AuthorImageViewModel GetAuthorImage(this KeyValueCollection data, string imageKey)
        {
            try
            {
                AuthorImageViewModel authorImage = new AuthorImageViewModel();

                var postprofilepic = data[imageKey];
                if (postprofilepic != null)
                {
                    var image = postprofilepic.ToString();
                    YoutubeOptions.Log.Info("YoutubeWorkItem - " + imageKey + " ImageBinary: " + image);

                    authorImage.Image = new ImageViewModel();

                    authorImage.Image.ImageByte = ImageHelper.ToByteArray(postprofilepic);

                    authorImage.Image.ImageSrc =
                        image == null || image == string.Empty || image == "null" ?
                        YoutubeDefaultData.DefualtAuthorImage :
                        ImageHelper.LoadImage(authorImage.Image.ImageByte);

                    authorImage.Image.IsEmpty = false;
                }
                else
                {
                    YoutubeOptions.Log.Info("YoutubeWorkItem - " + imageKey + " is null loading default");
                    authorImage.Image.IsEmpty = true;
                    authorImage.Image.ImageSrc = YoutubeDefaultData.DefualtAuthorImage;
                  // authorImage.Image.ImageSrc = "";

                }

                return authorImage;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem", ex);
                AuthorImageViewModel authorImage = new AuthorImageViewModel();
                authorImage.Image = new ImageViewModel() { ImageSrc = YoutubeDefaultData.DefualtAuthorImage };

                return authorImage;
            }
        }

        private static ImageViewModel GetImage(this KeyValueCollection data, string key)
        {
            try
            {
                ImageViewModel image = new ImageViewModel();
                var imageUrl = data[key];
                if (imageUrl != null)
                {
                    YoutubeOptions.Log.Info("YoutubeWorkItem - " + key + " : " + imageUrl.ToString());

                    image.ImageSrc = ImageHelper.LoadImage(ImageHelper.ToByteArray(imageUrl));
                    image.IsEmpty = false;
                }
                else
                {
                    YoutubeOptions.Log.Info("YoutubeWorkItem - " + key + " is null loading default");
                    image.IsEmpty = true;
                }

                return image;
            }
            catch (Exception ex)
            {
                YoutubeOptions.Log.Error("YoutubeWorkItem", ex);
                return new ImageViewModel() { IsEmpty = true };
            }
        }
    }
}
