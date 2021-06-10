using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.Entities
{
    public class FriendEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public string IDtoURL()
        {
            return "chat/" + UserId;
        }
    }
}
