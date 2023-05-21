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

            var emailMessage = new MimeKit.MimeMessage();
            emailMessage.From.Add(new MimeKit.MailboxAddress("", "dhnvehiclemgmt@gmail.com"));
            emailMessage.To.Add(new MimeKit.MailboxAddress("", receiverEmail));
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
