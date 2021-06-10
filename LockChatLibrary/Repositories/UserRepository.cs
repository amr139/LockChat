using LockChatLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace LockChatLibrary.Repositories
{
    public interface IUserRepository
    {
        void Add(UserEntity user);
        bool Exists(string username);

        UserEntity GetUserByEmail(string email);

        UserEntity GetUserById(int idUser);

        IEnumerable<UserEntity> List();
        bool Delete(int userId);

        bool StoreKeys(UserKeys pair);
        UserKeys GetKeys(int userId);
        PreKeyBundleEntity GetKeyBundle(int user);
    }
    public class UserRepository : IUserRepository
    {

        public void Add(UserEntity user)
        {
            var query = "INSERT INTO [dbo].[Users] ( FirstName, LastName, Email, PasswordHash) VALUES (@firstname, @lastname, @email, @passwordhash)";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("FirstName", user.FirstName);
                dp.Add("LastName", user.LastName);
                dp.Add("Email", user.Email);
                dp.Add("PasswordHash", user.PasswordHash);

                connection.Execute(query, dp);
            }
        }

        public bool Exists(string username)
        {
            if (GetUserByEmail(username) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserEntity GetUserByEmail(string email)
        {
            var query = "SELECT * FROM [dbo].[Users] WHERE Email=@email";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("Email", email);

                return connection.QueryFirstOrDefault<UserEntity>(query, dp);
            }
        }

        public UserEntity GetUserById(int idUser)
        {
            var query = "SELECT * FROM [dbo].[Users] WHERE Id=@id";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("Id", idUser);

                return connection.QueryFirstOrDefault<UserEntity>(query, dp);
            }
        }

        public IEnumerable<UserEntity> List()
        {
            var query = "SELECT * FROM [dbo].[Users]";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                return connection.Query<UserEntity>(query);
            }
        }

        public bool Delete(int userId)
        {
            var query = "DELETE FROM [dbo].[Users] WHERE Id=@id";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("Id", userId);

                connection.Execute(query, dp);
            }

            return true;
        }

        public bool StoreKeys(UserKeys pair)
        {
            if (GetKeys(pair.UserId) != null)
                throw new InvalidOperationException("User keys already exist");

            var query = @"INSERT INTO [dbo].[UserKeys] ( UserId, PublicKey, PrivateKey, UnsignedPrePublicKey, UnsignedPrePrivateKey, Signature, UnsignedPreKeyId, SignedPreKeyId, SignedPrePublicKey, SignedPrePrivateKey) 
                                                 VALUES (@UserId, @PublicKey, @PrivateKey, @UnsignedPrePublicKey, @UnsignedPrePrivateKey, @Signature, @UnsignedPreKeyId, @SignedPreKeyId, @SignedPrePublicKey, @SignedPrePrivateKey)";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("UserId", pair.UserId);
                dp.Add("PublicKey", pair.PublicKey);
                dp.Add("PrivateKey", pair.PrivateKey);
                dp.Add("UnsignedPrePublicKey", pair.UnsignedPrePublicKey);
                dp.Add("UnsignedPrePrivateKey", pair.UnsignedPrePrivateKey);
                dp.Add("SignedPrePublicKey", pair.SignedPrePublicKey);
                dp.Add("SignedPrePrivateKey", pair.SignedPrePrivateKey);
                dp.Add("Signature", pair.Signature);
                dp.Add("UnsignedPreKeyId", pair.UnsignedPreKeyId);
                dp.Add("SignedPreKeyId", pair.SignedPreKeyId);

                connection.Execute(query, dp);
            }

            return true;
        }

        public UserKeys GetKeys(int userId)
        {
            var query = @"SELECT 
                            UserId, 
                            PublicKey, 
                            PrivateKey, 
                            UnsignedPrePublicKey, 
                            UnsignedPrePrivateKey, 
                            SignedPrePublicKey, 
                            SignedPrePrivateKey, 
                            Signature, 
                            UnsignedPreKeyId, 
                            SignedPreKeyId 
                        FROM [dbo].[UserKeys] where UserId = @UserId";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("UserId", userId);

                return connection.QueryFirstOrDefault<UserKeys>(query, dp);
            }
        }

        public PreKeyBundleEntity GetKeyBundle(int user)
        {
            var query = @"SELECT    UserId, 
                                    UnsignedPreKeyId as PreKeyId, 
                                    UnsignedPrePublicKey as PreKeyPublic, 
                                    SignedPreKeyId, 
                                    SignedPrePublicKey as SignedPreKeyPublic, 
                                    Signature as SignedPreKeySignature, 
                                    PublicKey as IdentityKey 
                            FROM [dbo].[UserKeys] where UserId = @UserId";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var dp = new DynamicParameters();
                dp.Add("UserId", user);

                return connection.QueryFirstOrDefault<PreKeyBundleEntity>(query, dp);
            }
        }
    }
}
