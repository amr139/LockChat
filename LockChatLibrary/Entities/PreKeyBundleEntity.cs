using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.Entities
{
    public class PreKeyBundleEntity
    {
        public int UserId { get; set; }
        public uint RegistrationId { get { return 1; } }
        public uint DeviceId { get { return 1; } }
        public uint PreKeyId { get; set; }
        public byte[] PreKeyPublic { get; set; }
        public uint SignedPreKeyId { get; set; }
        public byte[] SignedPreKeyPublic { get; set; }
        public byte[] SignedPreKeySignature { get; set; }
        public byte[] IdentityKey { get; set; }
    }
}
