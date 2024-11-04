using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace UserManagement.Web.Services
{
    public class EmailService
    {
        public EmailService()
        {
        }

        public void SendEmail(string to, string subject, string body)
        {
            // Cria uma nova mensagem de email
            var mail = new MailMessage
            {
                // Define o remetente do email
                From = new MailAddress(ConfigurationManager.AppSettings["SMTP_User"]),
                Subject = subject,    // Assunto do email
                Body = body,          // Corpo da mensagem (conteúdo do email)
                IsBodyHtml = true     // Configura o corpo da mensagem para aceitar HTML
            };

            // Adiciona o destinatário do email
            mail.To.Add(to);

            // Configura o cliente SMTP, responsável por enviar o email
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTP_Host"]) // Define o servidor SMTP
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