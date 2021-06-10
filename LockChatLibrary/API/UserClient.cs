using LockChatLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LockChatLibrary.API
{
    public interface IUserClient
    {
        int AddUser(UserEntity user);
        AuthToken Authenticate(UserEntity user);
        UserEntity GetProfile();
        IEnumerable<UserEntity> List();
        bool Delete(UserEntity user);
        UserKeys GetKeys();
        PreKeyBundleEntity GetKeyBundle(UserEntity user);

        bool SaveKeys(UserKeys key);

    }
    public class UserClient : IUserClient
    {
        ApiConfig config { get; set; }
        public UserClient(ApiConfig config)
        {
            this.config = config;
        }

        public int AddUser(UserEntity user)
        {
            return ClientHelper.Post<int, UserEntity>(config, "users/register", user);
        }

        public AuthToken Authenticate(UserEntity user)
        {
            return ClientHelper.Post<AuthToken, UserEntity>(config, "users/authenticate", user);
        }

        public IEnumerable<UserEntity> List()
        {
            return ClientHelper.Post<IEnumerable<UserEntity>, int>(config, "users/list", 1);
        }
        public UserEntity GetProfile()
        {
            return ClientHelper.Post<UserEntity, int>(config, "users/getprofile", 1);
        }
        public bool Delete(UserEntity user)
        {
            return ClientHelper.Post<bool, UserEntity>(config, "users/delete", user);
        }
        public UserKeys GetKeys()
        {
            return ClientHelper.Post<UserKeys, object>(config, "users/getkeys", null);
        }
        public PreKeyBundleEntity GetKeyBundle(UserEntity user)
        {
            return ClientHelper.Post<PreKeyBundleEntity, UserEntity>(config, "users/getkeybundle", user);
        }

        public bool SaveKeys(UserKeys key)
        {
            return ClientHelper.Post<bool, UserKeys>(config, "users/generatekeys", key);

        }

    }
}
