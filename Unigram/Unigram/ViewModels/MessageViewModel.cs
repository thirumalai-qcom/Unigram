﻿using System;
using System.Collections.Generic;
using Telegram.Td.Api;
using Unigram.Common;
using Unigram.Services;
using Unigram.ViewModels.Delegates;

namespace Unigram.ViewModels
{
    public class MessageViewModel : MessageWithOwner
    {
        private readonly IPlaybackService _playbackService;
        private readonly WeakReference _delegate;

        private WeakAction _updateSelection;

        public MessageViewModel(IClientService clientService, IPlaybackService playbackService, IMessageDelegate delegato, Message message)
            : base(clientService, message)
        {
            _playbackService = playbackService;
            _delegate = new WeakReference(delegato);
        }

        public void SelectionChanged()
        {
            if (_updateSelection != null
                && _updateSelection.IsAlive
                && _updateSelection.Target != null)
            {
                _updateSelection.Execute();
            }
        }

        public void UpdateSelectionCallback(object target, Action action)
        {
            if (_updateSelection != null)
            {
                _updateSelection.MarkForDeletion();
            }

            if (target != null)
            {
                _updateSelection = new WeakAction(target, action);
            }
        }

        public IPlaybackService PlaybackService => _playbackService;
        public IMessageDelegate Delegate => _delegate.Target as IMessageDelegate;

        public bool IsInitial { get; set; } = true;

        public bool IsFirst { get; set; } = true;
        public bool IsLast { get; set; } = true;

        // Used only by animated emojis
        public Sticker Interaction { get; set; }


        public Photo GetPhoto() => _message.GetPhoto();

        public bool IsService() => _message.IsService();

        private bool? _isSaved;
        public bool IsSaved => _isSaved ??= _message.IsSaved(_clientService.Options.MyId);

        public bool IsSecret() => _message.IsSecret();

        public MessageViewModel ReplyToMessage { get; set; }
        public ReplyToMessageState ReplyToMessageState { get; set; } = ReplyToMessageState.None;

        /// <summary>
        /// This is used for additional content that's generated by the app
        /// </summary>
        public MessageContent GeneratedContent { get; set; }
        public bool GeneratedContentUnread { get; set; }

        public BaseObject GetSender()
        {
            if (_message.SenderId is MessageSenderUser user)
            {
                return ClientService.GetUser(user.UserId);
            }
            else if (_message.SenderId is MessageSenderChat chat)
            {
                return ClientService.GetChat(chat.ChatId);
            }

            return null;
        }

        public User GetViaBotUser()
        {
            if (_message.ViaBotUserId != 0)
            {
                return ClientService.GetUser(_message.ViaBotUserId);
            }

            if (ClientService.TryGetUser(_message.SenderId, out User user) && user.Type is UserTypeBot)
            {
                return user;
            }

            return null;
        }

        public Chat GetChat()
        {
            return ClientService.GetChat(_message.ChatId);
        }

        public void Replace(Message message)
        {
            _message = message;
        }

        private bool? _isShareable;
        public bool IsShareable => _isShareable ??= GetIsShareable();

        private bool GetIsShareable()
        {
            if (SchedulingState != null)
            {
                return false;
            }
            //else if (eventId != 0)
            //{
            //    return false;
            //}
            else if (IsSaved)
            {
                return true;
            }
            else if (Content is MessageSticker or MessageDice)
            {
                return false;
            }
            else if (ForwardInfo?.Origin is MessageForwardOriginChannel && !IsOutgoing)
            {
                return true;
            }
            else if (SenderId is MessageSenderUser senderUser)
            {
                if (Content is MessageText text && text.WebPage == null)
                {
                    return false;
                }

                var user = ClientService.GetUser(senderUser.UserId);
                if (user != null && user.Type is UserTypeBot)
                {
                    return true;
                }

                if (!IsOutgoing)
                {
                    if (Content is MessageGame or MessageInvoice)
                    {
                        return true;
                    }

                    var chat = ClientService.GetChat(ChatId);
                    if (chat != null && chat.Type is ChatTypeSupergroup super && !super.IsChannel)
                    {
                        var supergroup = ClientService.GetSupergroup(super.SupergroupId);
                        return supergroup != null && supergroup.Username.Length > 0 && Content is not MessageContact and not MessageLocation;
                    }
                }
            }
            else if (IsChannelPost)
            {
                if (ViaBotUserId == 0 && ReplyToMessageId == 0 || Content is not MessageSticker)
                {
                    return true;
                }
            }

            return false;
        }

        private bool? _hasSenderPhoto;
        public bool HasSenderPhoto => _hasSenderPhoto ??= GetHasSenderPhoto();

        private bool GetHasSenderPhoto()
        {
            if (IsService())
            {
                return false;
            }

            if (IsChannelPost)
            {
                return false;
            }
            else if (IsSaved)
            {
                return true;
            }
            else if (IsOutgoing)
            {
                return false;
            }

            var chat = GetChat();
            if (chat != null && (chat.Type is ChatTypeSupergroup || chat.Type is ChatTypeBasicGroup))
            {
                return true;
            }

            return false;
        }


        public int AnimationHash()
        {
            return base.GetHashCode();
        }

        public int EmojiHash()
        {
            return Content.GetHashCode();
        }

        public void UpdateWith(MessageViewModel message)
        {
            UpdateWith(message.Get());
        }

