
using System.Threading.Tasks;
using intro2NET.API.Models;

namespace intro2NET.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExistsCheck(string username);
    }
}
