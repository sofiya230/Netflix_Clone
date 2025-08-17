using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithProfilesAsync(int userId);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
