using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LockChatClient.Hubs
{
    public static class Mensajes
    {
        //Clase con los tipos de mensaje
        public const string RECIBIR = "MSGRecibido";
        public const string REGISTRO = "MSGRegistrado";
        public const string ENVIAR = "MSGEnviado";
    }
}
