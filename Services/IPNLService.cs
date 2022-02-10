using PaperTrading.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public interface IPNLService
    {
        Task AddPNL(PNL pNL);
        Task<IEnumerable<PNL>> GetUserPNLs(int userId, int take = 20);
    }
}