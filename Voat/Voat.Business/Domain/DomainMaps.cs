﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Voat.Data;
using Voat.Data.Models;
using Voat.Domain.Models;
using Voat.Domain.Query;
using Voat.Utilities;

namespace Voat.Domain
{
    public static class DomainMaps
    {
        public static IEnumerable<Domain.Models.Submission> Map(this IEnumerable<Data.Models.Submission> list)
        {
            var mapped = list.Select(x => x.Map()).ToList();
            return mapped;
        }

        public static IEnumerable<Domain.Models.Comment> Map(this IEnumerable<Data.Models.Comment> list, string subverse)
        {
            var mapped = list.Select(x => x.Map(subverse)).ToList();
            return mapped;
        }

        public static IEnumerable<Domain.Models.Comment> Map(this IEnumerable<Domain.Models.Comment> list, bool populateUserState = false)
        {
            var mapped = list.Select(x => { HydrateUserData(x); return x; }).ToList();
            return mapped;
        }

        public static IEnumerable<UserMessage> Map(this IEnumerable<UserMessage> list)
        {
            var mapped = list.Select(x => x.Map()).ToList();
            return mapped;
        }

        public static IEnumerable<Data.Models.Message> Map(this IEnumerable<Domain.Models.Message> list)
        {
            var mapped = list.Select(x => x.Map()).ToList();
            return mapped;
        }

        public static IEnumerable<Domain.Models.Message> Map(this IEnumerable<Data.Models.Message> list, bool protect = true)
        {
            var mapped = list.Select(x => x.Map(protect)).ToList();
            return mapped;
        }

        public static Data.Models.Message Map(this Domain.Models.Message message)
        {
            return new Data.Models.Message()
            {
                ID = message.ID,
                ParentID = message.ParentID,
                CorrelationID = message.CorrelationID,

                Content = message.Content,
                FormattedContent = message.FormattedContent,
                CommentID = message.CommentID,
                CreatedBy = message.CreatedBy,
                CreationDate = message.CreationDate,
                //Direction = (int)message.Direction,
                IsAnonymized = message.IsAnonymized,
                ReadDate = message.ReadDate,
                Recipient = message.Recipient,
                RecipientType = (int)message.RecipientType,
                Sender = message.Sender,
                SenderType = (int)message.SenderType,
                Title = message.Title,
                SubmissionID = message.SubmissionID,
                Subverse = message.Subverse,
                Type = (int)message.Type
            };
        }

        public static Domain.Models.Message Map(this Data.Models.Message message, bool protect = true)
        {
            var m = new Domain.Models.Message();

            m.ID = message.ID;
            m.ParentID = message.ParentID;
            m.CorrelationID = message.CorrelationID;

            m.Content = message.Content;
            m.FormattedContent = message.FormattedContent;
            //existing messages do not have formatted content saved - check here
            if (String.IsNullOrEmpty(m.FormattedContent))
            {
                Debug.Print("Formatting PM Content!");
                m.FormattedContent = Formatting.FormatMessage(m.Content);
            }

            m.CommentID = message.CommentID;
            m.CreatedBy = message.CreatedBy;
            m.CreationDate = message.CreationDate;
            m.IsAnonymized = message.IsAnonymized;

            m.ReadDate = message.ReadDate;

            if (message.IsAnonymized && protect)
            {
                m.Sender = message.ID.ToString();
            }
            else
            {
                m.Sender = message.Sender;
            }

            m.Recipient = message.Recipient;
            m.RecipientType = (IdentityType)message.RecipientType;
            m.SenderType = (IdentityType)message.SenderType;

            m.Title = message.Title;
            m.SubmissionID = message.SubmissionID;
            m.Subverse = message.Subverse;
            m.Type = (MessageType)message.Type;

            return m;
        }

        public static UserMessage Map(this UserMessage message)
        {
            UserMessage result = message;
            if (result != null)
            {
                if (String.IsNullOrEmpty(result.FormattedContent))
                {
                    result.FormattedContent = Formatting.FormatMessage(result.Content);
                }
            }
            return result;
        }

