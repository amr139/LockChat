using Dapper;
using LockChatLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace LockChatLibrary.Repositories
{
    public interface IMessageRepository
    {
        bool SaveMessage(MessageEntity msg);
        IEnumerable<MessageEntity> GetChat(int userid1, int userid2);
        IEnumerable<FriendEntity> GetFriends(int userid);
    }
    public class MessageRepository : IMessageRepository
    {
        public IEnumerable<MessageEntity> GetChat(int userid1, int userid2)
        {
            var query = @"SELECT Id, Stamp, SenderId, ReceiverId, EncryptedText FROM [dbo].[Messages] WHERE (SenderId = @user1 AND ReceiverId = @user2) OR (SenderId = @user2 AND ReceiverId = @user1)";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("User1", userid1);
                dp.Add("User2", userid2);

                return connection.Query<MessageEntity>(query, dp);
            }
        }
        
        public IEnumerable<FriendEntity> GetFriends(int userid)
        {
            var query = @"select ReceiverId as UserId, u.FirstName + u.LastName Name from dbo.Messages m left join dbo.Users u on m.ReceiverId=u.Id where m.senderid=@userid
union 
select SenderId as UserId, u.FirstName + u.LastName as Name from dbo.Messages m left join dbo.Users u on m.SenderId=u.Id where m.Receiverid=@userid";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("Userid", userid);

                return connection.Query<FriendEntity>(query, dp);
            }
        }

        public bool SaveMessage(MessageEntity msg)
        {
            
            var query = @"INSERT INTO [dbo].[Messages] (SenderId, ReceiverId, Stamp, EncryptedText) 
                                                 VALUES (@SenderId, @ReceiverId, @Stamp, @EncryptedText)";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("SenderId", msg.SenderId);
                dp.Add("ReceiverId", msg.ReceiverId);
                dp.Add("Stamp", msg.Stamp);
                dp.Add("EncryptedText", msg.EncryptedText);

                connection.Execute(query, dp);
            }

            return true;
        }
    }
}
