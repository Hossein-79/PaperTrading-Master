using PaperTrading.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public interface ICoinService
    {
        Task<Coin> GetCoin(string symbol);
        Task<IEnumerable<Coin>> GetCoins();
        Task<Coin> GetUsdt();
        Task Update(Coin coin);
    }
}