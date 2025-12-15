using System;
using System.Collections.Generic;
using System.Text;

namespace TiendInspire.Notificaciones.Email
{
    public  interface IEmailService
    {
        Task SendWelcomeMail(string toEamil);

        Task SendOrderConfirmationMail(string toEmail, int orderId, IEnumerable<TiendaInspire.Shared.Events.OrderItemEvent> items);
        Task SendOrderCancellationMail(string toEmail, int orderId);
    }
}
