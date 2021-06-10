using LockChatLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LockChatLibrary.API
{
    public interface IMessageClient
    {
        bool SaveMessage(MessageEntity msg);
        IEnumerable<MessageEntity> GetChat(int userid);

        IEnumerable<FriendEntity> GetFriends();

    }
    public class MessageClient : IMessageClient
    {
        ApiConfig config { get; set; }
        public MessageClient(ApiConfig config)
        {
            this.config = config;
        }

        public bool SaveMessage(MessageEntity msg)
        {
            return ClientHelper.Post<bool, MessageEntity>(config, "messages/SaveMessage", msg);
        }

        public IEnumerable<MessageEntity> GetChat(int userID)
        {
            return ClientHelper.Get<IEnumerable<MessageEntity>, int>(config, "messages/getchat/" + userID);
        }
        public IEnumerable<FriendEntity> GetFriends()
        {
            return ClientHelper.Get<IEnumerable<FriendEntity>, object>(config, "messages/getfriends");
        }

    }
}
