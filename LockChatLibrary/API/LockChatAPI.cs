using System;
using System.Collections.Generic;
using System.Text;

namespace LockChatLibrary.API
{
    public class LockChatAPI
    {

        private ApiConfig config;
        public IUserClient User { get; set; }
        public IMessageClient Message { get; set; }

        public LockChatAPI(ApiConfig apiConfig)
        {
            this.config = apiConfig;

            this.User = new UserClient(apiConfig);
            this.Message = new MessageClient(apiConfig);
        }

    }
}
