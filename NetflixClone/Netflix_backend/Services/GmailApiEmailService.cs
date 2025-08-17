using System.Net.Http;
using System.Text;
using NetflixClone.Interfaces;
using System.Text.Json;

namespace NetflixClone.Services
{
    public class GmailApiEmailService : IEmailService
    {
        private readonly ILogger<GmailApiEmailService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _gmailAddress;
        private readonly string _accessToken;

        public GmailApiEmailService(IConfiguration configuration, ILogger<GmailApiEmailService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _gmailAddress = configuration["Gmail:Address"] ?? throw new InvalidOperationException("Gmail address not configured");
            _accessToken = configuration["Gmail:AccessToken"] ?? throw new InvalidOperationException("Gmail access token not configured");
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetCode)
        {
            try
            {
                var emailContent = GeneratePasswordResetHtml(resetCode);
                var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailContent))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                var gmailMessage = new
                {
                    raw = base64Content
                };

                var json = JsonSerializer.Serialize(gmailMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.PostAsync("https://gmail.googleapis.com/gmail/v1/users/me/messages/send", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
                var emailContent = GenerateWelcomeHtml(firstName);
                var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailContent))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                var gmailMessage = new
                {
                    raw = base64Content
                };

                var json = JsonSerializer.Serialize(gmailMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.PostAsync("https://gmail.googleapis.com/gmail/v1/users/me/messages/send", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
                    catch (Exception ex)
        {
            return false;
        }
        }

        private string GeneratePasswordResetHtml(string resetCode)
        {
            return $@"
                From: {_gmailAddress}
                To: {_gmailAddress}
                Subject: Password Reset Code - NetflixClone
                MIME-Version: 1.0
                Content-Type: text/html; charset=utf-8

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
                        <div class='footer'>
                            <p>© 2024 NetflixClone. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        public async Task<bool> SendTwoFactorCodeAsync(string email, string code)
        {
            try
            {
                var emailContent = GenerateTwoFactorCodeHtml(code);
                var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailContent))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                var gmailMessage = new
                {
                    raw = base64Content
                };

                var json = JsonSerializer.Serialize(gmailMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.PostAsync("https://gmail.googleapis.com/gmail/v1/users/me/messages/send", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GenerateTwoFactorCodeHtml(string code)
        {
            return $@"
                From: {_gmailAddress}
                To: {_gmailAddress}
                Subject: Two-Factor Authentication Code - NetflixClone
                MIME-Version: 1.0
                Content-Type: text/html; charset=utf-8

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
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='color: #e50914;'>NetflixClone</h1>
                            <h2>Two-Factor Authentication Code</h2>
                        </div>
                        <p>You requested a two-factor authentication code for your NetflixClone account.</p>
                        <p>Use the following code to complete your login:</p>
                        <div class='code'>{code}</div>
                        <p><strong>This code will expire in 10 minutes.</strong></p>
                        <p>If you didn't request this code, please ignore this email.</p>
                        <div class='footer'>
                            <p>© 2024 NetflixClone. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GenerateWelcomeHtml(string firstName)
        {
            return $@"
                From: {_gmailAddress}
                To: {_gmailAddress}
                Subject: Welcome to NetflixClone!
                MIME-Version: 1.0
                Content-Type: text/html; charset=utf-8

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
                        <div class='footer'>
                            <p>© 2024 NetflixClone. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}

