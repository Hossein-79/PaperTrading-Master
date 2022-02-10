using PaperTrading.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public interface ITradeService
    {
        Task AddTrade(Trade trade);
        Task<IEnumerable<Trade>> GetCoinTradesBuy(int coinId, decimal price);
        Task<IEnumerable<Trade>> GetCoinTradesSell(int coinId, decimal price);
        Task<Trade> GetTrade(int tradeId);
        Task<IEnumerable<Trade>> GetUserTrades(int userId);
        Task Update(Trade trade);
        Task UpdateRange(IEnumerable<Wallet> wallet);
    }
}