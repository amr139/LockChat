using LockChatLibrary.Entities;
using LockChatLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using libsignal;
using libsignal.ecc;
using libsignal.state.impl;
using libsignal.util;
using System.Linq;


namespace LockChatLibrary.Services
{

    public interface IMessageService
    {
        bool SaveMessage(MessageEntity msg);
        IEnumerable<MessageEntity> GetChat(int userid1, int userid2);
        IEnumerable<FriendEntity> GetFriends(int userid);
    }

    public class MessageService : IMessageService
    {
        private IMessageRepository messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public IEnumerable<MessageEntity> GetChat(int userid1, int userid2)
        {
            return messageRepository.GetChat(userid1, userid2);
        }

        public bool SaveMessage(MessageEntity msg)
        {
            return messageRepository.SaveMessage(msg);
        }
        public IEnumerable<FriendEntity> GetFriends(int userid)
        {
            return messageRepository.GetFriends(userid);
        }
    }
}
