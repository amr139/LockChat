using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
