using NetflixClone.Interfaces;

namespace NetflixClone.Services
{
    public class ConsoleEmailService : IEmailService
    {
        public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        {
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetCode)
        {
            try
            {
                await Task.Delay(1000);
                return true;
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
                await Task.Delay(1000);
                return true;
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
                await Task.Delay(1000);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

