using PaperTrading.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public interface IWalletService
    {
        Task AddWallet(Wallet wallet);
        Task<Wallet> GetUserWallet(int userId, int coinId);
        Task<IEnumerable<Wallet>> GetUserWallets(int userId);
        Task Update(Wallet wallet);
        Task UpdateRange(IEnumerable<Wallet> wallet);
    }
}