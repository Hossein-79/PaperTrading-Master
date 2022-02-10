using PaperTrading.Models;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public interface IUserService
    {
        Task AddUser(User user);
        Task<User> GetUser(string name);
        Task<User> GetUser(int useId);
    }
}