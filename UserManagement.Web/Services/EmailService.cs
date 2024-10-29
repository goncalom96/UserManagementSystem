using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace UserManagement.Web.Services
{
    public class EmailService
    {
        public static void SendEmail(string to, string subject, string body)
        {
            // Cria uma nova mensagem de email
            var mail = new MailMessage();

            // Define o remetente do email
            mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_User"]);
            // Destinatário do email, adicionando o endereço à lista "To"
            mail.To.Add(to);
            // Assunto do email
            mail.Subject = subject;
            // Corpo da mensagem (conteúdo do email)
            mail.Body = body;
            // Configura o corpo da mensagem para aceitar HTML, permitindo formatação no email
            mail.IsBodyHtml = true;

            // Configura o cliente SMTP, responsável por enviar o email
            var smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTP_Host"]) // Define o servidor SMTP
            {
                Port = Convert.ToInt16(ConfigurationManager.AppSettings["SMTP_Port"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential
                (
                    ConfigurationManager.AppSettings["SMTP_User"],
                    ConfigurationManager.AppSettings["SMTP_Password"]
                ),

                // Habilita SSL (Secure Sockets Layer) para uma conexão segura
                EnableSsl = true
            };

            // Envia o email usando as configurações definidas acima
            smtp.Send(mail);
        }
    }
}