        public static Domain.Models.Submission Map(this Data.Models.Submission submission)
        {
            Domain.Models.Submission result = null;
            if (submission != null)
            {
                result = new Domain.Models.Submission()
                {
                    ID = submission.ID,
                    UserName = (submission.IsAnonymized ? submission.ID.ToString() : submission.UserName),

                    Title = submission.Title,
                    Url = submission.Url,
                    Content = (submission.Type == 1 ? submission.Content : (string)null),

                    //Support For Backwards compat, if FormattedContent is empty, do it here.
                    FormattedContent = (submission.Type == 1 && String.IsNullOrEmpty(submission.FormattedContent) ? Formatting.FormatMessage(submission.Content, true) : submission.FormattedContent),

                    LastEditDate = submission.LastEditDate,
                    ThumbnailUrl = VoatPathHelper.ThumbnailPath(submission.Thumbnail, true, true),
                    CommentCount = CommentCounter.CommentCount(submission.ID),
                    CreationDate = submission.CreationDate,
                    UpCount = (int)submission.UpCount,
                    Views = (int)submission.Views,
                    DownCount = (int)submission.DownCount,
                    Type = submission.Type == 1 ? SubmissionType.Text : SubmissionType.Link,
                    Subverse = submission.Subverse,
                    IsAnonymized = submission.IsAnonymized,
                    IsAdult = submission.IsAdult,
                    IsDeleted = submission.IsDeleted,
                    Rank = submission.Rank,
                    RelativeRank = submission.RelativeRank,
                    ArchiveDate = submission.ArchiveDate
                };

                //add flair if present
                if (!String.IsNullOrEmpty(submission.FlairCss) && !String.IsNullOrEmpty(submission.FlairLabel))
                {
                    result.Attributes.Add(new ContentAttribute() { ID = -1, Type = AttributeType.Flair, Name = submission.FlairLabel, CssClass = submission.FlairCss } );
                }
                if (result.IsAdult)
                {
                    result.Attributes.Add(new ContentAttribute() { ID = -1, Type = AttributeType.Data, Name = "NSFW", CssClass = "linkflairlabel" });
                }
                if (result.ArchiveDate.HasValue)
                {
                    result.Attributes.Add(new ContentAttribute() { ID = -1, Type = AttributeType.Data, Name = "Archived", CssClass = "linkflairlabel" });
                }
            }
            return result;
        }
        public static IEnumerable<Domain.Models.Set> Map(this IEnumerable<Voat.Data.Models.SubverseSet> list)
        {
            var mapped = list.Select(x => x.Map()).ToList();
            return mapped;
        }
        public static Domain.Models.Set Map(this Voat.Data.Models.SubverseSet subverseSet)
        {
            if (subverseSet != null)
            {
                var s = new Set();
                s.ID = subverseSet.ID;
                s.Name = subverseSet.Name;
                s.Title = subverseSet.Title;
                s.Description = subverseSet.Description;
                s.CreationDate = subverseSet.CreationDate;
                s.Type = (SetType)subverseSet.Type;
                s.UserName = subverseSet.UserName;
                s.SubscriberCount = subverseSet.SubscriberCount;
                s.IsPublic = subverseSet.IsPublic;
                return s;
            }
            return null;
        }

        public static Domain.Models.Comment Map(this Data.Models.Comment comment, string subverse, bool populateUserState = false)
        {
            Domain.Models.Comment result = null;
            if (comment != null)
            {
                result = MapToNestedComment(comment, subverse, populateUserState);
            }
            return result;
        }

        public static Models.SubverseInformation Map(this Subverse subverse)
        {
            SubverseInformation result = null;
            if (subverse != null)
            {
                result = new Models.SubverseInformation()
                {
                    Name = subverse.Name,
                    Title = subverse.Title,
                    Description = subverse.Description,
                    CreationDate = subverse.CreationDate,
                    SubscriberCount = (subverse.SubscriberCount == null ? 0 : subverse.SubscriberCount.Value),
                    Sidebar = subverse.SideBar,
                    //Type = subverse.Type,
                    IsAnonymized = subverse.IsAnonymized,
                    IsAdult = subverse.IsAdult,

                    //IsAdminDisabled = (subverse.IsAdminDisabled.HasValue ? subverse.IsAdminDisabled.Value : false),
                    CreatedBy = subverse.CreatedBy
                };
                result.FormattedSidebar = Formatting.FormatMessage(subverse.SideBar, true);
            }

            return result;
        }

        public static usp_CommentTree_Result MapToTree(this Data.Models.Comment comment)
        {
            usp_CommentTree_Result result = null;
            if (comment != null)
            {
                result = new usp_CommentTree_Result()
                {
                    ChildCount = 0,
                    Depth = 0,
                    Path = "",
                    Subverse = "",
                    ID = comment.ID,
                    ParentID = comment.ParentID,
                    Content = comment.Content,
                    FormattedContent = comment.FormattedContent,
                    CreationDate = comment.CreationDate,
                    LastEditDate = comment.LastEditDate,
                    SubmissionID = comment.SubmissionID,
                    UpCount = comment.UpCount,
                    DownCount = comment.DownCount,
                    IsDistinguished = comment.IsDistinguished,
                    IsDeleted = comment.IsDeleted,
                    IsAnonymized = comment.IsAnonymized,
                    UserName = comment.UserName
                };
            }
            return result;
        }

