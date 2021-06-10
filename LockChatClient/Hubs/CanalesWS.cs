using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LockChatClient.Hubs
{
    public class CanalesWS : Hub
    {

        //Metodo unico para enviar notificacion por el Hub al usuario que recibe mensaje
        public async Task EnviarMensaje(int idSender, int idReceiver)
        {
            await Clients.User(idReceiver.ToString()).SendAsync(Mensajes.RECIBIR,idSender);
        }

    }
}
