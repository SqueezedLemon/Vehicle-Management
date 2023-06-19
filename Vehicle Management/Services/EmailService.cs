using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;


namespace Vehicle_Management.Services
{
    public class EmailService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserManager> _userManager;
        private readonly IConfiguration _configuration;
        private static string[] Scopes = { GmailService.Scope.GmailSend };
        private static string ApplicationName = "Vehicle Management";
        public EmailService(ApplicationDbContext dbContext, UserManager<UserManager> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        public void SendEmail(string receiverEmail, string subject, string emailContent)
        {
            // SMTP server settings
            var smtpHost = _configuration["AppSettings:SmtpHost"];
            var smtpPort = 25;
            var smtpUsername = _configuration["AppSettings:SmtpUsername"];
            var smtpPassword = _configuration["AppSettings:SmtpPassword"];

            // Email details
            var fromAddress = _configuration["AppSettings:SmtpSenderAddress"];

            // Create the SMTP client with custom settings
            using var smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            // Create the email message
            using var mailMessage = new MailMessage(fromAddress, receiverEmail, subject, emailContent);
            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

            //Using Gmail to send mails
            //UserCredential credential;
            //using (var stream = new FileStream("token.json", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    string credPath = "~/Tokens";
            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        new ClientSecrets
            //        {
            //            ClientId = _configuration["AppSettings:GmailSecretId"],
            //            ClientSecret = _configuration["AppSettings:GmailSecretKey"]
            //        },
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //}

            //var service = new GmailService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = ApplicationName
            //});

            //var emailMessage = new MimeKit.MimeMessage();
            //emailMessage.From.Add(new MimeKit.MailboxAddress("", "dhnvehiclemgmt@gmail.com"));
            //emailMessage.To.Add(new MimeKit.MailboxAddress("", receiverEmail));
            //emailMessage.Subject = subject;
            //emailMessage.Body = new MimeKit.TextPart("plain")
            //{
            //    Text = emailContent
            //};

            //using (var stream = new MemoryStream())
            //{
            //    emailMessage.WriteTo(stream);
            //    var message = new Google.Apis.Gmail.v1.Data.Message
            //    {
            //        Raw = Convert.ToBase64String(stream.ToArray())
            //    };

            //    try
            //    {
            //        var request = service.Users.Messages.Send(message, "me");
            //        request.Execute();
            //        Console.WriteLine("Email sent successfully!");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
            //    }
            //}
        }

        public async Task SendEmailToAdmins(string subject, string emailContent)
        {
            UserCredential credential;
            using (var stream = new FileStream("token.json", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                string credPath = "~/Tokens";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = _configuration["AppSettings:GmailSecretId"],
                        ClientSecret = _configuration["AppSettings:GmailSecretKey"]
                    },
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (admins != null)
            {
                var adminEmails = admins.Select(u => u.Email);
                foreach (var adminEmail in adminEmails)
                {
                    var emailMessage = new MimeKit.MimeMessage();
                    emailMessage.From.Add(new MimeKit.MailboxAddress("", "dhnvehiclemgmt@gmail.com"));
                    emailMessage.To.Add(new MimeKit.MailboxAddress("", adminEmail));
                    emailMessage.Subject = subject;
                    emailMessage.Body = new MimeKit.TextPart("plain")
                    {
                        Text = emailContent
                    };

                    using (var stream = new MemoryStream())
                    {
                        emailMessage.WriteTo(stream);
                        var message = new Google.Apis.Gmail.v1.Data.Message
                        {
                            Raw = Convert.ToBase64String(stream.ToArray())
                        };

                        try
                        {
                            var request = service.Users.Messages.Send(message, "me");
                            request.Execute();
                            Console.WriteLine("Email sent successfully!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
                        }
                    }

                }
            }
        }
    }
}
