using libsignal;
using libsignal.ecc;
using libsignal.state.impl;
using libsignal.util;


namespace LockChatTest
{
    class TestInMemorySignalProtocolStore : InMemorySignalProtocolStore
    {
        public TestInMemorySignalProtocolStore()
            : base(generateIdentityKeyPair(), generateRegistrationId())
        { }

        private static IdentityKeyPair generateIdentityKeyPair()
        {
            ECKeyPair identityKeyPairKeys = Curve.generateKeyPair();
            var pub = new IdentityKey(identityKeyPairKeys.getPublicKey());
            var priv = identityKeyPairKeys.getPrivateKey();
            return new IdentityKeyPair(pub, priv);
        }

        private static uint generateRegistrationId()
        {
            return KeyHelper.generateRegistrationId(false);
        }
    }
}
