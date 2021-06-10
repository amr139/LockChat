using libsignal;
using libsignal.ecc;
using libsignal.util;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.Entities
{
    public class UserKeys
    {
        public int UserId { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] UnsignedPrePublicKey { get; set; }
        public byte[] UnsignedPrePrivateKey { get; set; }
        public byte[] SignedPrePublicKey { get; set; }
        public byte[] SignedPrePrivateKey { get; set; }
        public int UnsignedPreKeyId { get; set; }
        public byte[] Signature { get; set; }
        public int SignedPreKeyId { get; set; }

        public UserKeys(int idUser)
        {
            ECKeyPair identityKeyPairKeys = Curve.generateKeyPair();
            ECKeyPair userUnsignedPreKey = Curve.generateKeyPair();
            ECKeyPair userSignedPreKey = Curve.generateKeyPair();
            var pub = new IdentityKey(identityKeyPairKeys.getPublicKey());
            var priv = identityKeyPairKeys.getPrivateKey();
            int userUnsignedPreKeyId = new Random().Next((int)Medium.MAX_VALUE);
            byte[] userSignature = Curve.calculateSignature(priv, userSignedPreKey.getPublicKey().serialize());
            int userSignedPreKeyId = (int)new Random().Next((int)Medium.MAX_VALUE);

            var pubSerialized = pub.serialize();
            var pubSerializedGood = new byte[32];
            Array.Copy(pubSerialized, 1, pubSerializedGood, 0, 32);
            var unsignedPrePublicKeySerialized = userUnsignedPreKey.getPublicKey().serialize();
            var unsignedPrePublicKeySerializedGood = new byte[32];
            Array.Copy(unsignedPrePublicKeySerialized, 1, unsignedPrePublicKeySerializedGood, 0, 32);
            var signedPrePublicKeySerialized = userSignedPreKey.getPublicKey().serialize();
            var signedPrePublicKeySerializedGood = new byte[32];
            Array.Copy(signedPrePublicKeySerialized, 1, signedPrePublicKeySerializedGood, 0, 32);

            UserId = idUser;
            PublicKey = pubSerializedGood;
            PrivateKey = priv.serialize();
            UnsignedPrePublicKey = unsignedPrePublicKeySerializedGood;
            UnsignedPrePrivateKey = userUnsignedPreKey.getPrivateKey().serialize();
            SignedPrePublicKey = signedPrePublicKeySerializedGood;
            SignedPrePrivateKey = userSignedPreKey.getPrivateKey().serialize();
            UnsignedPreKeyId = userUnsignedPreKeyId;
            Signature = userSignature;
            SignedPreKeyId = userSignedPreKeyId;

        }
    }
}
