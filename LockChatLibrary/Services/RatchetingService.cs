using libsignal;
using libsignal.ecc;
using libsignal.protocol;
using libsignal.state;
using libsignal.state.impl;
using LockChatLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.Services
{
    public class RatchetingService
    {
        public SessionCipher aliceSessionCipher { get; set; }

        public RatchetingService(UserKeys keys, PreKeyBundleEntity bundle, SignalProtocolAddress ADDRESS)
        {
            IdentityKey alicePub = new IdentityKey(new DjbECPublicKey(keys.PublicKey));
            DjbECPrivateKey alicePriv = new DjbECPrivateKey(keys.PrivateKey);
            IdentityKeyPair aliceIdentityKeyPair = new IdentityKeyPair(alicePub, alicePriv);
            SignalProtocolStore aliceStore = new InMemorySignalProtocolStore(aliceIdentityKeyPair, (uint)keys.UserId);
            ECKeyPair signedPreKey = new ECKeyPair(new DjbECPublicKey(keys.SignedPrePublicKey), new DjbECPrivateKey(keys.SignedPrePublicKey));
            ECKeyPair unsignedPreKey = new ECKeyPair(new DjbECPublicKey(keys.UnsignedPrePublicKey), new DjbECPrivateKey(keys.UnsignedPrePrivateKey));
            aliceStore.StoreSignedPreKey((uint)keys.SignedPreKeyId, new SignedPreKeyRecord((uint)keys.SignedPreKeyId, (ulong)DateTime.UtcNow.Ticks, signedPreKey, keys.Signature));
            aliceStore.StorePreKey((uint)keys.UnsignedPreKeyId, new PreKeyRecord((uint)keys.UnsignedPreKeyId, unsignedPreKey));

            PreKeyBundle bobPreKeyBundle = new PreKeyBundle(
                                                    (uint)bundle.UserId,
                                                    bundle.DeviceId,
                                                    bundle.PreKeyId,
                                                    new DjbECPublicKey(bundle.PreKeyPublic),
                                                    bundle.SignedPreKeyId,
                                                    new DjbECPublicKey(bundle.SignedPreKeyPublic),
                                                    bundle.SignedPreKeySignature,
                                                    new IdentityKey(new DjbECPublicKey(bundle.IdentityKey))
                                                    );
            
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, ADDRESS);
            aliceSessionCipher = new SessionCipher(aliceStore, ADDRESS);
            aliceSessionBuilder.process(bobPreKeyBundle);
        }

        public byte[] Encrypt(string msg)
        {
            CiphertextMessage output = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(msg));
            return output.serialize();
        }
        public string Decrypt(byte[] msg)
        {
            return Encoding.UTF8.GetString(aliceSessionCipher.decrypt(new PreKeySignalMessage(msg)));
        }
    }
}
