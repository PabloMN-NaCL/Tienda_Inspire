using System;
using System.Collections.Generic;
using System.Text;

namespace TiendInspire.Notificaciones.Email
{
    internal interface IEmailService
    {
        Task SendWelcomeEmail(string toEamil);
    }
}