        public void UpdateWith(Message message)
        {
            _message.AuthorSignature = message.AuthorSignature;
            _message.CanBeDeletedForAllUsers = message.CanBeDeletedForAllUsers;
            _message.CanBeDeletedOnlyForSelf = message.CanBeDeletedOnlyForSelf;
            _message.CanBeEdited = message.CanBeEdited;
            _message.CanBeSaved = message.CanBeSaved;
            _message.CanBeForwarded = message.CanBeForwarded;
            _message.CanGetMessageThread = message.CanGetMessageThread;
            _message.CanGetStatistics = message.CanGetStatistics;
            _message.ChatId = message.ChatId;
            _message.ContainsUnreadMention = message.ContainsUnreadMention;
            //_message.Content = message.Content;
            //_message.Date = message.Date;
            _message.EditDate = message.EditDate;
            _message.ForwardInfo = message.ForwardInfo;
            _message.Id = message.Id;
            _message.IsChannelPost = message.IsChannelPost;
            _message.IsOutgoing = message.IsOutgoing;
            _message.IsPinned = message.IsPinned;
            _message.MessageThreadId = message.MessageThreadId;
            _message.MediaAlbumId = message.MediaAlbumId;
            _message.ReplyMarkup = message.ReplyMarkup;
            _message.ReplyInChatId = message.ReplyInChatId;
            _message.ReplyToMessageId = message.ReplyToMessageId;
            _message.SenderId = message.SenderId;
            _message.SendingState = message.SendingState;
            _message.Ttl = message.Ttl;
            _message.TtlExpiresIn = message.TtlExpiresIn;
            _message.ViaBotUserId = message.ViaBotUserId;
            _message.InteractionInfo = message.InteractionInfo;
            _message.UnreadReactions = message.UnreadReactions;

            if (_message.Content is MessageAlbum album)
            {
                FormattedText caption = null;

                if (album.IsMedia)
                {
                    foreach (var child in album.Messages)
                    {
                        var childCaption = child.Content?.GetCaption();
                        if (childCaption != null && !string.IsNullOrEmpty(childCaption.Text))
                        {
                            if (caption == null || string.IsNullOrEmpty(caption.Text))
                            {
                                caption = childCaption;
                            }
                            else
                            {
                                caption = null;
                                break;
                            }
                        }
                    }
                }
                else if (album.Messages.Count > 0)
                {
                    caption = album.Messages[album.Messages.Count - 1].Content.GetCaption();
                }

                album.Caption = caption ?? new FormattedText();
            }
        }
    }

    public class MessageWithOwner
    {
        protected readonly IClientService _clientService;
        protected Message _message;

        public MessageWithOwner(IClientService clientService, Message message)
        {
            _clientService = clientService;
            _message = message;
        }

        public override string ToString()
        {
            return _message.ToString();
        }

        public string CombinedId => $"{ChatId},{Id}";

        public IClientService ClientService => _clientService;

        public ReplyMarkup ReplyMarkup { get => _message.ReplyMarkup; set => _message.ReplyMarkup = value; }
        public MessageContent Content { get => _message.Content; set => _message.Content = value; }
        public long MediaAlbumId => _message.MediaAlbumId;
        public MessageInteractionInfo InteractionInfo { get => _message.InteractionInfo; set => _message.InteractionInfo = value; }
        public string AuthorSignature => _message.AuthorSignature;
        public long ViaBotUserId => _message.ViaBotUserId;
        public double TtlExpiresIn { get => _message.TtlExpiresIn; set => _message.TtlExpiresIn = value; }
        public int Ttl => _message.Ttl;
        public long ReplyToMessageId { get => _message.ReplyToMessageId; set => _message.ReplyToMessageId = value; }
        public long ReplyInChatId => _message.ReplyInChatId;
        public MessageForwardInfo ForwardInfo => _message.ForwardInfo;
        public IList<UnreadReaction> UnreadReactions { get => _message.UnreadReactions; set => _message.UnreadReactions = value; }
        public int EditDate { get => _message.EditDate; set => _message.EditDate = value; }
        public int Date => _message.Date;
        public bool ContainsUnreadMention { get => _message.ContainsUnreadMention; set => _message.ContainsUnreadMention = value; }
        public bool IsChannelPost => _message.IsChannelPost;
        public bool CanBeDeletedForAllUsers => _message.CanBeDeletedForAllUsers;
        public bool CanBeDeletedOnlyForSelf => _message.CanBeDeletedOnlyForSelf;
        public bool CanBeForwarded => _message.CanBeForwarded;
        public bool CanBeEdited => _message.CanBeEdited;
        public bool CanBeSaved => _message.CanBeSaved;
        public bool CanGetMessageThread => _message.CanGetMessageThread;
        public bool CanGetStatistics => _message.CanGetStatistics;
        public bool CanGetViewers => _message.CanGetViewers;
        public bool IsOutgoing { get => _message.IsOutgoing; set => _message.IsOutgoing = value; }
        public bool IsPinned { get => _message.IsPinned; set => _message.IsPinned = value; }
        public MessageSchedulingState SchedulingState => _message.SchedulingState;
        public MessageSendingState SendingState => _message.SendingState;
        public long ChatId => _message.ChatId;
        public long MessageThreadId => _message.MessageThreadId;
        public MessageSender SenderId => _message.SenderId;
        public long Id => _message.Id;

        public override bool Equals(object obj)
        {
            if (obj is Message y)
            {
                return Id == y.Id && ChatId == y.ChatId;
            }
            else if (obj is MessageViewModel ym)
            {
                return Id == ym.Id && ChatId == ym.ChatId;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ ChatId.GetHashCode();
        }

        public Message Get()
        {
            return _message;
        }
    }

    public enum ReplyToMessageState
    {
        None,
        Loading,
        Deleted,
        Hidden
    }
}