        public static usp_CommentTree_Result MapToTree(this Domain.Models.Comment comment)
        {
            usp_CommentTree_Result result = null;
            if (comment != null)
            {
                result = new usp_CommentTree_Result()
                {
                    ChildCount = 0,
                    Depth = 0,
                    Path = "",
                    Subverse = comment.Subverse,
                    ID = comment.ID,
                    ParentID = comment.ParentID,
                    Content = comment.Content,
                    FormattedContent = comment.FormattedContent,
                    CreationDate = comment.CreationDate,
                    LastEditDate = comment.LastEditDate,
                    SubmissionID = comment.SubmissionID,
                    UpCount = comment.UpCount,
                    DownCount = comment.DownCount,
                    IsDistinguished = comment.IsDistinguished,
                    IsDeleted = comment.IsDeleted,
                    IsAnonymized = comment.IsAnonymized,
                    UserName = comment.UserName
                };
            }
            return result;
        }

        public static NestedComment Map(this usp_CommentTree_Result treeComment, string submissionOwnerName, IEnumerable<VoteValue> commentVotes = null, IEnumerable<BlockedItem> userBlocks = null)
        {
            NestedComment result = null;
            if (treeComment != null)
            {
                result = new NestedComment();
                result.ID = treeComment.ID;
                result.ParentID = treeComment.ParentID;
                result.ChildCount = treeComment.ChildCount.Value;
                result.Children.TotalCount = result.ChildCount;
                result.Content = treeComment.Content;
                result.FormattedContent = treeComment.FormattedContent;
                result.UserName = treeComment.UserName;
                result.UpCount = (int)treeComment.UpCount;
                result.DownCount = (int)treeComment.DownCount;
                result.CreationDate = treeComment.CreationDate;
                result.IsAnonymized = treeComment.IsAnonymized;
                result.IsDeleted = treeComment.IsDeleted;
                result.IsDistinguished = treeComment.IsDistinguished;
                result.LastEditDate = treeComment.LastEditDate;
                result.SubmissionID = treeComment.SubmissionID;
                result.Subverse = treeComment.Subverse;
                result.IsSubmitter = (treeComment.UserName == submissionOwnerName);

                //Set User State and secure comment
                HydrateUserData(result, false, commentVotes, userBlocks);
            }
            return result;
        }

        public static NestedComment MapToNestedComment(this Data.Models.Comment comment, string subverse, bool populateUserState = false)
        {
            NestedComment result = null;
            if (comment != null)
            {
                result = new NestedComment();
                result.ID = comment.ID;
                result.ParentID = comment.ParentID;
                result.ChildCount = 0;
                result.Content = comment.Content;
                result.FormattedContent = comment.FormattedContent;
                result.UserName = comment.UserName;
                result.UpCount = (int)comment.UpCount;
                result.DownCount = (int)comment.DownCount;
                result.CreationDate = comment.CreationDate;
                result.IsAnonymized = comment.IsAnonymized;
                result.IsDeleted = comment.IsDeleted;
                result.IsDistinguished = comment.IsDistinguished;
                result.LastEditDate = comment.LastEditDate;
                result.SubmissionID = comment.SubmissionID;

                //Just a note, the entire Subverse in Data models for comments is a bit hacky as this info is needed in the app but data models don't contain it.
                if (String.IsNullOrEmpty(subverse))
                {
                    //TODO: need to convert pipeline to support this or pull this data out of the db
                    result.Subverse = "TODO";
                }
                else
                {
                    result.Subverse = subverse;
                }

                //Set User State and secure comment
                HydrateUserData(result, populateUserState);
            }
            return result;
        }

