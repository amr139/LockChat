using System;

namespace LockChatLibrary
{
    public class Configuration
    {
        public static string ConnectionString = $@"Data Source = (LocalDb)\MSSQLLocalDB;Initial Catalog = LockChatDB; Integrated Security = SSPI";
        public static string ApiUrl = "https://localhost:44316";
    }
}
