namespace NetflixClone.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string resetCode);
        Task<bool> SendWelcomeEmailAsync(string email, string firstName);
        Task<bool> SendTwoFactorCodeAsync(string email, string code);
    }
}
