using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Serivces.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUsername(string username);
        Task<User> Create(User user);
        Task<User> GetById(Guid userId);
    }
}
