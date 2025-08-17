using System.Net.Mail;
using System.Net;
using NetflixClone.Interfaces;

namespace NetflixClone.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly ILogger<GmailEmailService> _logger;
        private readonly string _gmailAddress;
        private readonly string _appPassword;

        public GmailEmailService(IConfiguration configuration, ILogger<GmailEmailService> logger)
        {
            _logger = logger;
            _gmailAddress = configuration["Gmail:Address"] ?? throw new InvalidOperationException("Gmail address not configured");
            _appPassword = configuration["Gmail:AppPassword"] ?? throw new InvalidOperationException("Gmail app password not configured");
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetCode)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_gmailAddress, _appPassword);
                    client.EnableSsl = true;
                    client.Timeout = 30000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_gmailAddress, "NetflixClone"),
                        Subject = "Password Reset Code - NetflixClone",
                        Body = GeneratePasswordResetHtml(resetCode),
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string firstName)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_gmailAddress, _appPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_gmailAddress, "NetflixClone"),
                        Subject = "Welcome to NetflixClone!",
                        Body = GenerateWelcomeHtml(firstName),
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendTwoFactorCodeAsync(string email, string code)
        {
            try
            {


                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_gmailAddress, _appPassword);
                    client.EnableSsl = true;
                    client.Timeout = 30000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_gmailAddress, "NetflixClone"),
                        Subject = "Two-Factor Authentication Code - NetflixClone",
                        Body = GenerateTwoFactorHtml(code),
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GeneratePasswordResetHtml(string resetCode)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Password Reset</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .code {{ font-size: 32px; font-weight: bold; text-align: center; color: #e50914; background: #f8f8f8; padding: 20px; border-radius: 8px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='color: #e50914;'>NetflixClone</h1>
                            <h2>Password Reset Request</h2>
                        </div>
                        <p>You requested a password reset for your NetflixClone account.</p>
                        <p>Use the following code to reset your password:</p>
                        <div class='code'>{resetCode}</div>
                        <p><strong>This code will expire in 10 minutes.</strong></p>
                        <p>If you didn't request this reset, please ignore this email.</p>
                    </div>
                </body>
                </html>";
        }

        private string GenerateWelcomeHtml(string firstName)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Welcome to NetflixClone</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .welcome {{ font-size: 24px; color: #e50914; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='color: #e50914;'>NetflixClone</h1>
                        </div>
                        <div class='welcome'>Welcome, {firstName}!</div>
                        <p>Thank you for joining NetflixClone! Your account has been successfully created.</p>
                        <p>You can now:</p>
                        <ul>
                            <li>Browse our extensive collection of movies and TV shows</li>
                            <li>Create multiple profiles for family members</li>
                            <li>Build your personal watchlist</li>
                            <li>Track your viewing history</li>
                        </ul>
                        <p>Start exploring now!</p>
                    </div>
                </body>
                </html>";
        }

        private string GenerateTwoFactorHtml(string code)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Two-Factor Authentication</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .code {{ font-size: 32px; font-weight: bold; text-align: center; color: #e50914; background: #f8f8f8; padding: 20px; border-radius: 8px; margin: 20px 0; }}
                        .security {{ background: #e8f5e8; padding: 15px; border-radius: 8px; margin: 20px 0; }}
                        .warning {{ background: #fff3cd; padding: 15px; border-radius: 8px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='color: #e50914;'>NetflixClone</h1>
                            <h2>Two-Factor Authentication</h2>
                        </div>
                        <p>You've requested to log in to your NetflixClone account.</p>
                        <p>Use the following verification code to complete your login:</p>
                        <div class='code'>{code}</div>
                        <div class='security'>
                            <p><strong>🔒 Security Notice:</strong></p>
                            <p>This code will expire in 10 minutes for your security.</p>
                            <p>If you didn't request this login, please change your password immediately.</p>
                        </div>
                        <div class='warning'>
                            <p><strong>⚠️ Important:</strong></p>
                            <p>Never share this code with anyone. NetflixClone staff will never ask for your verification code.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
