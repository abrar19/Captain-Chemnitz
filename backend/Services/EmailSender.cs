using backend.Models.Settings;
using MailKit.Security;
using MimeKit;

namespace backend.Services;

public class EmailSender
{
    public Boolean sendVerificationEmail(string TO, String Link,ApplicationSettings appSettings)
    {
        try
        {
            String email = appSettings.Verification_Email;
            string password = appSettings.Verfication_Email_Pass;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("SMTP credentials are not set.");
                return false;
            }
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Caption Chemnitz", email));
            message.To.Add(new MailboxAddress(TO, TO));
            message.Subject = "Verify Email";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                { Text = string.Format("<a href='{0 }' style='font - size:22px; padding: 10px; color: #ffffff'>Confirm Email Now </ a > ", Link) };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {

                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                

              
                
                client.Authenticate(email, password);

                client.Send(message);

                client.Disconnect(true);
            }

        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}