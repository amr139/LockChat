using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LockChatLibrary.Entities;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LockChatLibrary.Services;

namespace LockChatApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private IMessageService _messageService;
        private readonly AppSettings _appSettings;

        public MessagesController(
            IMessageService messageService,
            IOptions<AppSettings> appSettings)
        {
            _messageService = messageService;
            _appSettings = appSettings.Value;
        }


        [HttpPost("SaveMessage")]
        public bool SaveMessage(MessageEntity msg)
        {
            msg.SenderId=int.Parse(HttpContext.User.Claims.First(_ => _.Type == ClaimTypes.Name).Value);
            
            return _messageService.SaveMessage(msg);
        }

        [HttpGet("GetChat/{chatUser}")]
        public IEnumerable<MessageEntity> GetChat(int chatUser)
        {
            var idUser = int.Parse(HttpContext.User.Claims.First(_ => _.Type == ClaimTypes.Name).Value);
            return _messageService.GetChat(idUser, chatUser);
        }
        

        [HttpGet("GetFriends")]
        public IEnumerable<FriendEntity> GetFriends()
        {
            var idUser = int.Parse(HttpContext.User.Claims.First(_ => _.Type == ClaimTypes.Name).Value);
            return _messageService.GetFriends(idUser);
        }
        

    }
}
