using LockChatLibrary.Entities;
using LockChatLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using libsignal;
using libsignal.ecc;
using libsignal.state.impl;
using libsignal.util;
using System.Linq;


namespace LockChatLibrary.Services
{

    public interface IUserService
    {
        UserEntity Authenticate(string username, string password);
        IEnumerable<UserEntity> GetAll();
        UserEntity GetById(int id);
        UserEntity Create(UserEntity user);
        bool GenerateKeys(int user,UserKeys key);
        void Delete(int id);
        PreKeyBundleEntity GetKeyBundle(int user);
        UserKeys GetKeys(int user);
    }

    public class UserService : IUserService
    {
        private IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserEntity Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = userRepository.GetUserByEmail(username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<UserEntity> GetAll()
        {

            return userRepository.List();
        }


        public UserEntity GetById(int id)
        {
            return userRepository.GetUserById(id);
        }

        public UserEntity Create(UserEntity user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new InvalidOperationException("Password is required");

            if (userRepository.Exists(user.Email))
                throw new InvalidOperationException("Username \"" + user.Email + "\" is already taken");

            byte[] passwordHash;
            CreatePasswordHash(user.Password, out passwordHash);

            user.PasswordHash = passwordHash;

            userRepository.Add(user);
            user = userRepository.GetUserByEmail(user.Email);
            return user;
        }

        public void Delete(int id)
        {
            userRepository.Delete(id);
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            byte[] passwordHash;

            using (SHA256 sha256hash = SHA256.Create())
            {
                passwordHash = sha256hash.ComputeHash(sha256hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
                storedHash = sha256hash.ComputeHash(storedHash);
            }

            int accum = 0;

            for(int i = 0; i < storedHash.Length; i++)
            {
                accum |= (storedHash[i] ^ passwordHash[i]);
            }

            return accum == 0;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (SHA256 sha256hash = SHA256.Create())
            {
                passwordHash = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool GenerateKeys(int user, UserKeys key)
        {
            return userRepository.StoreKeys(key);
        }

        public UserKeys GetKeys(int user)
        {
            return userRepository.GetKeys(user);
        }

        public PreKeyBundleEntity GetKeyBundle(int user)
        {
            return userRepository.GetKeyBundle(user);
        }
    }
}