        public static NestedComment Map(this Domain.Models.Comment comment, bool populateMissingUserState = false)
        {
            NestedComment result = null;
            if (comment != null)
            {
                result = new NestedComment();
                result.ID = comment.ID;
                result.ParentID = comment.ParentID;
                result.ChildCount = 0;
                result.Content = comment.Content;
                result.FormattedContent = comment.FormattedContent;
                result.UserName = comment.UserName;
                result.UpCount = (int)comment.UpCount;
                result.DownCount = (int)comment.DownCount;
                result.CreationDate = comment.CreationDate;
                result.IsAnonymized = comment.IsAnonymized;
                result.IsDeleted = comment.IsDeleted;
                result.IsDistinguished = comment.IsDistinguished;
                result.LastEditDate = comment.LastEditDate;
                result.SubmissionID = comment.SubmissionID;
                result.Subverse = comment.Subverse;

                //Set User State and secure comment
                HydrateUserData(result, populateMissingUserState);
            }
            return result;
        }
        public static void HydrateUserData(IEnumerable<Domain.Models.Submission> submissions)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (submissions != null && submissions.Any()))
            {
                using (var repo = new Repository())
                {
                    var votes = repo.UserVoteStatus(Thread.CurrentPrincipal.Identity.Name, ContentType.Submission, submissions.Select(x => x.ID).ToArray());
                    foreach (var s in submissions)
                    {
                        var voteValue = votes.FirstOrDefault(x => x.ID == s.ID);
                        s.Vote = (voteValue == null ? 0 : voteValue.Value);
                    }
                    //saves are cached
                }
            }
        }
        public static void HydrateUserData(Domain.Models.Submission submission)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (submission != null))
            {
                using (var repo = new Repository())
                {
                    var vote = repo.UserVoteStatus(Thread.CurrentPrincipal.Identity.Name, ContentType.Submission, submission.ID);
                    submission.Vote = vote;
                    //saves are cached
                }
            }
        }
        public static void HydrateUserData(IEnumerable<Domain.Models.Comment> comments)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated && (comments != null && comments.Any()))
            {
                string userName = Thread.CurrentPrincipal.Identity.Name;
                if (!String.IsNullOrEmpty(userName))
                {
                    using (var repo = new Repository())
                    {
                        var votes = repo.UserVoteStatus(userName, ContentType.Comment, comments.Select(x => x.ID).ToArray());
                        var q = new QueryUserBlocks();
                        var blockedUsers = q.Execute().Where(x => x.Type == DomainType.User);

                        foreach (var comment in comments)
                        {
                            HydrateUserData(comment, false, votes, blockedUsers);
                            //comment.IsOwner = comment.UserName == userName;
                            //var voteValue = votes.FirstOrDefault(x => x.ID == comment.ID);
                            //comment.Vote = (voteValue == null ? 0 : voteValue.Value);
                            //comment.IsSaved = UserHelper.IsSaved(ContentType.Comment, comment.ID);
                            //Protect(comment);
                        }
                    }
                }
            }
        }
        public static void HydrateUserData(Domain.Models.Comment comment, bool populateMissingUserState = false, IEnumerable<VoteValue> commentVotes = null, IEnumerable<BlockedItem> userBlocks = null)
        {
            if (comment != null)
            {
                string userName = Thread.CurrentPrincipal.Identity.IsAuthenticated ? Thread.CurrentPrincipal.Identity.Name : null;
                if (!String.IsNullOrEmpty(userName))
                {
                    comment.IsOwner = comment.UserName == userName;
                    comment.Vote = 0;
                    if (commentVotes != null)
                    {
                        var vote = commentVotes.FirstOrDefault(x => x.ID == comment.ID);
                        comment.Vote = vote == null ? 0 : vote.Value;
                    }
                    else if (populateMissingUserState)
                    {
                        using (var repo = new Repository())
                        {
                            //comment.Vote = VotingComments.CheckIfVotedComment(userName, comment.ID);
                            comment.Vote = repo.UserVoteStatus(userName, ContentType.Comment, comment.ID);
                        }
                    }
                    //collapse comment threads when user is blocked
                    if (!comment.IsAnonymized && userBlocks != null && userBlocks.Any())
                    {
                        comment.IsCollapsed = userBlocks.Any(x => comment.UserName.IsEqual(x.Name));
                    }
                    comment.IsSaved = false;
                    comment.IsSaved = UserHelper.IsSaved(ContentType.Comment, comment.ID);
                }
                Protect(comment);
                ////Swap UserName
                //comment.UserName = (comment.IsAnonymized ? comment.ID.ToString() : comment.UserName);
            }
        }
        private static void Protect(Domain.Models.Comment comment)
        {
            if (comment != null)
            {
                //Swap UserName
                comment.UserName = (comment.IsAnonymized ? comment.ID.ToString() : comment.UserName);
                //Ensure Deleted - this isn't going to work with comment moderation log, I think
                if (comment.IsDeleted)
                {
                    comment.UserName = "";
                    comment.Content = "Deleted";
                    comment.FormattedContent = "<p>Deleted</p>";
                }

            }
        }
    }
}
