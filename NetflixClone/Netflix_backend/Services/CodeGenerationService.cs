using System.Security.Cryptography;

namespace NetflixClone.Services
{
    public interface ICodeGenerationService
    {
        string GenerateSecureCode();
        bool ValidateCode(string storedCode, string inputCode);
        bool IsCodeExpired(DateTime createdAt, int expiryMinutes = 10);
    }

    public class CodeGenerationService : ICodeGenerationService
    {
        public string GenerateSecureCode()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                
                var number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
                return number.ToString("D6");
            }
        }

        public bool ValidateCode(string storedCode, string inputCode)
        {
            if (string.IsNullOrEmpty(storedCode) || string.IsNullOrEmpty(inputCode))
                return false;

            return storedCode.Trim().Equals(inputCode.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public bool IsCodeExpired(DateTime createdAt, int expiryMinutes = 10)
        {
            return DateTime.UtcNow > createdAt.AddMinutes(expiryMinutes);
        }
    }
}


