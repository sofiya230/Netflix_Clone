using System.Text.Json;
using NetflixClone.Interfaces;

namespace NetflixClone.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly ILogger<ResendEmailService> _logger;

        public ResendEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<ResendEmailService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Resend:ApiKey"] ?? throw new InvalidOperationException("Resend API key not configured");
            _fromEmail = configuration["Resend:FromEmail"] ?? throw new InvalidOperationException("Resend from email not configured");
            _logger = logger;
            
            _httpClient.BaseAddress = new Uri("https://api.resend.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetCode)
        {
            try
            {
                var emailData = new
                {
                    from = _fromEmail,
                    to = email,
                    subject = "Password Reset Code - NetflixClone",
                    html = GeneratePasswordResetHtml(resetCode)
                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("emails", content);
                
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
                var emailData = new
                {
                    from = _fromEmail,
                    to = email,
                    subject = "Welcome to Netflix Clone!",
                    html = GenerateWelcomeHtml(firstName)
                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("emails", content);
                
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
                            <h1 style='color: #e50914;'>Netflix Clone</h1>
                            <h2>Password Reset Request</h2>
                        </div>
                        <p>You requested a password reset for your Netflix Clone account.</p>
                        <p>Use the following code to reset your password:</p>
                        <div class='code'>{resetCode}</div>
                        <p><strong>This code will expire in 10 minutes.</strong></p>
                        <p>If you didn't request this reset, please ignore this email.</p>
                        <div class='footer'>
                            <p>© 2024 Netflix Clone. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        public async Task<bool> SendTwoFactorCodeAsync(string email, string code)
        {
            try
            {
                var emailData = new
                {
                    from = _fromEmail,
                    to = email,
                    subject = "Two-Factor Authentication Code - NetflixClone",
                    html = GenerateTwoFactorCodeHtml(code)
                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("emails", content);
                
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
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Welcome to Netflix Clone</title>
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
                            <h1 style='color: #e50914;'>Netflix Clone</h1>
                        </div>
                        <div class='welcome'>Welcome, {firstName}!</div>
                        <p>Thank you for joining Netflix Clone! Your account has been successfully created.</p>
                        <p>You can now:</p>
                        <ul>
                            <li>Browse our extensive collection of movies and TV shows</li>
                            <li>Create multiple profiles for family members</li>
                            <li>Build your personal watchlist</li>
                            <li>Track your viewing history</li>
                        </ul>
                        <p>Start exploring now!</p>
                        <div class='footer'>
                            <p>© 2024 Netflix Clone. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
