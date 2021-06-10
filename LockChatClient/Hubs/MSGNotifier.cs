using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace LockChatClient.Hubs
{
    public class MSGNotifier : IAsyncDisposable
    {
        //Clase que controla el WebSocket-SignalR
        public const string HUBURL = "/chat";
        private readonly string _hubUrl;
        private HubConnection _hubConnection;
        private readonly string _username;
        private bool _started = false;

        public MSGNotifier(int idUser, string siteUrl)
        {
            //creamos el objeto con los datos de idUsuario y la URL
            if (string.IsNullOrWhiteSpace(idUser.ToString()))
                throw new ArgumentNullException(nameof(idUser));
            if (string.IsNullOrWhiteSpace(siteUrl))
                throw new ArgumentNullException(nameof(siteUrl));
            _username = idUser.ToString();
            _hubUrl = siteUrl.TrimEnd('/') + HUBURL;
        }
       
        //Iniciamos la conexion a SignalR
        public async Task StartAsync()
        {
            if (!_started)
            {
                _hubConnection = new HubConnectionBuilder()
                    .Build();
                //si escuchamos nuevos mensajes pasamos al handler
                _hubConnection.On<string, string>(Mensajes.RECIBIR, (user, message) =>
                {
                    HandleReceiveMessage(user, message);
                });
                //arrancamos la conexion
                await _hubConnection.StartAsync();
                _started = true;
                //envimaos un primer mensaje de inicio (se desprecia entre los clientes)
                await _hubConnection.SendAsync(Mensajes.REGISTRO, _username);
            }
        }
        //Handler de mensaje recibido
        public event MessageReceivedEventHandler MessageReceived;
        private void HandleReceiveMessage(string username, string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(username, message));
        }
        
        //Enviar mensaje
        public async Task SendAsync(string message)
        {
            if (!_started)
                throw new InvalidOperationException("Servidor no iniciado!");
            await _hubConnection.SendAsync(Mensajes.ENVIAR, _username, message);
        }

        //parar servidor (no se usa)
        public async Task StopAsync()
        {
            if (_started)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _started = false;
            }
        }

        //parar servidor (no se usa)
        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
    }

    //Clase para gestionar los eventos de los mensajes recibidos (task para permitir async-await)
    public delegate Task MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string username, string message)
        {
            Username = username;
            Message = message;
        }

        public string Username { get; set; }

        public string Message { get; set; }

    }

}
