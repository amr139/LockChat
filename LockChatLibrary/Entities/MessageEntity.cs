using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.Entities
{
    public class MessageEntity
    {
        public int Id { get; set; }
        public DateTimeOffset Stamp { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Text { get; set; }
        public byte[] EncryptedText { get; set; }
    }
}
