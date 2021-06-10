using libsignal;
using libsignal.ecc;
using libsignal.protocol;
using libsignal.state;
using libsignal.state.impl;
using libsignal.util;
using LockChatLibrary;
using LockChatLibrary.API;
using LockChatLibrary.Entities;
using LockChatLibrary.Repositories;
using LockChatLibrary.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace LockChatTest
{
    class Program
    {
        private static readonly SignalProtocolAddress BOB_ADDRESS = new SignalProtocolAddress("+14151231234", 1);
        private static readonly SignalProtocolAddress ALICE_ADDRESS = new SignalProtocolAddress("+14159998888", 1);

        private static readonly ECKeyPair aliceSignedPreKey = Curve.generateKeyPair();
        private static readonly ECKeyPair bobSignedPreKey = Curve.generateKeyPair();

        private static readonly uint aliceSignedPreKeyId = (uint)new Random().Next((int)Medium.MAX_VALUE);
        private static readonly uint bobSignedPreKeyId = (uint)new Random().Next((int)Medium.MAX_VALUE);

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //TestLocal2();
            //Test();
            //TestAuth();
            //TestLocal2();
        }

        static void TestLocal2()
        {
            var userRepository = new UserRepository();
            var userService = new UserService(userRepository);

            //userService.GenerateKeys(1);
            //userService.GenerateKeys(1002);
            //userService.GenerateKeys(3);

            var keysAlice = userService.GetKeys(1);
            var keysBob = userService.GetKeys(2);
            var bundleAlice = userService.GetKeyBundle(1);
            var bundleBob = userService.GetKeyBundle(2);

            var alice = new RatchetingService(keysAlice, bundleBob, BOB_ADDRESS);
            var bob = new RatchetingService(keysBob, bundleAlice, ALICE_ADDRESS);

            var msg = alice.Encrypt("hola");
            var clear = bob.Decrypt(msg);
        }

        static void TestLocal()
        {
            var userRepository = new UserRepository();
            var userService = new UserService(userRepository);

            //userService.GenerateKeys(1);
            //userService.GenerateKeys(2);

            var keys = userService.GetKeys(1);
            var bundle = userService.GetKeyBundle(2);

            IdentityKey alicePub = new IdentityKey(new DjbECPublicKey(keys.PublicKey));
            DjbECPrivateKey alicePriv = new DjbECPrivateKey(keys.PrivateKey);
            IdentityKeyPair aliceIdentityKeyPair = new IdentityKeyPair(alicePub, alicePriv);
            SignalProtocolStore aliceStore = new InMemorySignalProtocolStore(aliceIdentityKeyPair, 1);
            ECKeyPair signedPreKey = new ECKeyPair(new DjbECPublicKey(keys.SignedPrePublicKey), new DjbECPrivateKey(keys.SignedPrePublicKey));
            ECKeyPair unsignedPreKey = new ECKeyPair(new DjbECPublicKey(keys.UnsignedPrePublicKey), new DjbECPrivateKey(keys.UnsignedPrePrivateKey));
            aliceStore.StoreSignedPreKey((uint)keys.SignedPreKeyId, new SignedPreKeyRecord((uint)keys.SignedPreKeyId, (ulong)DateTime.UtcNow.Ticks, signedPreKey, keys.Signature));
            aliceStore.StorePreKey((uint)keys.UnsignedPreKeyId, new PreKeyRecord((uint)keys.UnsignedPreKeyId, unsignedPreKey));

            PreKeyBundle bobPreKeyBundle = new PreKeyBundle(
                                                    bundle.RegistrationId, 
                                                    bundle.DeviceId, 
                                                    bundle.PreKeyId, 
                                                    new IdentityKey(new DjbECPublicKey(bundle.PreKeyPublic)).getPublicKey(), 
                                                    bundle.SignedPreKeyId, 
                                                    new IdentityKey(new DjbECPublicKey(bundle.SignedPreKeyPublic)).getPublicKey(), 
                                                    bundle.SignedPreKeySignature, 
                                                    new IdentityKey(new DjbECPublicKey(bundle.IdentityKey))
                                                    );
            var result = Curve.verifySignature(new IdentityKey(new DjbECPublicKey(bundle.IdentityKey)).getPublicKey(), new DjbECPublicKey(bundle.SignedPreKeyPublic).serialize(), bundle.SignedPreKeySignature);

            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            aliceSessionBuilder.process(bobPreKeyBundle);

            CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            var serie = messageForBob.serialize();

        }

        static void TestAuth()
        {
            var config = new ApiConfig();
            config.BaseUrl = Configuration.ApiUrl;
            var userClient = new UserClient(config);

            var user = new UserEntity
            {
                Email = "test@test.com",
                Password = "testpassword",
                FirstName = "Test",
                LastName = "Test apellido"
            };
            userClient.AddUser(user);
            var token = userClient.Authenticate(user);
        }

        static void Test()
        {
            SignalProtocolStore aliceStore = new TestInMemorySignalProtocolStore();
            SignalProtocolStore bobStore = new TestInMemorySignalProtocolStore();

            PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
            PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            aliceSessionBuilder.process(bobPreKeyBundle);
            bobSessionBuilder.process(alicePreKeyBundle);

            CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

            //Assert.AreEqual(CiphertextMessage.PREKEY_TYPE, messageForBob.getType());
            //Assert.AreEqual(CiphertextMessage.PREKEY_TYPE, messageForAlice.getType());

            //Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new PreKeySignalMessage(messageForAlice.serialize()));
            byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeySignalMessage(messageForBob.serialize()));

            //Assert.AreEqual("sample message", Encoding.UTF8.GetString(alicePlaintext));
            //Assert.AreEqual("hey there", Encoding.UTF8.GetString(bobPlaintext));

            //Assert.AreEqual((uint)3, aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion());
            //Assert.AreEqual((uint)3, bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion());

            //Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            //Assert.AreEqual(CiphertextMessage.WHISPER_TYPE, aliceResponse.getType());

            byte[] responsePlaintext = bobSessionCipher.decrypt(new SignalMessage(aliceResponse.serialize()));

            //Assert.AreEqual("second message", Encoding.UTF8.GetString(responsePlaintext));
            //Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            //Assert.AreEqual(CiphertextMessage.WHISPER_TYPE, finalMessage.getType());

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new SignalMessage(finalMessage.serialize()));

            //Assert.AreEqual("third message", Encoding.UTF8.GetString(finalPlaintext));
            //Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        static private bool isSessionIdEqual(SignalProtocolStore aliceStore, SignalProtocolStore bobStore)
        {
            return ByteUtil.isEqual(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getAliceBaseKey(),
                                 bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getAliceBaseKey());
        }

        static private PreKeyBundle createAlicePreKeyBundle(SignalProtocolStore aliceStore)
        {
            ECKeyPair aliceUnsignedPreKey = Curve.generateKeyPair();
            int aliceUnsignedPreKeyId = new Random().Next((int)Medium.MAX_VALUE);
            var alicePrivateKey = aliceStore.GetIdentityKeyPair().getPrivateKey();
            var alicePublicKeySerialized = aliceSignedPreKey.getPublicKey().serialize();
            byte[] aliceSignature = Curve.calculateSignature(alicePrivateKey, alicePublicKeySerialized);

            PreKeyBundle alicePreKeyBundle = new PreKeyBundle(
                1, 
                1,
                (uint)aliceUnsignedPreKeyId, 
                aliceUnsignedPreKey.getPublicKey(),
                aliceSignedPreKeyId, 
                aliceSignedPreKey.getPublicKey(),
                aliceSignature, 
                aliceStore.GetIdentityKeyPair().getPublicKey());

            aliceStore.StoreSignedPreKey(aliceSignedPreKeyId, new SignedPreKeyRecord(aliceSignedPreKeyId, (ulong)DateTime.UtcNow.Ticks, aliceSignedPreKey, aliceSignature));
            aliceStore.StorePreKey((uint)aliceUnsignedPreKeyId, new PreKeyRecord((uint)aliceUnsignedPreKeyId, aliceUnsignedPreKey));

            return alicePreKeyBundle;
        }

        static private PreKeyBundle createBobPreKeyBundle(SignalProtocolStore bobStore)
        {
            ECKeyPair bobUnsignedPreKey = Curve.generateKeyPair();
            int bobUnsignedPreKeyId = new Random().Next((int)Medium.MAX_VALUE);
            byte[] bobSignature = Curve.calculateSignature(bobStore.GetIdentityKeyPair().getPrivateKey(),
                                                                     bobSignedPreKey.getPublicKey().serialize());

            PreKeyBundle bobPreKeyBundle = new PreKeyBundle(1, 1,
                                                            (uint)bobUnsignedPreKeyId, bobUnsignedPreKey.getPublicKey(),
                                                            bobSignedPreKeyId, bobSignedPreKey.getPublicKey(),
                                                            bobSignature, bobStore.GetIdentityKeyPair().getPublicKey());

            bobStore.StoreSignedPreKey(bobSignedPreKeyId, new SignedPreKeyRecord(bobSignedPreKeyId, (ulong)DateTime.UtcNow.Ticks, bobSignedPreKey, bobSignature));
            bobStore.StorePreKey((uint)bobUnsignedPreKeyId, new PreKeyRecord((uint)bobUnsignedPreKeyId, bobUnsignedPreKey));

            return bobPreKeyBundle;
        }
    }
}
